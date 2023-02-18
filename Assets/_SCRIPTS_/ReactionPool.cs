using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionPool : MonoBehaviour {
    public Spawning spawning;
    public MoleculePool moleculePool;
    public List<Reaction> reactions;

    [HideInInspector]
    public GameObject chosenReactionGO;
    private Reaction chosenReaction;


    public string StartRandomReaction() {
        chosenReaction = reactions[Random.Range(0, reactions.Count)];
        //chosenReactionGO = chosenReaction.gameObject;
        chosenReaction.enabled = true;
        //Instantiate(chosenReactionGO);
        return chosenReaction.name;
    }

    public void EndReaction() {
        chosenReaction.enabled = false;
    }
}
