using UnityEngine;

public class carSpawner : MonoBehaviour
{

    public GameObject[] cars;
    public float maxPos = -1.0f;
    public float minPos = -5.3f;
    public float delayTimer = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        globalVariables.timer = delayTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if(globalVariables.startGame) {
            if(globalVariables.timer > delayTimer) {
                globalVariables.timer -= globalVariables.playerSpeed / Time.deltaTime; // change interval according to speed of player
            } else {
                globalVariables.timer -= Time.deltaTime;
            }

            if(globalVariables.timer <= 0) {
                // get random car
                int idx = Random.Range(0, cars.Length);

                // get random position
                Vector3 carPos = new Vector3(Random.Range(minPos, maxPos), transform.position.y, transform.position.z);
                
                // spawn car
                Instantiate (cars[idx], carPos, transform.rotation);
                
                // set timer back to delay time
                globalVariables.timer = delayTimer;
            }
        }
    }
}
