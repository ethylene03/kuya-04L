using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
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
        if(countDown < 4) {
            countDown = Time.realtimeSinceStartup - startTime;
            changeCountDown((int)countDown);
        } 

    }

    public void Pause() {
        globalVariables.startGame = !globalVariables.startGame;
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
                globalVariables.startGame = true;
                break;
        }
    }
}
