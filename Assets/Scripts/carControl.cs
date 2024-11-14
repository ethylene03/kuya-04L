using UnityEngine;

public class carControl : MonoBehaviour
{
    public float carSpeed;
    public float maxPos = 2.0f;
    Vector3 position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
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
