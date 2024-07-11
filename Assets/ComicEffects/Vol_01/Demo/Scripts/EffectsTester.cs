using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsTester : MonoBehaviour {

    public GameObject testEffect;

    public void SpawnTestEffect()
    {
        if(testEffect != null) {
            GameObject.Instantiate(testEffect);
        }
    }
}
