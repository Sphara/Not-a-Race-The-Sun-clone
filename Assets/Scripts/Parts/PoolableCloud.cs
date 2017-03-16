using UnityEngine;
using System.Collections;

public class PoolableCloud : GenericPoolableObject {

    CloudGenerator cg;
    Vector3 LocalWindModifier;

    override protected void Start()
    {
        base.Start();

        cg = transform.parent.GetComponent<CloudGenerator>();

        if (cg == null)
        {
            Debug.LogError("Cloud Generator not found or isn't a parent of cloud " + GetInstanceID());
            gameObject.SetActive(false);
        }

        LocalWindModifier = Vector3.zero; // Change to some little values, maybe modify slightly in update ?
    }

    override protected void FixedUpdate()
    {
        base.FixedUpdate();

        Vector3 velocity = cg.Wind + LocalWindModifier;

        transform.Translate(velocity * Time.deltaTime);
    }
}
