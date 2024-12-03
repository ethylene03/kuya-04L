using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using QFSW.QC;
using System;
using System.Net;
using System.Collections.Concurrent;



public class UserDisplayController : MonoBehaviour
{
    private BroadcastManager broadcastManager;
    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
// Prefab for displaying a player
    [SerializeField] private GameObject playerPrefab;

    // Parent where player prefabs will be instantiated
    [SerializeField] private  Transform playerListParent;

    // Port for notifying new connected client
    private int BROADCAST_PORT = 7780;
    private string BROADCAST_MESSAGE = "KUYA04L_new_player";
    


    private void Start()
    {
        broadcastManager = new BroadcastManager(BROADCAST_PORT); // Use a different port if needed
        broadcastManager.IsListening = !NetworkManager.Singleton.IsHost;
        broadcastManager.HandleOnReceive = DisplayPlayers;

        if(NetworkManager.Singleton != null){
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadComplete;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        UpdatePlayerBoard();
        
    }


    private void OnDestroy()
    {
        broadcastManager.IsListening = false;
        // Unsubscribe to avoid memory leaks
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoadComplete;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void Update(){
        while (messageQueue.TryDequeue(out string message))
        {
            Debug.Log("Dequeueing " + message);
            // Process the dequeued message
            UpdatePlayerBoard();
        }
    }


    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected with ID: {clientId}");

        // Broadcast to display players
        BroadcastDisplayPlayers();
    }

    private void OnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        Debug.Log($"Scene synchronization complete for client: {clientId} in scene: {sceneName}");

        // Broadcast to display players
        BroadcastDisplayPlayers();
    }


    [Command]
    private void BroadcastDisplayPlayers(){
        // Only host will broadcast
        if (NetworkManager.Singleton.IsHost){
            broadcastManager.SendBroadcast(BROADCAST_MESSAGE);
            UpdatePlayerBoard();
        }
    }

    [Command]
    private void DisplayPlayers(string receivedMessage = "KUYA04L_new_player", IPEndPoint ipAddress = null){
        Debug.Log("Display players broadcast received. " + receivedMessage + " from: " + ipAddress?.Address);

        if (receivedMessage != BROADCAST_MESSAGE){
            return;
        }
        messageQueue.Enqueue(receivedMessage);
    }
    
    [Command]
    private void UpdatePlayerBoard()
    {


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
