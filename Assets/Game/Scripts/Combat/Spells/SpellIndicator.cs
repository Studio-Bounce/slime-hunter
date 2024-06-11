using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SpellIndicator : MonoBehaviour
{
    public abstract void ShowIndicator();

    public abstract void HideIndicator();
}
