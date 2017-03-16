using UnityEngine;
using System.Collections;

public static class GlobalSettings {

    public static int MaxSpeed = 10;
    public static int Acceleration = 50;

    public static int HorizontalAcceleration = 50;
    public static int MaxHorizontalSpeed = 100;

    public static float PlayerGravity = 10.0f;

    public static int DespawnZone = -5;
    public static int SpawnZone = 75;
    public static int PopZoneWidth = 100;

    public static int CollisionTolerance = 10;
    public static float RaisingSpeed = 1;
    public static float FallingSpeed = 1;
    public static int ApparitionZone = 50;
    public static int CrushZone = 50;
    public static int CrushHeight = 3;
    public static float CrushingAcceleration = 1.5f;

    public static float PopChanceBonus = 0.3f;

    public static Vector3 PlayerPosition = new Vector3(0, 0, 0);
}
