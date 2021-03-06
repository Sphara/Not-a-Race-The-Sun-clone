﻿using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;

// YOU ERE WORKING IN HERE ON THE WRAPPING, READ THE COMMENTS

/*
 * The idea is : if the active map is declared wrapping & PlayerPosition +- ApparitionZone is outside the map, create a new map left/right & load it
 * */

public class MapGenerator : MonoBehaviour {

    public GameObject BasicObject;
    public GameObject BasicObjectNonKinematic;
    public GameObject RaisingObject;
    public GameObject Ramp;
    public GameObject Thwomp;
    public GameObject Pyramid;
    public GameObject MovingCube;

    public GameObject Parent;

    public Rigidbody Player;

    public BuffGenerator BuffSpawner;

    protected LinkedList<ActiveMapClass> ActiveMaps;
    protected List<ActiveMapClass> ToDelete;

    protected List<string> PartsA; // The majority of the objects
    protected List<string> PartsB; // The things you can't use too much (like ramps)
    protected Dictionary<string, InformationHolder> InfoDic;

    protected virtual void Start()
    {
        Pool.CreatePool(BasicObject, "BasicObject", 100, 10000, 10, Parent);
        Pool.CreatePool(BasicObjectNonKinematic, "BasicObjectNonKinematic", 50, 2500, 10, Parent);
        Pool.CreatePool(RaisingObject, "RaisingObject", 100, 5000, 10, Parent);
        Pool.CreatePool(Ramp, "Ramp", 100, 3000, 10, Parent);
        Pool.CreatePool(Thwomp, "Thwomp", 100, 5000, 10, Parent);
        Pool.CreatePool(Pyramid, "Pyramid", 100, 3000, 10, Parent);
        Pool.CreatePool(MovingCube, "MovingCube", 100, 3000, 10, Parent);

        ActiveMaps = new LinkedList<ActiveMapClass>();
        ToDelete = new List<ActiveMapClass>();

        InfoDic = new Dictionary<string, InformationHolder>();
        PartsA = new List<string>() { "Thwomp", "RaisingObject", "Pyramid", "MovingCube" };
        PartsB = new List<string>() { "Ramp" };

        InfoDic.Add("Ramp", new InformationHolder(0.5f, Vector3.one, Vector3.one, 0, 0));
        InfoDic.Add("RaisingObject", new InformationHolder(0.5f, new Vector3(1, 1, 1), new Vector3(10, 15, 20), 0, 0));
        InfoDic.Add("Thwomp", new InformationHolder(0.5f, new Vector3(1, 1, 1), new Vector3(10, 15, 20), 0, 0));
        InfoDic.Add("Pyramid", new InformationHolder(0.0f, new Vector3(1, 1, 1), new Vector3(10, 25, 10), 0, 360));
        InfoDic.Add("MovingCube", new InformationHolder(0.0f, new Vector3(3, 3, 3), new Vector3(10, 10, 10), 0, 0));
    }

    protected virtual void Update()
    {
        /*
         * Rather than doing that, you can just calculate what are the new "active" tiles since the last call and lookup these ones 
         * 
         * 
         * 
         * 
         */

        foreach (ActiveMapClass map in ActiveMaps) {

            for (; map.SpawnZ + map.offset.z < Player.transform.position.z + GlobalSettings.SpawnZone; map.SpawnZ++)
            {
                if (map.MapLayers.ContainsKey(map.SpawnZ))
                {
                    foreach (Vector3 vec in map.MapLayers[map.SpawnZ])
                    {
                        // if horizontal check because i'm about to do something ugly here

                        foreach (MapEntry entry in map.ActiveMap[vec])
                        {
                            entry.Spawn(map.offset); // MUH COMPLEXITY
                        }
                    }
                }
            }

            if (map.SpawnZ > map.size)
            {
                ToDelete.Add(map);
            }
        }

        foreach (ActiveMapClass del in ToDelete)
        {
            ActiveMaps.Remove(del);
        }

        /*
        if (Input.GetKeyDown(KeyCode.T))
        {
            Map map = GenerateMap(10, 10);

            Test(map); // Testing is VERY resource/time consuming for now, since i do everything in a single block without multithreading & other fancy things, SO DONT SPAM IT
        }
        */
    }

    public void LoadActiveMap(Map map, bool isWrapping = false)
    {
        LoadActiveMap(map, Vector3.zero, isWrapping);
    }

    public void LoadActiveMap(Map map, Vector3 _offset, bool isWrapping = false)
    {
        Dictionary<string, List<MapEntry>> innerMap = map.GetMap();

        Dictionary<int, List<Vector3>> layers = new Dictionary<int, List<Vector3>>();
        Dictionary<Vector3, List<MapEntry>> activeMap = new Dictionary<Vector3, List<MapEntry>>(new Vector3Comparer());

        foreach (string key in innerMap.Keys)
        {
            Vector3 newKey = key.StringToVector3();
            int z = (int)newKey.z;
            activeMap.Add(newKey, innerMap[key]);

            if (layers.ContainsKey(z))
            {
                layers[z].Add(newKey);
            }
            else
            {
                layers.Add(z, new List<Vector3>() { newKey });
            }
        }

        int size = 0;

        foreach (int i in layers.Keys)
        {
            // I HAVE NO IDEA HOW TO INDENT THIS (pretty sure i'm not supposed to do it like that though)

            layers[i].Sort(
                delegate (Vector3 a, Vector3 b)
                {
                    if (a.z < b.z) return -1;
                    else if (a.z == b.z) return 0;
                    else return 1;
                } 
            );

            if (i > size)
            {
                size = i;
            }
        }

        ActiveMapClass newActiveMapClass = new ActiveMapClass (activeMap, _offset, layers, size, isWrapping);

        ActiveMaps.AddLast(newActiveMapClass);

        Debug.Log("New active map loaded : total = " + ActiveMaps.Count);

        // DO the requirement thing to load enough objects

    }

