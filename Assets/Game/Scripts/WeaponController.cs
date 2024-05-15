using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponController : MonoBehaviour
{
    [Tooltip("Assumes the weapons prefab handle is at positioned at origin (0,0,0)")]
    public Transform handPivot;
    public Vector3 handPivotOffset;
    public Vector3 handPivotForward;
    public WeaponSO weaponSO;

    void Start()
    {
        Debug.Assert(handPivot != null, "Requires hand location for weapon");
        Debug.Assert(weaponSO != null, "Requires weapon scriptable object");

        // This assumes the weaponModel has the handle position at (0, 0)
        GameObject weaponGO = Instantiate(weaponSO.weaponModel, handPivot);
        weaponGO.transform.forward = handPivotForward;
        weaponGO.transform.position += handPivotOffset;
    }

    void Update()
    {
        
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(handPivot.position + handPivotOffset, 0.03f);
            Gizmos.DrawRay(handPivot.position + handPivotOffset, handPivotForward);
        }
    }
#endif
}
