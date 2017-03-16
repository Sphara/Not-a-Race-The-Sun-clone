using UnityEngine;
using System.Collections;

public class GenericBuff : GenericPoolableObject {

    public Buff buff;
    public Material color;

    Renderer ren;

    override protected void Start()
    {
        base.Start();
    }

    virtual protected void Update()
    {
        transform.Rotate(new Vector3(50, 20, 70) * Time.deltaTime);
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        IBuffable toBuff;

        if ((toBuff = other.GetComponent<IBuffable>()) != null)
        {
            toBuff.Buff(buff);
            Pool.Destroy(mName, gameObject);
        }
    }

    virtual public void ApplyColor()
    {
        if (ren == null)
        {
            ren = GetComponent<Renderer>();
        }

        ren.material = color;
    }
}
