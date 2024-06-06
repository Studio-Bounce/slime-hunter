using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassPosition : MonoBehaviour

{
    private GameObject tracker;

    private Material grassMat;

    // Start is called before the first frame update
    void Start()
    {
        grassMat = GetComponent<Renderer>().material;
        tracker = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (tracker != null)
        {
            Vector3 trackerPos = tracker.GetComponent<Transform>().position;
            grassMat.SetVector("_trackerPosition", trackerPos);
        }
    }
}
