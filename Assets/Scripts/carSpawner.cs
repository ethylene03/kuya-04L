using UnityEngine;

public class carSpawner : MonoBehaviour
{

    public GameObject car;
    public float maxPos = 2.0f;
    public float delayTimer = 5f;
    float timer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = delayTimer;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0) {
            Vector3 carPos = new Vector3(Random.Range(-maxPos, maxPos), transform.position.y, transform.position.z);
            Instantiate (car, carPos, transform.rotation);
            timer = delayTimer;
        }
    }
}
