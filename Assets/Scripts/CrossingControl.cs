using UnityEngine;

public class CrossingControl : MonoBehaviour
{

    void Update()
    {
        // set speed
        transform.Translate (new Vector3(0, 1, 0) * globalVariables.playerSpeed * 1700 * Time.deltaTime);

        if(transform.localPosition.y <= -8f) {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "Crossing" && gameObject.tag == "Crossing") {
            Destroy (col.gameObject);
        }
    }
}
