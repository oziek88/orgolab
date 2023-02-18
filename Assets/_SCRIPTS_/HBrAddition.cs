using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HBrAddition : Reaction {
    public MouseManager mouseManager;
    public ReactionPool reactionPool;
    public MoleculePool moleculePool;

    public GameObject GameClock;
    public Text instructor;
    public bool collisionsAllowed = true;

    private bool inHOOH = false;
    private Acid HBr;
    private GameObject HBrGO;
    private Bond HBrBond;
    private Alkene alkene;
    private GameObject alkeneGO;
    private GameObject carbon1;
    private Atom c1Details;
    private GameObject carbon2;
    private Atom c2Details;

    void Start() {
        Spawning.onChangeSolution += CheckForHOOH;
        Atom.onCollision += CheckCollision;

        instructor = GameObject.Find("InstructorPanel").GetComponentInChildren<Text>();

        List<GameObject> reactants = GrabReactants();
        StartCoroutine(reactionPool.spawning.SpawnMolecules(reactants));
    }
    private void Update() {
        if(Input.touchCount == 2) {
            List<GameObject> hits = mouseManager.TwoFingerTouch();
            if(hits.Count == 0) {
                return;
            } else {
                if(ReferenceEquals(hits[0] ?? hits[1], HBrGO)) {
                    HBrBond.BondBreak();
                }
            }
        }
    }
    private List<GameObject> GrabReactants() {
        List<GameObject> reactants = new List<GameObject>();

        HBrGO = Instantiate(moleculePool.GrabHBr(), transform);
        HBr = HBrGO.GetComponent<Acid>();
        HBrBond = HBrGO.GetComponentInChildren<Bond>();
        HBrBond.BondCanBreak(true);
        reactants.Add(HBrGO);

        alkeneGO = Instantiate(moleculePool.GrabRandomAlkene(), transform);
        alkene = alkeneGO.GetComponent<Alkene>();
        reactants.Add(alkeneGO);

        carbon1 = alkene.dbPrimaryCarbon == null ? alkene.dbSecondaryCarbon : alkene.dbPrimaryCarbon;
        c1Details = carbon1.GetComponent<Atom>();
        carbon2 = alkene.dbTertiaryCarbon == null ? alkene.dbSecondaryCarbon : alkene.dbTertiaryCarbon;
        c2Details = carbon2.GetComponent<Atom>();

        HBrGO.SetActive(false);
        alkeneGO.SetActive(false);

        return reactants;
    }

    public void CheckForHOOH(Spawning spawning, string solution) {
        if(solution == "HOOH") {
            inHOOH = true;
        } else {
            inHOOH = false;
        }
    }

    public void CheckCollision(Atom atomScript, GameObject atom, GameObject collidingAtom) {
        if(collisionsAllowed) {
            // Without Hydrogen Peroxide in Solution
            if(!inHOOH) {
                // If Hydrogen collides with alkene
                if(collidingAtom.name == "Hydrogen") {
                    if(ReferenceEquals(atom, carbon1)) {
                        c1Details.PlayHydrogenAddedClip();
                        Destroy(collidingAtom);
                        c2Details.PlayUnstableAtomClip();
                        c2Details.charge.GivePositiveCharge();
                    }
                }

                // If Bromine collides with alkene
                if(collidingAtom.name == "Bromine") {
                    if(ReferenceEquals(atom, carbon2) && c2Details.charge.currentCharge == ElectronCharge.Charge.Positive) {
                        c2Details.StopUnstableAtomClip();
                        c2Details.charge.RemoveCharge();

                        // form bromine bond to carbon2
                        GameObject bromine = collidingAtom;
                        Destroy(bromine.GetComponent<Rigidbody2D>());
                        bromine.transform.parent = alkene.transform;
                        bromine.transform.localScale = new Vector3(1, 1, 1);
                        bromine.transform.localPosition = new Vector3(2f, 0.5f, 0);

                        // completed the reaction
                        reactionPool.spawning.completedReaction = true;
                    } else if(ReferenceEquals(atom, carbon2) && c2Details.charge.currentCharge != ElectronCharge.Charge.Positive) {
                        Debug.Log("Bromine doesn't react first in this reaction");
                        StartCoroutine(NotBrFirst());
                    } else if(!ReferenceEquals(atom, carbon2)) {
                        Debug.Log("Bromine doesn't react with this carbon");
                        StartCoroutine(NotBrHere());
                    }
                }
            } else {
                // WITH Hydrogen Peroxide in Solution
                // If Hydrogen collides with alkene
                if(collidingAtom.name == "Hydrogen") {
                    if(ReferenceEquals(atom, carbon2)) {
                        c2Details.PlayHydrogenAddedClip();
                        Destroy(collidingAtom);
                        //c1Details.anim.CrossFade(c1Details.clips[1]);
                        c1Details.PlayUnstableAtomClip();
                        c1Details.charge.GivePositiveCharge();
                    }
                }

                // If Bromine collides with alkene
                if(collidingAtom.name == "Bromine") {
                    if(ReferenceEquals(atom, carbon1) && c1Details.charge.currentCharge == ElectronCharge.Charge.Positive) {
                        c1Details.StopUnstableAtomClip();
                        c1Details.charge.RemoveCharge();

                        // form bromine bond to carbon1
                        GameObject bromine = collidingAtom;
                        Destroy(bromine.GetComponent<Rigidbody2D>());
                        bromine.transform.parent = alkene.transform;
                        bromine.transform.localScale = new Vector3(1, 1, 1);
                        bromine.transform.localPosition = new Vector3(-1.5f, 1.2f, 0);

                        // completed the reaction
                        reactionPool.spawning.completedReaction = true;
                    } else if(ReferenceEquals(atom, carbon1) && c1Details.charge.currentCharge != ElectronCharge.Charge.Positive) {
                        Debug.Log("Bromine doesn't react first in this reaction");
                        StartCoroutine(NotBrFirst());
                    } else if(!ReferenceEquals(atom, carbon1)) {
                        Debug.Log("Bromine doesn't react with this carbon");
                        StartCoroutine(NotBrHere());
                    }
                }
            }
        }
    }

    private void OnDestroy() {
        Spawning.onChangeSolution -= CheckForHOOH;
        Atom.onCollision -= CheckCollision;
    }

    IEnumerator NotBrFirst() {
        collisionsAllowed = false;
        StartCoroutine(GameClock.GetComponent<GameClockTimer>().MinusPoints(20));
        instructor.text = "Bromine doesn't react first in this reaction";
        yield return new WaitForSeconds(1);
        collisionsAllowed = true;
    }

    IEnumerator NotBrHere() {
        collisionsAllowed = false;
        StartCoroutine(GameClock.GetComponent<GameClockTimer>().MinusPoints(50));
        instructor.text = "Bromine wants to react with the unstable carbon";
        yield return new WaitForSeconds(1);
        collisionsAllowed = true;
    }

    IEnumerator NotHHere() {
        collisionsAllowed = false;
        StartCoroutine(GameClock.GetComponent<GameClockTimer>().MinusPoints(50));
        instructor.text = "Hydrogen doesn't react with that carbon";
        yield return new WaitForSeconds(1);
        collisionsAllowed = true;
    }
}
