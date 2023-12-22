using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molecule : Solute {
    public enum Charge { None, Positive, Negative}
    public ElectronCharge.Charge currentCharge = ElectronCharge.Charge.None;
    [HideInInspector] public Atom ion = null;

    public void ChangeCharge(ElectronCharge.Charge newCharge, Atom newIon) {
        currentCharge = newCharge;
        switch(currentCharge) {
            case ElectronCharge.Charge.None:
                ion?.charge.RemoveCharge();
                ion = null;
                break;
            case ElectronCharge.Charge.Negative:
                ion = newIon;
                ion.charge.GiveNegativeCharge();
                break;
            case ElectronCharge.Charge.Positive:
                ion = newIon;
                ion.charge.GivePositiveCharge();
                break;
        }
    }
}
