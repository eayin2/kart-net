using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CenterOfMass : MonoBehaviour {

    [SerializeField] Vector3 centerOfMass = Vector3.zero;

    private void Start()
    {
        // Override the center of mass, to enhance stability
        GetComponent<Rigidbody>().centerOfMass += centerOfMass;
    }

    // Called by the editor to show "gizmos" in the Scene view. Used to
    // help visualize the overriden center of mass.
    private void OnDrawGizmosSelected()
    {
        // Draw a green sphere where the updated center of mass will be.
        Gizmos.color = Color.green;

        var currentCenterOfMass =
            this.GetComponent<Rigidbody>().worldCenterOfMass;

        Gizmos.DrawSphere(currentCenterOfMass + centerOfMass, 0.125f);
    }
}
