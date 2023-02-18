using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronCharge : MonoBehaviour {
    public GameObject negativeCharge;
    public GameObject positiveCharge;
    public enum Charge { None, Negative, Positive };
    public Charge currentCharge;

    private void Update() {
        transform.rotation = Quaternion.identity;
    }

    public void GiveNegativeCharge() {
        negativeCharge.SetActive(true);
        positiveCharge.SetActive(false);
        currentCharge = Charge.Negative;
    }
    public void GivePositiveCharge() {
        negativeCharge.SetActive(false);
        positiveCharge.SetActive(true);
        currentCharge = Charge.Positive;
    }

    public void RemoveCharge() {
        negativeCharge.SetActive(false);
        positiveCharge.SetActive(false);
        currentCharge = Charge.None;
    }
}
