using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using Kuya04LPlayer;
using System.Collections.Generic;
using Unity.VisualScripting;
using QFSW.QC;


public class NetworkManagerController : MonoBehaviour
{
    [SerializeField] private playBtn playBtnScript;

    // Port number to send the message. This should match with the listening device.
    public int BROADCAST_PORT = 7778;
    public string hostIp;
    public ushort hostPort;

    private UdpClient udpClient;
    private UnityTransport unityTransport;
    private Boolean isSearchingGame = true;
    private string START_GAME_MESSAGE = "KUYA04L_GAMEHOST";

    
    private  int MAX_CLIENTS = 4;

    // Store player id and player name while the objects are not spawned yet
    public NetworkVariable<List<PlayerData>> playerNames = new NetworkVariable<List<PlayerData>>(new List<PlayerData>());

    private void Start(){
        // Handles broadcasting task
        udpClient = new UdpClient(BROADCAST_PORT);

        // Handles multiplayer networking
        unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if(isSearchingGame){
            udpClient.BeginReceive(OnReceive, null);
        }
        // Simulate button click logic using playBtnScript
        if (playBtnScript != null)
        {
            // Example: React to playBtn's click through shared logic
            playBtnScript.TriggerOnClick = HandleStartButton;
        }
    }

    private void HandleStartButton()
    {
        // NetworkManager.Singleton.StartClient();
        if (hostIp.IsNullOrEmpty()){
            Debug.Log("No active game session. Creating a game in the network.");
            CreateGame();
        } else {
            Debug.Log("There is active game session in ip: " + hostIp);
            JoinGame();
        }
    }

    // Broadcasts to LAN that they will start a game.
    private void CreateGame(){
        try {
            // Do not listen to other broadcast that is searching a game. One active game at a time.
            isSearchingGame = false;

            // Enable Broadcasting
            udpClient.EnableBroadcast = true;
            
            // Send broadcast message every one second
            InvokeRepeating(nameof(BroadcastStartGame), 0, 1.0f);

            NetworkManager.Singleton.StartHost();

        } catch (SystemException e){
            Debug.Log(e);
        }
    }

    private void JoinGame(){
        Debug.Log("Joining game at: " + hostIp);
        isSearchingGame = false;

        // Connect to the ip of the host
        if(!hostIp.IsNullOrEmpty()){
            unityTransport.ConnectionData.Address = hostIp;
            unityTransport.ConnectionData.Port = hostPort;

            // Join active game
            try {
                 Debug.Log("Client attempting to start.");
                 Debug.Log($"Attempting to connect to host at {unityTransport.ConnectionData.Address}:{unityTransport.ConnectionData.Port}");
                Boolean isClientListening = NetworkManager.Singleton.StartClient();
                 Debug.Log("Client started. " + isClientListening);
                ListConnectedClients();
            } catch(SystemException ex){
                Debug.Log("Cannot start client. " + ex);
            }
            
        } else {
            Debug.Log("Cannot join game. No host ip.");
        }


    }

    private void BroadcastStartGame(){
        try {
            // end
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, BROADCAST_PORT);

            // Encode message
            byte[] data = Encoding.UTF8.GetBytes(START_GAME_MESSAGE);

            // Send message
            udpClient.Send(data, data.Length, endPoint);

            Debug.Log("Broadcast sent: " + START_GAME_MESSAGE);
        } catch(SystemException e){
            Debug.Log(e);
        }
    }

     private void OnReceive(IAsyncResult result){
        if (!isSearchingGame) return;
        // Get address of the sending broadcast
        IPEndPoint endPoint = new IPEndPoint( IPAddress.Broadcast, BROADCAST_PORT);
        
        // Decode broadcasted message
        byte[] data = udpClient.EndReceive(result, ref endPoint);
        
        string message = Encoding.UTF8.GetString(data);
        
        Debug.Log($"Received broadcast from {endPoint.Address}: {message}");

        if (message == START_GAME_MESSAGE){

            // store ip address of host
            hostIp = endPoint.Address.ToString();
            hostPort = (ushort) endPoint.Port;

            Debug.Log("Host game detected: " + hostIp);
        } else {
            Debug.Log("Broadcast received but not from the game.");
        }

        udpClient.BeginReceive(OnReceive, null);
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        // Subscribe to client connected and disconnected events
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
        // Unsubscribe from events to prevent memory leaks
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("Client Connected " + clientId);
        int currentConnections = NetworkManager.Singleton.ConnectedClients.Count;

         if (currentConnections > MAX_CLIENTS)
        {
            Debug.Log("Max connections reached. Disconnecting client " + clientId);
            NetworkManager.Singleton.DisconnectClient(clientId);
        }
        else
        {
            // You can assign a name for each player
            string playerName = "Player" + clientId;

            // Add the player data to the NetworkVariable
            if (NetworkManager.Singleton.IsServer){
                AddPlayerName(clientId, playerName);
            }

            Debug.Log("Client " + clientId + " connected. Name: " + playerName + "Total clients: " + currentConnections);
            
        }

        ListConnectedClients(); // List clients when a new client connects
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} disconnected.");
        ListConnectedClients(); // List clients after one disconnects
    }

    [Command]
    private void ListConnectedClients()
    {
        Debug.Log("listing clients");
        // Get the list of connected clients
        var connectedClients = NetworkManager.Singleton.ConnectedClientsList;
        Debug.Log("No. of connectedClients " + connectedClients.Count);
        // Display each connected client
        foreach (var client in connectedClients)
        {
            Debug.Log($"Client ID: {client.ClientId}");
        }

        Debug.Log(playerNames);

        foreach( var player in playerNames.Value){
            Debug.Log("player " + player);
        }

    }
    
    private void AddPlayerName(ulong clientId, string playerName)
    {
        // Add player info to the list
        Debug.Log("AddPlayerName " + clientId + playerName);
        playerNames.Value.Add(new PlayerData(clientId, playerName));
    }

        public struct PlayerData
    {
        public ulong clientId;
        public string name;

        public PlayerData(ulong id, string playerName)
        {
            clientId = id;
            name = playerName;
        }
    }
}
