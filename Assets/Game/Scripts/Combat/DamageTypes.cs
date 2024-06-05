using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Damage
{
    public float value;
    public float knockback;
    public Vector3 direction;
    public StatusEffect effect;
}