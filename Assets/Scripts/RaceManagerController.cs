using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManagerController : MonoBehaviour
{
    [SerializeField] private List<GameObject> jeepPrefab;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded");
        if (NetworkManager.Singleton.IsServer)
        {
            SpawnPlayerJeeps();
        }
    }

    private void SpawnPlayerJeeps()
    {
        Debug.Log("SpawnPlayers");
        int index = 0;
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log("Spawning jeep for client " + clientId);
            GameObject selectedJeep = jeepPrefab[index];
            // Instantiate the car prefab
            GameObject car = Instantiate(selectedJeep);

            // Assign ownership to the respective client
            car.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            index ++;
        }
    }
}
