using UnityEngine;
using System.Collections.Generic;

// Works fine, but there might be problems with tiling later
public class GroundGenerator : MonoBehaviour {

    public GameObject GroundTile;
    public GameObject Parent;

    int Scale = 1000; // Must be a multiple of 10 because i can't do maths
    float mDespawnPos;
    Vector3 mScale;
    Vector3 mPosition;
    Dictionary<Vector3, GameObject> mGroundTiles;

    void Start()
    {
        mDespawnPos = 0 - (Scale * 2);
        mScale = new Vector3(Scale / 10, 1, Scale / 10); // Turns out that 1 scale unit is 10 pos units or something else, but it works that way. Did i mention i suck at maths ?
        mPosition = Vector3.zero;
        Pool.CreatePool(GroundTile, "GroundTiles", 10, 50, 10, Parent);
        mGroundTiles = new Dictionary<Vector3, GameObject>();

        mGroundTiles.Add(mPosition, Pool.Spawn("GroundTiles", mPosition, Quaternion.identity, mScale));
    }

    void Update()
    {
        mPosition.x = RoundPosition(GlobalSettings.PlayerPosition.x);
        mPosition.z = RoundPosition(GlobalSettings.PlayerPosition.z);

        Vector3 pos = Vector3.zero;

        for (int i = 0; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                pos.x = mPosition.x + j * Scale;
                pos.z = mPosition.z + i * Scale;
                CheckTile(pos);
            }
        }

        CleanGroundTiles(); // Maybe i shouldn't clean that often

    }

    void CleanGroundTiles()
    {
        foreach (GameObject go in mGroundTiles.Values)
        {
            if (go.transform.position.z < mDespawnPos)
            {
                Pool.Destroy("GroundTiles", go);
            }
        }
    }

    void CheckTile(Vector3 pos)
    {
        if (!mGroundTiles.ContainsKey(pos))
        {
            mGroundTiles.Add(pos, Pool.Spawn("GroundTiles", pos, Quaternion.identity, mScale));

            if (pos.z > mDespawnPos + Scale * 2)
            {
                mDespawnPos = pos.z - Scale * 2;
            }
        }
    }


    // This might be useful elsewhere
    int RoundPosition(float pos)
    {
        return ((int)Mathf.Round(pos / Scale) * Scale);
    }
}
