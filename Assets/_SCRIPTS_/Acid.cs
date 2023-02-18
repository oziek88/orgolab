using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acid : MonoBehaviour {
    public List<Acid> acids;

    [HideInInspector]
    public Acid chosenAcid = null;

    public Acid ChooseRandomAcid() {
        chosenAcid = acids[Random.Range(0, acids.Count)];
        return chosenAcid;
    }

    public Acid ChooseHBr() {
        Acid hBr = acids.Find(acid => acid.name == "HydrogenBromide");
        return hBr;
    }
}
