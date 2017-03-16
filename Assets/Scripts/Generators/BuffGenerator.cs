using UnityEngine;
using System.Collections;

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

    void Start () {
        Pool.CreatePool(BasicBuff, "Buff", 25, 100, 25, gameObject);
	}
	
	void Update () {
        popChance += ((player.velocity.z * (GlobalSettings.PopChanceBonus / 10)) / GlobalSettings.MaxSpeed) * Time.timeScale;

        if (popChance > 1)
        {
            popChance -= 1;
            PopRandomBuff();
        }
    }

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
