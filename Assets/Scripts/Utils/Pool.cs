using UnityEngine;
using System.Collections.Generic;

// Next thing : have a list of function per pool that are applied at new object creation

public class PoolableObject : MonoBehaviour {

    GenericPoolableObject mObj;

    // blah blah getters setters at some point i was doing things here and i might do it again so this'll stay like that
    public void SetObject(GenericPoolableObject obj) {
        mObj = obj;
    }

    public GenericPoolableObject GetPoolableObject()
    {
        return mObj;
    }

    public GameObject GetGameObject()
    {
        return mObj.gameObject;
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        gameObject.transform.localScale = scale;
        gameObject.SetActive(true);

        if (mObj != null)
            mObj.Reset();

        return gameObject;
    }

    public GameObject Spawn(Vector3 position)
    {
        gameObject.transform.position = position;
        gameObject.SetActive(true);

        if (mObj != null)
            mObj.Reset();

        return gameObject;
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
    }
}

public class _Pool {
    List<PoolableObject> mPoolableObjects;

    GameObject mPrefab;
    GameObject mPoolParent;

    int mMaxPoolSize;
    int mPoolSizeIncrement;
    int mName = 0;
    public string mPoolName { get; set; }

    public _Pool(GameObject prefab, string poolName, int size, int maxSize, int incrementNumber, GameObject parent) {

        if (prefab == null) {
            Debug.LogError("Pool prefab is null for pool " + poolName);
            return;
        }

        mPoolableObjects = new List<PoolableObject>();
        mPrefab = prefab;
        mMaxPoolSize = maxSize;
        mPoolParent = parent;
        mPoolName = poolName;
        ChangeIncrement(incrementNumber);
        Populate(size < maxSize ? size : maxSize);
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        for (int i = 0; i < mPoolableObjects.Count; i++)
        {
            if (!mPoolableObjects[i].gameObject.activeSelf)
            {
                return mPoolableObjects[i].Spawn(position, rotation, scale);
            }
        }

        if (mPoolableObjects.Count < mMaxPoolSize)
        {
            Expand(mPoolSizeIncrement);
            return Spawn(position, rotation, scale);
        }
        else
        {
            Debug.LogWarning("Maximum size reached for pool " + mPoolName);
            return null;
        }
    }

    public GameObject Spawn(Vector3 position)
    {
        for (int i = 0; i < mPoolableObjects.Count; i++)
        {
            if (!mPoolableObjects[i].gameObject.activeSelf)
            {
                return mPoolableObjects[i].Spawn(position);
            }
        }

        if (mPoolableObjects.Count < mMaxPoolSize)
        {
            Expand(mPoolSizeIncrement);
            return Spawn(position);
        }
        else
        {
            Debug.LogWarning("Maximum size reached for pool " + mPoolName);
            return null;
        }
    }

    public void Destroy(GameObject obj)
    {
        PoolableObject pobj = obj.GetComponent<PoolableObject>();

        if (pobj == null)
        {
            Debug.LogWarning("Tried to destroy an unpooled object with a pool method. Object name : " + obj.name);
            return;
        }

        pobj.Destroy();
    }

    public int Expand(int quantity)
    {
        if (quantity < 1 || mPoolableObjects.Count == mMaxPoolSize)
        {
            Debug.LogWarning("Failed to expand pool " + mPoolName);
            return 0;
        }

        return Populate(mMaxPoolSize >= quantity + mPoolableObjects.Count ? quantity : mMaxPoolSize - mPoolableObjects.Count);
    }

    public int Shrink(int quantity)
    {
        if (quantity < 1 || mPoolableObjects.Count == 0)
        {
            Debug.LogWarning("Failed to shrink pool " + mPoolName);
            return 0;
        }

        return Depopulate(mPoolableObjects.Count >= quantity ? quantity : mPoolableObjects.Count);
    }

    public void DestroyPool()
    {
        List<PoolableObject> list = new List<PoolableObject>(mPoolableObjects);

        foreach (PoolableObject pobj in list)
            DestroyPooledObject(pobj);
    }

