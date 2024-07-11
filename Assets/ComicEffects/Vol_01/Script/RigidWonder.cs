using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidWonder : MonoBehaviour {

    private Rigidbody _rigidBody;

    [SerializeField]
    private float _wonderSpeed = 1f;

    [SerializeField]
    private float _maxWonderDelay = 2f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        StartCoroutine(Wonder());
    }


    IEnumerator Wonder()
    {
        while (true) {
            Vector3 direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            float endTime = Time.time + Random.Range(0f,_maxWonderDelay);
            while (Time.time <= endTime) {
                _rigidBody.velocity = direction * _wonderSpeed;
                yield return null;
            }
        }
    }
}
