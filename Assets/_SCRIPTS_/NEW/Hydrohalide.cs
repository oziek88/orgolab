using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hydrohalide : Molecule {
    public enum Halogen { Floride, Chloride, Bromide, Iodine };
    public Halogen attachedHalogen;

    public Bond hXBond = null;

    private bool hasPriorityToBreak = true;
    private Dictionary<Halogen, int> bondBreakPriority = new Dictionary<Halogen, int>() {
        { Halogen.Floride, 4 },
        { Halogen.Chloride, 3 },
        { Halogen.Bromide, 2 },
        { Halogen.Iodine, 1 } 
    };


    private void Awake() {
        MouseManager.onTwoFingerTouch += TwoFingerTouch;
        SolutionController.onSolutionUpdated += UpdatePriority;

        SolutionController.Instance.AddToSolution(this);
    }

    private void UpdatePriority(SolutionController solutionController) {
        bool containsReactant = false;
        foreach(Solute solute in solutionController.currentlyInSolution) {
            if(solute is Hydrocarbon hydrocarbon) {
                if(hydrocarbon.currentType == Hydrocarbon.Type.Alkene || hydrocarbon.currentType == Hydrocarbon.Type.Alkyne) {
                    containsReactant = true;
                }
            }
        }

        if(!containsReactant) {
            hasPriorityToBreak = false;
            return;
        }

        string hint = "";
        int count = 0;
        foreach(Solute solute in solutionController.currentlyInSolution) {
            if(solute is Hydrohalide hydrohalide) {
                count += 1;
                int halidePriority = bondBreakPriority[hydrohalide.attachedHalogen];
                if(halidePriority < bondBreakPriority[attachedHalogen]) {
                    hasPriorityToBreak = false;
                    break;
                } else {
                    hasPriorityToBreak = true;
                    hint = "Start off by breaking the bond of a hydrohalide.";
                }
            }
        }

        if(count > 1) {
            hint += $" In this case, {this.name.Replace("(Clone)", "")} reacts more readily because it is a stronger acid. This is due to its bond length being longer than the other hydrohalides in solution.";
        }
        
        if(!string.IsNullOrEmpty(hint) && !SolutionController.Instance.ionsInSolution) {
            solutionController.UpdateHint(hint);
        }
    }

    private void TwoFingerTouch(MouseManager mm, List<GameObject> hits) {
        if(hits.Count == 0) {
            return;
        } else {
            if(ReferenceEquals(hits[0] ?? hits[1], hXBond.atom1.gameObject) || ReferenceEquals(hits[0] ?? hits[1], hXBond.atom2.gameObject)) {
                if(hasPriorityToBreak) {
                    hXBond.BondBreak(transform.parent, atom1CanBond: true, "+", atom2CanBond: true, "-");
                }
            }
        }
    }

    private void OnDestroy() {
        SolutionController.Instance.RemoveFromSolution(this);

        MouseManager.onTwoFingerTouch -= TwoFingerTouch;
        SolutionController.onSolutionUpdated -= UpdatePriority;
    }
}
