using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HBrAddition:Reaction {
    public MouseManager mouseManager;
    public ReactionPool reactionPool;
    public MoleculePool moleculePool;

    public GameObject GameClock;
    public Text instructor;
    public bool collisionsAllowed = true;

    private bool inHOOH = false;
    private Halide HBr;
    private GameObject HBrGO;
    private Bond HBrBond;
    private Alkene alkene;
    private GameObject alkeneGO;
    private Bond alkeneDoubleBond;
    private GameObject carbon1;
    private Atom c1Details;
    private GameObject carbon2;
    private Atom c2Details;
    private List<string> halides = new List<string> {"Chlorine", "Bromine"};

    private void OnEnable() {
        ReactionPool.onStartReaction += StartReaction;
        Spawning.onChangeSolution += CheckForHOOH;
        Atom.onCollision += CheckCollision;
    }

    private void OnDisable() {
        ReactionPool.onStartReaction -= StartReaction;
        Spawning.onChangeSolution -= CheckForHOOH;
        Atom.onCollision -= CheckCollision;
    }

    private void Update() {
        if(Input.touchCount == 2) {
            List<GameObject> hits = mouseManager.TwoFingerTouch();
            if(hits.Count == 0) {
                return;
            } else {
                if(ReferenceEquals(hits[0] ?? hits[1], HBrGO)) {
                    HBrBond.BondBreak(transform, atom1CanBond: true, atom2CanBond: true);
                    ;
                }
            }
        }
    }

    public void StartReaction(ReactionPool rp) {
        List<GameObject> reactants = GrabReactants();
        StartCoroutine(rp.spawning.SpawnMolecules(reactants, "HOOH"));
    }

    private List<GameObject> GrabReactants() {
        List<GameObject> reactants = new List<GameObject>();

        HBrGO = Instantiate(moleculePool.GrabHBr(), transform);
        HBr = HBrGO.GetComponent<Halide>();
        HBrBond = HBrGO.GetComponentInChildren<Bond>();
        HBrBond.BondCanBreak(true);
        reactants.Add(HBrGO);

        alkeneGO = Instantiate(moleculePool.GrabRandomAlkene(), transform);
        alkene = alkeneGO.GetComponent<Alkene>();
        reactants.Add(alkeneGO);

        carbon1 = alkene.dbPrimaryCarbon == null ? alkene.dbSecondaryCarbon : alkene.dbPrimaryCarbon;
        c1Details = carbon1.GetComponent<Atom>();
        c1Details.ToggleBonding(true);
        carbon2 = alkene.dbTertiaryCarbon == null ? alkene.dbSecondaryCarbon : alkene.dbTertiaryCarbon;
        c2Details = carbon2.GetComponent<Atom>();
        c2Details.ToggleBonding(true);

        foreach(Transform child in alkeneGO.transform) {
            if(child.GetComponent<Bond>()?.currentBondType == Bond.BondType.Double) {
                alkeneDoubleBond = child.GetComponent<Bond>();
            }
        }

        HBrGO.SetActive(false);
        alkeneGO.SetActive(false);

        return reactants;
    }

    public void CheckForHOOH(Spawning spawning, string solution) {
        if(solution == "HOOH") {
            inHOOH = true;
        } else {
            inHOOH = false;
        }
    }

    public void CheckCollision(Atom atomScript, GameObject atom, GameObject collidingAtom) {
        if(collisionsAllowed && collidingAtom.GetComponent<Atom>() != null) {
            Atom collidingAtomDetails = collidingAtom.GetComponent<Atom>();

            // Without Hydrogen Peroxide in Solution
            if(!inHOOH) {
                // If Hydrogen collides with alkene
                if(collidingAtomDetails.elementName == "Hydrogen" && collidingAtomDetails.CanBond()) {
                    if(ReferenceEquals(atom, carbon1)) {
                        c1Details.PlayHydrogenAddedClip();
                        Destroy(collidingAtom);
                        alkeneDoubleBond.ChangeBondType(Bond.BondType.Single);
                        c2Details.PlayUnstableAtomClip();
                        c2Details.charge.GivePositiveCharge();
                    }
                }

                // If Bromine collides with alkene
                if(halides.Contains(collidingAtomDetails.elementName) && collidingAtomDetails.CanBond()) {
                    if(ReferenceEquals(atom, carbon2) && c2Details.charge.currentCharge == ElectronCharge.Charge.Positive) {
                        c2Details.StopUnstableAtomClip();
                        c2Details.charge.RemoveCharge();
                        alkeneDoubleBond.CreateNeighboringBond(Bond.BondType.Single, alkeneGO, collidingAtom, c2Details, collidingAtomDetails);

                        // completed the reaction
                        reactionPool.EndReaction();
                    } else if(ReferenceEquals(atom, carbon2) && c2Details.charge.currentCharge != ElectronCharge.Charge.Positive) {
                        Debug.Log("Bromine doesn't react first in this reaction");
                        StartCoroutine(NotBrFirst());
                    } else if(!ReferenceEquals(atom, carbon2)) {
                        Debug.Log("Bromine doesn't react with this carbon");
                        StartCoroutine(NotBrHere());
                    }
                }
            } else {
                // WITH Hydrogen Peroxide in Solution
                // If Hydrogen collides with alkene
                if(collidingAtomDetails.elementName == "Hydrogen" && collidingAtomDetails.CanBond()) {
                    if(ReferenceEquals(atom, carbon2)) {
                        c2Details.PlayHydrogenAddedClip();
                        Destroy(collidingAtom);
                        alkeneDoubleBond.ChangeBondType(Bond.BondType.Single);
                        c1Details.PlayUnstableAtomClip();
                        c1Details.charge.GivePositiveCharge();
                    }
                }

                // If Bromine collides with alkene
                if(halides.Contains(collidingAtomDetails.elementName) && collidingAtomDetails.CanBond()) {
                    if(ReferenceEquals(atom, carbon1) && c1Details.charge.currentCharge == ElectronCharge.Charge.Positive) {
                        c1Details.StopUnstableAtomClip();
                        c1Details.charge.RemoveCharge();
                        alkeneDoubleBond.CreateNeighboringBond(Bond.BondType.Single, alkeneGO, collidingAtom, c1Details, collidingAtomDetails);

                        // completed the reaction
                        reactionPool.EndReaction();
                    } else if(ReferenceEquals(atom, carbon1) && c1Details.charge.currentCharge != ElectronCharge.Charge.Positive) {
                        Debug.Log("Bromine doesn't react first in this reaction");
                        StartCoroutine(NotBrFirst());
                    } else if(!ReferenceEquals(atom, carbon1)) {
                        Debug.Log("Bromine doesn't react with this carbon");
                        StartCoroutine(NotBrHere());
                    }
                }
            }
        }
    }

    IEnumerator NotBrFirst() {
        collisionsAllowed = false;
        StartCoroutine(GameClock.GetComponent<GameClockTimer>().MinusPoints(5));
        instructor.text = "Bromine doesn't react first in this reaction";
        yield return new WaitForSeconds(1);
        collisionsAllowed = true;
    }

    IEnumerator NotBrHere() {
        collisionsAllowed = false;
        StartCoroutine(GameClock.GetComponent<GameClockTimer>().MinusPoints(10));
        instructor.text = "Bromine wants to react with the unstable carbon";
        yield return new WaitForSeconds(1);
        collisionsAllowed = true;
    }

    IEnumerator NotHHere() {
        collisionsAllowed = false;
        StartCoroutine(GameClock.GetComponent<GameClockTimer>().MinusPoints(10));
        instructor.text = "Hydrogen doesn't react with that carbon";
        yield return new WaitForSeconds(1);
        collisionsAllowed = true;
    }
}
