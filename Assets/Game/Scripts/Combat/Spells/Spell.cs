using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Spell : MonoBehaviour
{
    public abstract void Cast(Vector3 target = default(Vector3));
}
