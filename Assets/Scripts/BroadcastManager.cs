using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class BroadcastManager
{
    private int broadcastPort;
    private UdpClient udpClient;
    private IPEndPoint broadcastEndpoint;
    private bool isListening;


    public int BroadcastPort
    {
        get => broadcastPort;
        set
        {
            if (isListening)
            {
                Debug.LogError("Cannot change port while listening.");
                return;
            }
            broadcastPort = value;
        }
    }

    public bool IsListening
    {
        get => isListening;
        set
        {
            if (value) StartListening();
            else StopListening();
        }
    }

        // Delegate definition for handling received messages
    public delegate void OnReceiveHandler(string receivedMessage, IPEndPoint senderEndpoint);

    // Assignable delegate property
    public OnReceiveHandler HandleOnReceive { get; set; }

    public BroadcastManager(int port)
    {
        broadcastPort = port;
        isListening = false;
        HandleOnReceive = DefaultHandleOnReceive;
        udpClient = new UdpClient(broadcastPort);
        udpClient.EnableBroadcast = true;

        broadcastEndpoint = new IPEndPoint(IPAddress.Parse(GetBroadcastAddress()), broadcastPort);
    }

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

    public string GetLocalIPAddress()
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
        return null; 
    }

    public void SendBroadcast(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            Debug.LogError("Cannot send an empty message.");
            return;
        }

        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        Debug.Log("SendBroadcast " + udpClient);
        udpClient.Send(messageBytes, messageBytes.Length, broadcastEndpoint);
        Debug.Log($"Broadcast sent: {message}. via {broadcastEndpoint}");
    }

    private void StartListening()
    {
        if (isListening) return;

        isListening = true;
        
        Debug.Log($"Listening for broadcasts on port {broadcastPort}...");
        BeginReceive();
    }

    private void StopListening()
    {
        if (!isListening) return;

        isListening = false;
        udpClient?.Close();
        udpClient = null;
        Debug.Log("Stopped listening.");
    }

    private void BeginReceive()
    {
        udpClient.BeginReceive(OnReceive, null);
    }

    private void OnReceive(IAsyncResult result)
    {
        if (!isListening || udpClient == null) return;

        IPEndPoint senderEndpoint = new IPEndPoint(IPAddress.Any, broadcastPort);
        byte[] receivedBytes = udpClient.EndReceive(result, ref senderEndpoint);
        string receivedMessage = Encoding.UTF8.GetString(receivedBytes);


        HandleOnReceive(receivedMessage, senderEndpoint);

        if (isListening) BeginReceive(); // Continue listening
    }

    private void DefaultHandleOnReceive(string receivedMessage, IPEndPoint senderEndpoint){

        Debug.Log($"Message received from {senderEndpoint.Address}: {receivedMessage}");
    }

    public void CloseBroadcast(){
        // Stop listening if currently active
        if (isListening)
        {
            StopListening();
        }

        // Dispose of the UDP client
        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
        }

        Debug.Log($"Broadcast on port {broadcastPort} has been closed.");
    }
}