    public void UnloadActiveMaps()
    {
        ActiveMaps.Clear();
        // Unload stuff
    }

    // DONT USE LOL
    public virtual Map GenerateMap(int x, int y, float density = 1)
    {
        Map map = new Map();

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                if ((j + i) % 5 == 0)
                    map.AddItem(new MapEntry("BasicObject", new Vector3(i, 0, j), Quaternion.identity, Vector3.one));
            }
        }

        return map;
    }

    protected bool StoreMap(Map map, string name, bool overwrite)
    {
        string mapPath = Application.persistentDataPath + "/" + name;

        if (File.Exists(mapPath))
        {
            Debug.LogWarning(overwrite ? "Overriding map " + name : "Tried to save a map named " + name + ", but it already exists");

            if (!overwrite)
                return false;
            else
                File.Delete(mapPath); // I do know that file.Create overwrites a file, but still
        }

        DataContractSerializer dcs = new DataContractSerializer(typeof(Map));
        MemoryStream ms = new MemoryStream();
        FileStream fs = File.Create(mapPath);

        dcs.WriteObject(ms, map);

        ms.Seek(0, SeekOrigin.Begin);

        fs.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);

        fs.Close();
        ms.Close();

        return true;
    }

    protected Map LoadMap(string name)
    {
        string mapPath = Application.persistentDataPath + "/" + name;

        if (!File.Exists(mapPath))
        {
            Debug.LogWarning("Requested map " + name + " does not exist");
            return null;
        }

        FileStream fs = File.Open(mapPath, FileMode.Open);

        XmlDictionaryReader xdr = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
        DataContractSerializer dcs = new DataContractSerializer(typeof(Map));

        Map map = (Map)dcs.ReadObject(xdr, false);

        fs.Close();
        xdr.Close();

        return map;
    }

    /// <summary>
    ///  Pop a part at a random place in the map
    /// </summary>
    /// <param name="map">The map</param>
    /// <param name="s">The name of the part (retrieved in infoDic)</param>
    /// <param name="x">Max X range (min is 0)</param>
    /// <param name="y">Max Y range (min is 0)</param>
    ///
    public virtual void AddPartAtRandomPlace(Map map, string s, int x, int y)
    {
        Vector3 position = new Vector3(Random.Range(0, x), InfoDic[s].YPosition, Random.Range(0, y));
        Vector3 scale = new Vector3(Random.Range(InfoDic[s].minScale.x, InfoDic[s].maxScale.x), Random.Range(InfoDic[s].minScale.y, InfoDic[s].maxScale.y), Random.Range(InfoDic[s].minScale.z, InfoDic[s].maxScale.z));
        Quaternion rotation = Quaternion.Euler(0, Random.Range(InfoDic[s].minRotation, InfoDic[s].maxRotation), 0);

        // Change to something else when there are more noScaleRotation items (rn there are just ramps)
        map.AddItem(new MapEntry(s, position, rotation, scale, s == "Ramp" ? true : false));
    }

    public virtual void AddPart(Map map, string s, Vector3 position)
    {
        // Maybe override position.y with InfoDic[s].Yposition ?
        Vector3 scale = new Vector3(Random.Range(InfoDic[s].minScale.x, InfoDic[s].maxScale.x), Random.Range(InfoDic[s].minScale.y, InfoDic[s].maxScale.y), Random.Range(InfoDic[s].minScale.z, InfoDic[s].maxScale.z));
        Quaternion rotation = Quaternion.Euler(0, Random.Range(InfoDic[s].minRotation, InfoDic[s].maxRotation), 0);

        // Same as in AddPartAtRandomPlace
        map.AddItem(new MapEntry(s, position, rotation, scale, s == "Ramp" ? true : false));
    }

    public bool Test(Map map)
    {
        string name = "test" + map.GetHashCode();

        if (StoreMap(map, name, true))
        {
            Debug.Break();
            Map newMap = LoadMap(name);

            if (!newMap.Equals(map))
            {
                Debug.Log("Test failure on equal maps");
                return false;
            }
            else
            {
                Debug.Log("Test success on equal maps");
            }

            newMap.AddItem(new MapEntry("TEST", Vector3.zero, Quaternion.identity, Vector3.one));

            if (newMap.Equals(map))
            {
                Debug.Log("Test failure on different maps");
                return false;
            }
            else
            {
                Debug.Log("Test success on different maps");
                return true;
            }
        }
        else
        {
            Debug.LogWarning("Failure of test function : map test wasn't stored properly");
            return false;
        }
    }

    protected class ActiveMapClass
    {
        public Dictionary<Vector3, List<MapEntry>> ActiveMap;
        public Dictionary<int, List<Vector3>> MapLayers;
        public int SpawnZ;
        public int size;
        public Vector3 offset;
        public bool isWrapping;

        public  ActiveMapClass(Dictionary<Vector3, List<MapEntry>> activeMap, Vector3 _offset, Dictionary<int, List<Vector3>> layers, int _size, bool _isWrapping = false)
        {
            offset = _offset;
            ActiveMap = activeMap;
            MapLayers = layers;
            size = _size;
            SpawnZ = 0;
            isWrapping = _isWrapping;
        }
    }

    // Describes 1 item on the map
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
