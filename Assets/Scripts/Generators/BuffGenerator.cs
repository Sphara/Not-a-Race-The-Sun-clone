using UnityEngine;
using System.Collections.Generic;

public class BuffGenerator : MonoBehaviour {

    public GameObject BasicBuff;
    public Rigidbody player;

    public Material DJump;
    public Material Shield;
    public Material LowGrav;
    public Material TimeWarp;
    public Material Key;
    public Material Portal;
    public Material All;

    public bool IsPlayerHoldingAKey = false;

    float popChance = 0.0f;

    Dictionary<Buff, string> BuffTypeToStringDic;

    void Start () {
        BuffTypeToStringDic = new Dictionary<Buff, string>();

        BuffTypeToStringDic.Add(Buff.ALL, "BuffAll");
        BuffTypeToStringDic.Add(Buff.DJUMP, "BuffDoubleJump");
        BuffTypeToStringDic.Add(Buff.KEY, "BuffKey");
        BuffTypeToStringDic.Add(Buff.LOWGRAV, "BuffLowGravity");
        BuffTypeToStringDic.Add(Buff.PORTAL, "BuffPortal");
        BuffTypeToStringDic.Add(Buff.SHIELD, "BuffShield");
        BuffTypeToStringDic.Add(Buff.TIME, "BuffTimeSlow");

        // The pools are full because i have no system to apply a function on an item at creation within a pool (but it might come at some point)
        Pool.CreatePool(BasicBuff, "BuffAll", 100, 100, 25, gameObject);
        Pool.CreatePool(BasicBuff, "BuffDoubleJump", 100, 100, 25, gameObject);
        Pool.CreatePool(BasicBuff, "BuffKey", 100, 100, 25, gameObject);
        Pool.CreatePool(BasicBuff, "BuffLowGravity", 100, 100, 25, gameObject);
        Pool.CreatePool(BasicBuff, "BuffPortal", 100, 100, 25, gameObject);
        Pool.CreatePool(BasicBuff, "BuffShield", 100, 100, 25, gameObject);
        Pool.CreatePool(BasicBuff, "BuffTimeSlow", 100, 100, 25, gameObject);

        SetupBuffPool("BuffAll", All, Buff.ALL);
        SetupBuffPool("BuffDoubleJump", DJump, Buff.DJUMP);
        SetupBuffPool("BuffKey", Key, Buff.KEY);
        SetupBuffPool("BuffLowGravity", LowGrav, Buff.LOWGRAV);
        SetupBuffPool("BuffPortal", Portal, Buff.PORTAL);
        SetupBuffPool("BuffShield", Shield, Buff.SHIELD);
        SetupBuffPool("BuffTimeSlow", TimeWarp, Buff.TIME);
    }

    void SetupBuffPool(string name, Material color, Buff bufftype)
    {
        List<GameObject> goList = Pool.GetGameObjects(name);

        foreach (GameObject go in goList)
        {
            GenericBuff buff = go.GetComponent<GenericBuff>();

            buff.buff = bufftype;
            buff.color = color;
            buff.mName = name;

            buff.ApplyColor();
        }
    }

    /*
    void Update () {
        popChance += ((player.velocity.z * (GlobalSettings.PopChanceBonus / 10)) / GlobalSettings.MaxSpeed) * Time.timeScale;

        if (popChance > 1)
        {
            popChance -= 1;
            PopRandomBuff(); //We're gonna pop buffs from MapGenerator now
        }
    }
    */

    public void AddBuff(Map map, Vector3 position, Buff buffType)
    {
        map.AddItem(new MapEntry(BuffTypeToStringDic[buffType], position, Quaternion.identity, Vector3.zero, true));
    }

    public void AddRandomBuff(Map map, Vector3 position)
    {
        Buff buff = GetRandomBuff();

        AddBuff(map, position, buff);
    }

    // I'll work on it sometimes
    Buff GetRandomBuff()
    {
        int i = Random.Range(0, 101);

        if (i < 40)
        {
            return Buff.DJUMP;
        }
        else if (i < 55)
        {
            return Buff.TIME;
        }
        else if (i < 70)
        {
            return Buff.LOWGRAV;
        }
        else if (i < 85)
        {
            return Buff.SHIELD;
        }
        else if (i < 95)
        {
            return IsPlayerHoldingAKey ? Buff.PORTAL : Buff.KEY;
        }
        else if (i < 101)
        {
            return Buff.ALL;
        }

        return Buff.NONE;
    }

    // Not tested i don't know why i did it so be careful
    Buff GetRandomWeightUnweighted()
    {
        List<Buff> buffList = new List<Buff>(BuffTypeToStringDic.Keys);

        return buffList[Random.Range(0, buffList.Count)];
    }

    // Is broken for now, there's no buff pool anymore
    void PopRandomBuff()
    {
        GenericBuff BuffObject;

        // Can't put buffs in the air rn because stuff is moving, i'll need a real map generator
        Vector3 position = new Vector3(Random.Range(-GlobalSettings.PopZoneWidth, GlobalSettings.PopZoneWidth), 0.5f, GlobalSettings.SpawnZone - 5) + GlobalSettings.PlayerPosition;

        BuffObject = Pool.Spawn("Buff", position).GetComponent<GenericBuff>();
        BuffObject = SetRandomBuff(BuffObject);

        // TEST 
        //BuffObject.buff = Buff.TIME;
    }

    GenericBuff SetRandomBuff(GenericBuff buff)
    {
        int i = Random.Range(0, 101); // Isn't actually inclusive

        // At some point i'll do something less ugly
        if (i < 40)
        {
            buff.buff = Buff.DJUMP;
            buff.color = DJump;
        }
        else if (i < 55)
        {
            buff.buff = Buff.TIME;
            buff.color = TimeWarp;
        }
        else if (i < 70)
        {
            buff.buff = Buff.LOWGRAV;
            buff.color = LowGrav;
        }
        else if (i < 85)
        {
            buff.buff = Buff.SHIELD;
            buff.color = Shield;
        }
        else if (i < 95)
        {
            buff.buff = IsPlayerHoldingAKey ? Buff.PORTAL : Buff.KEY;
            buff.color = IsPlayerHoldingAKey ? Portal : Key;
        }
        else if (i < 101)
        {
            buff.buff = Buff.ALL;
            buff.color = All;
        }

        buff.ApplyColor();

        return buff;
    }
}
