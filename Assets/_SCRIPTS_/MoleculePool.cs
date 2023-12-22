using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculePool:MonoBehaviour {
    public Alkene alkene;
    public Alkyne alkyne;
    public Acid acid;
    public Base base_;
    public Halide halide;
    public Alcohol alcohol;

    private List<GameObject> chosenMolecules = new List<GameObject>();

    public List<Hydrocarbon> alkenes;
    public List<Hydrocarbon> alkynes;
    public List<Hydrohalide> hydrohalides;
    public List<Alcohol> alcohols;

    public GameObject GrabRandomAlkene() {
        return alkene.ChooseRandomAlkene().gameObject;
    }

    public Hydrocarbon ChooseRandomAlkene() {
        return alkenes[Random.Range(0, alkenes.Count)];
    }

    public Hydrocarbon ChooseRandomAlkyne() {
        return alkynes[Random.Range(0, alkynes.Count)];
    }

    public Hydrohalide ChooseRandomHydrohalide() {
        return hydrohalides[Random.Range(0, hydrohalides.Count)];
    }

    public Hydrohalide ChooseHydrogenBromide() {
        Hydrohalide hBr = hydrohalides.Find(x => x.attachedHalogen == Hydrohalide.Halogen.Bromide);
        return hBr;
    }

    public GameObject GrabRandomAlcohol() {
        return alcohol.ChooseRandomAlcohol().gameObject;
    }

    public GameObject GrabRandomAcid() {
        return acid.ChooseRandomAcid().gameObject;
    }

    public GameObject GrabHBr() {
        return halide.ChooseHBr().gameObject;
    }

    public void AssignMolecules(bool isAlkene = false, bool isAlkyne = false, bool isAcid = false, bool isBase_ = false) {
        if(isAlkene) {
            chosenMolecules.Add(alkene.ChooseRandomAlkene().gameObject);
        }
    }
}
