using System;
using System.Collections.Concurrent;
using System.Net;
using QFSW.QC;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class trackMove : MonoBehaviour
{
    Vector2 offset;
    public GameObject crossingObject;
    public GameObject[] landmarks;
    private float spawnCrossingAt;
    public float[] landmarkDistance = new float[]{ 10f, 20f };
    private float spawnLandmarksAt;
    private int landmarkIdx;
    public GameObject gameOverText;
    public carControl playerCar;
    private BroadcastManager broadcastManager;
    private GameConstants gameConstants;
    public GameObject winText;
    public GameObject loseText;
    private bool isWinner = false;
    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();


    void SpawnObjectAtOffset(GameObject obj)
    {
        Vector3 spawnPosition = new Vector3(obj.transform.localPosition.x, 7.4f, obj.transform.localPosition.z);
        Instantiate(obj, spawnPosition, obj.transform.rotation);
    }

    void Start() {
        Debug.Log("trackMove Start");
        isWinner = false;
        gameConstants = new GameConstants();
        Debug.Log("gameConstants " + gameConstants);
        broadcastManager = new BroadcastManager(gameConstants.GAME_OVER_PORT);
        broadcastManager.HandleOnReceive = broadcastReceive;
        broadcastManager.IsListening = true;
        Debug.Log("broadcastManager " + broadcastManager);

        playerCar.currentOffset.Value = 0;
        spawnCrossingAt = Random.Range(playerCar.currentOffset.Value + 5f, playerCar.currentOffset.Value + 15f);
        spawnLandmarksAt = Random.Range(landmarkDistance[0], landmarkDistance[1]);
        landmarkIdx = 0;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.P)) {
            globalVariables.startGame = !globalVariables.startGame;
        }

        if (!playerCar){
            Debug.Log("PlayerCar not attached.");
            return;
        }

        while (messageQueue.TryDequeue(out string message))
        {
            Debug.Log("Dequeueing " + message);
            // Process the dequeued message
            EndGame(message);
        }


        if(globalVariables.startGame) {
            Time.timeScale = 1;

            // calculate offset base on player speed
            float offsetValue = Mathf.Clamp(globalVariables.playerSpeed, 0, globalVariables.maxPlayerSpeed);
            globalVariables.offsetValue = offsetValue;
            playerCar.currentOffset.Value += offsetValue;
            // Debug.Log("offset: " + offsetValue + ", currOffset: " + playerCar.currentOffset.Value);
            
            MoveTrack();

            AdjustOpponentCars();

            // spawn crossing
            if(playerCar.currentOffset.Value >= spawnCrossingAt) {
                SpawnObjectAtOffset(crossingObject);
                spawnCrossingAt = Random.Range(playerCar.currentOffset.Value + 5f, playerCar.currentOffset.Value + 15f);
            }

            // spawn landmarks
            float maxDistance = 10f;
            if(playerCar.currentOffset.Value >= maxDistance) {
                // abot na sa SM
                SpawnObjectAtOffset(landmarks[3]);

                
                HandleWinner();
            } else if(playerCar.currentOffset.Value >= spawnLandmarksAt && landmarkIdx < 3) {
                SpawnObjectAtOffset(landmarks[landmarkIdx]);
                spawnLandmarksAt = Random.Range(playerCar.currentOffset.Value + landmarkDistance[0], playerCar.currentOffset.Value + landmarkDistance[1]);

                landmarkIdx = (landmarkIdx + 1) % 4;
            }
        } else {
            Time.timeScale = 0;
        }
    }

    [Command]
    public void HandleWinner(){
        isWinner = true;
        broadcastManager.SendBroadcast(gameConstants.GAME_OVER_LOSE);
        broadcastManager.SendBroadcast(gameConstants.GAME_OVER_LOSE);
        EndGame(gameConstants.GAME_OVER_WIN);
    }

    [Command]
    public void TestLose(){
        EndGame(gameConstants.GAME_OVER_LOSE);
    }

    [Command]

    public void TestWin(){
        EndGame(gameConstants.GAME_OVER_WIN);
    }

    public void MoveTrack() {
        offset = new Vector2(0, playerCar.currentOffset.Value);
        // Debug.Log("trackMove offset: " + offset);

        GetComponent<Renderer> ().material.mainTextureOffset = offset;
    }

    void broadcastReceive(string receivedMessage = null, IPEndPoint endPoint = null){
        messageQueue.Enqueue(receivedMessage);
    }

    void EndGame(string receivedMessage = null) {
        try {
            if(isWinner && receivedMessage == gameConstants.GAME_OVER_LOSE) return;
            Debug.Log("EndGame "+ receivedMessage);
            globalVariables.startGame = false;
            
            if(loseText == null || winText == null) return;
            
            Debug.Log("endgame 2");
            Debug.Log("gameConstants "  + gameConstants);
            gameConstants = new GameConstants();
            Debug.Log("gameConstants 2 "  + gameConstants);

            
            if(receivedMessage == gameConstants.GAME_OVER_WIN){
                Debug.Log("Win");
                // winText.SetActive(true);
                // loseText.SetActive(false);
                TMP_Text winTMP_Text= winText.GetComponent<TMP_Text>();
                winTMP_Text.text = "You Win";
            } else if (receivedMessage == gameConstants.GAME_OVER_LOSE) {
                Debug.Log("Lose");
                // winText.SetActive(false);
                // loseText.SetActive(true);
                TMP_Text winTMP_Text= winText.GetComponent<TMP_Text>();
                winTMP_Text.text = "You Lose";
            }

            
            Time.timeScale = 0;
            new WaitForSecondsRealtime(5f);
            SceneManager.LoadScene("players-board");
        } catch (SystemException e){
            Debug.Log(e);
        }
    }

    void AdjustOpponentCars(){
        // Fetch all opponent cars and update their positions based on their synced speed
        var opponentCars = GameObject.FindGameObjectsWithTag("PlayerCar");
        float ownerOffset = 0f;

        if (opponentCars.Length == 1){
            carControl carObj = opponentCars[0].GetComponent<carControl>();
            if(carObj.IsOwner){
                EndGame(gameConstants.GAME_OVER_WIN);
            }
        }
       
        foreach (var car in opponentCars) {
            carControl carObj = car.GetComponent<carControl>();

            if (carObj.IsOwner){
                ownerOffset = carObj.currentOffset.Value;
            }
        }

        // Debug.Log("ownerOffset " + ownerOffset);

        foreach (var car in opponentCars)
        {
            carControl carObj = car.GetComponent<carControl>();
            // Debug.Log("CarObj id: " + carObj.OwnerClientId);
            // Debug.Log("position: " + car.transform.position);

            if (!carObj.IsOwner)
            {
                float newY = car.transform.position.y + ((carObj.currentOffset.Value - playerCar.currentOffset.Value) * 10);
                car.transform.localPosition = new Vector3(car.transform.position.x, newY, car.transform.position.z);
            }
        }
    }

}
