using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class trackMove : MonoBehaviour
{
    Vector2 offset;
    public GameObject crossingObject;
    public GameObject[] landmarks;
    private float spawnCrossingAt;
    public float[] landmarkDistance = new float[]{ 10f, 20f };
    private float spawnLandmarksAt;
    private int landmarkIdx;
    public GameObject gameOverText;
    public carControl playerCar;


    void SpawnObjectAtOffset(GameObject obj)
    {
        Vector3 spawnPosition = new Vector3(obj.transform.localPosition.x, 7.4f, obj.transform.localPosition.z);
        Instantiate(obj, spawnPosition, obj.transform.rotation);
    }

    void Start() {
        playerCar.currentOffset.Value = 0;
        spawnCrossingAt = Random.Range(playerCar.currentOffset.Value + 5f, playerCar.currentOffset.Value + 15f);
        spawnLandmarksAt = Random.Range(landmarkDistance[0], landmarkDistance[1]);
        landmarkIdx = 0;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.P)) {
            globalVariables.startGame = !globalVariables.startGame;
        }

        if (!playerCar){
            Debug.Log("PlayerCar not attached.");
            return;
        }

        if(globalVariables.startGame) {
            Time.timeScale = 1;

            // calculate offset base on player speed
            float offsetValue = Mathf.Clamp(globalVariables.playerSpeed, 0, globalVariables.maxPlayerSpeed);
            globalVariables.offsetValue = offsetValue;
            playerCar.currentOffset.Value += offsetValue;
            Debug.Log("offset: " + offsetValue + ", currOffset: " + playerCar.currentOffset.Value);
            
            // set track to move
            offset = new Vector2(0, playerCar.currentOffset.Value);
            Debug.Log("trackMove offset: " + offset);

            GetComponent<Renderer> ().material.mainTextureOffset = offset;

            AdjustOpponentCars();

            // spawn crossing
            if(playerCar.currentOffset.Value >= spawnCrossingAt) {
                SpawnObjectAtOffset(crossingObject);
                spawnCrossingAt = Random.Range(playerCar.currentOffset.Value + 5f, playerCar.currentOffset.Value + 15f);
            }

            // spawn landmarks
            float maxDistance = 50f;
            if(playerCar.currentOffset.Value >= maxDistance) {
                // abot na sa SM
                SpawnObjectAtOffset(landmarks[3]);

                // end game
                EndGame();
            } else if(playerCar.currentOffset.Value >= spawnLandmarksAt && landmarkIdx < 3) {
                SpawnObjectAtOffset(landmarks[landmarkIdx]);
                spawnLandmarksAt = Random.Range(playerCar.currentOffset.Value + landmarkDistance[0], playerCar.currentOffset.Value + landmarkDistance[1]);

                landmarkIdx = (landmarkIdx + 1) % 4;
            }
        } else {
            Time.timeScale = 0;
        }
    }

    void EndGame() {
        globalVariables.startGame = false;
        if(gameOverText != null)
            gameOverText.SetActive(true);
        Time.timeScale = 0;
        new WaitForSeconds(0.5f);
    }

    void AdjustOpponentCars(){
        // Fetch all opponent cars and update their positions based on their synced speed
       var opponentCars = GameObject.FindGameObjectsWithTag("PlayerCar");
       float ownerOffset = 0f;
       
        foreach (var car in opponentCars) {
            carControl carObj = car.GetComponent<carControl>();

            if (carObj.IsOwner){
                ownerOffset = carObj.currentOffset.Value;
            }
        }

        Debug.Log("ownerOffset " + ownerOffset);

        foreach (var car in opponentCars)
        {
            carControl carObj = car.GetComponent<carControl>();
            Debug.Log("CarObj id: " + carObj.OwnerClientId);
            Debug.Log("position: " + car.transform.position);

            if (!carObj.IsOwner)
            {
                float newY = car.transform.position.y + ((carObj.currentOffset.Value - playerCar.currentOffset.Value) * 10);
                car.transform.localPosition = new Vector3(car.transform.position.x, newY, car.transform.position.z);
            }
        }
    }

}
