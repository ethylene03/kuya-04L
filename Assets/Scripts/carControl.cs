using QFSW.QC;
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
    private VariableJoystick movementJoystick;
    public GameObject controls;
    private bool isAccelerating = false;
    private bool isBraking = false;
    private bool isSlowingDown = false;

    void Start()
    {
        if (IsLocalPlayer){
            Debug.Log("I am owner. " + OwnerClientId);
            InstantiateControls();
        }
        Debug.Log("carControl start " + OwnerClientId);
        position = transform.position;
    }

    [Command]
    public void InstantiateControls(){
        if(controls != null){
            Debug.Log("Instantiating controls");
            GameObject controller = Instantiate(controls);
            movementJoystick = controller.GetComponentInChildren<VariableJoystick>();
            Debug.Log("Joystick " + movementJoystick);
        } else {
            Debug.Log("Controls not found.");
        }
    }

    void Update()
    {
        if(!IsOwner){ 
            // Debug.Log("Not the owner");
            return;
        }

        if (movementJoystick == null)
        {
            Debug.LogWarning("Joystick not initialized yet.");
            return;
        }

        
        if(movementJoystick.Direction.x == 0) {
            position.x += Input.GetAxis("Horizontal") * carSpeed * Time.deltaTime;
        } else{
            position.x += movementJoystick.Direction.x * carSpeed * Time.deltaTime;
        }
        // if (movementJoystick != null) {
        //     Debug.Log("movementJoystick NOT null");
        // } else {
        //     Debug.Log("movementJoystick null");
        // }
        // Debug.Log("movementJoystick " + movementJoystick.Direction.x);
        // Debug.Log("position.x before " + position.x);
        position.x = Mathf.Clamp (position.x, -maxPos, maxPos);
        // Debug.Log("position.x after " + position.x);
        // Debug.Log("transform.position before: " + transform.position);
        transform.position = position;
        // Debug.Log("transform.position after: " + transform.position);

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
        Vector3 currentScale = obj.transform.localScale;
        obj.transform.localScale = new Vector3(currentScale.x, currentScale.y - 20f, currentScale.z);

        Vector3 currentPosition = obj.transform.localPosition;
        obj.transform.localPosition = new Vector3(currentPosition.x, currentPosition.y - 40f, currentPosition.z);
    }

    public void setSlowDown(GameObject obj) {
        isSlowingDown = true;
        isAccelerating = false;
        isBraking = false;

        //transform object to simulate animation
        Vector3 currentScale = obj.transform.localScale;
        obj.transform.localScale = new Vector3(currentScale.x, currentScale.y + 20f, currentScale.z);

        Vector3 currentPosition = obj.transform.localPosition;
        obj.transform.localPosition = new Vector3(currentPosition.x, currentPosition.y + 40f, currentPosition.z);
    }

    public void setBrakePressed(GameObject obj) {
        isBraking = true;
        isAccelerating = false;
        isSlowingDown = false;

        //transform object to simulate animation
        Vector3 currentScale = obj.transform.localScale;
        obj.transform.localScale = new Vector3(currentScale.x, currentScale.y - 20f, currentScale.z);

        Vector3 currentPosition = obj.transform.localPosition;
        obj.transform.localPosition = new Vector3(currentPosition.x, currentPosition.y - 40f, currentPosition.z);
    }

    public void setBrakeUnpressed(GameObject obj) {
        //transform object to simulate animation
        Vector3 currentScale = obj.transform.localScale;
        obj.transform.localScale = new Vector3(currentScale.x, currentScale.y + 20f, currentScale.z);

        Vector3 currentPosition = obj.transform.localPosition;
        obj.transform.localPosition = new Vector3(currentPosition.x, currentPosition.y + 40f, currentPosition.z);
    }
}
