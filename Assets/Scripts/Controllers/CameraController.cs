using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public CameraModes CameraMode;

    public Vector3 offset;

    public Vector3 maxOffset;
	public float followSpeed;

    public GameObject myObjectToFollow;

    Rigidbody rb;
    CameraShake cs;

    void Start()
    {
        cs = GetComponent<CameraShake>();
        rb = myObjectToFollow.GetComponent<Rigidbody>();
    }

	void LateUpdate () {
        if (CameraMode == CameraModes.SOFTFOLLOW)
            SoftFollow();
        else if (CameraMode == CameraModes.HARDFOLLOW)
            HardFollow();
        else if (CameraMode == CameraModes.NONE)
            NoFollow();
        else if (CameraMode == CameraModes.CUTSCENE)
            Cutscene();

        cs.Shake();
    }

    void HardFollow()
    {
        Vector3 newPos = Vector3.Lerp(offset, maxOffset, rb.velocity.z / GlobalSettings.MaxHorizontalSpeed);
            
        transform.position = myObjectToFollow.transform.position + newPos;
    }

    void SoftFollow()
    {
        // Do some magic to follow smoothly the player, this isn't working with inconsistent framerate

        Vector3 tempFollowedObjectWithOffset = myObjectToFollow.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, tempFollowedObjectWithOffset, followSpeed * Time.deltaTime);
    }

    void NoFollow()
    {

    }

    void Cutscene()
    {

    }
}

public enum CameraModes
{
    SOFTFOLLOW = 0,
    HARDFOLLOW = 1,
    CUTSCENE = 2,
    NONE = 3
};