using Unity.Netcode;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private GameObject kartPrefab;
    public KartScriptableObject playerPref;
    private GameObject _kartPlayer;

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback +=
            OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback +=
            OnClientDisconnectCallback;
    }
    
    private void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log("Client " + clientId + " connected");
        if (!NetworkManager.Singleton.IsServer || kartPrefab == null) return;
        
        // Instanziierung des GameObject
        _kartPlayer = Instantiate(
            kartPrefab,
            playerPref.spawnPoints[(int) clientId % 4],
            transform.rotation);
        
        // Spawn PlayerObject
        var playerNet = _kartPlayer.GetComponent<NetworkObject>();  
        playerNet.SpawnAsPlayerObject(clientId);
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer || kartPrefab == null) return;
        _kartPlayer.GetComponent<NetworkObject>();
        var playerNet = _kartPlayer.GetComponent<NetworkObject>();  
        playerNet.Despawn(true);
    }
}
