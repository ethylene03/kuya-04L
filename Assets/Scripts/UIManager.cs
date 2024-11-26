using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text pauseBTN; 
    public TMP_Text textTimer;
    public TMP_Text counter;
    private float timerTime = 0f;

    private float startTime = 0f; 
    public float countDown = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        // timer
        timerTime += Time.deltaTime;
        DisplayTime(timerTime);

        // countdown
        if(countDown < 8) {
            countDown = Time.realtimeSinceStartup - startTime;
            changeCountDown((int)countDown / 2);
        } 

    }

    public void Pause() {
        globalVariables.startGame = !globalVariables.startGame;

        // if game is running
        if(Time.timeScale == 1) {
            pauseBTN.text = "D";

        // if game is paused
        } else if(Time.timeScale == 0) {
            pauseBTN.text = "II";
        }
    }

    // display timerTime in text format
    void DisplayTime(float timeToDisplay) {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        textTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // change start counter
    void changeCountDown(int count) {
        switch(count) {
            case 0:
                counter.text = "3";
                break;

            case 1:
                counter.text = "2";
                break;

            case 2:
                counter.text = "1";
                break;

            case 3:
                counter.text = "GO!";
                globalVariables.startGame = true;
                break;
            
            default:
                counter.text = "";
                break;
        }
    }
}
