using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private void Update()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mouseRay.origin, mouseRay.direction, out RaycastHit hitInfo, 100.0f);
        transform.position = hitInfo.point;
    }
}
