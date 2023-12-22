using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Halide : MonoBehaviour {
    public List<Halide> halides;

    public Halide ChooseHBr() {
        Halide hBr = halides.Find(halide => halide.name == "HydrogenBromide");
        return hBr;
    }
}
