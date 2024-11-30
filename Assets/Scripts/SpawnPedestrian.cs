using UnityEngine;

public class SpawnPedestrian : MonoBehaviour
{

    public GameObject[] people;
    public float[] maxX = new float[] { 8.56f, -7.86f };
    public float[] minX = new float[] { 8.05f, -8.45f };
    public float maxY = 4.5f;
    public float minY = -4.5f;
    public float delayTimer = 2f;

    private float prevSpeed = 0;

    void Start() {
        globalVariables.timer = delayTimer;
    }

    void Update() {
        if(globalVariables.startGame) {
            globalVariables.timer -= Time.deltaTime;

            if(globalVariables.timer <= 0) {
                // get random car
                int idx = Random.Range(0, people.Length);

                // get random side (left or right)
                int xval = Random.Range(0, maxX.Length);

                // get random position
                Vector3 position = new Vector3(Random.Range(minX[xval], maxX[xval]), Random.Range(minY, maxY), transform.position.z);
                
                // spawn car
                Instantiate (people[idx], position, transform.rotation);
                
                // set timer back to delay time
                globalVariables.timer = delayTimer;
            }
        }
    }
}
