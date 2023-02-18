using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculePool:MonoBehaviour {
    public Alkene alkene;
    public Alkyne alkyne;
    public Acid acid;
    public Base base_;

    private List<GameObject> chosenMolecules = new List<GameObject>();

    public GameObject GrabRandomAlkene() {
        return alkene.ChooseRandomAlkene().gameObject;
    }

    public GameObject GrabHBr() {
        return acid.ChooseHBr().gameObject;
    }

    public void AssignMolecules(bool isAlkene = false, bool isAlkyne = false, bool isAcid = false, bool isBase_ = false) {
        if(isAlkene) {
            chosenMolecules.Add(alkene.ChooseRandomAlkene().gameObject);
        }
    }
}
