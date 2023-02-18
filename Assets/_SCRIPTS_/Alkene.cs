using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alkene : MonoBehaviour {
    public List<Alkene> alkenes;

    public Alkene chosenAlkene = null;
    public GameObject dbTertiaryCarbon = null;
    public GameObject dbSecondaryCarbon = null;
    public GameObject dbPrimaryCarbon = null;

    public Alkene ChooseRandomAlkene() {
        chosenAlkene = alkenes[Random.Range(0, alkenes.Count)];
        dbTertiaryCarbon = chosenAlkene.dbTertiaryCarbon;
        dbSecondaryCarbon = chosenAlkene.dbSecondaryCarbon;
        dbPrimaryCarbon = chosenAlkene.dbPrimaryCarbon;
        return chosenAlkene;
    }
}
