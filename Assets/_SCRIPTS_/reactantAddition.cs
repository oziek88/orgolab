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
        //Spawning.onChangeSolution += checkForHOOH;

        //instructor = GameObject.Find("InstructorPanel").GetComponentInChildren<Text>();
        //anim = alkene.GetComponent<Animation>();
        //foreach(AnimationState state in anim) {
        //    clips.Add(state.name);
        //}
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
