using UnityEngine;

public class oppCarControl : MonoBehaviour
{
    private float carSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        carSpeed = Random.Range(0, globalVariables.oppositeCarSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        // get random car speed for opposite cars
        carSpeed += globalVariables.playerSpeed;

        // set speed
        transform.Translate (new Vector3(0, 1, 0) * carSpeed * Time.deltaTime);
    }
}
