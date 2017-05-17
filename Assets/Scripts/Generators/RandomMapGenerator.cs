using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class RandomMapGenerator : MapGenerator
{
    private List<string> PartsA; // The majority of the objects
    private List<string> PartsB; // The things you can't use too much (like ramps)
    private Dictionary<string, InformationHolder> InfoDic;

    protected override void Start()
    {
        base.Start();

        InfoDic = new Dictionary<string, InformationHolder>();
        PartsA = new List<string>() { "Thwomp", "RaisingObject", "Pyramid", "MovingCube" };
        PartsB = new List<string>() { "Ramp" };

        InfoDic.Add("Ramp", new InformationHolder(0.5f, Vector3.one, Vector3.one, 0, 0));
        InfoDic.Add("RaisingObject", new InformationHolder(0.5f, new Vector3(1, 1, 1), new Vector3(10, 15, 20), 0, 0));
        InfoDic.Add("Thwomp", new InformationHolder(0.5f, new Vector3(1, 1, 1), new Vector3(10, 15, 20), 0, 0));
        InfoDic.Add("Pyramid", new InformationHolder(0.0f, new Vector3(1, 1, 1), new Vector3(10, 25, 10), 0, 360));
        InfoDic.Add("MovingCube", new InformationHolder(0.0f, new Vector3(3, 3, 3), new Vector3(10, 10, 10), 0, 0));

    }

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

    public virtual void PopPart(Map map, string s, int x, int y)
    {
        Vector3 position = new Vector3(Random.Range(0, x), InfoDic[s].YPosition, Random.Range(0, y));
        Vector3 scale = new Vector3(Random.Range(InfoDic[s].minScale.x, InfoDic[s].maxScale.x), Random.Range(InfoDic[s].minScale.y, InfoDic[s].maxScale.y), Random.Range(InfoDic[s].minScale.z, InfoDic[s].maxScale.z));
        Quaternion rotation = Quaternion.Euler(0, Random.Range(InfoDic[s].minRotation, InfoDic[s].maxRotation), 0);

        map.AddItem(new MapEntry(s, position, rotation, scale));
    }

    /*
     * Type A : Raising, thwomp, pyramid, movingCube 
     * 
     * Type B : Ramp = (add trampoline ?)
     * 
     * Change to : 3 configurations :
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

        List<string> TotalParts = PartsA;
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
            Debug.Log((((float)Configuration[i] / 100.0f) * x * y * ((float)density / 100.0f)));
            for (int nbr = 0;  nbr < (((float)Configuration[i] / 100.0f) * x * y * ((float)density / 100.0f)); nbr++) {
                PopPart(map, TotalParts[i], x, y);
            }
        }

        return map;
    }

    public struct InformationHolder
    {
        public float YPosition;
        public Vector3 minScale, maxScale;
        public float minRotation, maxRotation;

        public InformationHolder(float _y, Vector3 _minScale, Vector3 _maxScale, float _minRotation, float _maxRotation)
        {
            YPosition = _y;
            minScale = _minScale;
            maxScale = _maxScale;
            minRotation = _minRotation;
            maxRotation = _maxRotation;
        }
    }
}
