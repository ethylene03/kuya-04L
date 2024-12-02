using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using WebSocketSharp;
using Kuya04LPlayer;
using System.Collections.Generic;
using QFSW.QC;


public class NetworkManagerController : MonoBehaviour
{
    public static NetworkManagerController Instance { get; private set; }
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


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Preserve across scenes
        }
    }

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
            // // playBtnScript.TriggerOnClick = JoinGame;
            // // playBtnScript.TriggerOnClick = () =>  NetworkManager.Singleton.StartClient();
            // playBtnScript.TriggerOnClick = () =>  {
            //     try {
            //         Debug.Log("Starting client");
            //         NetworkManager.Singleton.StartClient();
            //         Debug.Log("Done starting client. ");
            //         getAddress();

            //     } catch(SystemException ex){
            //         Debug.Log("Can't start. " + ex);
            //     }
                
            // };

            // // playBtnScript.TriggerOnClick = () =>  {
            // //     try {
            // //         Debug.Log("Starting Host");
            // //         NetworkManager.Singleton.StartHost();
            // //         Debug.Log("Done starting Host. ");
            // //         getAddress();

            // //     } catch(SystemException ex){
            // //         Debug.Log("Can't start Host. " + ex);
            // //     }
                
            // // };
        }
    }

    [Command]
    private void setAddress(string ipAddress, ushort port){
        unityTransport.ConnectionData.Address = ipAddress;
        unityTransport.ConnectionData.Port = port;
        Debug.Log("Done changing address.");
        getAddress();
    }

    [Command]
    private void getAddress(){
        Debug.Log("Connected to: " + unityTransport.ConnectionData.Address + ":" + unityTransport.ConnectionData.Port);
    }

    [Command]
    private string GetLocalIPAddress()
    {
        foreach (var networkInterface in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (networkInterface.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return networkInterface.ToString();
            }
        }
        return null; // No suitable IP address found
    }

    [Command]
    private string GetHotspotIPAddress(){
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && 
                networkInterface.OperationalStatus == OperationalStatus.Up)
            {
                foreach (var ip in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.Address.ToString();
                    }
                }
            }
        }

        return null; // Hotspot IP address not found
    }

    [Command]
    private void FindGatewayIPAddress(){
        foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus == OperationalStatus.Up)
            {
                var properties = networkInterface.GetIPProperties();
                foreach (var gateway in properties.GatewayAddresses)
                {
                    Debug.Log($"Gateway IP: {gateway.Address}");
                }
            }
        }
    }

    private void HandleStartButton()
    {
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

            Debug.Log("Creating game.");

            getAddress();
            
            string localIPAddress = GetLocalIPAddress();

            if (!localIPAddress.IsNullOrEmpty()){
                unityTransport.ConnectionData.Address = localIPAddress;

                // Send broadcast message every one second
                InvokeRepeating(nameof(BroadcastStartGame), 0, 1.0f);

                NetworkManager.Singleton.StartHost();
            } 


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


            getAddress();

            // Join active game
            try {
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
        
        Debug.Log($"Received broadcast from {endPoint.Address}:{endPoint.Port}: {message}");

        if (message == START_GAME_MESSAGE){

            // store ip address of host
            hostIp = endPoint.Address.ToString();

            Debug.Log("Host game detected: " + hostIp);
        } else {
            Debug.Log("Broadcast received but not from the game.");
        }

        udpClient.BeginReceive(OnReceive, null);
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        if (NetworkManager.Singleton != null){
            // Subscribe to client connected and disconnected events
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null){
            // Unsubscribe from events to prevent memory leaks
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        } 

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

        ListConnectedClients(); // List clients when a new client connects
    }


    [Command]
    private void checkIsClient(){
        Debug.Log("Is client " + NetworkManager.Singleton.IsClient);
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

    }
}
