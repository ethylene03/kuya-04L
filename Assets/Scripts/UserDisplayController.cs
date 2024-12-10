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

    // Prefab for displaying a player
    [SerializeField] private GameObject playerPrefab;

    // Parent where player prefabs will be instantiated
    [SerializeField] private  Transform playerListParent;
    [SerializeField] private GameObject playBtn;
    [SerializeField] private GameObject maxPlayersPrefab;

    private GameConstants gameConstants = new GameConstants();

    private BroadcastManager broadcastManager;
    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();


    
    private void Start()
    {
        broadcastManager = new BroadcastManager(gameConstants.NEW_PLAYER_PORT); // Use a different port if needed
        broadcastManager.IsListening = !NetworkManager.Singleton.IsHost;
        broadcastManager.HandleOnReceive = DisplayPlayers;

        if(NetworkManager.Singleton != null){
            // NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadComplete;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    
        }

        UpdatePlayerBoard();
        
        
    }


    private void OnDestroy()
    {
        Debug.Log("UserDisplayController OnDestroy");
        broadcastManager.CloseBroadcast();
        broadcastManager = null;

        // Unsubscribe to avoid memory leaks
        if (NetworkManager.Singleton != null)
        {
            // NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoadComplete;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
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


    public void HandleStartButton(){
        if (NetworkManagerController.Instance != null)
        {
            NetworkManagerController.Instance.StopBroadcastStartGame();
        }
        if (NetworkManager.Singleton.IsHost)
        {
            string sceneName = "straight-road";
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    public void HandleBackButton(){
        Debug.Log("BackButton");

        if (NetworkManagerController.Instance != null){
            SceneManager.LoadScene("home");
            NetworkManagerController.Instance.RestartNetworkManager();
            
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected with ID: {clientId}");

        ShowPlayerBoard();
        HideMaxPlayers();

        // Broadcast to display players
        BroadcastDisplayPlayers();
    }

    private void OnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        Debug.Log($"Scene synchronization complete for client: {clientId} in scene: {sceneName}");

        // Broadcast to display players
        BroadcastDisplayPlayers();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        string rejectionReason = NetworkManager.Singleton.DisconnectReason;

        Debug.Log($"Client {clientId} disconnected. " + rejectionReason);
        
        if(!NetworkManager.Singleton.IsHost && rejectionReason == gameConstants.EXCEED_MAX_CLIENTS){
            ShowMaxPlayers();
            HidePlayerBoard();
            throw new SystemException ("Client cannot join. Probably exceed max client.");
        } else {
            NetworkManagerController.Instance.RestartNetworkManager();
        }

        BroadcastDisplayPlayers();
    }

    [Command]
    private void BroadcastDisplayPlayers(){
        // Only host will broadcast
        if (NetworkManager.Singleton.IsHost){
            broadcastManager.SendBroadcast(gameConstants.NEW_PLAYER_MESSAGE);
            UpdatePlayerBoard();
        }
    }

    [Command]
    private void DisplayPlayers(string receivedMessage = "KUYA04L_new_player", IPEndPoint ipAddress = null){
        Debug.Log("Display players broadcast received. " + receivedMessage + " from: " + ipAddress?.Address);

        if (receivedMessage != gameConstants.NEW_PLAYER_MESSAGE){
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

        if ((NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer )
            && NetworkManager.Singleton.ConnectedClients.Count == gameConstants.MAX_CLIENTS
        ){
            // startBtnScript.ShowButton();
        } else {
            // startBtnScript.HideButton();
        }

    }

    [Command]
    private void HideMaxPlayers(){
        maxPlayersPrefab.SetActive(false);
    }

    [Command]
    private void ShowMaxPlayers(){
        maxPlayersPrefab.SetActive(true);
    }

    private void HidePlayerBoard(){
        this.gameObject.SetActive(false);
    }

    private void ShowPlayerBoard(){
        this.gameObject.SetActive(true);
    }



}
