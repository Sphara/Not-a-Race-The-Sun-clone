using UnityEngine;
using System.Collections;

// Might want to change to some smoothed fall
public class CrushingPoolableObject : GenericPoolableObject {

    bool isFalling;
    bool hasCollided = false;

    Rigidbody rb;
    Collider coll;
    CameraShake cs;

    int FallTicks = 0;
    float DelayedStart;
    float TimeModifier;

    protected override void Start()
    {
        base.Start();

        cs = Camera.main.GetComponent<CameraShake>();
        rb = GetComponent <Rigidbody>();
        coll = GetComponent<Collider>();
        PutInTheSky();
        isFalling = false;
        DelayedStart = Random.Range(0.0f, Mathf.PI * 2);
        TimeModifier = Random.Range(0.5f, 1.5f);

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        float targetHeight = transform.localScale.y / 2.0f;

        if (!isFalling && (GlobalSettings.PlayerPosition.z + GlobalSettings.CrushZone) >= transform.position.z)
        {
            Crush();
        }

        if (!isFalling)
        {
            Vector3 newPosition = transform.position;

            newPosition.y += Mathf.Cos((Time.timeSinceLevelLoad * TimeModifier) + DelayedStart) * 0.1f;

            transform.position = newPosition;
        }

        if (isFalling && !hasCollided && transform.position.y > targetHeight)
        {
            FallNoPhysics(targetHeight); // REPLACE BY SOMETHING THAT CAN HANDLE INTERPOLATION (It'll be unity physics coz u too lazy to do your own)
        }
    }

    protected void FallPhysics(float targetHeight)
    {
        //Throw the fucker down and have it NOT bouncing
    }

    protected void FallNoPhysics(float targetHeight)
    {
        Vector3 newPosition = transform.position;
        float fall = (GlobalSettings.FallingSpeed * Time.fixedDeltaTime) * (Mathf.Pow(GlobalSettings.CrushingAcceleration, 4) * FallTicks);

        FallTicks++;

        if (transform.position.y - fall > targetHeight)
        {
            newPosition.y -= fall;
        }
        else
        {
            newPosition.y = targetHeight;
            Quake();
        }

        Collider[] cl = Physics.OverlapBox(newPosition, coll.bounds.extents, transform.rotation, ~(1 << LayerMask.NameToLayer("Ground"))); // THE DOC IS F*CKING WRONG AGAIN

        if (cl.Length > 0)
        {
            bool goodCollision = false;

            foreach (Collider c in cl)
            {
                if (c.gameObject.name != name)
                {


                    goodCollision = true;
                    Quake();
                }
            }

            if (goodCollision)
            {
                rb.isKinematic = false;
                float newMass = transform.localScale.x + transform.localScale.y + transform.localScale.z;

                rb.mass = newMass * newMass;
                rb.drag = 1;

                rb.AddForce(new Vector3(0, -fall * newMass, 0), ForceMode.VelocityChange);

                hasCollided = true;
            }
        }

        if (!hasCollided)
            transform.position = newPosition;
    }

    protected virtual void Quake()
    {
        float maxDist = 50;
        float dist = Vector3.Distance(GlobalSettings.PlayerPosition, transform.position);

        if (dist < maxDist)
        {
            float shake = (maxDist - dist) / (maxDist * 10);

            cs.AddShake(new Shake(shake, 1, shake));
        }

        // Add sound ?
    }

    protected virtual void PutInTheSky()
    {
        Vector3 newPosition = transform.position;

        newPosition.y = (transform.localScale.y / 2.0f) * GlobalSettings.CrushHeight;

        transform.position = newPosition;
    }

    protected virtual void Crush()
    {
        isFalling = true;
    }

    public override void Reset()
    {
        PutInTheSky();
        FallTicks = 0;

        if (hasCollided)
        {
            rb.isKinematic = true;
            hasCollided = false;
        }

        isFalling = false;
    }
}
