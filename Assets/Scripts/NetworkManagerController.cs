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
using Unity.VisualScripting;


public class NetworkManagerController : MonoBehaviour
{
    public static NetworkManagerController Instance { get; private set; }
    [SerializeField] private playBtn playBtnScript;

    // Port number to send the message. This should match with the listening device.
    private int BROADCAST_PORT = 7778;
    public string hostIp;

    private UdpClient udpClient;
    private UnityTransport unityTransport;
    private Boolean isSearchingGame = true;
    private string START_GAME_MESSAGE = "KUYA04L_GAMEHOST";

    
    private  int MAX_CLIENTS = 2;

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
            if (
                networkInterface.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && 
                networkInterface.ToString().StartsWith("192")
            )
            {
                return networkInterface.ToString();
            }
        }
        throw new SystemException("No local ip address found."); 
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
        // Do not listen to other broadcast that is searching a game. One active game at a time.
        isSearchingGame = false;

        // Enable Broadcasting
        udpClient.EnableBroadcast = true;

        Debug.Log("Creating game.");

        getAddress();

        // try {
            string localIPAddress = GetLocalIPAddress();

            if (!localIPAddress.IsNullOrEmpty()){
                unityTransport.ConnectionData.Address = localIPAddress;

                // Send broadcast message every one second
                InvokeRepeating(nameof(BroadcastStartGame), 0, 1.0f);

                NetworkManager.Singleton.StartHost();
            } 
        // } catch (SystemException ex){
        //     Debug.Log("CreateGame error: " + ex);
        // }
            
    }

    [Command]
    private void CloseGameInvite(){
        CancelInvoke(nameof(BroadcastStartGame));
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
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(GetBroadcastAddress()), BROADCAST_PORT);

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
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, BROADCAST_PORT);
        
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

    [Command]
    private string GetBroadcastAddress()
    {
        string subnetMask = "255.255.255.0";
        try
        {
            // Get local IP address
            string localIP = GetLocalIPAddress();
            // Use a default subnet mask for typical hotspots (255.255.255.0)
           

            // Calculate broadcast address
            string[] ipParts = localIP.Split('.');
            string[] maskParts = subnetMask.Split('.');
            string[] broadcastParts = new string[4];

            for (int i = 0; i < 4; i++)
            {
                broadcastParts[i] = (int.Parse(ipParts[i]) | (~int.Parse(maskParts[i]) & 255)).ToString();
            }

            return string.Join(".", broadcastParts);
        }
        catch
        {
            return subnetMask; // Return null if an error occurs
        }
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        if (NetworkManager.Singleton != null){
            // Subscribe to client connected and disconnected events
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApproval;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null){
            // Unsubscribe from events to prevent memory leaks
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.ConnectionApprovalCallback -= OnConnectionApproval;
        } 

    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("Client Connected " + clientId);

        ListConnectedClients(); // List clients when a new client connects
    }


    private void OnConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("Connection request received.");

        // Check the current number of connected clients
        int connectedClients = NetworkManager.Singleton.ConnectedClients.Count;

        if (connectedClients < MAX_CLIENTS)
        {
            // Approve connection but don't create player object automatically
            response.Approved = true;
            response.CreatePlayerObject = false; // Prevent automatic player object creation
            response.Pending = false;

            Debug.Log($"Connection approved. Current connected clients: {connectedClients + 1}/{MAX_CLIENTS}");
        }
        else
        {
            // Reject connection if the limit is exceeded
            response.Approved = false;
            response.Pending = false;

            Debug.LogWarning("Connection rejected. Server is full.");
        }
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