    // Will NOT destroy objects (Maybe a force mode to do it ?)
    public void ChangeMaxSize(int newMaxSize)
    {
        if (newMaxSize < 1)
        {
            Debug.LogWarning("Incorrect max size of " + newMaxSize + " requested for pool " + mPoolName + ", aborting");
            return;
        }

        if (newMaxSize >= mPoolableObjects.Count)
        {
            mMaxPoolSize = newMaxSize;
        }
        else
        {
            Depopulate(mPoolableObjects.Count - newMaxSize);

            if (mPoolableObjects.Count != newMaxSize)
            {
                Debug.LogWarning("There were not enough free objects to destroy, new max size set to " + mPoolableObjects.Count);

                mMaxPoolSize = mPoolableObjects.Count;
            }
            else
            {
                mMaxPoolSize = newMaxSize;
            }
        }
    }

    public void ChangeIncrement(int newIncrement)
    {

        if (newIncrement < 1) {

            Debug.LogWarning("Incorrect increment of " + newIncrement + " assigned to pool " + mPoolName + ", changing to 1");
            newIncrement = 1;
        }

        mPoolSizeIncrement =  newIncrement;
    }

    // You might wanna change that so it can be more useful
    public void Info()
    {
        Debug.Log("Pool name : " + mPoolName + " Pool size" + mPoolableObjects.Count + " Pool max size : " + mMaxPoolSize + " PoolIncrement : " + mPoolSizeIncrement);
    }

    int Populate(int quantity)
    {
        int i = 0;

        while (i < quantity)
        {
            CreatePoolableObject();
            i++;
        }

        return i;
    }

    int Depopulate(int quantity)
    {
        List<PoolableObject> list = GetAvailableObjects();
        int iNumberOfFreeObjects;

        for (iNumberOfFreeObjects = 0; iNumberOfFreeObjects < quantity; iNumberOfFreeObjects++) {

            if (iNumberOfFreeObjects >= list.Count)
            {
                Debug.LogWarning("Can't depopulate the pool " + mPoolName + " enough because there aren't any disabled gameobjects to delete");
                return iNumberOfFreeObjects;
            }

            DestroyPooledObject(list[iNumberOfFreeObjects]);
        }

        return iNumberOfFreeObjects;
    }

    public List<GenericPoolableObject> GetPoolableObjects()
    {
        List<GenericPoolableObject> list = new List<GenericPoolableObject>();

        for (int i = 0; i < mPoolableObjects.Count; i++)
        {
            list.Add(mPoolableObjects[i].GetPoolableObject());
        }

        return list;
    }

    public List<GameObject> GetGameObjects()
    {
        List<GameObject> list = new List<GameObject>();

        for (int i = 0; i < mPoolableObjects.Count; i++)
        {
            list.Add(mPoolableObjects[i].GetGameObject());
        }

        return list;
    }

    List<PoolableObject> GetAvailableObjects()
    {
        List<PoolableObject> list = new List<PoolableObject>();

        for (int i = 0; i < mPoolableObjects.Count; i++)
        {
            if (!mPoolableObjects[i].gameObject.activeSelf)
            {
                list.Add(mPoolableObjects[i]);
            }
        }

        return list;
    }

    void DestroyPooledObject(PoolableObject pobj)
    {
        mPoolableObjects.Remove(pobj);
        GameObject.Destroy(pobj.gameObject);
    }

    PoolableObject CreatePoolableObject()
    {
        GameObject obj = GameObject.Instantiate(mPrefab);
        PoolableObject pobj = obj.AddComponent<PoolableObject>();
        pobj.SetObject(obj.GetComponent<GenericPoolableObject>());
        pobj.gameObject.SetActive(false);
        mPoolableObjects.Add(pobj);

        obj.name = obj.name.Remove(obj.name.Length - 7) + mName;
        mName++;

        if (mPoolParent != null)
        {
            obj.transform.SetParent(mPoolParent.transform);
        }

        return pobj;
    }    
}



/// <summary>
///  Helper class to get global access to pools
/// </summary>
public static class Pool
{
    static Dictionary<string, _Pool> sPoolDictionary;
    static bool sInitialised = false;

    static void Initialize() {
        sPoolDictionary = new Dictionary<string, _Pool>();
        sInitialised = true;
    }

    public static void CreatePool(GameObject prefab, string poolName, int size, int maxSize = 100, int incrementSize = 5, GameObject parent = null)
    {
        if (!sInitialised)
            Initialize();

        _Pool pool = new _Pool(prefab, poolName, size, maxSize, incrementSize, parent);
        sPoolDictionary.Add(poolName, pool);
    }

