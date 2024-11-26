using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;

public class networkManagerUI : MonoBehaviour
{

    [SerializeField] private Button startBtn;
    [SerializeField] private Button joinBtn;
    [SerializeField] private Button updateIPBtn;

    [SerializeField] private TMP_Text displayIPAddr;
    [SerializeField] private TMP_InputField inputIPAddr;

    private UnityTransport unityTransport;
    
    private void Awake(){

        startBtn.onClick.AddListener(()=>{
            print("startBtn clicked");
            NetworkManager.Singleton.StartHost();
        });

        joinBtn.onClick.AddListener(()=>{
            print("Join Btn Pressed");
            try
            {
                NetworkManager.Singleton.StartClient();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Client failed to start: {ex.Message}\n{ex.StackTrace}");
            }
            
        });

        updateIPBtn.onClick.AddListener(()=>{
            print("update IP btn clicked");
            UpdateAddress();
        });
    }

    void Start () {
        unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        UpdateAddressDisplay();
    }

    public void UpdateAddressDisplay()
    {
        if (unityTransport != null && displayIPAddr != null)
        {
            print("updating address display");
            string currentAddress = unityTransport.ConnectionData.Address;
            displayIPAddr.text = "IP: " + currentAddress;
            print(displayIPAddr.text );
        }
        else
        {
            Debug.LogError("UnityTransport or TMP_Text reference is missing.");
        }
    }

    public void UpdateAddress()
    {
        string newAddress = inputIPAddr.text;

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


}
