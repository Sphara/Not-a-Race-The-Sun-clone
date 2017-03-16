using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* blah blah todo
 * 
 * List of cool features to add :
 * Pads to do stuff (jump/acceleration/...)
 * Actual usable items
 * Passive upgrades
 * Stuff taht follows player's X for a certain time, then tries to crush him/pop in front of him
 * Rogue-like talent tree maybe ?
 * More complex obstacles (Events !), like floating stairs, lot of crushing stuff at once...
 * 
*/

// Separate code between the actual controller getting the inputs & the controller doing the physics stuff & the rest because it's getting messy

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Vector3 input;
    int targetXDirection = 0;

    bool isTryingToJump = false;
    bool isOnTheGround = false;
    bool canJump = false;
    int BonusJumps = 0;

    public int JumpVelocity = 10;

    public float TimeAirborne = 0;
    public float TimeOffTheGround = 0;

    public float collisionOffset = 0.25f; // The collider MUST be symmetric (Being a sphere is a plus)

    LinkedList<GameObject> JumpList;

    public ParticleSystem ParticleCollision;
    Collider particleCollider;

    //Stop doing that ffs
    bool warned = false;
    float TimeSinceLastDoubleJump;

    bool isGravityModified = false;
    float gravityFadeOutTime = 0.0f;

    public float gravityMultiplier = 1.0f;

    int shieldCharges = 0;

    // Add an abstraction layer for the particles ?
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        JumpList = new LinkedList<GameObject>();

        ParticleCollision.Stop();
        TimeSinceLastDoubleJump = 0;
        if (rb == null)
        {
            Debug.LogError("Your project might be on fire, idk");
        }

        rb.velocity = new Vector3(0, 0, 20);
    }

    void Update()
    {

        targetXDirection = (int)Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
            isTryingToJump = true;

    }

    void FixedUpdate()
    {
        TimeSinceLastDoubleJump += Time.fixedDeltaTime;
        input = new Vector3(0, transform.position.y > 0 ? -GlobalSettings.PlayerGravity * gravityMultiplier : 0, rb.velocity.z < GlobalSettings.MaxSpeed ? GlobalSettings.Acceleration : 0);

        if (isGravityModified)
            UpdatePlayerGravity();

        if (targetXDirection != 0 && Mathf.Abs(rb.velocity.x) < GlobalSettings.MaxHorizontalSpeed)
        {
            input.x = targetXDirection * GlobalSettings.HorizontalAcceleration;

            if (Mathf.Sign(targetXDirection) != Mathf.Sign(rb.velocity.x))
            {
                input.x *= 1.5f;
            }
        }

        if (rb.velocity.y < 0)
        {
            if (!canJump)
                TimeAirborne += Time.fixedDeltaTime;

            if (TimeAirborne > 1)
            {
                input.y *= (TimeAirborne * TimeAirborne);
            }
        }

        Displacement(input);

        rb.AddForce(input, ForceMode.Acceleration);

        if (isTryingToJump && canJump)
        {
            rb.velocity = new Vector3(rb.velocity.x, JumpVelocity * (1 / gravityMultiplier), rb.velocity.z);
            canJump = false;
        }
        else if (isTryingToJump && BonusJumps > 0 && TimeSinceLastDoubleJump > 1.0f)
        {
            BonusJumps--;
            rb.velocity = new Vector3(rb.velocity.x, JumpVelocity * (1 / gravityMultiplier), rb.velocity.z);
            TimeSinceLastDoubleJump = 0;
        }

        if (!isOnTheGround)
            TimeOffTheGround += Time.fixedDeltaTime;

        isTryingToJump = false;

        if (rb.velocity.z < 5 && !warned)
        {
            Die(EDeathCause.LOWSPEED, "Velocity's too low, you dun goofed");
            warned = true;
        }
        else if (rb.velocity.z > 23) // tis magic
        {
            warned = false;
        }
    }

    // The problem is when you come diagonally so the raycast start point is already inside the collided object 
    // Solution is multiple raycasts.
    Vector3 Displacement(Vector3 input)
    {
        RaycastHit hit;
        bool hits = false;
        int layerMask = (1 << 10); // P-layer
        layerMask |= (1 << 11);
        float XValue = ((rb.velocity.x + (input.x * Time.fixedDeltaTime)) * Time.fixedDeltaTime);
        float ZValue = ((rb.velocity.z + (input.z * Time.fixedDeltaTime)) * Time.fixedDeltaTime);
        // Y Value is ignored because hitting something fast enough to need this function is guaranteed to kill you anyways

        layerMask = ~layerMask;

        List<Vector3> startingPoints = GetStartingPoints();

        // Change all this part when you'll refactor
        foreach (Vector3 startingPoint in startingPoints)
        {
            Debug.DrawRay(startingPoint, new Vector3(XValue, 0, 0), Color.red);
            if (Physics.Raycast(startingPoint, new Vector3(XValue, 0, 0), out hit, Mathf.Abs(XValue), layerMask))
            {
                Vector3 newPos = hit.point;

                newPos.x += Mathf.Sign(-XValue) * collisionOffset;

                transform.position = newPos;
                input.x = 0.1f * Mathf.Sign(XValue);

                newPos = Vector3.zero;
                newPos.x = Mathf.Sign(XValue) * (collisionOffset * 2);

                ParticleCollision.transform.localPosition = newPos;
                ParticleCollision.Play();
                particleCollider = hit.collider;
                hits = true;
            }

        }

        if (hits)
        {

        }

        return input;
    }

    List<Vector3> GetStartingPoints()
    {
        List<Vector3> startingPoints = new List<Vector3>();

        Vector3 startingPoint = transform.position;
        startingPoint.x += Mathf.Sign(rb.velocity.x) * (collisionOffset); // Change rb

        startingPoints.Add(startingPoint);

        return startingPoints;
    }

    void ResetJump()
    {
        canJump = true;
        TimeAirborne = 0.0f;
    }

    public void ChangePlayerGravity(float duration, float multiplier)
    {
        if (isGravityModified)
        {
            Debug.Log("Tried to modify player gravity while it was already modified");
            return;
        }

        if (multiplier == 0)
        {
            Debug.Log("You're about to divide something by 0. Don't do it ! (I am the player controlller)");
            return;
        }

        gravityFadeOutTime = Time.time + duration;
        gravityMultiplier = multiplier;

        isGravityModified = true;
    }

    void UpdatePlayerGravity()
    {
        if (Time.time >= gravityFadeOutTime && canJump)
        {
            gravityMultiplier = 1;
            isGravityModified = false;
        }
    }

    public void AddDoubleJump()
    {
        if (BonusJumps < 3)
            BonusJumps++;
    }

    public void AddShield()
    {
        if (shieldCharges < 3)
        {
            shieldCharges++;
        }
    }

    public bool ShieldProc(EDeathCause cause = EDeathCause.CRUSHED)
    {
        if (shieldCharges < 1)
            return false;

        Vector3 rayOrigin = transform.position;

        shieldCharges--;

        for (int i = 0; i < 30; i++)
        {
            rayOrigin.y += 2;

            if (!Physics.Raycast(rayOrigin, new Vector3(0, 0, 1), 30)) {
                transform.position = rayOrigin;
                isOnTheGround = false;
                canJump = false;
                TimeAirborne = 0.0f;
                return true;
            }
        }

        return false;

        // You might want to do something special for when you're crushed (temporary invincibility ?)
    }

    void OnCollisionEnter(Collision c)
    {

        if (c.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            TimeOffTheGround = 0;
            isOnTheGround = true;
            canJump = true;
        }

        foreach (ContactPoint cont in c.contacts)
        {
            Vector3 p = transform.InverseTransformPoint(cont.point);

            // Landing on smth
            if (p.y < -0.3)
            {
                JumpList.AddFirst(c.gameObject);
                ResetJump();                
            }

            //Faceplant
            if (cont.normal.z < -0.8f)
            {
                Die(EDeathCause.FRONTALCOLLISION, "Heh. You died !");
            }

            //Getting landed on by something 
            if (p.y > 0.1 && Mathf.Abs(cont.normal.y) > 0.2) {
                Die(EDeathCause.CRUSHED, !canJump ? "You got something heavy on your face" : "You got crushed.");
            }

        }
    }

    void Die(EDeathCause cause, string deathMessage)
    {
        if (cause == EDeathCause.CRUSHED || cause == EDeathCause.FRONTALCOLLISION)
        {
            if (ShieldProc(cause))
            {
                return;
            }
        }

        Debug.LogWarning(deathMessage + " (Cause of death : " + cause.ToString() + ")");
    }

    void OnCollisionExit(Collision c)
    {
        if (JumpList.Contains(c.gameObject))
            JumpList.Remove(c.gameObject);

        if (c.collider == particleCollider)
            ParticleCollision.Stop();

        if (c.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isOnTheGround = false;
        }

        if (JumpList.Count == 0)
        {
            canJump = false;
        }
    }
}
