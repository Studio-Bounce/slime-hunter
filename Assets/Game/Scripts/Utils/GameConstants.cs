using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    // -------------- Unity Layers --------------
    public static int EnemyLayer { get; } = 7;
    public static int PlayerLayer { get; } = 8;
    public static int EnemyBoundaryLayer { get; } = 9;
    public static int IgnoreLightingLayer { get; } = 10;

    // -------------- Unity Tags --------------
    public static string VirtualCameraTag { get; } = "MainVirtualCamera";
    public static string MapCameraTag { get; } = "MapCamera";
}
