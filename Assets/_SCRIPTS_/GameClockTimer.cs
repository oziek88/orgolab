using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClockTimer:MonoBehaviour {
    public int startTime;
    public Text gameClock;
    public ScoreKeeper scoreKeeper;

    private bool isFreeplay;
    private int countdown;
    private int smallFontSize = 180;
    private int bigFontSize = 200;
    

    // Use this for initialization
    void Start() {
        isFreeplay = GameObject.Find("MainMenuCanvas") != null ? GameObject.Find("MainMenuCanvas").GetComponent<StartGame>().isFreeplay : false;
        if(isFreeplay) {
            gameClock.enabled = false;
        } else {
            ResetCountdown();
        }
    }

    private void Update() {
       if(countdown <= 0) {
            GameOver();
        }
    }

    void FixedUpdate() {

        //if(spawningController.completedReaction == true && Countdown > 0) {
        //    runTimer = false;
        //    if(levelScore == 0) {
        //        levelScore = Countdown;
        //        Countdown = startTime;
        //        scoreKeeper.totalScore += levelScore;
        //    }

        //} else if(runTimer && Countdown > 0) {
        //    if(levelScore != 0) {
        //        levelScore = 0;
        //        gameClock.color = new Color(0, 255, 223, 0.15f);
        //        gameClock.fontSize = smallFontSize;
        //    }

        //    Countdown -= 1;

        //    if(!minusPoints) {
        //        gameClock.text = Countdown.ToString();
        //    }

        //    if(Countdown == 1000) {
        //        gameClock.color = new Color(255, 206, 0, 0.4f);
        //        gameClock.fontSize = bigFontSize;
        //        if(isFreeplay) {
        //            Countdown += 1;
        //        }
        //    }
        //    if(Countdown == 995) {
        //        gameClock.color = new Color(255, 206, 0, 0.2f);
        //        gameClock.fontSize = smallFontSize;
        //    }
        //    if(Countdown == 500) {
        //        gameClock.color = new Color(255, 0, 0, 0.4f);
        //        gameClock.fontSize = bigFontSize;
        //    }
        //    if(Countdown == 495) {
        //        gameClock.color = new Color(255, 0, 0, 0.2f);
        //        gameClock.fontSize = smallFontSize;
        //    }
        //    if(Countdown == 100) {
        //        gameClock.color = new Color(255, 0, 0, 0.4f);
        //        gameClock.fontSize = bigFontSize;
        //    }
        //} else if(spawningController.completedReaction == false && Countdown <= 0) {
        //    gameClock.text = "You Lose";
        //    gameClock.fontSize = 100;
        //} else if(spawningController.completedReaction == true && Countdown <= 0) {
        //    gameClock.text = "Never Give Up!";
        //    gameClock.color = new Color(0, 206, 0, 0.4f);
        //    gameClock.fontSize = 80;
        //}
    }

    private void ResetCountdown() {
        countdown = startTime;
        gameClock.fontSize = smallFontSize;
        gameClock.text = countdown.ToString();
    }

    public void StartCountdown() {
        StartCoroutine(Countdown());
    }

    public IEnumerator Countdown() {
        ResetCountdown();
        gameClock.text = countdown.ToString();
        while(countdown >= 0) {
            yield return new WaitForSeconds(1);
            countdown--;
            gameClock.text = countdown.ToString();
            if(countdown == 10) {
                gameClock.fontSize = bigFontSize;
            }
        }
    }

    public void GameOver() {
        StopAllCoroutines();
        gameClock.fontSize = 100;
        gameClock.text = "Game Over";
    }

    public void AwardPoints() {
        StopAllCoroutines();
        scoreKeeper.AddPoints(countdown);
    }

    public IEnumerator MinusPoints(int points) {
        gameClock.color = Color.red;
        gameClock.fontSize = bigFontSize;
        gameClock.text = "-" + points.ToString();
        countdown -= points;
        yield return new WaitForSeconds(1);
        gameClock.color = Color.white;
        gameClock.fontSize = smallFontSize;
        gameClock.text = countdown.ToString();
    }
}
