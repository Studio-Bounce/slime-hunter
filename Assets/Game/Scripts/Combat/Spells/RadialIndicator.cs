using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialIndicator : SpellIndicator
{
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.red;
    public LayerMask hitLayers;

    [SerializeField] private Transform sourceIndicator;
    [SerializeField] private Transform targetIndicator;

    [SerializeField] private Renderer sourceRenderer;
    [SerializeField] private Renderer targetRenderer;

    private Vector2 gamepadSource = Vector2.zero;
    private Vector2 gamepadTarget = Vector2.zero;

    public override Vector3 GetTarget {  get { return targetIndicator.transform.position; } }

    private void Start()
    {
        Debug.Assert(sourceIndicator != null, "Requires prefab for sourceIndicator");
        Debug.Assert(targetIndicator != null, "Requires prefab for targetIndicator");

        InitializeScaleMaterial();
        HideIndicator();
    }

    private void InitializeScaleMaterial()
    {
        sourceIndicator.localScale = new Vector3(castRange, castRange, castRange);
        targetIndicator.localScale = new Vector3(areaOfEffect, areaOfEffect, areaOfEffect);
        Material material = sourceRenderer.material;
        material.SetFloat("_Scale", castRange);
        material.SetColor("_Color", activeColor);
        material = targetRenderer.material;
        material.SetFloat("_Scale", areaOfEffect);
        material.SetColor("_Color", activeColor);
    }

    private void Update()
    {
        if (InputManager.IsGamepad)
        {
            AimIndicator_Gamepad();
        } else
        {
            AimIndicator_Keyboard();
        }
    }

    private void AimIndicator_Keyboard()
    {
        Ray ray = CameraManager.ActiveCamera.ScreenPointToRay(InputManager.PointerPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayers))
        {
            Vector3 direction = hit.point - sourceIndicator.position;

            if (direction.magnitude > castRange)
            {
                direction = direction.normalized * castRange;
            }

            targetIndicator.position = sourceIndicator.position + direction;
        }
    }

    private void AimIndicator_Gamepad()
    {
        gamepadTarget += InputManager.JoystickDelta;
        Vector2 gamepadDelta = gamepadTarget - gamepadSource;
        gamepadTarget = gamepadSource + gamepadDelta;

        Ray ray = CameraManager.ActiveCamera.ScreenPointToRay(gamepadTarget);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayers))
        {
            Vector3 direction = hit.point - sourceIndicator.position;

            if (direction.magnitude > castRange)
            {
                direction = direction.normalized * castRange;
                gamepadTarget -= InputManager.JoystickDelta;
            }

            targetIndicator.position = sourceIndicator.position + direction;
        }
    }

    public override void ShowIndicator(SpellSO spellSO)
    {
        Active = true;
        castRange = spellSO.castRange;
        areaOfEffect = spellSO.areaOfEffect;
        sourceRenderer.enabled = (castRange > 0) ? true : false;
        targetRenderer.enabled = true;
        InitializeScaleMaterial();
        GameManager.Instance.ApplyTempTimeScale(0.15f, 2.0f);
        CameraManager.Instance.SetSaturation(-20);
        CameraManager.Instance.SetChromatic(0.5f);

        if (InputManager.IsGamepad)
        {
            InputManager.Instance.TogglePlayerMovement(false);
            gamepadSource = CameraManager.ActiveCamera.WorldToScreenPoint(sourceIndicator.position);
            gamepadTarget = gamepadSource;
        }
    }

    public override void HideIndicator()
    {
        Active = false;
        sourceRenderer.enabled = false;
        targetRenderer.enabled = false;
        GameManager.Instance.TimeNormal();
        CameraManager.Instance.SetSaturation(8);
        CameraManager.Instance.SetChromatic(0.0f);

        if (InputManager.IsGamepad) InputManager.Instance.TogglePlayerMovement(true);
    }

    public override void ToggleReady(bool ready)
    {
        Color col = ready ? activeColor : inactiveColor;
        sourceRenderer.material.SetColor("_Color", col);
        targetRenderer.material.SetColor("_Color", col);
    }
}
