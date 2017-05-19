using UnityEngine;
using System.Collections;

// Deprecated af, use the other map generation classes

public class BasicMapGeneration : MonoBehaviour
{

    public GameObject mObj;
    public GameObject ramp;
    public GameObject thwomp;
    public GameObject pyramid;
    public GameObject movingCube;

    public GameObject parent;

    public Rigidbody player;

    float popChance = 0.0f;
    
    void Start()
    {
        Pool.CreatePool(mObj, "Test", 100, 500, 10, parent);
        Pool.CreatePool(ramp, "Ramp", 25, 100, 10, parent);
        Pool.CreatePool(thwomp, "Thwomp", 50, 250, 10, parent);
        Pool.CreatePool(pyramid, "Pyramid", 50, 250, 10, parent);
        Pool.CreatePool(movingCube, "MovingCube", 50, 250, 10, parent);
    }

    void Update()
    {
        popChance += ((player.velocity.z * GlobalSettings.PopChanceBonus) / GlobalSettings.MaxSpeed) * Time.timeScale;

        if (popChance >= 1)
        {
            int nbr;

            if ((nbr = Random.Range(0, 100)) > 55)
            {
                if (nbr < 60)
                {
                    PopMovingCube();
                }
                if (nbr < 70)
                {
                    PopPyramid();
                }
                else if (nbr < 98)
                {
                    PopCube();
                }
                else
                {
                    PopRamp();
                }

            }

            popChance -= 1;
        }
    }

    // Everything you just did was awful
    void PopRamp()
    {
        Vector3 position = new Vector3(Random.Range(-GlobalSettings.PopZoneWidth, GlobalSettings.PopZoneWidth), 0.5f, GlobalSettings.SpawnZone) + GlobalSettings.PlayerPosition;

        Pool.Spawn("Ramp", position);
    }

    void PopPyramid()
    {
        Vector3 position = new Vector3(Random.Range(-GlobalSettings.PopZoneWidth, GlobalSettings.PopZoneWidth), 0.0f, GlobalSettings.SpawnZone) + GlobalSettings.PlayerPosition;
        int scaleY = Random.Range(1, 25);
        int scaleX = Random.Range(1, 10);
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        Pool.Spawn("Pyramid", position, rotation, new Vector3(scaleX, scaleY, scaleX));
    }

    void PopMovingCube()
    {
        Vector3 position = new Vector3(Random.Range(-GlobalSettings.PopZoneWidth, GlobalSettings.PopZoneWidth), 0.0f, GlobalSettings.SpawnZone) + GlobalSettings.PlayerPosition;
        Vector3 scale = new Vector3(Random.Range(3, 10), Random.Range(3, 10), Random.Range(3, 10)); // same color as some of the buffs, so we dont' want confusion

        position.y = scale.y / 2;

        Pool.Spawn("MovingCube", position, Quaternion.identity, scale);
    }

    void PopCube()
    {
        Vector3 position = new Vector3(Random.Range(-GlobalSettings.PopZoneWidth, GlobalSettings.PopZoneWidth), 0.5f, GlobalSettings.SpawnZone) + GlobalSettings.PlayerPosition;
        Vector3 scale = new Vector3(Random.Range(1, 10), Random.Range(1, 15), Random.Range(1, 20));

        if (Random.Range(1, 100) > 75)
        {
            Pool.Spawn("Thwomp", position, Quaternion.identity, scale);
        }
        else
        {
            Pool.Spawn("Test", position, Quaternion.identity, scale);
        }
    }
}
