using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionPool : MonoBehaviour {
    public delegate void ReactionEventHandler(ReactionPool reactionPool);
    public static event ReactionEventHandler onStartReaction;

    public Spawning spawning;
    public MoleculePool moleculePool;
    public List<Reaction> reactions;

    [HideInInspector]
    public GameObject chosenReactionGO;
    private Reaction chosenReaction;


    public string StartRandomReaction() {
        chosenReaction = reactions[Random.Range(1, reactions.Count)];
        chosenReaction.enabled = true;
        onStartReaction?.Invoke(this);
        return chosenReaction.name;
    }

    public void EndReaction() {
        StartCoroutine(AwardPoints());
    }

    public IEnumerator AwardPoints() {
        spawning.gameClock.AwardPoints();
        yield return new WaitForSeconds(2);
        ResetPool();
    }

    private void ResetPool() {
        foreach(Transform molecule in chosenReaction.transform) {
            Destroy(molecule.gameObject);
        }
        chosenReaction.enabled = false;
        spawning.InitiallizeSpawn();
    }
}
