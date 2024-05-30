using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Damage
{
    public int value;
    public int knockback;
    public Vector3 direction;
    public StatusEffect effect;
}