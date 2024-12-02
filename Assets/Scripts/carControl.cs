using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class carControl : NetworkBehaviour
{
    public float carSpeed = 5.0f;

    // scale is: 1 kph = 0.00041 units (MAX 120 kph = MAX 0.05)
    // acceleration interval would be about 1 kph
    // break interval would be about 24 kph
    private float accelerateInterval = 0.0005f;
    private float breakInterval = 0.01f;
    public float maxPos = 5.3f;
    
    Vector3 position;
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
        if(movementJoystick.Direction.x == 0) {
            position.x += Input.GetAxis("Horizontal") * carSpeed * Time.deltaTime;
        } else{
            position.x += movementJoystick.Direction.x * carSpeed * Time.deltaTime;
        }

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
            float speed = globalVariables.playerSpeed + accelerateInterval * Time.deltaTime;
            globalVariables.playerSpeed = Mathf.Max(0, speed);
        }
    }

    private void SlowDown() {
        if(globalVariables.playerSpeed > accelerateInterval) {
            float speed = globalVariables.playerSpeed - (accelerateInterval * Time.deltaTime);
            globalVariables.playerSpeed = Mathf.Max(0, speed);
        } else {
            globalVariables.playerSpeed = 0;
        }
    }

    private void Brake() {
        if(globalVariables.playerSpeed > breakInterval) {
            float speed = globalVariables.playerSpeed - (breakInterval * Time.deltaTime);
            globalVariables.playerSpeed = Mathf.Max(0, speed);
        } else {
            globalVariables.playerSpeed = 0;
        }
    }

    public void setAccelerate(GameObject obj) {
        // Debug.Log("pressed down");
        isAccelerating = true;
        isBraking = false;
        isSlowingDown = false;

        //transform object to simulate animation
        obj.transform.localScale = new Vector3(89.262f, 74.82434f, 1f);
        obj.transform.localPosition = new Vector3(-456.84f, -377.23f, -1f);
    }

    public void setSlowDown(GameObject obj) {
        isSlowingDown = true;
        isAccelerating = false;
        isBraking = false;

        //transform object to simulate animation
        obj.transform.localScale = new Vector3(89.262f, 91.2492f, 1f);
        obj.transform.localPosition = new Vector3(-456.84f, -349.92f, -1f);
    }

    public void setBrakePressed(GameObject obj) {
        isBraking = true;
        isAccelerating = false;
        isSlowingDown = false;

        //transform object to simulate animation
        obj.transform.localScale = new Vector3(91.6488f, 58.62067f, 1f);
        obj.transform.localPosition = new Vector3(-747.6732f, -424.3f, -1f);
    }

    public void setBrakeUnpressed(GameObject obj) {
        //transform object to simulate animation
        obj.transform.localScale = new Vector3(91.6488f, 88.8192f, 1f);
        obj.transform.localPosition = new Vector3(-747.6732f, -389.5452f, -1f);
    }
}
