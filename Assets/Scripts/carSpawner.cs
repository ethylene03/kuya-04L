using Unity.Netcode;
using UnityEngine;

public class carSpawner : NetworkBehaviour
{

    public GameObject car;
    public float maxPos = 2.0f;
    public float delayTimer = 5f;
    float timer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = delayTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        timer -= Time.deltaTime;

        if(timer <= 0) {
            Vector3 carPos = new Vector3(Random.Range(-maxPos, maxPos), transform.position.y, transform.position.z);
            CarSpawnerServerRPC (carPos);
            timer = delayTimer;
        }
    }

    [ServerRpc]
    private void CarSpawnerServerRPC(Vector3 carPosition){
        GameObject carInstance = Instantiate(car, carPosition, transform.rotation);
        carInstance.GetComponent<NetworkObject>().Spawn();
    }
}
