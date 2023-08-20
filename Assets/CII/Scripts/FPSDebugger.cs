using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSDebugger : MonoBehaviour
{
    int frameCount;
    float prevTime;

    [SerializeField] TextMeshProUGUI fpsText;

    void Start()
    {
        frameCount = 0;
        prevTime = 0.0f;
    }

    void Update()
    {
        ++frameCount;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            // Debug.LogFormat("{0}fps", frameCount / time);
            fpsText.text = (frameCount / time).ToString() + " fps";

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
    }
}
