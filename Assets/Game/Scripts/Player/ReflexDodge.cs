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
    public float dodgeTriggerWindowDuration = 0.0f;

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
        StartCoroutine(CheckReflexDodgeForSeconds(dodgeTriggerWindowDuration));
    }

    // Give some leeway for what counts as a dodge
    IEnumerator CheckReflexDodgeForSeconds(float duration)
    {
        float timer = 0.0f;
        while (timer <= duration)
        {
            // Check if we're in reflex mode or not
            if (!_isReflex)
            {
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
            timer += Time.deltaTime;
            yield return null;
        }
        
    }

    private IEnumerator BeginReflexDodge()
    {
        // Start Reflex
        GameManager.Instance.ApplyReflexTime(dodgeSlowMultiplier, dodgeSlowLength);
        CameraManager.Instance.SmoothSetVignette(0.2f, 0.02f);
        CameraManager.Instance.SmoothSetChromatic(0.5f, 0.02f);
        yield return new WaitForSeconds(dodgeSlowLength*dodgeSlowMultiplier);

        // End Reflex
        _isReflex = false;
        CameraManager.Instance.SmoothSetVignette(0f, 0.2f);
        CameraManager.Instance.SmoothSetChromatic(0f, 0.2f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, dodgeRadius);
    }
#endif
}
