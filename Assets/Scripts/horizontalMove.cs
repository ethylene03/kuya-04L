using UnityEngine;
using System.Collections;

public class horizontalMove : MonoBehaviour
{
    public float speed;
    Vector2 offset;

    void Start() {

    }

    void Update() {
        if(Time.timeScale != 1)
            Time.timeScale = 1;

        offset = new Vector2(Time.time * speed, 0);
        GetComponent<Renderer> ().material.mainTextureOffset = offset;
    }
}
