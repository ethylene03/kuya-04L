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

        if(globalVariables.startGame) {
            Time.timeScale = 1;

            // calculate offset base on player speed
            float offsetValue = Mathf.Clamp(globalVariables.playerSpeed, 0, globalVariables.maxPlayerSpeed);
            globalVariables.offsetValue = offsetValue;
            globalVariables.currentOffset += offsetValue;
            Debug.Log("speed: " + globalVariables.playerSpeed + ", offset: " + offsetValue + ", currOffset: " + globalVariables.currentOffset);
            
            // set track to move
            offset = new Vector2(0, globalVariables.currentOffset);
            GetComponent<Renderer> ().material.mainTextureOffset = offset;

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
}
