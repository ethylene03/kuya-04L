using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class networkManagerUI : MonoBehaviour
{

    [SerializeField] private Button startBtn;
    [SerializeField] private Button joinBtn;
    
    private void Awake(){

        startBtn.onClick.AddListener(()=>{
            NetworkManager.Singleton.StartHost();
        });

        joinBtn.onClick.AddListener(()=>{
            print("Join Btn Pressed");
            NetworkManager.Singleton.StartClient();
        });
    }

}
