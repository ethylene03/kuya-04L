using Unity.Netcode;
using UnityEngine;

public class carControl : NetworkBehaviour
{
    public float carSpeed;
    public float maxPos = 2.0f;

    private Joystick joystick;
    Vector3 position;


    void Start()
    {
        float randomX = Random.Range(-maxPos, maxPos);
        position = transform.position;
        position.x = randomX;
        transform.position = position;
        Debug.Log(OwnerClientId + " = " + position.x);

        if (joystick == null)
        {
            joystick = FindFirstObjectByType<Joystick>();

            if (joystick == null)
            {
                Debug.LogError("Joystick not found in the scene. Make sure it's added and active.");
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner || !joystick) return;

        
        float horizontalInput = joystick.Horizontal;
        // Debug.Log(horizontalInput);
        // float horizontalInput = Input.GetAxis ("Horizontal");
        
        position.x += horizontalInput * carSpeed * Time.deltaTime;
        position.x = Mathf.Clamp (position.x, -maxPos, maxPos);
        transform.position = position;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "EnemyCar") {
            Destroy (col.gameObject);
        }
    }



}
