using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// FPS count & various GUI stuff (player position tracker atm)
/// </summary>

public class HUDFPS : MonoBehaviour 
{
	public  float updateInterval = 0.5F;
	public GameObject player;

    public PlayerController pc;

	float acc = 0;
	int frames = 0;
	float timeLeft;
	
	string format;
    Rigidbody rb;

	public GUIStyle FPSControllerStyle;

	void Start()
	{
		timeLeft = updateInterval;
        rb = player.GetComponent<Rigidbody>();
        pc = player.GetComponent<PlayerController>();
	}

	void Update () {

		timeLeft -= Time.deltaTime;
		acc += Time.timeScale / Time.deltaTime;
		frames++;
		
		if(timeLeft <= 0.0)
		{
			float fps = acc / frames;
			format = System.String.Format("{0:F2} FPS", fps);
			
			timeLeft = updateInterval;
			acc = 0.0F;
			frames = 0;
		}
	}

	void OnGUI()
	{
		GUI.Label(new Rect(Screen.width / 20, Screen.height / 20, 50, 20), format, FPSControllerStyle);
		GUI.Label(new Rect(Screen.width / 20, Screen.height / 12, 50, 20), "Position = [" + player.transform.position.x.ToString("0.00") + "," + player.transform.position.y.ToString("0.00") + "," + player.transform.position.z.ToString("0.00") + "]", FPSControllerStyle);
        GUI.Label(new Rect(Screen.width / 20, Screen.height / 8, 50, 20), "Velocity = [" + rb.velocity.x.ToString("0.00") + "," + rb.velocity.y.ToString("0.00") + "," + rb.velocity.z.ToString("0.00") + "]", FPSControllerStyle);
        GUI.Label(new Rect(Screen.width - Screen.width / 15, Screen.height / 20, 50, 20), pc.TimeOffTheGround.ToString("0.00"), FPSControllerStyle);
        GUI.Label(new Rect(Screen.width - Screen.width / 15, Screen.height / 12, 50, 20), pc.TimeAirborne.ToString("0.00"), FPSControllerStyle);

    }
}