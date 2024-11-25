using UnityEngine;

public class oppCarControl : MonoBehaviour
{
    private float carSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // get random car speed for opposite cars
        carSpeed = Random.Range(0, globalVariables.oppositeCarSpeed);

        // set speed
        transform.Translate (new Vector3(0, 1, 0) * carSpeed * Time.deltaTime);
    }
}
