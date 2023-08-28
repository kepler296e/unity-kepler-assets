using UnityEngine;
using System.Collections.Generic;

public class DayNightCycle : MonoBehaviour
{
    private Light sun;
    public GameObject clouds;
    [Range(0, 1)]
    public float daytime = 0.5f;
    public float timeMultiplier = 1f;
    public float maxIntensity = 1f;
    public Gradient cloudsColor;

#if UNITY_EDITOR
    public void EditorSetup()
    {
        sun = GetComponent<Light>();
    }

    public void UpdateEditor()
    {
        Update();
    }
#endif

    void Start()
    {
        sun = GetComponent<Light>();
    }

    void Update()
    {
        sun.transform.localRotation = Quaternion.Euler((daytime * 360f) - 90, 30, 0);

        float dot = Mathf.Clamp01(Vector3.Dot(sun.transform.forward, Vector3.down));
        float intensity = (maxIntensity * dot);
        sun.intensity = intensity;
        sun.bounceIntensity = intensity;
        RenderSettings.ambientIntensity = intensity;
        RenderSettings.reflectionIntensity = intensity;

        clouds.GetComponent<Renderer>().material.SetColor("_CloudColor", cloudsColor.Evaluate(dot));

        daytime += ((Time.deltaTime) / 360f) * timeMultiplier;
        if (daytime >= 1) daytime = 0;
    }
}