using UnityEngine;
using Unity.Netcode;

public class CarController : NetworkBehaviour
{    
    public KartScriptableObject playerPref;

    // WheelCollider
    public WheelCollider leftFrontWheel;
    public WheelCollider rightFrontWheel;
    public float maxTorque = 1000.0F;
    public float maxBrakeTorque = 100.0F;
    public float steerAngle = 20;
    public float maxSpeed = 1000;
    // Nach unten gerichtete Kraft zur Fahrzeugstabilisierung
    public float downforce = 200f;
    // Zulässige Schlupfgrenze
    public float slipLimit = 0.3F;
    public float currentMotorTorque = 0;
    private Rigidbody myRigidbody;
    // Puffervariable fuer die Geschwindigkeit
    private float velocityMagnitude = 0;
    // Network variables
    private Transform kartPrefab;
    private NetworkVariable<float> nVertical = new NetworkVariable<float>(
        0F,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> nHorizontal = new NetworkVariable<float>(
        0F,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> nBreak = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    
    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        var playerModel = transform.Find("KartVisual/PlayerIdle/Template_Character");
        var renderer = playerModel.gameObject.GetComponent<SkinnedMeshRenderer>();
        renderer.material.SetColor(
            "_BaseColor",
            playerPref.playerColors[(int) OwnerClientId % 4]);
    }

    void FixedUpdate()
    {
        if (!IsServer) return;
        float thrustTorque = 0;
        thrustTorque = currentMotorTorque * nVertical.Value;
        // Vektorlaenge der aktuellen Geschwindigkeit zwischenspeichern
        velocityMagnitude = myRigidbody.velocity.magnitude;
        float speed = velocityMagnitude;
        // Geschwindigkeit in km/h umrechnen
        speed *= 3.6f;
        // Wenn Max-Geschwindigkeit erreicht, dann Drehmoment auf 0 setzen
        if (speed >= maxSpeed)
            thrustTorque = 0;
        // currentMotorTorque wird auf die 2 Antriebsraeder aufgeteilt
        leftFrontWheel.motorTorque = thrustTorque / 2;
        rightFrontWheel.motorTorque = thrustTorque / 2;
        if (nBreak.Value)
        {
            leftFrontWheel.brakeTorque = maxBrakeTorque;
            rightFrontWheel.brakeTorque = maxBrakeTorque;
        }
        else
        {
            leftFrontWheel.brakeTorque = 0;
            rightFrontWheel.brakeTorque = 0;
        }
        leftFrontWheel.steerAngle = steerAngle * nHorizontal.Value;
        rightFrontWheel.steerAngle = steerAngle * nHorizontal.Value;
        // Fahrzeugstabilisierung
        AddDownForce();
        // Durchdrehen verhindern
        TractionControl();
    }

    // Methode zum Verhindern des Reifendurchdrehens
    private void TractionControl()
    {
        WheelHit wheelHit;
        // Reifen-Kontakt-Informationen vom linken Rad abfragen
        leftFrontWheel.GetGroundHit(out wheelHit);
        // Schlupf zum Anpassen des Drehmoments uebergeben
        AdjustTorque(wheelHit.forwardSlip);
        // Analog für das rechte Rad
        rightFrontWheel.GetGroundHit(out wheelHit);
        AdjustTorque(wheelHit.forwardSlip);
    }

    // Drehmoment anpassen
    private void AdjustTorque(float forwardSlip)
    {
        // Ueberschreitet Schlupf Grenzwert wird currentMotorTorque reduziert
        if (forwardSlip >= slipLimit && currentMotorTorque >= 0)
        {
            currentMotorTorque -= 10;
        }
        else
        {
            // ansonsten wird currentMotorTorque angehoben
            currentMotorTorque += 10;
            // Begrenzung von currentMotorTorque auf den Maximalwert
            if (currentMotorTorque > maxTorque)
                currentMotorTorque = maxTorque;
        }
    }

    // Fahrzeugstabilitaet und Bodenhaftung
    private void AddDownForce()
    {
        myRigidbody.AddForce(-transform.up * downforce * velocityMagnitude);
    }

    void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKey(KeyCode.Space))
            nBreak.Value = true;
        else 
            nBreak.Value = false;
        nHorizontal.Value = Input.GetAxis("Horizontal");
        nVertical.Value = Input.GetAxis("Vertical");
    }
}
