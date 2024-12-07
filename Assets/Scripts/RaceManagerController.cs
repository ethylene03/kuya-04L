using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
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

        // Define the base position and offsets for the grid
        Vector3 basePosition = new Vector3(-0.2f, -1, 0); // Starting position (adjust as needed)
        float xOffset = 1.5f; // Horizontal distance between jeeps

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log("Spawning jeep for client " + clientId);
            GameObject selectedJeep = jeepPrefab[index];


            basePosition += (index + 1) * new Vector3( xOffset, 0, 0);


            // Instantiate the car prefab
            GameObject car = Instantiate(selectedJeep, basePosition, Quaternion.identity);

            // Assign ownership to the respective client
            car.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

            index ++;
        }
    }
}
