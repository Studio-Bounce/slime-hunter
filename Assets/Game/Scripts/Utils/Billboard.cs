using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform cam;

    public bool liveUpdate = false;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (liveUpdate)
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);
        }
    }
#endif
}
