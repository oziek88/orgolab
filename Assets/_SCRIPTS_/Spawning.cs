using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawning:MonoBehaviour {
    public delegate void SpawningEventHandler(Spawning spawning, string solution = "");
    public static event SpawningEventHandler onChangeSolution;

    public GameObject roof;

    public bool completedReaction = false;

    private int delayTimer;
    public int deathDelay = 90;
    public int spawnDelay = 100;

    public GameObject GameClock;

    public GameObject productParent;

    public ReactionPool reactionPool;



    public List<GameObject> alkenes;

    public Text instructorText;

    private string[] instructions = new string[3]; // [0] = HBr Addition, [1] = with propene, [2] = in Hydrogen Peroxide


    void Start() {
        // Initiallize first spawn
        //StartCoroutine(SpawnReactants());

        InitiallizeSpawn();

    }

    void Update() {
        if(completedReaction == true && GameClock.GetComponent<GameClockTimer>().Countdown > 0) {
            // Begin delayTimer
            delayTimer += 1;

            if(delayTimer == deathDelay) {
                // Delete productParent
                DestroyProduct();
            }

            if(delayTimer == spawnDelay) {
                // Spawn new reactants
                StartCoroutine(SpawnReactants());

                // Set completedReaction back to false
                completedReaction = false;

                // Reset delayTimer
                delayTimer = 0;
            }
        }
    }

    public void InitiallizeSpawn() {
        string reactionName = reactionPool.StartRandomReaction();
        instructions[0] = "Solve the " + reactionName;

        if(reactionName == "HBrAddition") {
            int i = Random.Range(0, 3);
            if(i == 0) {
                onChangeSolution?.Invoke(this, "HOOH");
                instructions[2] = " in Hydrogen Peroxide";
            }
        }
    }

    public IEnumerator SpawnMolecules(List<GameObject> molecules) {
        foreach(GameObject molecule in molecules) {
            StartCoroutine(Spawn(molecule));
            yield return new WaitForSeconds(1);
        }
    }

    public IEnumerator Spawn(GameObject molecule) {
        // Remove roof
        roof.SetActive(false);
        molecule.SetActive(true);

        Vector2 randomSpawnVelocity = new Vector2(Random.Range(-3f, 3f), Random.Range(-14f, -6f));
        float randomSpawnRotation = Random.Range(-120f, 120f);

        molecule.GetComponent<Rigidbody2D>().velocity = randomSpawnVelocity;
        molecule.GetComponent<Rigidbody2D>().angularVelocity = randomSpawnRotation;

        yield return new WaitForSeconds(1);

        // Attach roof
        roof.SetActive(true);
    }

    IEnumerator SpawnReactants() {
        //// Remove roof
        //roof.SetActive(false);

        //GameObject ReactionGroupsTank = GameObject.Find("ReactionGroups");

        ////foreach(Transform ReactionGroup in ReactionGroupsTank.transform) {
        ////    ReactionGroups.Add(ReactionGroup.gameObject);
        ////}

        //GameObject Reactants = reactionPool.reactions[Random.Range(0, reactionPool.reactions.Count)];
        //instructions[0] = "Solve the " + Reactants.name;

        //if(Reactants.name == "HBrAddition") {
        //    int i = Random.Range(0, 3);
        //    if(i == 0) {
        //        onChangeSolution?.Invoke(this, "HOOH");
        //        instructions[2] = " in Hydrogen Peroxide";
        //    }
        //}

        //foreach(Transform reactant in Reactants.transform) {
        //    yield return new WaitForSeconds(1);

        //    randomSpawnVelocity = new Vector2(Random.Range(-3f, 3f), Random.Range(-14f, -6f));
        //    randomSpawnRotation = Random.Range(-120f, 120f);

        //    if(reactant.name == "alkenes") {
        //        foreach(Transform alkene in reactant.transform) {
        //            alkenes.Add(alkene.gameObject);
        //        }

        //        GameObject chosenAlkene = alkenes[Random.Range(0, alkenes.Count)];
        //        instructions[1] = " with " + chosenAlkene.name;

        //        if(chosenAlkene.CompareTag("ProductParent")) {
        //            productParent = Instantiate(chosenAlkene.gameObject, transform.position, transform.rotation);
        //            productParent.GetComponent<Rigidbody2D>().velocity = randomSpawnVelocity;
        //            productParent.GetComponent<Rigidbody2D>().angularVelocity = randomSpawnRotation;
        //        }
        //    } else {
        //        GameObject molecule = Instantiate(reactant.gameObject, transform.position, transform.rotation);
        //        molecule.GetComponent<Rigidbody2D>().velocity = randomSpawnVelocity;
        //        molecule.GetComponent<Rigidbody2D>().angularVelocity = randomSpawnRotation;
        //    }

        //    /*
        //    if (reactant.CompareTag("ProductParent"))
        //    {
        //        productParent = Instantiate(reactant.gameObject, transform.position, transform.rotation);
        //        productParent.GetComponent<Rigidbody2D>().velocity = randomSpawnVelocity;
        //        productParent.GetComponent<Rigidbody2D>().angularVelocity = randomSpawnRotation;
        //    }
        //    else
        //    {
        //        GameObject molecule = Instantiate(reactant.gameObject, transform.position, transform.rotation);
        //        molecule.GetComponent<Rigidbody2D>().velocity = randomSpawnVelocity;
        //        molecule.GetComponent<Rigidbody2D>().angularVelocity = randomSpawnRotation;
        //    }
        //    */

        //}
        //instructorText.text = string.Join("", instructions);

        yield return new WaitForSeconds(1);

        //GameClock.GetComponent<GameClockTimer>().runTimer = true;

        //// Attach roof
        //roof.SetActive(true);
    }

    void DestroyProduct() {
        Destroy(productParent);
        onChangeSolution?.Invoke(this);
        instructions = new string[3];
    }
}
