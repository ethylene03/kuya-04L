using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class trackMove : MonoBehaviour
{
    Vector2 offset;
    public GameObject crossingObject;
    public float spawnCrossingAt;

    void SpawnObjectAtOffset()
    {
        Vector3 spawnPosition = new Vector3(0f, 7.4f, 99f);
        Instantiate(crossingObject, spawnPosition, Quaternion.identity);
    }

    void Start() {
        globalVariables.currentOffset = 0;
        spawnCrossingAt = Random.Range(globalVariables.currentOffset + 5f, globalVariables.currentOffset + 15f);
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
                SpawnObjectAtOffset();
                spawnCrossingAt = Random.Range(globalVariables.currentOffset + 5f, globalVariables.currentOffset + 15f);
            }
        } else {
            Time.timeScale = 0;
        }
    }
}
