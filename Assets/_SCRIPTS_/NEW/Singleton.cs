using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton:MonoBehaviour {
    private static Singleton _instance;
    public static Singleton Instance { get { return _instance; } }
    private void Awake() {
        // If there is an instance, and it's not me, delete myself.

        if(Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }
}
