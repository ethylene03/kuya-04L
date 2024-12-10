using UnityEngine;

public class PeopleControl : MonoBehaviour
{
    public float baseSpeed;
    private float speed;
    public carControl playerCar;
    public AudioClip pickupSound; // Add a public field for the sound effect
    private AudioSource audioSource;

    void Start()
    {
        // randomize base speed
        baseSpeed = Random.Range(-3f, 3f);
        // baseSpeed = 5f;

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing on this GameObject.");
        }
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
        trackMove trackScript = GameObject.Find("Background").GetComponent<trackMove>();

        if(obj.gameObject.tag == "PlayerCar") {
            Debug.Log("Picked up!");
            // destroy current object
            Destroy(gameObject);

            // play sound effect
            if (audioSource != null || pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            // add offset
            if(playerCar.IsOwner) {
                playerCar.currentOffset.Value += 5f * Time.deltaTime;

                if(trackScript != null)
                    trackScript.MoveTrack();
            }
        }
    }
}
