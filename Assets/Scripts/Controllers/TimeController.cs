using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeController : MonoBehaviour {

    [Range(0.01f, 2)]
    public float timeScale = 1;

    [SerializeField]
    bool isModifiable = true;
    bool isDistortionInProgress = false;

	void Update () {
        if (isModifiable)
            Time.timeScale = timeScale;
	}

    public void Distortion(float targetTimeScale, float duration, float fadeInDuration, float fadeOutDuration)
    {
        if (fadeInDuration + fadeOutDuration > duration)
        {
            Debug.LogWarning("Requested a timescale change with incoherent parameters");
            return;
        }

        if (isDistortionInProgress)
        {
            Debug.LogWarning("Requested a timescale change while another is still active");
            return;
        }

        IEnumerator coroutine;

        List<KeyValuePair<float, float>> timeTable = new List<KeyValuePair<float, float>>()
        {
            new KeyValuePair<float, float> (targetTimeScale, fadeInDuration),
            new KeyValuePair<float, float> (targetTimeScale, duration - (fadeOutDuration + fadeInDuration)),
            new KeyValuePair<float, float> (Time.timeScale, fadeOutDuration)
        };

        isDistortionInProgress = true;
        isModifiable = false;

        coroutine = TimeDistortion(timeTable, Time.time);
        StartCoroutine(coroutine);
    }

    IEnumerator TimeDistortion(List<KeyValuePair<float, float>> TimeTable, float baseTime)
    {
        int i = 0;
        float baseTimeScale = Time.timeScale;

        // Blah blah blah later
        while (true)
        {
            float t = Mathf.InverseLerp(baseTime, baseTime + TimeTable[i].Value, Time.time);
            float newTimeScale = Mathf.Lerp(baseTimeScale, TimeTable[i].Key, t);

            Time.timeScale = newTimeScale;
            timeScale = newTimeScale;

            yield return null;

            if (baseTime + TimeTable[i].Value < Time.time)
            {
                //Debug.Log(i + " " + baseTime + TimeTable[i].Value + "/" + Time.time);
                baseTimeScale = Time.timeScale;
                baseTime = Time.time;
                i++;
            }

            if (i >= TimeTable.Count)
                break;
        }

        isDistortionInProgress = false;
        isModifiable = false;
    }
}
