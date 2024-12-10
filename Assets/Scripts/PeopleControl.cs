using UnityEngine;

public class PeopleControl : MonoBehaviour
{
    public float baseSpeed;
    private float speed;
    public carControl playerCar;

    void Start()
    {
        // randomize base speed
        baseSpeed = Random.Range(-3f, 3f);
        // baseSpeed = 5f;
    }

    void Update()
    {
        // get speed relative to player's speed
        speed = baseSpeed - (globalVariables.playerSpeed * 250);
        // Debug.Log("oppSpeed: " + speed + ", ur speed: " + globalVariables.playerSpeed);

        // set speed
        transform.Translate (new Vector3(0, 1, 0) * speed * Time.deltaTime);

        // destroy if out of frame
        if(transform.localPosition.y <= -8f || transform.localPosition.y >= 6.73f) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D obj) {
        if(obj.gameObject.tag == "PlayerCar") {
            Debug.Log("Picked up!");
            // destroy current object
            Destroy(gameObject);

            // add offset
            if(playerCar.IsOwner) {
                playerCar.currentOffset.Value += 1f;
            }
        }
    }
}
