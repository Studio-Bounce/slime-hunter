using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WanderBoundary : MonoBehaviour
{
    public Vector3 Center { get { return transform.position; } }
    public abstract bool InBounds(Vector3 pos);
}
