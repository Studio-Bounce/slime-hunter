using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSpawner : MonoBehaviour
{
    public GameObject objectRef;

    private void Start()
    {
        Respawn();
    }

    public void Respawn()
    {
        Debug.Log("Respawn");
        StartCoroutine(RunRespawn());
    }

    IEnumerator RunRespawn()
    {
        CharacterController cc = objectRef.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            yield return null;
            objectRef.transform.position = transform.position;
            yield return null;
            cc.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        DebugExtension.DebugArrow(transform.position + new Vector3(0, 1, 0), Vector3.down, Color.blue);
    }
}
