using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text pauseBTN; 
    public TMP_Text textTimer;
    public TMP_Text counter;
    private float time = 0f;

    private float startTimer = 0f; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(startTimer >= 6) {
            time += Time.deltaTime;
        } else {
            startTimer += Time.deltaTime;
        }

        DisplayTime(time);
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

    // display time in text format
    void DisplayTime(float timeToDisplay) {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        textTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // change start counter
    public void Counter() {
        int count = (int)startTimer / 2;
        switch(count) {
            case 0:
                counter.text = "3";
                break;

            case 1:
                counter.text = "2";
                break;

            case 2:
                counter.text = "3";
                break;

            case 3:
                counter.text = "Go!";
                globalVariables.startGame = true;
                break;
        }
    }
}
