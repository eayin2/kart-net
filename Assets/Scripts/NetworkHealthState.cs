using System;
using System.Text.RegularExpressions;
using Unity.Netcode;
using UnityEngine;

public class NetworkHealthState : NetworkBehaviour
{
    [HideInInspector]
    public NetworkVariable<int> healthPoint = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        healthPoint.Value = 100;
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        if (!IsServer) return;
        if (!collider.name.Contains("KartBall")) return;
        var kartBallClientId = Convert.ToUInt64(Regex.Match(
            collider.name, @"\d+").Value);
        if (this.OwnerClientId == kartBallClientId) return;
        healthPoint.Value -= 10;
        if (healthPoint.Value <= 0) GameOver(this.OwnerClientId);
    }

    private void GameOver(ulong clientId)
    {
        var conClients = NetworkManager.ConnectedClientsList;
        foreach (NetworkClient nClient in conClients)
            Debug.Log("Client: " + nClient.ClientId);
        // Objekt wird nicht hier sondern im ConnectionManager despawned
        NetworkManager.Singleton.DisconnectClient(clientId);
        if (conClients.Count > 1) return;
        // Server herunterfahren, wenn nur noch 1 Client verbunden ist
        NetworkManager.Singleton.Shutdown();
    }
}
