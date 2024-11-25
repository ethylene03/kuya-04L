using Unity.Netcode;
using UnityEngine;

public class carControl : NetworkBehaviour
{
    public float carSpeed;
    public float maxPos = 2.0f;
    Vector3 position;


    void Start()
    {
        float randomX = Random.Range(-maxPos, maxPos);
        position = transform.position;
        position.x = randomX;
        transform.position = position;
        Debug.Log(OwnerClientId + " = " + position.x);
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;
        position.x += Input.GetAxis ("Horizontal") * carSpeed * Time.deltaTime;
        position.x = Mathf.Clamp (position.x, -maxPos, maxPos);
        transform.position = position;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "EnemyCar") {
            Destroy (col.gameObject);
        }
    }

}
