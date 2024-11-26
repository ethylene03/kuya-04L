using UnityEngine;

public class oppCarControl : MonoBehaviour
{
    private float baseSpeed;

    void Start()
    {
        // randomize base speed
        baseSpeed = Random.Range(0, globalVariables.oppositeCarSpeed);
    }

    void Update()
    {
        // get speed relative to player's speed
        float speed = baseSpeed + globalVariables.offsetValue;

        // set speed
        transform.Translate (new Vector3(0, 1, 0) * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "EnemyCar" && gameObject.tag == "EnemyCar") {
            Destroy (col.gameObject);
            Destroy (gameObject);
        }
    }
}
