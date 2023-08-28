using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : NetworkBehaviour
{
    public GameObject cameraHolder;
    [SerializeField] private float speed = 0.125F;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;
    
    void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name != "MainScene") return;
        Vector3 targetPosition =
            transform.position + transform.rotation * positionOffset;
        Vector3 lerpPosition = Vector3.Lerp(
            cameraHolder.transform.position,
            targetPosition,
            speed);
        cameraHolder.transform.position = lerpPosition;
        Quaternion targetRotation =
            transform.rotation * Quaternion.Euler(rotationOffset);
        Quaternion lerpRot = Quaternion.Lerp(
            cameraHolder.transform.rotation,
            targetRotation, 
            speed);
        cameraHolder.transform.rotation = lerpRot;
     
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        cameraHolder.SetActive(true);
        base.OnNetworkSpawn();
    }
}
 