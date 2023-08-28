using Unity.Netcode;
using UnityEngine;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField]
    private RectTransform HealthScale;

    private void OnDisable()
    {     
        GetComponent<NetworkHealthState>().healthPoint.OnValueChanged -=
            UpdateHealth;
    }
    
    private void OnEnable()
    {
        GetComponent<NetworkHealthState>().healthPoint.OnValueChanged +=
            UpdateHealth;
    }
    
    private void UpdateHealth(int previousValue, int newValue)
    {
        HealthScale.transform.localScale = new Vector3(
            newValue / 100f, 1, 1);
    }
}