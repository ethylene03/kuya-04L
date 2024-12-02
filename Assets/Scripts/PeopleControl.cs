using UnityEngine;

public class PeopleControl : MonoBehaviour
{
    public float baseSpeed;
    private float speed;

    void Start()
    {
        // randomize base speed
        baseSpeed = Random.Range(-3f, 3f);
        // baseSpeed = 5f;
    }

    void Update()
    {
        // get speed relative to player's speed
        speed = baseSpeed - (globalVariables.playerSpeed * 1000);
        // Debug.Log("oppSpeed: " + speed + ", ur speed: " + globalVariables.playerSpeed);

        // set speed
        transform.Translate (new Vector3(0, 1, 0) * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "People" && gameObject.tag == "People") {
            Destroy (col.gameObject);
        }
    }
}
