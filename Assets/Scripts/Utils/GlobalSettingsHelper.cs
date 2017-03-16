using UnityEngine;
using System.Collections;

//Do some Custom Editor magic to have something a lil less ugly
public class GlobalSettingsHelper : MonoBehaviour {

    [Header("Speed related stuff")]

    [Range(-50, 50)]
    public int MaxSpeed = 10;

    [Range(0, 100)]
    public int Acceleration = 50;

    [Range(0, 200)]
    public int MaxHorizontalSpeed = 50;

    [Range(0, 100)]
    public int HorizontalAcceleration = 50;

    [Header("Spawn/Despawn")]

    [Range(0, -100)]
    public int DespawnZone = -5;

    [Range(0, 1000)]
    public int SpawnZone = 80;

    [Range(0, 1000)]
    public int ApparitionZone = 50;

    [Range(0, 1000)]
    public int CrushZone = 50;

    [Range(10, 500)]
    public int PopZoneWideness = 100;

    [Header("Physics stuff")]

    [Range(1.0f, 100.0f)]
    public float PlayerGravity = 10;

    [Range(1, 50)]
    public int CollisionTolerance = 10;

    public GameObject player;

    [Header("Misc")]

    [Range(0.01f, 100.0f)]
    public float RaisingSpeed = 1;

    [Range(0.01f, 100.0f)]
    public float FallingSpeed = 1;

    [Range(1.0f, 5.0f)]
    public float CrushingAcceleration = 1.01f;

    [Range(1, 20)]
    public int CrushHeight = 1;

    [Range(1.0F, 10.0F)]
    public float BonusPopChance = 1.3f;


    void Sync()
    {
        GlobalSettings.Acceleration = Acceleration;
        GlobalSettings.MaxSpeed = MaxSpeed;
        GlobalSettings.PlayerGravity = PlayerGravity;
        GlobalSettings.HorizontalAcceleration = HorizontalAcceleration;
        GlobalSettings.MaxHorizontalSpeed = MaxHorizontalSpeed;
        GlobalSettings.DespawnZone = (int)player.transform.position.z + DespawnZone;
        GlobalSettings.SpawnZone = SpawnZone; // Not relative to the player because spawns already add playerPos to pop zone
        GlobalSettings.PopZoneWidth = PopZoneWideness;
        GlobalSettings.CollisionTolerance = CollisionTolerance;
        GlobalSettings.RaisingSpeed = RaisingSpeed;
        GlobalSettings.ApparitionZone = ApparitionZone;
        GlobalSettings.FallingSpeed = FallingSpeed;
        GlobalSettings.CrushHeight = CrushHeight;
        GlobalSettings.CrushZone = CrushZone;
        GlobalSettings.PopChanceBonus = BonusPopChance;

        GlobalSettings.PlayerPosition = player.transform.position;
        GlobalSettings.PlayerPosition.y = 0; // We don't want stuff to start spawning in air when the player is jumping
    }

    void Start()
    {
        Sync();
    }

    void Update()
    {
        Sync();
    }

}
