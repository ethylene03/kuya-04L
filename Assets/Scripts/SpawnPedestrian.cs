using UnityEngine;

public class SpawnPedestrian : MonoBehaviour
{

    public GameObject[] people;
    public float[] maxX = new float[] { 8.56f, -7.86f };
    public float[] minX = new float[] { 8.05f, -8.45f };
    public float maxY = 4.5f;
    public float minY = -4.5f;
    public float minDelay = 0f;
    public float maxDelay = 2f;
    public carControl playerCar;

    private float timer;

    void Start() {
        timer = minDelay;
    }

    void Update() {
        if(globalVariables.startGame) {
            timer -= Time.deltaTime;

            if(timer <= 0) {
                // get random car
                int idx = Random.Range(0, people.Length);

                // get random side (left or right)
                int xval = Random.Range(0, maxX.Length);

                // get random position
                Vector3 position = new Vector3(Random.Range(minX[xval], maxX[xval]), Random.Range(minY, maxY), transform.position.z);
                
                // spawn car
                GameObject newPedestrian = Instantiate (people[idx], position, transform.rotation);
                PeopleControl peopleScript = newPedestrian.GetComponent<PeopleControl>();
                peopleScript.playerCar = this.playerCar;
                
                // set timer back to delay time
                timer = Random.Range(minDelay, maxDelay + 1);
            }
        }
    }
}
