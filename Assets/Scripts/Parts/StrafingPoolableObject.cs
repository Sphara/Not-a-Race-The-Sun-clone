using UnityEngine;
using System.Collections;

public class StrafingPoolableObject : GenericPoolableObject {

    public Vector3 direction;
    protected float force;
    protected Rigidbody rb;
    public float lastDirectionChange = 0.0f;

	protected override void Start() {
        base.Start();

        rb = GetComponent<Rigidbody>();
        force = 15;
        direction = new Vector3(0, 0, 0);

        Reset();
	}
	
	protected override void FixedUpdate() {

        base.FixedUpdate();

        if (rb.velocity.x < force) // To change to some proper stuff, works for now
            rb.AddForce(direction, ForceMode.Acceleration);

        if (Mathf.Abs(rb.velocity.x) < 0.1f && lastDirectionChange > 3.0f)
        {
            direction.x *= -1;
            lastDirectionChange = 0;
        }

        lastDirectionChange += Time.fixedDeltaTime;
	}

    public override void Reset() {
        direction = new Vector3((Random.Range(1, 3) % 2) == 0 ? 1 : -1, 0, 0); // change speed at some point
        direction.x *= force;

        Vector3 newPos = transform.position;

        newPos.y = transform.localScale.y / 2;

        transform.position = newPos;
    }
}
