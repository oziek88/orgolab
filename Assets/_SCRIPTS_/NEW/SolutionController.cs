using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolutionController : MonoBehaviour {
    private static SolutionController _instance;
    public static SolutionController Instance { get { return _instance; } }
    
    public delegate void SolutionControllerEventHandler(SolutionController solutionController);
    public static event SolutionControllerEventHandler onSolutionUpdated;

    public MoleculePool moleculePool;
    public PhysicsMaterial2D boundsMaterial;


    [HideInInspector] public List<Solute> currentlyInSolution = new List<Solute>();
    [HideInInspector] public bool containerFull = false;
    [HideInInspector] public Vector2 centerOfContainer = Vector2.zero;
    [HideInInspector] public bool ionsInSolution = false;
    [HideInInspector] public bool markov = true;
    private bool containsHOOH = false;
    private bool containsHeat = false;
    private float containerVolume = 0;
    private string hint = "Click 'Add' to start putting molecules into your solution";

    private void Awake() {
        // If there is an instance, and it's not me, delete myself.

        if(Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }

    public void Spawn(Solute solute) {
        GetSolutionVolume();

        if(!containerFull) {
            GameObject newMolecule = Instantiate(solute.gameObject, transform);
            Rigidbody2D rigidbody = newMolecule.GetComponent<Rigidbody2D>();
            Vector3 pos = newMolecule.transform.localPosition;

            newMolecule.transform.localPosition = new Vector3(pos.x - rigidbody.centerOfMass.x, pos.y, pos.z);

            Vector2 randomSpawnVelocity = new Vector2(Random.Range(-3f, 3f), Random.Range(-12f, -8f));
            float randomSpawnRotation = Random.Range(-120f, 120f);
            rigidbody.velocity = randomSpawnVelocity;
            rigidbody.angularVelocity = randomSpawnRotation;
        } else {
            Debug.Log("Container is too full. Delete some molecules.");
        }
    }

    public void GetSolutionVolume() {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        float totalVolume = 0;
        foreach(Collider2D collider in colliders) {
            Bounds bounds = collider.bounds;
            float volume = bounds.size.x * bounds.size.y;
            totalVolume += volume;
        }

        containerFull = totalVolume > containerVolume / 2;
    }

    public int SolutionContains(Solute solute) {
        int count = 0;
        foreach(Solute s in currentlyInSolution) {
            if(solute.GetType() == s.GetType()) {
                count += 1;
            }
        }
        return count;
    }

    private void UpdateIonsInSolution() {
        int ionCount = 0;
        foreach(Solute solute in currentlyInSolution) {
            if(solute is Atom) {
                ionCount += 1;
            } else if(solute is Molecule molecule) {
                if(molecule.currentCharge != ElectronCharge.Charge.None) {
                    ionCount += 1;
                }
            }
        }

        ionsInSolution = ionCount >= 2 ? true : false;
    }

    private void CheckMarkov(Solute solute) {
        markov = true;
        if(containsHeat && containsHOOH) {
            if(solute is Hydrohalide hydrohalide) {
                if(hydrohalide.attachedHalogen == Hydrohalide.Halogen.Bromide) {
                    markov = false;
                    return;
                }
            }
        }
    }

    public void UpdateHeat(bool heat) {
        containsHeat = heat;
    }

    public void UpdateSolvent(string solvent) {
        if(solvent == "HOOH") {
            containsHOOH = true;
        } else {
            containsHOOH = false;
        }
    }

    public void AddToSolution(Solute solute, bool update = true) {
        currentlyInSolution.Add(solute);

        if(currentlyInSolution.Count >= 2) {
            UpdateHint("These molecules don't react. Try adding different molecules to the solution.");
        }

        if(update) {
            UpdateSolution();
        }
    }

    public void AddToSolution(List<Solute> solutes, bool update = true) {
        foreach(Solute solute in solutes) {
            currentlyInSolution.Add(solute);
        }

        if(currentlyInSolution.Count >= 2) {
            UpdateHint("These molecules don't react. Try adding different molecules to the solution.");
        }

        if(update) {
            UpdateSolution();
        }
    }

    public void RemoveFromSolution(List<Solute> solutes, bool update = true, bool trash = false) {
        foreach(Solute solute in solutes) {
            currentlyInSolution.Remove(solute);
        }
        if(update) {
            UpdateSolution();
        }
    }

    public void RemoveFromSolution(Solute solute, bool update = true, bool trash = false) {
        currentlyInSolution.Remove(solute);
        if(!trash) {
            CheckMarkov(solute);
        }
        if(update) {
            UpdateSolution();
        }
        if(currentlyInSolution.Count < 2) {
            UpdateHint("Add molecules into the solution to start up a reaction.");
        }
    }

    public void UpdateSolution() {
        currentlyInSolution.RemoveAll(x => x == null);
        UpdateIonsInSolution();
        onSolutionUpdated?.Invoke(this);
    }

    public void UpdateHint(string hintText) {
        hint = hintText;
    }

    public string GetHint() {
        return hint;
    }

    public void GenerateCollidersAcrossScreen(float optionsPanelHeight) {
        Canvas canvas = FindObjectOfType<Canvas>();
        float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        float delta = optionsPanelHeight / canvasHeight;

        Vector2 rUCorner = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.nearClipPlane));
        Vector2 lDCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, delta, Camera.main.nearClipPlane));
        float l = rUCorner.y + Mathf.Abs(lDCorner.y);
        float w = rUCorner.x + Mathf.Abs(lDCorner.x);
        containerVolume = l * w;
        centerOfContainer = (rUCorner + lDCorner) / 2;

        Vector2[] colliderpoints;

        GameObject upperEdgeGO = new GameObject("upperEdge");
        EdgeCollider2D upperEdge = upperEdgeGO.AddComponent<EdgeCollider2D>();
        colliderpoints = upperEdge.points;
        colliderpoints[0] = new Vector2(lDCorner.x, rUCorner.y);
        colliderpoints[1] = new Vector2(rUCorner.x, rUCorner.y);
        upperEdge.points = colliderpoints;
        PlatformEffector2D upperEdgePlatform = upperEdgeGO.AddComponent<PlatformEffector2D>();
        upperEdgePlatform.rotationalOffset = 180;
        upperEdge.usedByEffector = true;
        upperEdge.sharedMaterial = boundsMaterial;

        GameObject lowerEdgeGO = new GameObject("lowerEdge");
        EdgeCollider2D lowerEdge = lowerEdgeGO.AddComponent<EdgeCollider2D>();
        colliderpoints = lowerEdge.points;
        colliderpoints[0] = new Vector2(lDCorner.x, lDCorner.y);
        colliderpoints[1] = new Vector2(rUCorner.x, lDCorner.y);
        lowerEdge.points = colliderpoints;
        PlatformEffector2D lowerEdgePlatform = lowerEdgeGO.AddComponent<PlatformEffector2D>();
        lowerEdgePlatform.rotationalOffset = 0;
        lowerEdge.usedByEffector = true;
        lowerEdge.sharedMaterial = boundsMaterial;

        GameObject leftEdgeGO = new GameObject("leftEdge");
        EdgeCollider2D leftEdge = leftEdgeGO.AddComponent<EdgeCollider2D>();
        colliderpoints = leftEdge.points;
        colliderpoints[0] = new Vector2(lDCorner.x, lDCorner.y);
        colliderpoints[1] = new Vector2(lDCorner.x, rUCorner.y);
        leftEdge.points = colliderpoints;
        PlatformEffector2D leftEdgePlatform = leftEdgeGO.AddComponent<PlatformEffector2D>();
        leftEdgePlatform.rotationalOffset = -90;
        leftEdge.usedByEffector = true;
        leftEdge.sharedMaterial = boundsMaterial;

        GameObject rightEdgeGO = new GameObject("rightEdge");
        EdgeCollider2D rightEdge = rightEdgeGO.AddComponent<EdgeCollider2D>();
        colliderpoints = rightEdge.points;
        colliderpoints[0] = new Vector2(rUCorner.x, rUCorner.y);
        colliderpoints[1] = new Vector2(rUCorner.x, lDCorner.y);
        rightEdge.points = colliderpoints;
        PlatformEffector2D rightEdgePlatform = rightEdgeGO.AddComponent<PlatformEffector2D>();
        rightEdgePlatform.rotationalOffset = 90;
        rightEdge.usedByEffector = true;
        rightEdge.sharedMaterial = boundsMaterial;
    }
}
