using UnityEngine;
using System.Collections.Generic;

public class RandomMapGenerator : MapGenerator
{
    private List<string> Parts;
    private Dictionary<string, InformationHolder> InfoDic;

    // WE WERE HERE

    // Get a better algorithm to get the spawns

    protected override void Start()
    {
        base.Start();

        InfoDic = new Dictionary<string, InformationHolder>();
        Parts = new List<string>() { "Thwomp", "RaisingObject", "Ramp", "Pyramid", "MovingCube" };

        InfoDic.Add("Ramp", new InformationHolder(0.5f, Vector3.one, Vector3.one, 0, 0));
        InfoDic.Add("RaisingObject", new InformationHolder(0.5f, new Vector3(1, 1, 1), new Vector3(10, 15, 20), 0, 0));
        InfoDic.Add("Thwomp", new InformationHolder(0.5f, new Vector3(1, 1, 1), new Vector3(10, 15, 20), 0, 0));
        InfoDic.Add("Pyramid", new InformationHolder(0.0f, new Vector3(1, 1, 1), new Vector3(10, 25, 10), 0, 360));
        InfoDic.Add("MovingCube", new InformationHolder(0.0f, new Vector3(3, 3, 3), new Vector3(10, 10, 10), 0, 0));

        Map map = GenerateMap(100, 300);
        LoadActiveMap(map, new Vector3(-50, 0, 25));
    }

    protected override void Update()
    {
        base.Update();

        if (ActiveMaps.Count == 0)
        {
            Map map = GenerateMap(200, 300);
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
     * Type B : Ramp
     * 
     * Change to : 3 configurations :
     * - 40% of 1 A, others A + B are equally distributed
     * - 25% of two A, then same as first configuration
     * - All A + B equally distributed
     */

    public override Map GenerateMap(int x, int y)
    {
        Map map = new Map();

        // Replace from here
        int available = (x * y) / 200;

        Parts.Shuffle();

        foreach (string s in Parts)
        {
            int nbr = Random.Range(0, available);
            available -= nbr;

            while (nbr > 0)
            {
                PopPart(map, s, x, y);
                nbr--;
            }
        }

        // To there

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
