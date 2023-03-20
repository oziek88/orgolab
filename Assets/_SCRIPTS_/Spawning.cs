using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawning:MonoBehaviour {
    public delegate void SpawningEventHandler(Spawning spawning, string solution = "");
    public static event SpawningEventHandler onChangeSolution;

    public ReactionPool reactionPool;
    public GameClockTimer gameClock;
    public GameObject roof;

    public Text instructorText;
    private string[] instructions = new string[3]; // [0] = HBr Addition, [1] = with propene, [2] = in Hydrogen Peroxide

    void Start() {
        InitiallizeSpawn();
    }

    public void InitiallizeSpawn() {
        string reactionName = reactionPool.StartRandomReaction();
        instructions[0] = "Solve the " + reactionName;
        gameClock.StartCountdown();
    }

    public IEnumerator SpawnMolecules(List<GameObject> molecules, string solution = "") {
        int i = Random.Range(0, 3);
        if(i == 0) {
            onChangeSolution?.Invoke(this, solution);
            instructions[2] = "in " + solution;
        } else {
            onChangeSolution?.Invoke(this);
        }

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
}
