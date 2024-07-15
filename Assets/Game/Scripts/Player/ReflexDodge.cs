using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class ReflexDodge : MonoBehaviour
{
    public Player player;
    public LayerMask hurtLayerMask;

    public float dodgeRadius = 1.0f;
    public float dodgeSlowMultiplier = 0.5f;
    public float dodgeSlowLength = 1.0f;

    private bool _isReflex = false;
    private PlayerController _playerController;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = player?.GetComponent<PlayerController>();
        _playerController.OnPlayerDashEvent += CheckReflexDodge;
    }

    private void CheckReflexDodge()
    {
        // Check if we're in reflex mode or not
        if (_isReflex) return;

        // Perform OverlapSphere check
        Collider[] colliders = Physics.OverlapSphere(transform.position, dodgeRadius, hurtLayerMask);

        foreach (Collider collider in colliders)
        {
            DamageDealer damageDealer = collider.GetComponent<DamageDealer>();
            if (damageDealer == null) continue;
            if (!damageDealer.Active) continue;

            if (_playerController.IsDashing)
            {
                _isReflex = true;
                StartCoroutine(BeginReflexDodge());
                break; // Exit loop after starting the coroutine
            }
        }
    }

    private IEnumerator BeginReflexDodge()
    {
        // Start Reflex
        GameManager.Instance.PlayerSpeedMultiplier = 1 / dodgeSlowMultiplier;
        GameManager.Instance.ApplyTempTimeScale(dodgeSlowMultiplier, dodgeSlowLength);
        StartCoroutine(CameraManager.Instance.SmoothSetVignette(0.25f, 0.1f));
        StartCoroutine(CameraManager.Instance.SmoothSetChromatic(0.5f, 0.1f));
        yield return new WaitForSeconds(dodgeSlowLength*dodgeSlowMultiplier);

        // End Reflex
        _isReflex = false;
        GameManager.Instance.PlayerSpeedMultiplier = 1;
        StartCoroutine(CameraManager.Instance.SmoothSetVignette(0f, 0.1f));
        StartCoroutine(CameraManager.Instance.SmoothSetChromatic(0f, 0.1f));
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, dodgeRadius);
    }
#endif
}
