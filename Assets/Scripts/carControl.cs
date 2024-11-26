using UnityEngine;
using UnityEngine.UIElements;

public class carControl : MonoBehaviour
{
    public float carSpeed = 5.0f;
    public float speedInterval = 0.05f;
    public float maxPos = 5.3f;
    Vector3 position;
    public GameObject background;
    public GameObject spawn;
    public Joystick movementJoystick;
    private bool isAccelerating = false;
    private bool isBraking = false;
    private bool isSlowingDown = false;

    void Start()
    {
        position = transform.position;
    }

    void Update()
    {
        position.x += movementJoystick.Direction.x * carSpeed * Time.deltaTime;
        position.x = Mathf.Clamp (position.x, -maxPos, maxPos);
        transform.position = position;

        if(isAccelerating) {
            Accelerate();
        }

        if(isBraking) {
            Brake();
        }

        if(isSlowingDown) {
            SlowDown();
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "EnemyCar") {
            // destroy player
            Destroy (gameObject);

            // endgame (pause)
            globalVariables.startGame = false;
        }
    }

    private void Accelerate() {
        if(globalVariables.playerSpeed < globalVariables.maxPlayerSpeed) {
            float speed = globalVariables.playerSpeed + speedInterval * Time.deltaTime;
            globalVariables.playerSpeed = Mathf.Max(0, speed);
        }
    }

    private void SlowDown() {
        if(globalVariables.playerSpeed > speedInterval) {
            float speed = globalVariables.playerSpeed - (speedInterval * Time.deltaTime);
            globalVariables.playerSpeed = Mathf.Max(0, speed);
        } else {
            globalVariables.playerSpeed = 0;
        }
    }

    private void Brake() {
        globalVariables.playerSpeed = 0;
    }

    public void setAccelerate() {
        isAccelerating = true;
        isBraking = false;
        isSlowingDown = false;
    }

    public void setSlowDown() {
        isSlowingDown = true;
        isAccelerating = false;
        isBraking = false;
    }

    public void setBrake() {
        isBraking = true;
        isAccelerating = false;
        isSlowingDown = false;
    }
}
