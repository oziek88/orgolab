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

    private Vector2 randomSpawnVelocity;
    private float randomSpawnRotation;

    public GameObject GameClock;

    public GameObject productParent;

    public List<GameObject> ReactionGroups;
    public List<GameObject> alkenes;

    public Text instructorText;

    private string[] instructions = new string[3]; // [0] = HBr Addition, [1] = with propene, [2] = in Hydrogen Peroxide


    void Start() {
        // Initiallize first spawn
        StartCoroutine(SpawnReactants());
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

    IEnumerator SpawnReactants() {
        // Remove roof
        roof.SetActive(false);

        GameObject ReactionGroupsTank = GameObject.Find("ReactionGroups");

        //foreach(Transform ReactionGroup in ReactionGroupsTank.transform) {
        //    ReactionGroups.Add(ReactionGroup.gameObject);
        //}

        GameObject Reactants = ReactionGroups[Random.Range(0, ReactionGroups.Count)];
        instructions[0] = "Solve the " + Reactants.name;

        if(Reactants.name == "HBrAddition") {
            int i = Random.Range(0, 3);
            if(i == 0) {
                onChangeSolution?.Invoke(this, "HOOH");
                instructions[2] = " in Hydrogen Peroxide";
            }
        }

        foreach(Transform reactant in Reactants.transform) {
            yield return new WaitForSeconds(1);

            randomSpawnVelocity = new Vector2(Random.Range(-3f, 3f), Random.Range(-14f, -6f));
            randomSpawnRotation = Random.Range(-120f, 120f);

            if(reactant.name == "alkenes") {
                foreach(Transform alkene in reactant.transform) {
                    alkenes.Add(alkene.gameObject);
                }

                GameObject chosenAlkene = alkenes[Random.Range(0, alkenes.Count)];
                instructions[1] = " with " + chosenAlkene.name;

                if(chosenAlkene.CompareTag("ProductParent")) {
                    productParent = Instantiate(chosenAlkene.gameObject, transform.position, transform.rotation);
                    productParent.GetComponent<Rigidbody2D>().velocity = randomSpawnVelocity;
                    productParent.GetComponent<Rigidbody2D>().angularVelocity = randomSpawnRotation;
                }
            } else {
                GameObject molecule = Instantiate(reactant.gameObject, transform.position, transform.rotation);
                molecule.GetComponent<Rigidbody2D>().velocity = randomSpawnVelocity;
                molecule.GetComponent<Rigidbody2D>().angularVelocity = randomSpawnRotation;
            }

            /*
            if (reactant.CompareTag("ProductParent"))
            {
                productParent = Instantiate(reactant.gameObject, transform.position, transform.rotation);
                productParent.GetComponent<Rigidbody2D>().velocity = randomSpawnVelocity;
                productParent.GetComponent<Rigidbody2D>().angularVelocity = randomSpawnRotation;
            }
            else
            {
                GameObject molecule = Instantiate(reactant.gameObject, transform.position, transform.rotation);
                molecule.GetComponent<Rigidbody2D>().velocity = randomSpawnVelocity;
                molecule.GetComponent<Rigidbody2D>().angularVelocity = randomSpawnRotation;
            }
            */

        }
        instructorText.text = string.Join("", instructions);

        yield return new WaitForSeconds(1);

        GameClock.GetComponent<GameClockTimer>().runTimer = true;

        // Attach roof
        roof.SetActive(true);
    }

    void DestroyProduct() {
        Destroy(productParent);
        onChangeSolution?.Invoke(this);
        instructions = new string[3];
    }
}
