using UnityEngine;
using System.Collections;

public class trackMove : MonoBehaviour
{
    Vector2 offset;

    void Start() {

    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            globalVariables.startGame = !globalVariables.startGame;
        }

        if(globalVariables.startGame) {
            Time.timeScale = 1;
            offset = new Vector2(0, Time.time * globalVariables.playerSpeed);
            GetComponent<Renderer> ().material.mainTextureOffset = offset;
        } else {
            Time.timeScale = 0;
        }
    }
}
