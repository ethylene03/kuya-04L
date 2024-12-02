using UnityEngine;

public class CrossingControl : MonoBehaviour
{

    void Update()
    {
        // set speed
        float newPos = transform.localPosition.y - (globalVariables.offsetValue * 10);
        transform.localPosition = new Vector3(transform.localPosition.x, newPos, transform.localPosition.z);

        if(transform.localPosition.y <= -8f) {
            Destroy(gameObject);
        }
    }
}
