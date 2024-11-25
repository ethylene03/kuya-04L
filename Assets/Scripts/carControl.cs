using UnityEngine;
using UnityEngine.UIElements;

public class carControl : MonoBehaviour
{
    public float carSpeed;
    public float maxPos = 5.3f;
    Vector3 position;
    public GameObject background;
    public GameObject spawn;
    public Joystick movementJoystick;

    void Start()
    {
        position = transform.position;
    }

    void Update()
    {
        position.x += movementJoystick.Direction.x * carSpeed * Time.deltaTime;
        position.x = Mathf.Clamp (position.x, -maxPos, maxPos);
        transform.position = position;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "EnemyCar") {
            // destroy player
            Destroy (gameObject);

            // endgame (pause)
            globalVariables.startGame = false;
        }
    }
}
