using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class reactantAddition:MonoBehaviour {

    public GameObject alkene;

    public bool collisionsAllowed = true;

    public GameObject GameClock;

    public Animation anim;
    public List<string> clips = new List<string>(new string[] { });

    private Text instructor;
    private bool inHOOH = false;

    // Use this for initialization
    void Start() {
        Spawning.onChangeSolution += checkForHOOH;

        instructor = GameObject.Find("InstructorPanel").GetComponentInChildren<Text>();
        anim = alkene.GetComponent<Animation>();
        foreach(AnimationState state in anim) {
            clips.Add(state.name);
        }
        /* Maintain This Structure in the Animation Component!
         * 
         * clips[0] = add H
         * clips[1] = unstable carbon
         * clips[2] = add Br
         * clips[3] = add H antimarkov
         * clips[4] = unstable carbon antimarkov
         * clips[5] = add Br antimarkov
         * 
         */
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnDestroy() {
        Spawning.onChangeSolution -= checkForHOOH;
    }

    public void checkForHOOH(Spawning spawning, string solution) {
        if(solution == "HOOH") {
            inHOOH = true;
        } else {
            inHOOH = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision2D) {
        // Without Hydrogen Peroxide in Solution
        if(!inHOOH) {
            if(collision2D.name == "Bromine" && collisionsAllowed) {
                if(this.gameObject.tag == "carbonBr" && anim.IsPlaying(clips[1]) ||
                    this.gameObject.tag == "carbonHBr" && anim.IsPlaying(clips[1])) {
                    anim.Stop(clips[1]);
                    anim.Play(clips[2]);

                    GameObject bromine = collision2D.gameObject;
                    Destroy(bromine.GetComponent<Rigidbody2D>());
                    //bromine.GetComponent<Animation>().Stop("unstableAtom");

                    bromine.transform.parent = alkene.transform;
                    bromine.transform.localScale = new Vector3(1, 1, 1);
                    bromine.transform.localPosition = new Vector3(2f, 0.5f, 0);

                    // completed the reaction
                    GameObject.Find("Controller").GetComponent<Spawning>().completedReaction = true;
                } else if(this.gameObject.tag == "carbonBr" && !anim.IsPlaying(clips[1])) {
                    Debug.Log("Bromine doesn't react first in this reaction");
                    StartCoroutine(NotBrFirst());
                } else if(this.gameObject.tag == "carbonBr" && anim.IsPlaying(clips[4]) ||
                           this.gameObject.tag == "carbonHBr" && anim.IsPlaying(clips[4])) {
                    anim.Stop(clips[4]);
                    anim.Play(clips[5]);

                    GameObject bromine = collision2D.gameObject;
                    Destroy(bromine.GetComponent<Rigidbody2D>());
                    //bromine.GetComponent<Animation>().Stop("unstableAtom");

                    bromine.transform.parent = alkene.transform;
                    bromine.transform.localScale = new Vector3(1, 1, 1);
                    bromine.transform.localPosition = new Vector3(-2f, -0.5f, 0);

                    // completed the reaction
                    GameObject.Find("Controller").GetComponent<Spawning>().completedReaction = true;

                } else if(this.gameObject.tag != "carbonBr") {
                    Debug.Log("Bromine doesn't react with this carbon");
                    StartCoroutine(NotBrHere());
                }
            }


            // If Carbon collides with a Hydrogen.
            if(collision2D.name == "Hydrogen" && collisionsAllowed) {
                if(this.gameObject.tag == "carbonH") {
                    anim.Play(clips[0]);
                    Destroy(collision2D.gameObject);
                    anim.CrossFade(clips[1]);
                } else if(this.gameObject.tag == "carbonHBr" && this.gameObject.name == "Carbon2") {
                    anim.Play(clips[0]);
                    Destroy(collision2D.gameObject);
                    anim.CrossFade(clips[1]);
                } else if(this.gameObject.tag == "carbonHBr" && this.gameObject.name == "Carbon1") {
                    anim.Play(clips[3]);
                    Destroy(collision2D.gameObject);
                    anim.CrossFade(clips[4]);
                } else if(this.gameObject.tag != "carbonH" && this.gameObject.tag != "carbonHBr") {
                    Debug.Log(this.gameObject.tag);
                    StartCoroutine(NotHHere());
                }

            }
        }
        // WITH Hydrogen Peroxide in Solution
        else {
            if(collision2D.name == "Bromine" && collisionsAllowed) {
                if(this.gameObject.tag == "carbonH" && anim.IsPlaying(clips[1]) ||
                    this.gameObject.tag == "carbonHBr" && anim.IsPlaying(clips[1])) {
                    anim.Stop(clips[1]);
                    anim.Play(clips[2]);

                    GameObject bromine = collision2D.gameObject;
                    Destroy(bromine.GetComponent<Rigidbody2D>());
                    //bromine.GetComponent<Animation>().Stop("unstableAtom");

                    bromine.transform.parent = alkene.transform;
                    bromine.transform.localScale = new Vector3(1, 1, 1);
                    bromine.transform.localPosition = new Vector3(2f, 0.5f, 0);

                    // completed the reaction
                    GameObject.Find("Controller").GetComponent<Spawning>().completedReaction = true;
                } else if(this.gameObject.tag == "carbonH" && !anim.IsPlaying(clips[4])) {
                    Debug.Log("Bromine doesn't react first in this reaction");
                    StartCoroutine(NotBrFirst());
                } else if(this.gameObject.tag == "carbonH" && anim.IsPlaying(clips[4]) ||
                           this.gameObject.tag == "carbonHBr" && anim.IsPlaying(clips[4])) {
                    anim.Stop(clips[4]);
                    anim.Play(clips[5]);

                    GameObject bromine = collision2D.gameObject;
                    Destroy(bromine.GetComponent<Rigidbody2D>());
                    //bromine.GetComponent<Animation>().Stop("unstableAtom");

                    bromine.transform.parent = alkene.transform;
                    bromine.transform.localScale = new Vector3(1, 1, 1);
                    bromine.transform.localPosition = new Vector3(-1.5f, 1.2f, 0);

                    // completed the reaction
                    GameObject.Find("Controller").GetComponent<Spawning>().completedReaction = true;

                } else if(this.gameObject.tag != "carbonH" && this.gameObject.tag != "carbonHBr") {
                    Debug.Log("Bromine doesn't react with this carbon");
                    StartCoroutine(NotBrHere());
                }
            }


            // If Carbon collides with a Hydrogen.
            if(collision2D.name == "Hydrogen" && collisionsAllowed) {
                if(this.gameObject.tag == "carbonBr") {
                    anim.Play(clips[3]);
                    Destroy(collision2D.gameObject);
                    anim.CrossFade(clips[4]);
                } else if(this.gameObject.tag == "carbonHBr" && this.gameObject.name == "Carbon2") {
                    anim.Play(clips[0]);
                    Destroy(collision2D.gameObject);
                    anim.CrossFade(clips[1]);
                } else if(this.gameObject.tag == "carbonHBr" && this.gameObject.name == "Carbon1") {
                    anim.Play(clips[3]);
                    Destroy(collision2D.gameObject);
                    anim.CrossFade(clips[4]);
                } else if(this.gameObject.tag != "carbonBr" && this.gameObject.tag != "carbonHBr") {
                    Debug.Log(this.gameObject.tag);
                    StartCoroutine(NotHHere());
                }

            }
        }


    }

    IEnumerator NotBrFirst() {
        collisionsAllowed = false;
        StartCoroutine(GameClock.GetComponent<GameClockTimer>().MinusPoints(20));
        instructor.text = "Bromine doesn't react first in this reaction";
        yield return new WaitForSeconds(1);
        collisionsAllowed = true;
    }

    IEnumerator NotBrHere() {
        collisionsAllowed = false;
        StartCoroutine(GameClock.GetComponent<GameClockTimer>().MinusPoints(50));
        instructor.text = "Bromine wants to react with the unstable carbon";
        yield return new WaitForSeconds(1);
        collisionsAllowed = true;
    }

    IEnumerator NotHHere() {
        collisionsAllowed = false;
        StartCoroutine(GameClock.GetComponent<GameClockTimer>().MinusPoints(50));
        instructor.text = "Hydrogen doesn't react with that carbon";
        yield return new WaitForSeconds(1);
        collisionsAllowed = true;
    }
}
