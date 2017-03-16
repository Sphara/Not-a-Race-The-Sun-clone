using UnityEngine;
using System.Collections.Generic;


public class TestPool : MonoBehaviour {

    public GameObject prefab;
    public GameObject parent;
    List<GameObject> list;

	void Start () {
        list = new List<GameObject>();
        Pool.CreatePool(prefab, "Test", 10, 10, 5, parent);
        Pool.CreatePool(prefab, "Batman", 4, 5 , 6, parent);
        Pool.CreatePool(prefab, "Menhir", 50, 12, 18, gameObject);
    }

    void Update () {

        if (Input.GetKeyDown(KeyCode.S)) {

            Vector3 position = new Vector3(UnityEngine.Random.Range(-10, 10), 0.5f, GlobalSettings.SpawnZone);

            GameObject obj = Pool.Spawn("Test", position, Quaternion.identity, Vector3.one);

        if (obj != null)
            list.Add(obj);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            List<GameObject> oList = new List<GameObject>();

            foreach (GameObject go in list) {
                if (go.activeSelf)
                {
                    oList.Add(go);
                }
            }

            if (oList.Count > 0)
                Pool.Destroy("Test", oList[UnityEngine.Random.Range(0, oList.Count)]);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            list.Clear();

            Pool.Reset();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Pool.CreatePool(prefab, "Test", 10, 12, 5);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Pool.Info("Test");
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Pool.ShrinkPool("Test", 3);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Pool.ExpandPool("Test", 4);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            List<string> l = Pool.List();

            foreach (string s in l)
            {
                Debug.Log(s);
            }
        }

    }
}
