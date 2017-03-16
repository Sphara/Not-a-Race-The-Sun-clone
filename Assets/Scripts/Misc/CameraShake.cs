using UnityEngine;
using System.Collections.Generic;

// Test displacement vs angle change
public class CameraShake : MonoBehaviour {

    List<Shake> shakeList;
    float Intensity;

    public PlayerController pc;

    void Start()
    {
        shakeList = new List<Shake>();
    }

    public void AddShake(Shake shake)
    {
        shakeList.Add(shake);
    }

    public void Shake()
    {
        Intensity = 0.0f;

        for (int i = shakeList.Count - 1; i >= 0; i--)
        {
            shakeList[i].Duration -= Time.deltaTime;
            Intensity += shakeList[i].Intensity;

            // Linear is fine
            shakeList[i].Intensity -= (shakeList[i].Smoothing * Time.deltaTime);

            if (shakeList[i].Duration <= 0)
            {
                shakeList.RemoveAt(i);
            }
        }

        //After calculations for shaking decay
        if (pc.TimeAirborne > 0)
            return;

        Vector3 shake = Random.insideUnitSphere * Intensity;
        shake.z = 0;

        transform.position += shake;
    }
}

public class Shake
{
    public float Intensity;
    public float Duration;
    public float Smoothing;

    public Shake(float intensity, float duration = 1.0f, float smoothing = 0.0f)
    {
        Intensity = intensity;
        Duration = duration;
        Smoothing = smoothing;
    }
}