using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acid : MonoBehaviour {
    public List<Acid> acids;

    public List<GameObject> separatingHydrogens;
    public List<Bond> hydrogenBonds;

    [HideInInspector]
    public Acid chosenAcid = null;

    public Acid ChooseRandomAcid() {
        chosenAcid = acids[Random.Range(0, acids.Count)];
        return chosenAcid;
    }
}
