using System.Collections;
using QFSW.QC;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private Joystick movementJoystick;
    public GameObject controls;
    private bool isAccelerating = false;
    private bool isBraking = false;
    private bool isSlowingDown = false;

    void Start()
    {
 
        position = transform.position;
        StartCoroutine(SetupControlsWithDelay());

    }

    IEnumerator SetupControlsWithDelay()
    {
        // Wait for a short delay (you can adjust the time as needed)
        yield return new WaitForSeconds(1);
        SetupJoystick();
        SetupAccelerate();
        SetupBrake();
    }


    private void SetupJoystick(){

        if(movementJoystick == null && IsOwner){
            movementJoystick = FindFirstObjectByType<Joystick>();

            if (movementJoystick == null){
                Debug.LogError("Cannot find joystick in the scene.");
            }
        }
    }

    private void SetupAccelerate(){
        GameObject accelerator = GameObject.Find("Accelerate");

        if (accelerator == null){
            Debug.LogError("Cannot find accelerator in the scene.");
            return;
        }

        EventTrigger eventTrigger = accelerator.AddComponent<EventTrigger>();

        // -------- Pointer Down ------------
        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;

        // Define what happens when PointerDown is triggered
        pointerDown.callback.AddListener((eventData) => { setAccelerate(accelerator); });

        // Add the entry to the EventTrigger
        eventTrigger.triggers.Add(pointerDown);

        // -------- Pointer Up ------------
        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;

        // Define what happens when PointerDown is triggered
        pointerUp.callback.AddListener((eventData) => { setSlowDown(accelerator); });

        // Add the entry to the EventTrigger
        eventTrigger.triggers.Add(pointerUp);
    }

        private void SetupBrake(){
        GameObject brake = GameObject.Find("Brake");

        if (brake == null){
            Debug.LogError("Cannot find Brake in the scene.");
            return;
        }

        EventTrigger eventTrigger = brake.AddComponent<EventTrigger>();

        // -------- Pointer Down ------------
        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;

        // Define what happens when PointerDown is triggered
        pointerDown.callback.AddListener((eventData) => { setBrakePressed(brake); });

        // Add the entry to the EventTrigger
        eventTrigger.triggers.Add(pointerDown);

        // -------- Pointer Up ------------
        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;

        // Define what happens when PointerDown is triggered
        pointerUp.callback.AddListener((eventData) => { setBrakeUnpressed(brake); });

        // Add the entry to the EventTrigger
        eventTrigger.triggers.Add(pointerUp);
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
        if(!IsOwner || !movementJoystick){ 
            // Debug.Log("Not the owner");
            return;
        }

        
        if(movementJoystick.Direction.x == 0) {
            position.x += Input.GetAxis("Horizontal") * carSpeed * Time.deltaTime;
        } else{
            position.x += movementJoystick.Direction.x * carSpeed * Time.deltaTime;
        }
  
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
        if(!IsOwner) return;

        Debug.Log("pressed down");
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
        if(!IsOwner) return;

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
        if(!IsOwner) return;

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
        if(!IsOwner) return;

        //transform object to simulate animation
        Vector3 currentScale = obj.transform.localScale;
        obj.transform.localScale = new Vector3(currentScale.x, currentScale.y + 20f, currentScale.z);

        Vector3 currentPosition = obj.transform.localPosition;
        obj.transform.localPosition = new Vector3(currentPosition.x, currentPosition.y + 40f, currentPosition.z);
    }
}
