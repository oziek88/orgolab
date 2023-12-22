using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alcohol : MonoBehaviour {
    public List<Alcohol> alcohols;

    [HideInInspector]
    public Alcohol chosenAlcohol = null;

    public Alcohol ChooseRandomAlcohol() {
        chosenAlcohol = alcohols[Random.Range(0, alcohols.Count)];
        return chosenAlcohol;
    }
}
