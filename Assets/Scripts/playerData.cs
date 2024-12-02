using Unity.Netcode;
using UnityEngine;

namespace Kuya04LPlayer {
    public class PlayerData : NetworkBehaviour
    {
        public NetworkVariable<int> score = new NetworkVariable<int>(0);  // Example: Player score
        public NetworkVariable<string> playerName = new NetworkVariable<string>("Player");  // Example: Player name

        // Random name generator for example purposes
        private static string[] playerNames = { "Alpha", "Bravo", "Charlie", "Delta", "Echo" };

        public override void OnNetworkSpawn()
        {
            if (IsOwner) // Only assign a name to the local player
            {
                playerName.Value = playerNames[Random.Range(0, playerNames.Length)];
            }
        }

        // Method to update score (can be called by the server)
        public void UpdateScore(int newScore)
        {
            if (IsServer) // Only the server can modify score
            {
                score.Value = newScore;
            }
        }
    }
}