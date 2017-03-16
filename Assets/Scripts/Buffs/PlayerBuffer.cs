using UnityEngine;
using System.Collections.Generic;

public class PlayerBuffer : MonoBehaviour, IBuffable {

    public BuffGenerator bg;
    public TimeController tc;

    delegate void BuffFunction(); 

    Dictionary<Buff, BuffFunction> buffDic;
    PlayerController pc;

    void Start()
    {
        pc = GetComponent<PlayerController>();
        buffDic = new Dictionary<global::Buff, BuffFunction>();

        buffDic.Add(global::Buff.DJUMP, DoubleJump);
        buffDic.Add(global::Buff.ALL, Everything);
        buffDic.Add(global::Buff.LOWGRAV, Gravity);
        buffDic.Add(global::Buff.PORTAL, Door);
        buffDic.Add(global::Buff.KEY, Key);
        buffDic.Add(global::Buff.SHIELD, Shield);
        buffDic.Add(global::Buff.TIME, Time);
    }

    public void Buff(Buff buff)
    {
        BuffFunction func;

        if (!buffDic.ContainsKey(buff))
        {
            Debug.LogWarning("Buff " + buff.ToString() + " recieved by " + gameObject.name + " isn't implemented");
            return;
        }

        func = buffDic[buff];

        func();
    }

    void Shield()
    {
        pc.AddShield();
    }

    void Key()
    {
        bg.IsPlayerHoldingAKey = true;
    }

    void Door()
    {
        if (bg.IsPlayerHoldingAKey)
        {
            bg.IsPlayerHoldingAKey = false;
            // Do some doory stuff here
        }
    }

    void Gravity()
    {
        pc.ChangePlayerGravity(5.0f, 0.3f);
    }

    void Time()
    {
        tc.Distortion(0.4f, 3, 0.5f, 0.5f);
        // things to change : red blocks fall
        // Apply some timey overlay here (purple/wobwobwob ?)
    }

    void DoubleJump()
    {
        pc.AddDoubleJump();
    }

    void Everything()
    {
        foreach (BuffFunction bf in buffDic.Values) {
            if (bf != Everything) {
                bf();
            }
        }
    }
}