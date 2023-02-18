using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour {
    public delegate void AtomColliderEventHandler(Atom atom, GameObject atomGO, GameObject collidingAtomGO);
    public static event AtomColliderEventHandler onCollision;

    public ElectronCharge charge;
    
    public float pivot; //rect.pivot.x = pivot for rotatating around carbon left of bond, (1 - pivot) to rotate around carbon right of bond
    public float bondCount;
    public bool canBond = false;

    [HideInInspector]
    public Animation anim;
    public List<string> clips = new List<string>(new string[] { });

    private void Start() {
        if(GetComponent<Animation>() != null) {
            anim = GetComponent<Animation>();
            foreach(AnimationState state in anim) {
                clips.Add(state.name);
            }
        }
    }

    public void PlayHydrogenAddedClip() {
        anim.Play(clips[0]);
    }

    public void PlayUnstableAtomClip() {
        if(anim.IsPlaying(clips[0])) {
            anim.CrossFade(clips[1]);
        } else {
            anim.Play(clips[1]);
        }
    }

    public void StopUnstableAtomClip() {
        anim.Stop(clips[1]);
    }

    void OnTriggerEnter2D(Collider2D collision2D) {
        onCollision?.Invoke(this, gameObject, collision2D.gameObject);
    }
} 
