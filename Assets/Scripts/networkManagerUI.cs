using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;

public class networkManagerUI : MonoBehaviour
{

    [SerializeField] private Button startBtn;
    [SerializeField] private Button joinBtn;
    [SerializeField] private Button updateIPBtn;

    [SerializeField] private TMP_Text displayIPAddr;
    [SerializeField] private TMP_Text availableGame;
    [SerializeField] private TMP_InputField inputIPAddr;
    private SynchronizationContext mainThreadContext;
    private UnityTransport unityTransport;
    private UdpClient udpClient;
    private bool isListening = true;
    private string hostIp;

    public int broadcastPort = 7779;
    public string broadcastMessage = "KUYA04L_GAMEHOST";

    
    private void Awake(){

        startBtn.onClick.AddListener(()=>{
            StartGame();
        });

        joinBtn.onClick.AddListener(()=>{
            print("Join Btn Pressed");
            ConnectToHost();
        });

        updateIPBtn.onClick.AddListener(()=>{
            print("update IP btn clicked");
            UpdateAddress(displayIPAddr.text);
        });
    }

    void Start () {
        mainThreadContext = SynchronizationContext.Current;
        unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        udpClient = new UdpClient(broadcastPort);
        udpClient.BeginReceive(OnReceive, null);
        
        UpdateAddressDisplay();


    }

    private void StartGame(){
        print("startBtn clicked");
        udpClient.EnableBroadcast = true;
        NetworkManager.Singleton.StartHost();
        isListening = false;
        InvokeRepeating(nameof(BroadcastStartGame), 0, 1.0f);
    }

    private void BroadcastStartGame(){
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
        byte[] data = Encoding.UTF8.GetBytes(broadcastMessage);
        udpClient.Send(data, data.Length, endPoint);
        Debug.Log("Broadcast sent: " + broadcastMessage);
    }

    public void UpdateAddressDisplay()
    {
        if (unityTransport != null && displayIPAddr != null)
        {
            print("updating address display");
            mainThreadContext.Post(_ =>
            {
                string currentAddress = unityTransport.ConnectionData.Address;
                displayIPAddr.text = "IP: " + currentAddress;
            }, null);
            
            print(displayIPAddr.text );
        }
        else
        {
            Debug.LogError("UnityTransport or TMP_Text reference is missing.");
        }
    }

    public void UpdateAvailableGameDisplay()
    {
        if(availableGame != null){
            Debug.Log("UpdateAvailableGameDisplay");
            Debug.Log("Host IP: " + hostIp);
            try {
                mainThreadContext.Post(_ =>
                {
                    availableGame.text = "Host IP: " + hostIp;
                }, null);
                
            }catch (System.Exception ex){
                Debug.Log("UpdateAvailableGameDisplay Error " + ex);
            }
            
        } else {
            Debug.Log("Cannot update availableGame");
        }

    }

    public void UpdateAddress(string newAddress)
    {

        if (!string.IsNullOrEmpty(newAddress))
        {
            unityTransport.ConnectionData.Address = newAddress;
            Debug.Log("Address updated to: " + newAddress);

            // Refresh the displayed address
            UpdateAddressDisplay();
        }
        else
        {
            Debug.LogWarning("Address field is empty. Please enter a valid address.");
        }
    }

    private void OnReceive(IAsyncResult result){
        if (!isListening) return;
        
        IPEndPoint endPoint = new IPEndPoint( IPAddress.Any, broadcastPort);
        
        byte[] data = udpClient.EndReceive(result, ref endPoint);
        
        string message = Encoding.UTF8.GetString(data);
        
        Debug.Log($"Received broadcast from {endPoint.Address}: {message}");

        if (message == broadcastMessage){
            hostIp = endPoint.Address.ToString();
            Debug.Log("new ip " + hostIp);
            UpdateAvailableGameDisplay();
        } else {
            Debug.Log("not");
        }
        udpClient.BeginReceive(OnReceive, null);
    }

    private void ConnectToHost(){
        try
        {
            isListening = false;
            Debug.Log("Connecting to " + hostIp);
            UpdateAddress(hostIp);
            Debug.Log("Starting client");
            NetworkManager.Singleton.StartClient();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Client failed to start: {ex.Message}\n{ex.StackTrace}");
        }
    }

    void OnApplicationQuit(){
        isListening = false;
        udpClient?.Close();
    }
    


}
