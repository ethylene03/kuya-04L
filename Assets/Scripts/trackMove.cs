using UnityEngine;

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
        globalVariables.currentOffset = 0;
        spawnCrossingAt = Random.Range(globalVariables.currentOffset + 5f, globalVariables.currentOffset + 15f);
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
            float offsetValue = Mathf.Clamp(playerCar.playerSpeed.Value, 0, playerCar.maxPlayerSpeed);
            globalVariables.offsetValue = offsetValue;
            globalVariables.currentOffset += offsetValue;
            // Debug.Log("speed: " + playerCar.playerSpeed.Value + ", offset: " + offsetValue + ", currOffset: " + globalVariables.currentOffset);
            
            // set track to move
            offset = new Vector2(0, globalVariables.currentOffset);
            Debug.Log("trackMove offset: " + offset);
            
            GetComponent<Renderer> ().material.mainTextureOffset = offset;

            AdjustOpponentCars();

            // spawn crossing
            if(globalVariables.currentOffset >= spawnCrossingAt) {
                SpawnObjectAtOffset(crossingObject);
                spawnCrossingAt = Random.Range(globalVariables.currentOffset + 5f, globalVariables.currentOffset + 15f);
            }

            // spawn landmarks
            if(globalVariables.currentOffset >= spawnLandmarksAt) {
                SpawnObjectAtOffset(landmarks[landmarkIdx]);
                spawnLandmarksAt = Random.Range(globalVariables.currentOffset + landmarkDistance[0], globalVariables.currentOffset + landmarkDistance[1]);

                if(landmarkIdx == 3) {
                    // end game
                    globalVariables.startGame = false;
                    EndGame();
                } else
                    landmarkIdx = (landmarkIdx + 1) % 4;
            }
        } else {
            Time.timeScale = 0;
        }
    }

    void EndGame() {
        if(gameOverText != null)
            gameOverText.SetActive(true);
        Time.timeScale = 0;
    }

    void AdjustOpponentCars(){
        // Fetch all opponent cars and update their positions based on their synced speed
    //    var opponentCars = GameObject.FindGameObjectsWithTag("PlayerCar");
       
    //     foreach (var car in opponentCars)
    //     {
    //         carControl carObj = car.GetComponent<carControl>();
    //         if (!carObj.IsOwner)
    //         {
    //             float opponentSpeed = carObj.playerSpeed.Value;
    //             car.transform.position += Vector3.up * opponentSpeed * Time.deltaTime;
    //         }
    //     }
    }

}
