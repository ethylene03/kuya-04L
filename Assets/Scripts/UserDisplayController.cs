using TMPro;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using QFSW.QC;
using System;


public class UserDisplayController : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
// Prefab for displaying a player
    [SerializeField] private GameObject playerPrefab;

    // Parent where player prefabs will be instantiated
    [SerializeField] private  Transform playerListParent;

    private void Start()
    {

        if(NetworkManager.Singleton != null){
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadComplete;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        DisplayPlayers();
        
    }
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected with ID: {clientId}");

        // Call DisplayPlayer or other related logic
        DisplayPlayers();
    }

    private void OnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        Debug.Log($"Scene synchronization complete for client: {clientId} in scene: {sceneName}");

        // Call your DisplayPlayer method
        DisplayPlayers();
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoadComplete;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    
    [Command]

    private void DisplayPlayers()
    {
        Debug.Log("Display players");

        if (NetworkManagerController.Instance == null)
        {
            Debug.LogError("NetworkManagerController instance is null.");
            return;
        }

        // Clear existing player prefabs
        try {   
            foreach (Transform child in playerListParent)
            {
                Destroy(child.gameObject);
            }
        } catch (SystemException ex){

            Debug.Log("Error in Clearing prefabs. " + ex);
        }
     
        var playerNames = NetworkManagerController.Instance.playerNames;

        Debug.Log("players count: " + playerNames.Value.Count);
        // Access playerNames and create prefabs

        // Validate playerPrefab
        if (playerPrefab == null) {
            Debug.LogError("playerPrefab is not assigned. Please assign a valid prefab.");
            return;
        }

        // Validate playerListParent
        if (playerListParent == null) {
            Debug.LogError("playerListParent is not assigned. Please assign a valid Transform.");
            return;
        }

        try {
            foreach (var player in NetworkManager.Singleton.ConnectedClientsList) {
                Debug.Log("player : " + player.ClientId.ToString());
                GameObject playerGO = Instantiate(playerPrefab, playerListParent);
                TMP_Text uiText = playerGO.GetComponentInChildren<TMP_Text>();
                if (uiText != null) {
                    uiText.text = player.ClientId.ToString(); // Update the text
                }
            }
        } catch (SystemException ex){
            Debug.Log("Error instantiating userDisplay. " + ex);
        }

    }
}
