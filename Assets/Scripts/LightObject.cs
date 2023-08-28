using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LightObject : MonoBehaviour
{
    public float battery;
    private float maxBattery;
    public Slider batterySlider;
    public Image sliderFill;
    public AudioClip startAudio, constantAudio;

    public List<Light> lights = new List<Light>();
    private List<float> originalIntensities = new List<float>();

    private AudioSource audioSource;

    private bool isOn;
    public bool canTurn;

    void Awake()
    {
        // Store the intensity of all lights to change them later according to the battery level
        foreach (Light light in lights) originalIntensities.Add(light.intensity);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        maxBattery = battery;
    }

    void Update()
    {
        if (isOn)
        {
            battery -= Time.deltaTime;
            isOn = battery > 0;
            if (constantAudio != null && !audioSource.isPlaying) PlayConstantAudio();
            foreach (Light light in lights)
            {
                light.enabled = true;
                light.intensity = battery / maxBattery * originalIntensities[lights.IndexOf(light)];
            }
        }
        else
        {
            if (constantAudio != null && audioSource.clip == constantAudio) audioSource.Stop();
            foreach (Light light in lights) light.enabled = false;
        }

        batterySlider.value = battery;
        sliderFill.color = Color.Lerp(Color.red, Color.green, batterySlider.value / batterySlider.maxValue);

        if (canTurn && Input.GetKeyDown(KeyCode.N))
        {
            if (!isOn)
            {
                PlayStartAudio();
                if (canTurn && battery > 0) isOn = true;
            }
            else isOn = false;
        }
    }

    void PlayStartAudio()
    {
        audioSource.clip = startAudio;
        audioSource.loop = false;
        audioSource.Play();
    }

    void PlayConstantAudio()
    {
        audioSource.clip = constantAudio;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void OnTriggerStay(Collider other)
    {
        if (isOn && other.gameObject.TryGetComponent(out LightObject lightObject)) lightObject.battery += Time.deltaTime;
    }
}