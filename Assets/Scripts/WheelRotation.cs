using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    public float rotationSpeed = 50f; // Rotation speed in degrees per second

    void Update()
    {
        // Rotate the sprite around the Z-axis
        transform.Rotate(-Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
