using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hydrocarbon : Molecule {
    public enum Type { Alkane, Alkene, Alkyne, Benzene };
    public Type currentType;
    public Bond priorityBond;

    private GameObject priorityCarbon = null;
    private GameObject equalPriorityCarbon = null;
    private GameObject priorityCollision = null;
    private bool priorityHydrocarbon = true;

    private void Awake() {
        Atom.onCollision += CheckCollision;
        SolutionController.onSolutionUpdated += UpdatePriority;

        SolutionController.Instance.AddToSolution(this);
    }

    private void ChangeType(Type newType, Bond bondToChange, Bond.BondType newBondType) {
        bondToChange.ChangeBondType(newBondType);
        currentType = newType;

    }

    private void RemovePriorities() {
        priorityCarbon = null;
        equalPriorityCarbon = null;
        priorityCollision = null;
    }

    private void SetPriority(List<Solute> solution) {
        if(currentCharge != ElectronCharge.Charge.None) {
            priorityHydrocarbon = true;
        } else {
            foreach(Solute solute in solution) {
                if(solute is Hydrocarbon hydrocarbon) {
                    if(currentType == Type.Alkyne && hydrocarbon.currentType == Type.Alkene) {
                        priorityHydrocarbon = false;
                    } else {
                        priorityHydrocarbon = true;
                    }
                }
            }
        }
    }

    private void SetPriorityCarbons(bool markov = true) {
        if(priorityHydrocarbon) {
            if(currentCharge != ElectronCharge.Charge.None) {
                priorityCarbon = ion.gameObject;
                return;
            }

            GameObject atom1 = priorityBond.atom1.gameObject;
            GameObject atom2 = priorityBond.atom2.gameObject;
            float atom1BondCount = priorityBond.atom1.bondCount;
            float atom2BondCount = priorityBond.atom2.bondCount;
            if(atom1BondCount == atom2BondCount) {
                priorityCarbon = atom1;
                equalPriorityCarbon = atom2;
            } else {
                if(markov) {
                    priorityCarbon = atom1BondCount < atom2BondCount ? atom1 : atom2;
                } else {
                    priorityCarbon = atom1BondCount > atom2BondCount ? atom1 : atom2;
                }
            }
        }
    }

    private void SetPriorityCollisions(List<Solute> solution) {
        bool hasCharge = currentCharge != ElectronCharge.Charge.None;
        string hint = "";
        string name = currentType.ToString();
        foreach(Solute solute in solution) {
            if(solute is Atom atom) {
                if(!hasCharge && atom.charge.currentCharge == ElectronCharge.Charge.Positive) {
                    priorityCollision = atom.gameObject;
                    hint = $"{atom.elementName} needs electrons to become stable. The {name} in solution contains a pi bond that can donate electrons to the {atom.elementName}. Remember, ions like to attach to the least substituted carbon unless the reaction is anti-markovnikov (anti-markovnikov occurs when HBr, heat, and hydrogen peroxide are present).";
                } else if(hasCharge && atom.charge.currentCharge == ElectronCharge.Charge.Negative) {
                    priorityCollision = atom.gameObject;
                    hint = $"The {name} is short on electrons and can get some by bonding with the {atom.elementName}.";
                }
            }
            
            if(solute is Molecule molecule) {
                if(!hasCharge && molecule.currentCharge == ElectronCharge.Charge.Positive) {
                    priorityCollision = molecule.ion.gameObject;
                    hint = $"The {molecule.ion.elementName} in {molecule.name.Replace("(Clone)", "")} needs electrons to become stable. The {name} in solution contains a pi bond that can donate electrons to the {molecule.ion.elementName}.";
                } else if(hasCharge && molecule.currentCharge == ElectronCharge.Charge.Negative) {
                    priorityCollision = molecule.ion.gameObject;
                    hint = $"The {name} is short on electrons and can get some by bonding with the {molecule.name.Replace("(Clone)", "")}.";
                }
            }
        }

        if(!string.IsNullOrEmpty(hint)) {
            SolutionController.Instance.UpdateHint(hint);
        }
    }

    private void UpdatePriority(SolutionController solutionController) {
        RemovePriorities();
        List<Solute> solution = solutionController.currentlyInSolution;
        SetPriority(solution);
        SetPriorityCarbons(solutionController.markov);
        SetPriorityCollisions(solution);
    }

    private void CheckCollision(Atom atomScript, GameObject atom, GameObject collidingAtom) {
        if(collidingAtom != null) {
            if((ReferenceEquals(atom, priorityCarbon) || ReferenceEquals(atom, equalPriorityCarbon)) && ReferenceEquals(collidingAtom, priorityCollision)) {
                Atom collidingAtomDetails = collidingAtom.GetComponentInParent<Atom>();
                string collidingAtomName = collidingAtomDetails.elementName;

                if(collidingAtomName == "Hydrogen") {
                    atomScript.PlayHydrogenAddedClip();
                    ChangeType(Type.Alkane, priorityBond, Bond.BondType.Single);
                    ChangeCharge(ElectronCharge.Charge.Positive, ReferenceEquals(atom, priorityBond.atom1.gameObject) ? priorityBond.atom2 : priorityBond.atom1);
                    SolutionController.Instance.RemoveFromSolution(collidingAtomDetails, trash: true);
                    Destroy(collidingAtom);
                }

                if(atomScript.charge.currentCharge == ElectronCharge.Charge.Positive &&
                    collidingAtomDetails.charge.currentCharge == ElectronCharge.Charge.Negative) {
                        SolutionController.Instance.RemoveFromSolution(collidingAtomDetails, update: false);
                        collidingAtomDetails.charge.RemoveCharge();
                        ChangeCharge(ElectronCharge.Charge.None, null);
                        Bond newBond = priorityBond.CreateNeighboringBond(Bond.BondType.Single, gameObject, collidingAtom, atomScript, collidingAtomDetails);
                        AlkylHalide alkylHalide = gameObject.AddComponent<AlkylHalide>();
                        alkylHalide.halogenBond = newBond;
                        SolutionController.Instance.AddToSolution(alkylHalide);
                        Destroy(this);
                }
            }
        }
    }   

    private void OnDestroy() {
        SolutionController.Instance.RemoveFromSolution(this);

        SolutionController.onSolutionUpdated -= UpdatePriority;
        Atom.onCollision -= CheckCollision;
    }
}
