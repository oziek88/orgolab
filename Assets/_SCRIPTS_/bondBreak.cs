using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bondBreak:MonoBehaviour {

    public GameObject HBr;

    private Text instructor;
    private GameClockTimer gameClock;
    private int currentStep = 0;

    // Use this for initialization
    void Start() {
        instructor = GameObject.Find("InstructorPanel").GetComponentInChildren<Text>();
        gameClock = GameObject.Find("GameClock").GetComponent<GameClockTimer>();
    }

    // Update is called once per frame
    void Update() {
        if(currentStep == 0 && gameClock.Countdown < (gameClock.startTime - 300)) {
            instructor.text = "Use two fingers to tap the Hydrogen Bromine molecule and break the bond";
            currentStep = 1;
        } else if(currentStep == 2 && gameClock.Countdown < (gameClock.startTime - 400)) {
            instructor.text = "Try attaching the hydrogen to one of the carbons in the double bond";
            currentStep = 3;
        }
        Debug.Log(currentStep);

        if(Input.touchCount == 2) {

            // Store both touches.
            Touch touchOne = Input.GetTouch(0);
            Touch touchTwo = Input.GetTouch(1);
            // Get World Position of both touches
            Vector3 touchOneWorldPos3D = Camera.main.ScreenToWorldPoint(touchOne.position);
            Vector3 touchTwoWorldPos3D = Camera.main.ScreenToWorldPoint(touchTwo.position);
            // Convert touches to 2D
            Vector2 touchOne2D = new Vector2(touchOneWorldPos3D.x, touchOneWorldPos3D.y);
            Vector2 touchTwo2D = new Vector2(touchTwoWorldPos3D.x, touchTwoWorldPos3D.y);

            Vector2 dir = Vector2.zero;

            // Send out ray
            RaycastHit2D hit1 = Physics2D.Raycast(touchOne2D, dir);
            RaycastHit2D hit2 = Physics2D.Raycast(touchTwo2D, dir);

            // This is to break bonds when fingers are spread apart while touch molecule.
            // Currently molecule bond is broken by a two finger tap.
            /*
			// Find the position in the previous frame of each touch.
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            Vector2 touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchOnePrevPos - touchTwoPrevPos).magnitude;
            float touchDeltaMag = (touchOne.position - touchTwo.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            */

            if(this.gameObject.transform.GetChild(2).gameObject) {

                // Did both touches connect with a collider
                if(hit1.collider != null || hit2.collider != null) {

                    // Is the collider the selected molecule
                    if((hit1.transform.gameObject == HBr || hit2.transform.gameObject == HBr)) {
                        // play bondStretch animation
                        HBr.GetComponent<Animation>().Play("bondStretch");
                        currentStep = 2;
                    } else {
                        return;
                    }
                }
            }
        }
    }

    void FixedUpdate() {
        if(!this.gameObject.transform.GetChild(2).gameObject) {
            return;
        }
        if(this.gameObject.transform.GetChild(2).gameObject.transform.localScale.x == 2) {
            BondBreak();
        }
    }

    void BondBreak() {
        int children = this.gameObject.transform.childCount;
        Vector2 v1 = new Vector2(-50f, 5f);
        Vector2 v2 = new Vector2(50f, -5f);

        for(int i = 0; i < children; ++i) {
            var c = this.gameObject.transform.GetChild(i);
            Vector2 moleculePosition = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y);
            Vector2 childPosition = new Vector2(c.position.x, c.position.y);
            var atomLocation = moleculePosition.x - childPosition.x;

            c.gameObject.AddComponent<CircleCollider2D>();
            c.gameObject.AddComponent<Rigidbody2D>();
            c.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            c.gameObject.GetComponent<Rigidbody2D>().drag = 1;

            if(atomLocation > 0) {
                c.gameObject.GetComponent<Rigidbody2D>().AddForce(v1);
            } else {
                c.gameObject.GetComponent<Rigidbody2D>().AddForce(v2);
            }

            if(c.gameObject.name == "singleBond") {
                this.gameObject.transform.DetachChildren();
                Destroy(c.gameObject);
                Destroy(this.gameObject);
            }

            // unstable atom animations on bond break (animation bug on Hydrogen GameObject / temporary comment-out)
            /*
            if (c.gameObject.name == "Hydrogen")
            {
                c.gameObject.GetComponent<Animation>().Play("unstableHydrogen");
            }
            else if (c.gameObject.name == "Bromine")
            {
                c.gameObject.GetComponent<Animation>().Play("unstableAtom");
            }
            */
        }
    }
}


