using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoMove : MonoBehaviour {

    public float Speed = 0.25f;

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * Speed);
        transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * Speed * 20);
    }
}
