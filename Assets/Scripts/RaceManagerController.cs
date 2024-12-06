using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManagerController : MonoBehaviour
{
    [SerializeField] private List<GameObject> jeepPrefab;
    [SerializeField] private VariableJoystick movementJoystick;


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

    private void SpawnJeep(){

        // if(!IsOwner) return;

        List<ulong> clients = (List<ulong>)NetworkManager.Singleton.ConnectedClientsIds;
        Debug.Log("List of clients ");
        Debug.Log(clients);
        int ownerIndex = Random.Range(0, jeepPrefab.Count);

        Vector3 basePosition = new Vector3(-0.2f, -1, 0);
        float xOffset = 1.5f;
        basePosition += (1 + ownerIndex) * new Vector3( xOffset, 0, 0);

        GameObject selectedJeep = jeepPrefab[ownerIndex];
        GameObject car = Instantiate(selectedJeep, basePosition, Quaternion.identity);
        car.GetComponent<NetworkObject>().Spawn();


        carControl carController = car.GetComponent<carControl>();
        
        if (carController != null && movementJoystick != null)
        {
            Debug.Log("Assigning movementJoystick");
            VariableJoystick joyStick = Instantiate(movementJoystick);
            // carController.movementJoystick = joyStick;
        } else {
            Debug.Log("Can't find carController");
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
