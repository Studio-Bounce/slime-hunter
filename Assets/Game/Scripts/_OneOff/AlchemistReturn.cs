using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AlchemistReturn : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera bridgeCamera;
    [SerializeField] Transform bridgeLocation;
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float turnSpeed = 2.0f;
    [SerializeField] MoveBridge bridge;
    [SerializeField] List<Transform> pathTillShop = new List<Transform>();
    int pathTIdx;
    public UnityEvent onReachShop;

    Animator animator;
    readonly int MoveSpeed = Animator.StringToHash("blendSpeed");
    public float animatorMoveBlend = 0.5f;

    CinemachineVirtualCamera currentCamera;
    [Header("For debugging")]
    public  bool goingToBridge = false;
    public bool loweringBridge = false;
    bool _startedLowering = false;
    public bool goingToShop = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        goingToBridge = false;
        loweringBridge = false;
        _startedLowering = false;
        goingToShop = false;
    }

    private void Update()
    {
        if (goingToBridge)
        {
            Vector3 direction = (bridgeLocation.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, bridgeLocation.position) > 0.2f)
            {
                transform.position += moveSpeed * Time.deltaTime * transform.forward;
            }
            else
            {
                goingToBridge = false;
                // Lower the bridge
                loweringBridge = true;
                animator.SetFloat(MoveSpeed, 0);
            }
        }
        else if (loweringBridge)
        {
            if (!_startedLowering)
            {
                currentCamera = CameraManager.ActiveCineCamera;
                InputManager.Instance.TogglePlayerControls(false);
                CameraManager.Instance.ChangeVirtualCamera(bridgeCamera);

                _startedLowering = true;
                bridge.MoveTheBridgeWithDelay(1.0f);
            }
            else if (bridge.IsBridgeDown())
            {
                CameraManager.Instance.ChangeVirtualCamera(currentCamera);
                InputManager.Instance.TogglePlayerControls(true);
                animator.SetFloat(MoveSpeed, animatorMoveBlend);

                loweringBridge = false;
                // Walk towards shop now
                goingToShop = true;
                pathTIdx = 0;
            }
        }
        else if (goingToShop)
        {
            Transform target = pathTillShop[pathTIdx];
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) > 0.2f)
            {
                transform.position += moveSpeed * Time.deltaTime * transform.forward;
            }
            else
            {
                ++pathTIdx;
                if (pathTIdx >= pathTillShop.Count)
                {
                    goingToShop = false;
                    onReachShop.Invoke();
                    Destroy(gameObject);
                }
            }
        }
    }

    // Called from dialogue system
    public void MoveToBridge()
    {
        goingToBridge = true;
        animator.SetFloat(MoveSpeed, animatorMoveBlend);
    }
}
