using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element
{
    Physical,
    Fire,
    Ice,
    Toxin
}

[Serializable]
public struct Damage
{
    public int value;
    public int knockback;
    public Vector3 direction;
    public Element element;
}