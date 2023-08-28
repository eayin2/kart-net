using System.Collections;
using UnityEngine;
using Unity.Netcode;

[System.Serializable]
public class ShootBall : NetworkBehaviour
{
    [SerializeField] private GameObject kbPrefab;
    [Tooltip("BallRoot is a child of CarRoot")] [SerializeField]
    private AudioSource shootSoundEffect; 

    void Update()
    {
        if (!IsLocalPlayer) return;
        if (Input.GetButtonDown("Fire1"))
            ShootServerRpc();
    }
    
    [ServerRpc]
    private void ShootServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        
        // Create KartBall
        var kbStartPos = new Vector3(
            transform.position.x + transform.forward.x * 2,
            0.25F,
            transform.position.z + transform.forward.z * 2);
        GameObject kb = (GameObject)Instantiate(
            kbPrefab,
            kbStartPos,
            Quaternion.identity,
            transform);
        kb.name = "KartBall" + clientId;
        kb.transform.rotation = Quaternion.LookRotation(transform.forward);
        
        // Spawn KartBall
        var kbNet = kb.GetComponent<NetworkObject>();
        kbNet.Spawn();
        
        // Schussrichtung bestimmen und Ball abschiessen
        var rb = GetComponent<Rigidbody>();
        var shootDirection = new Vector3(
            transform.forward.x,
            0, 
            transform.forward.z);
        shootSoundEffect.Play();
        kb.GetComponent<Rigidbody>().AddForce(shootDirection * 30000.0f);

        // Despawn after 10 s
        StartCoroutine(DespawnKb(kb));
    }

    IEnumerator DespawnKb(GameObject kb)
    {
        yield return new WaitForSeconds(10f);
        var kbNet = kb.GetComponent<NetworkObject>();
        kbNet.Despawn();
        Destroy(kb);
        yield return null;
    }
}