    public static GameObject Spawn(string name, Vector3 position, Quaternion rotation, Vector3 scale)
    {

        if (!sPoolDictionary.ContainsKey(name))
        {
            Debug.LogWarning("(Spawn) Invalid pool name used :" + name);
            return null;
        }

        return sPoolDictionary[name].Spawn(position, rotation, scale);
    }

    public static GameObject Spawn(string name, Vector3 position)
    {

        if (!sPoolDictionary.ContainsKey(name))
        {
            Debug.LogWarning("(Spawn) Invalid pool name used :" + name);
            return null;
        }

        return sPoolDictionary[name].Spawn(position);
    }

    public static void Destroy(string name, GameObject obj)
    {
        if (!sPoolDictionary.ContainsKey(name))
        {
            Debug.LogWarning("(Destroy) Invalid pool name used :" + name);
            return;
        }

        sPoolDictionary[name].Destroy(obj);
    }

    // WILL DESTROY OBJECTS, PROCEED WITH CARE
    public static void DestroyPool(string name)
    {
        if (!sPoolDictionary.ContainsKey(name))
        {
            Debug.LogWarning("(DestroyPool) Invalid pool name used :" + name);
            return;
        }

        sPoolDictionary[name].DestroyPool();
        sPoolDictionary.Remove(name);
    }

    public static int ShrinkPool(string name, int quantity)
    {
        if (!sPoolDictionary.ContainsKey(name))
        {
            Debug.LogWarning("(Shrink) Invalid pool name used :" + name);
            return 0;
        }

        return sPoolDictionary[name].Shrink(quantity);
    }

    public static int ExpandPool(string name, int quantity)
    {
        if (!sPoolDictionary.ContainsKey(name))
        {
            Debug.LogWarning("(Expand) Invalid pool name used :" + name);
            return 0;
        }

        return sPoolDictionary[name].Expand(quantity);
    }

    public static List<GameObject> GetGameObjects(string name)
    {
        if (!sPoolDictionary.ContainsKey(name))
        {
            Debug.LogWarning("(GetObjects) Invalid pool name used :" + name);
            return null;
        }

        return sPoolDictionary[name].GetGameObjects();
    }

    public static List<GenericPoolableObject> GetGenericObjects(string name)
    {
        if (!sPoolDictionary.ContainsKey(name))
        {
            Debug.LogWarning("(GetObjects) Invalid pool name used :" + name);
            return null;
        }

        return sPoolDictionary[name].GetPoolableObjects();
    }

    public static void Reset()
    {
        List<string> list = new List<string>(sPoolDictionary.Keys);

        foreach (string s in list)
            DestroyPool(s);

        sPoolDictionary.Clear();
    }

    public static void Info(string name)
    {
        if (!sPoolDictionary.ContainsKey(name))
        {
            Debug.LogWarning("(Info) Invalid pool name used : " + name);
            return;
        }

        sPoolDictionary[name].Info();
    }

    public static void Rename(string oName, string nName)
    {
        if (!sPoolDictionary.ContainsKey(oName))
        {
            Debug.LogWarning("(Rename) Invalid pool name used : " + oName);
            return;
        }

        if (sPoolDictionary.ContainsKey(nName))
        {
            Debug.LogWarning("(Rename) Pool " + nName + " already in usage");
            return;
        }

        sPoolDictionary[oName].mPoolName = nName;
        sPoolDictionary.Add(nName, sPoolDictionary[oName]);
        sPoolDictionary.Remove(oName);
    }

    public static void ChangeIncrement(string name, int newIncrement)
    {
        if (!sPoolDictionary.ContainsKey(name))
        {
            Debug.LogWarning("(ChangeIncrement) Invalid pool name used : " + name);
            return;
        }

        sPoolDictionary[name].ChangeIncrement(newIncrement);
    }

    public static void ChangeMaxSize(string name, int newMaxSize)
    {
        if (!sPoolDictionary.ContainsKey(name))
        {
            Debug.LogWarning("(ChangeMaxSize) Invalid pool name used : " + name);
            return;
        }

        sPoolDictionary[name].ChangeMaxSize(newMaxSize);
    }

    public static void DumpInfo()
    {
        foreach (_Pool pool in sPoolDictionary.Values) {
            pool.Info();
        }
    }

    public static List<string> List()
    {
        return new List<string> (sPoolDictionary.Keys);
    }
}