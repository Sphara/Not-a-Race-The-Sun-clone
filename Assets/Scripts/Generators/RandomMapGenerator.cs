using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class RandomMapGenerator : MapGenerator
{

    // Call buffGenerator.AddBuff() to add buffs to the map
    // Add buffs like you have actually some gamedesign notions

    protected override void Update()
    {
        base.Update();

        if (ActiveMaps.Count == 0)
        {
            Map map = GenerateMap(200, 300, 1);
            Vector3 newOffset = new Vector3(-50, 0, 100 + GlobalSettings.SpawnZone) + Player.position;
            newOffset.y = 0.0f;

            LoadActiveMap(map, newOffset);
        }
    }

    /*
     * Type A : Raising, thwomp, pyramid, movingCube 
     * 
     * Type B : Ramp = (add trampoline ?)
     * 
     * 3 configurations :
     * - 50% of 1 A, others A + B are equally distributed
     * - 30% of two A, then same as first configuration
     * - All A + B equally distributed
     */

    // density should be between like 0 and 1ish (but hey, you do you, fry your computer as much as you like)
    public override Map GenerateMap(int x, int y, float density)
    {
        Map map = new Map();

        PartsA.Shuffle();
        PartsB.Shuffle();

        List<string> TotalParts = new List<string>(PartsA);
        TotalParts.AddRange(PartsB);

        List<int> Configuration = new List<int>();

        switch (Random.Range(0, 3))
        {
            case 0:

                Configuration.Add(50);

                for (int i = 1; i < TotalParts.Count; i++)
                {
                    Configuration.Add(50 / (TotalParts.Count - 1));
                }

                break;

            case 1:

                Configuration.Add(30);
                Configuration.Add(30);

                for (int i = 2; i < TotalParts.Count; i++)
                {
                    Configuration.Add(40 / (TotalParts.Count - 2));
                }

                break;

            case 2:

                for (int i = 0; i < TotalParts.Count; i++)
                {
                    Configuration.Add(100 / TotalParts.Count);
                }

                break;

            default:
                break;
        }
        
        for (int i = 0; i < Mathf.Min(TotalParts.Count, Configuration.Count); i++) // Trust nobody, not even yourself
        {
            //Debug.Log(TotalParts[i] + " : " + (((float)Configuration[i] / 100.0f) * x * y * ((float)density / 100.0f)));
            for (int nbr = 0;  nbr < (((float)Configuration[i] / 100.0f) * x * y * ((float)density / 100.0f)); nbr++) {
                AddPartAtRandomPlace(map, TotalParts[i], x, y);
            }
        }

        for (int i = 0; i < (x * y) / 300; i++)
        {
            BuffSpawner.AddRandomBuff(map, new Vector3(Random.Range(0, x), 0.5f, Random.Range(0, y)));
        }

        return map;
    }
}
