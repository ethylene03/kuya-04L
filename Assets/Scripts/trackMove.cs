using UnityEngine;
using System.Collections;

public class trackMove : MonoBehaviour
{
    Vector2 offset;

    void Start() {
        globalVariables.currentOffset = 0;
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
            // Debug.Log("speed: " + globalVariables.playerSpeed + ", offset: " + offsetValue + ", currOffset: " + currentOffset);
            
            // set track to move
            offset = new Vector2(0, globalVariables.currentOffset);
            GetComponent<Renderer> ().material.mainTextureOffset = offset;
        } else {
            Time.timeScale = 0;
        }
    }
}
