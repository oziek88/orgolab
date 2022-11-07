using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClockTimer:MonoBehaviour {

    public float startTime;
    public float Countdown;
    private float levelScore = 0;
    public UnityEngine.UI.Text GameClock;

    private bool minusPoints = false;
    private bool isFreeplay;

    public bool runTimer = false;

    public GameObject Controller;

    // Use this for initialization
    void Start() {
        //startTime = 50;
        Countdown = startTime;
        isFreeplay = GameObject.Find("MainMenuCanvas") != null ? GameObject.Find("MainMenuCanvas").GetComponent<StartGame>().isFreeplay : false;
        if(isFreeplay) {
            GameClock.enabled = false;
            GameObject.Find("ScoreBoard").SetActive(false);
        };
    }

    // Update is called once per frame
    void FixedUpdate() {

        if(Controller.GetComponent<Spawning>().completedReaction == true && Countdown > 0) {
            runTimer = false;
            if(levelScore == 0) {
                levelScore = Countdown;
                Countdown = startTime;
                GameObject.Find("TotalScore").GetComponent<ScoreKeeper>().totalScore += levelScore;
            }

        } else if(runTimer && Countdown > 0) {
            if(levelScore != 0) {
                levelScore = 0;
                GameClock.color = new Color(0, 255, 223, 0.15f);
                GameClock.fontSize = 72;
            }

            Countdown -= 1;

            if(!minusPoints) {
                GameClock.text = Countdown.ToString();
            }

            if(Countdown == 1000) {
                GameClock.color = new Color(255, 206, 0, 0.4f);
                GameClock.fontSize = 80;
                if(isFreeplay) {
                    Countdown += 1;
                }
            }
            if(Countdown == 995) {
                GameClock.color = new Color(255, 206, 0, 0.2f);
                GameClock.fontSize = 72;
            }
            if(Countdown == 500) {
                GameClock.color = new Color(255, 0, 0, 0.4f);
                GameClock.fontSize = 80;
            }
            if(Countdown == 495) {
                GameClock.color = new Color(255, 0, 0, 0.2f);
                GameClock.fontSize = 72;
            }
            if(Countdown == 100) {
                GameClock.color = new Color(255, 0, 0, 0.4f);
                GameClock.fontSize = 80;
            }
        } else if(Controller.GetComponent<Spawning>().completedReaction == false && Countdown <= 0) {
            GameClock.text = "You Lose";
            GameClock.fontSize = 40;
        } else if(Controller.GetComponent<Spawning>().completedReaction == true && Countdown <= 0) {
            GameClock.text = "Never Give Up!";
            GameClock.color = new Color(0, 206, 0, 0.4f);
            GameClock.fontSize = 28;
        }
    }

    public IEnumerator MinusPoints(float points) {
        minusPoints = true;

        GameClock.color = new Color(255, 0, 0, 0.4f);
        GameClock.fontSize = 80;
        GameClock.text = "-" + points.ToString();
        Countdown -= points;
        yield return new WaitForSeconds(1);
        if(Countdown > 1001) {
            GameClock.color = new Color(0, 255, 223, 0.2f);
        } else if(Countdown <= 1001 && Countdown > 501) {
            GameClock.color = new Color(255, 206, 0, 0.2f);
        } else if(Countdown <= 501) {
            GameClock.color = new Color(255, 0, 0, 0.2f);
        }
        GameClock.fontSize = 72;
        GameClock.text = Countdown.ToString();

        minusPoints = false;
    }
}
