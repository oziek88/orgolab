using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Alkene : MonoBehaviour {
    public List<Alkene> alkenes;

    public Alkene chosenAlkene = null;
    public GameObject dbTertiaryCarbon = null;
    public GameObject dbSecondaryCarbon = null;
    public GameObject dbPrimaryCarbon = null;
    public Bond doubleBond = null;

    [HideInInspector]
    public GameObject ion = null;
    public GameObject priorityCollision = null;

    private void Start() {

    }
    public Alkene ChooseRandomAlkene() {
        chosenAlkene = alkenes[Random.Range(0, alkenes.Count)];
        dbTertiaryCarbon = chosenAlkene.dbTertiaryCarbon;
        dbSecondaryCarbon = chosenAlkene.dbSecondaryCarbon;
        dbPrimaryCarbon = chosenAlkene.dbPrimaryCarbon;

        foreach(Transform child in transform) {
            if(child.GetComponent<Bond>()?.currentBondType == Bond.BondType.Double) {
                doubleBond = child.GetComponent<Bond>();
            }
        }

        return chosenAlkene;
    }

    

    public void CheckCollision(Atom atomScript, GameObject atom, GameObject collidingAtom, bool markovnikov = false) {
        if(collidingAtom.GetComponent<Atom>() != null) {
            Atom collidingAtomDetails = collidingAtom.GetComponent<Atom>();
            if(!collidingAtomDetails.CanBond()) {
                return;
            }

            string collidingAtomName = collidingAtomDetails.elementName;

            Atom reactiveCarbon;
            Atom tangentCarbon;
            if(markovnikov) {
                reactiveCarbon = dbPrimaryCarbon != null ? dbPrimaryCarbon.GetComponent<Atom>() : dbSecondaryCarbon.GetComponent<Atom>();
                tangentCarbon = dbTertiaryCarbon != null ? dbTertiaryCarbon.GetComponent<Atom>() : dbSecondaryCarbon.GetComponent<Atom>();
            } else {
                reactiveCarbon = dbTertiaryCarbon != null ? dbTertiaryCarbon.GetComponent<Atom>() : dbSecondaryCarbon.GetComponent<Atom>();
                tangentCarbon = dbPrimaryCarbon != null ? dbPrimaryCarbon.GetComponent<Atom>() : dbSecondaryCarbon.GetComponent<Atom>();
            }

            bool tangentHasCharge = tangentCarbon.charge.currentCharge != ElectronCharge.Charge.None;

            if(collidingAtomDetails.CanBond()) {
                if(tangentHasCharge) {
                    tangentCarbon.StopUnstableAtomClip();
                    tangentCarbon.charge.RemoveCharge();
                    doubleBond.CreateNeighboringBond(Bond.BondType.Single, gameObject, collidingAtom, tangentCarbon, collidingAtomDetails);
                } else {
                    if(collidingAtomName == "Hydrogen") {
                        if(ReferenceEquals(atom, reactiveCarbon)) {
                            collidingAtomDetails.PlayHydrogenAddedClip();
                            Destroy(collidingAtom);
                            doubleBond.ChangeBondType(Bond.BondType.Single);
                            tangentCarbon.PlayUnstableAtomClip();
                            tangentCarbon.charge.GivePositiveCharge();
                        }
                    }
                }
            }
        }
    }
}
