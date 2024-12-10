using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("HomeManager");
        playBtn play = FindFirstObjectByType<playBtn>();
        play.TriggerOnClick = () => { NetworkManagerController.Instance.HandleStartButton();};
        // Debug.Log(NetworkManagerController.Instance != null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
