using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidCatalyzedHydration:Reaction {
    public MouseManager mouseManager;
    public ReactionPool reactionPool;
    public MoleculePool moleculePool;

    private Acid acid;
    private GameObject acidGO;
    private Alcohol alcohol;
    private GameObject alcoholGO;

    private Alkene alkene;
    private GameObject alkeneGO;
    private Bond alkeneDoubleBond;
    private GameObject carbon1;
    private Atom c1Details;
    private GameObject carbon2;
    private Atom c2Details;

    private void OnEnable() {
        ReactionPool.onStartReaction += StartReaction;
        Atom.onCollision += CheckCollision;
    }

    private void OnDisable() {
        ReactionPool.onStartReaction -= StartReaction;
        Atom.onCollision -= CheckCollision;
    }

    void Update() {
        if(Input.touchCount == 2) {
            List<GameObject> hits = mouseManager.TwoFingerTouch();
            if(hits.Count == 0) {
                return;
            } else {
                if(ReferenceEquals(hits[0] ?? hits[1], acid.separatingHydrogens[0]) || ReferenceEquals(hits[0] ?? hits[1], acid.hydrogenBonds[0])) {
                    acid.hydrogenBonds[0].BondBreak(transform, atom1CanBond: true, atom2CanBond: true, atom2Charge: "-");
                    if(acid.separatingHydrogens.Count > 1) {
                        acid.hydrogenBonds[1].BondCanBreak(false);
                    }
                } else if(acid.separatingHydrogens.Count > 1 && (ReferenceEquals(hits[0] ?? hits[1], acid.separatingHydrogens[1]) || ReferenceEquals(hits[0] ?? hits[1], acid.hydrogenBonds[1]))) {
                    acid.hydrogenBonds[1].BondBreak(transform, atom1CanBond: true, atom2CanBond: true, atom2Charge: "-");
                    acid.hydrogenBonds[0].BondCanBreak(false);
                }
            }
        }
    }

    public void StartReaction(ReactionPool rp) {
        List<GameObject> reactants = GrabReactants();
        StartCoroutine(rp.spawning.SpawnMolecules(reactants));
    }

    private List<GameObject> GrabReactants() {
        List<GameObject> reactants = new List<GameObject>();

        acidGO = Instantiate(moleculePool.GrabRandomAcid(), transform);
        acid = acidGO.GetComponent<Acid>();
        foreach(Bond bond in acid.hydrogenBonds) {
            bond.BondCanBreak(true);
        }
        reactants.Add(acidGO);

        alcoholGO = Instantiate(moleculePool.GrabRandomAlcohol(), transform);
        alcohol = alcoholGO.GetComponent<Alcohol>();
        reactants.Add(alcoholGO);

        alkeneGO = Instantiate(moleculePool.GrabRandomAlkene(), transform);
        alkene = alkeneGO.GetComponent<Alkene>();
        reactants.Add(alkeneGO);

        carbon1 = alkene.dbPrimaryCarbon == null ? alkene.dbSecondaryCarbon : alkene.dbPrimaryCarbon;
        c1Details = carbon1.GetComponent<Atom>();
        carbon2 = alkene.dbTertiaryCarbon == null ? alkene.dbSecondaryCarbon : alkene.dbTertiaryCarbon;
        c2Details = carbon2.GetComponent<Atom>();

        foreach(Transform child in alkeneGO.transform) {
            if(child.GetComponent<Bond>()?.currentBondType == Bond.BondType.Double) {
                alkeneDoubleBond = child.GetComponent<Bond>();
            }
        }

        acidGO.SetActive(false);
        alcoholGO.SetActive(false);
        alkeneGO.SetActive(false);

        return reactants;
    }

    public void CheckCollision(Atom atomScript, GameObject atom, GameObject collidingAtom) {
        //if(collisionsAllowed && collidingAtom.GetComponent<Atom>() != null) {
        //    Atom collidingAtomDetails = collidingAtom.GetComponent<Atom>();

        //    // Without Hydrogen Peroxide in Solution
        //    if(!inHOOH) {
        //        // If Hydrogen collides with alkene
        //        if(collidingAtomDetails.elementName == "Hydrogen" && collidingAtomDetails.CanBond()) {
        //            if(ReferenceEquals(atom, carbon1)) {
        //                c1Details.PlayHydrogenAddedClip();
        //                Destroy(collidingAtom);
        //                alkeneDoubleBond.ChangeBondType(Bond.BondType.Single);
        //                c2Details.PlayUnstableAtomClip();
        //                c2Details.charge.GivePositiveCharge();
        //            }
        //        }

        //        // If Bromine collides with alkene
        //        if(halides.Contains(collidingAtomDetails.elementName) && collidingAtomDetails.CanBond()) {
        //            if(ReferenceEquals(atom, carbon2) && c2Details.charge.currentCharge == ElectronCharge.Charge.Positive) {
        //                c2Details.StopUnstableAtomClip();
        //                c2Details.charge.RemoveCharge();
        //                alkeneDoubleBond.CreateNeighboringBond(Bond.BondType.Single, alkeneGO, collidingAtom, c2Details, collidingAtomDetails);

        //                // completed the reaction
        //                reactionPool.EndReaction();
        //            } else if(ReferenceEquals(atom, carbon2) && c2Details.charge.currentCharge != ElectronCharge.Charge.Positive) {
        //                Debug.Log("Bromine doesn't react first in this reaction");
        //                StartCoroutine(NotBrFirst());
        //            } else if(!ReferenceEquals(atom, carbon2)) {
        //                Debug.Log("Bromine doesn't react with this carbon");
        //                StartCoroutine(NotBrHere());
        //            }
        //        }
        //    } else {
        //        // WITH Hydrogen Peroxide in Solution
        //        // If Hydrogen collides with alkene
        //        if(collidingAtomDetails.elementName == "Hydrogen" && collidingAtomDetails.CanBond()) {
        //            if(ReferenceEquals(atom, carbon2)) {
        //                c2Details.PlayHydrogenAddedClip();
        //                Destroy(collidingAtom);
        //                alkeneDoubleBond.ChangeBondType(Bond.BondType.Single);
        //                c1Details.PlayUnstableAtomClip();
        //                c1Details.charge.GivePositiveCharge();
        //            }
        //        }

        //        // If Bromine collides with alkene
        //        if(halides.Contains(collidingAtomDetails.elementName) && collidingAtomDetails.CanBond()) {
        //            if(ReferenceEquals(atom, carbon1) && c1Details.charge.currentCharge == ElectronCharge.Charge.Positive) {
        //                c1Details.StopUnstableAtomClip();
        //                c1Details.charge.RemoveCharge();
        //                alkeneDoubleBond.CreateNeighboringBond(Bond.BondType.Single, alkeneGO, collidingAtom, c1Details, collidingAtomDetails);

        //                // completed the reaction
        //                reactionPool.EndReaction();
        //            } else if(ReferenceEquals(atom, carbon1) && c1Details.charge.currentCharge != ElectronCharge.Charge.Positive) {
        //                Debug.Log("Bromine doesn't react first in this reaction");
        //                StartCoroutine(NotBrFirst());
        //            } else if(!ReferenceEquals(atom, carbon1)) {
        //                Debug.Log("Bromine doesn't react with this carbon");
        //                StartCoroutine(NotBrHere());
        //            }
        //        }
        //    }
        //}
    }
}
