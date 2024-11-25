using UnityEngine;
using System.Collections;
using Unity.Netcode;
using NUnit.Framework;


public class trackMove  : NetworkBehaviour
{
    public float speed;
    Vector2 offset;

    void Start() {

    }

    void Update() {
        if(!IsServer) return;

        offset = new Vector2(0, Time.time * speed);
        trackMoveClientRPC(offset);
    }

    [ClientRpc]
    private void trackMoveClientRPC(Vector2 offset){
        if (GetComponent<Renderer>() != null) {
            GetComponent<Renderer>().material.mainTextureOffset = offset;
        }
    }
}
