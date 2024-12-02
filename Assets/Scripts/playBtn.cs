using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class playBtn : MonoBehaviour
{
    public string targetScene;
    public Action TriggerOnClick;

    private void OnMouseDown()
    {
        try {
            Debug.Log("onclick");
            if (!string.IsNullOrEmpty(targetScene))
            {
                SceneManager.LoadScene(targetScene);
                TriggerOnClick?.Invoke();
            }
            else
            {
                Debug.LogWarning("Target scene name is not set.");
            }
        } catch(SystemException e){
            Debug.Log(e);
        }

    }

}
