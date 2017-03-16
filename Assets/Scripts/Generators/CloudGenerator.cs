using UnityEngine;
using System.Collections;

public class CloudGenerator : MonoBehaviour {

    public GameObject Cloud;
    public Vector3 Wind;

    void Start () {

        if (Cloud == null)
        {
            Debug.LogError("Cloud prefab not found");
        }

        Pool.CreatePool(Cloud, "Cloud", 10, 50, 10, gameObject);

    }
	
	void Update () {

        if (Random.Range(0, 1000) == 42)
        {
            CreateCloud();
        }
	}

    void CreateCloud()
    {
        Vector3 position = new Vector3(Random.Range(-GlobalSettings.PopZoneWidth, GlobalSettings.PopZoneWidth), Random.Range(60, 120), 500) + GlobalSettings.PlayerPosition;
        Vector3 scale = new Vector3(Random.Range(10, 100), Random.Range(2, 20), Random.Range(10, 100));
        Quaternion rotation = Quaternion.identity;

        Pool.Spawn("Cloud", position, rotation, scale);
    }
}
