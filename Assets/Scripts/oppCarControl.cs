using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class oppCarControl : MonoBehaviour
{
    public float baseSpeed;
    private float speed;

    void Start()
    {
        // randomize base speed
        // baseSpeed = Random.Range(0, globalVariables.oppositeCarSpeed);
        // baseSpeed = 5f;
    }

    void Update()
    {
        // get speed relative to player's speed
        speed = baseSpeed + (globalVariables.playerSpeed * 500);
        // Debug.Log("oppSpeed: " + speed + ", ur speed: " + globalVariables.playerSpeed);

        // set speed
        transform.Translate (new Vector3(0, 1, 0) * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "EnemyCar" && gameObject.tag == "EnemyCar") {
            Destroy (col.gameObject);
        }
    }

    public float BaseSpeed {
        get { return baseSpeed; }

        set {
            if(value >= 0) {
                baseSpeed = Random.Range(value, globalVariables.oppositeCarSpeed);
            }    
        }
    }
}
