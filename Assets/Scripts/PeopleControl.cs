using UnityEngine;

public class PeopleControl : MonoBehaviour
{
    public float baseSpeed;
    private float speed;

    // to bop
    public float amplitude = 1f; // The height of the bop
    public float frequency = 3f;  // How fast the bop moves up and down
    private Vector3 startPosition;

    void Start()
    {
        // randomize base speed
        baseSpeed = Random.Range(-3f, 3f);
        startPosition = transform.position;
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

    void Bop() {
        // Calculate the new Y position using a sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;

        // Apply the new position to the game object
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
