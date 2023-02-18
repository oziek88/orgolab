using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomCollider : MonoBehaviour {
    public delegate void AtomColliderEventHandler(AtomCollider atomCollider, GameObject atom, GameObject collidingAtom);
    public static event AtomColliderEventHandler onCollision;

    
    void OnTriggerEnter2D(Collider2D collision2D) {
        onCollision?.Invoke(this, collision2D.gameObject, gameObject);
    }
}
