using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class NetworkManagerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  // Port number to send the message. This should match with the listening device.
    public int BROADCAST_PORT = 7778;
    public string hostIp;

    private UdpClient udpClient;
    private UnityTransport unityTransport;
    private Boolean isSearchingGame = true;
    private string START_GAME_MESSAGE = "KUYA04L_GAMEHOST";

    [SerializeField] private playBtn playBtnScript;



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
        if (hostIp.IsNullOrEmpty()){
            Debug.Log("No active game session. Creating a game in the network.");
            CreateGame();
        } else {
            Debug.Log("There is active game session in ip: " + hostIp);
            JoinGame();
            // Handle joining game
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

            // NetworkManager.Singleton.StartHost();

        } catch (SystemException e){
            Debug.Log(e);
        }
    }

    private void JoinGame(){
        Debug.Log("Joining game at: " + hostIp);
        isSearchingGame = false;
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
            // NetworkManager.Singleton.StartClient();
            Debug.Log("Host game detected: " + hostIp);
        } else {
            Debug.Log("Broadcast received but not from the game.");
        }

        udpClient.BeginReceive(OnReceive, null);
    }
}
