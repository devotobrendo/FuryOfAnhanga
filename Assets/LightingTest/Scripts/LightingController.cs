using System.Collections;
using UnityEngine;

public class LightingController : MonoBehaviour
{
    [SerializeField] private Texture2D skyboxNight;
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;
 
    [SerializeField] private Gradient gradientNightToSunrise;
    [SerializeField] private Gradient gradientSunriseToDay;
    [SerializeField] private Gradient gradientDayToSunset;
    [SerializeField] private Gradient gradientSunsetToNight;

    [SerializeField] private float intensityNightToSunrise;
    [SerializeField] private float intensitySunriseToDay;
    [SerializeField] private float intensityDayToSunset;
    [SerializeField] private float intensitySunsetToNight;

    [SerializeField] private float timeTransition;
 
    [SerializeField] private Light globalLight;

    private int currentState = 0;

    private void Start()
    {
        RenderSettings.skybox.SetTexture("_Texture1", skyboxNight);
        RenderSettings.skybox.SetTexture("_Texture2", skyboxSunrise);
        RenderSettings.skybox.SetFloat("_Blend", 0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SkyboxStateChange();
        }
    }

    private void SkyboxStateChange()
    {
        currentState = (currentState + 1) % 4;

        if (currentState == 0)
        {
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, timeTransition));
            StartCoroutine(LerpLight(gradientSunsetToNight, globalLight.intensity, intensitySunsetToNight, timeTransition));
        }
        else if (currentState == 1)
        {
            StartCoroutine(LerpSkybox(skyboxNight, skyboxSunrise, timeTransition));
            StartCoroutine(LerpLight(gradientNightToSunrise, globalLight.intensity, intensityNightToSunrise, timeTransition));
        }
        else if (currentState == 2)
        {
            StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, timeTransition));
            StartCoroutine(LerpLight(gradientSunriseToDay, globalLight.intensity, intensitySunriseToDay, timeTransition));
        }
        else if (currentState == 3)
        {
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, timeTransition));
            StartCoroutine(LerpLight(gradientDayToSunset, globalLight.intensity, intensityDayToSunset, timeTransition));
        }
    }
 
    private IEnumerator LerpSkybox(Texture2D a, Texture2D b, float time)
    {
        RenderSettings.skybox.SetTexture("_Texture1", a);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", 0);
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", i / time);
            yield return null;
        }
        RenderSettings.skybox.SetTexture("_Texture1", b);
    }
 
    private IEnumerator LerpLight(Gradient lightGradient, float initialIntensity, float targetIntensity, float time)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            float t = i / time;
            globalLight.color = lightGradient.Evaluate(t);
            globalLight.intensity = Mathf.Lerp(initialIntensity, targetIntensity, t);
            RenderSettings.fogColor = globalLight.color;
            yield return null;
        }
        globalLight.intensity = targetIntensity;
    }
}
