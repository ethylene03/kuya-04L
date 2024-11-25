using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class countDown : MonoBehaviour
{
    public int countdownTime = 3; // Countdown duration in seconds
    public Text countdownDisplay; // UI Text to display the countdown
    public carControl carController; // Reference to the car control script
    public trackMove backgroundScroll; // Reference to background scroll script

    private void Start()
    {
        // Disable controls initially
        carController.enabled = false;
        backgroundScroll.enabled = false;

        // Start the countdown coroutine
        StartCoroutine(CountdownToStart());
    }

    private IEnumerator CountdownToStart()
    {
        while (countdownTime > 0)
        {
            // Display the countdown
            countdownDisplay.text = countdownTime.ToString();

            // Wait for one second
            yield return new WaitForSeconds(1);

            // Decrement the countdown
            countdownTime--;
        }

        // Hide the countdown display
        countdownDisplay.text = "GO!";
        yield return new WaitForSeconds(1);
        countdownDisplay.gameObject.SetActive(false);

        // Enable controls
        carController.enabled = true;
        backgroundScroll.enabled = true;
    }
}
