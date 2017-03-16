using System;
using UnityEngine;
using System.Runtime.Serialization;
using System.Collections.Generic;

/* Turns out you can serialize but cannot deserialize Unity types. Who knew ? Certainly not me. */

[DataContract(Name = "MAP")]
public class Map {

    [DataMember(Name = "MAPCONTAINER", Order = 0)]
    Dictionary<string, List<MapEntry>> map;
    
    // Maybe we want to add a Requirement field for the MapGenerator to know what is needed in the map

    public Map()
    {
        map = new Dictionary<string, List<MapEntry>>();
    }

    // Blah blah no deserialization for Vector3s
    public Dictionary<Vector3, List<MapEntry>> GetActiveMap()
    {
        Dictionary<Vector3, List<MapEntry>> newMap = new Dictionary<Vector3, List<MapEntry>>(new Vector3Comparer());

        foreach (string key in map.Keys) {
            Vector3 newKey = key.StringToVector3();

            newMap.Add(newKey, map[key]);
        }

        return newMap;
    }

    public Dictionary<string, List<MapEntry>> GetMap()
    {
        return map;
    }

    public void AddItem(MapEntry entry) {

        string position = entry.GetPosition().ToString();

        if (map.ContainsKey(position))
        {
            map[position].Add(entry);
        }
        else
        {
            map.Add(position, new List<MapEntry> { entry });
        }
    }

    public string DumpMap()
    {
        string res = "";

        foreach (List<MapEntry> lme in map.Values) {
            foreach (MapEntry me in lme) {
                res += me.ToString();
            }
        }

        return res;
    }

    public bool Equals(Map other)
    {
        if (other == null)
            return false;

        if (map.Count != other.map.Count)
            return false;

        List<MapEntry> val;

        foreach (KeyValuePair<string, List<MapEntry>> kvp in map)
        {
            if (other.map.TryGetValue(kvp.Key, out val))
            {
                foreach (MapEntry entry in kvp.Value)
                {
                    if (!val.Contains(entry))
                        return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        Map other = obj as Map;

        return Equals(other);

    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
