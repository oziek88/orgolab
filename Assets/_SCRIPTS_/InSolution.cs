using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InSolution : MonoBehaviour {
    public Text solutionBeingAdded;

	void Start() {
        Spawning.onChangeSolution += changeSolution;
    }

    private void OnDestroy() {
        Spawning.onChangeSolution -= changeSolution;
    }

    public void changeSolution(Spawning spawning, string solution) {
		solutionBeingAdded.text = solution;
	}
}
