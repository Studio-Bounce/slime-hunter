using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    // Global Variables
    public static readonly float GLOBAL_KNOCKBACK_MULTIPLIER = 0.25f;

    public Player playerRef;
    public readonly int playerMaxHealth = 100;
    public int playerHealth = 100;

    public Canvas screenCanvas;

    public int PlayerHealth
    {
        get { return playerHealth; }
        set
        {
            playerHealth = value;
            OnPlayerHealthChange?.Invoke(playerHealth);
        }
    }

    public event Action<int> OnPlayerHealthChange;

    private void Awake()
    {
        if (screenCanvas == null)
        {
            GameObject canvasObject = new GameObject("ScreenCanvas");
            canvasObject.transform.SetParent(null, false);
            screenCanvas = canvasObject.AddComponent<Canvas>();
            screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<GraphicRaycaster>();
        }
    }

    private void OnDestroy()
    {
        Instance.OnPlayerHealthChange = null;
    }
}
