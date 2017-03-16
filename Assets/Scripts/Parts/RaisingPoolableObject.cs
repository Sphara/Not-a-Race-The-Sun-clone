using UnityEngine;
using System.Collections;

public class RaisingPoolableObject : GenericPoolableObject {

    bool isRaising;
    public float offset = 0;

    protected override void Start()
    {
        base.Start();

        PutUnderground();
        isRaising = false;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        float targetHeight = (transform.localScale.y) / 2.0f + (offset * transform.localScale.y);

        if (!isRaising && (GlobalSettings.PlayerPosition.z + GlobalSettings.ApparitionZone) >= transform.position.z)
        {
            Raise();
        }

        if (isRaising && transform.position.y < targetHeight)
        {
            Vector3 newPosition = transform.position;

            if (transform.position.y + GlobalSettings.RaisingSpeed * Time.fixedDeltaTime <= targetHeight)
            {
                newPosition.y += GlobalSettings.RaisingSpeed * Time.fixedDeltaTime;
            }
            else
            {
                newPosition.y = targetHeight;
            }

            transform.position = newPosition;
        }
    }

    protected virtual void PutUnderground()
    {
        Vector3 newPosition = transform.position;

        newPosition.y = -(transform.localScale.y / 2.0f) + (offset * transform.localScale.y) - 0.1f; //Avoid weird quad overlap

        transform.position = newPosition;
    }

    protected virtual void Raise()
    {
        isRaising = true;
    }

    public override void Reset()
    {
        PutUnderground();
        isRaising = false;
    }
}
