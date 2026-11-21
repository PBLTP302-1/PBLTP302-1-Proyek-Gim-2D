using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class LightZone : MonoBehaviour
{
    [Header("Global Light (Light2D)")]
    [SerializeField] private Light2D globalLight;
    
    [Header("Point Lights (Light2D)")]
    [SerializeField] private Light2D playerLight;
    [SerializeField] private Light2D[] extraLights;
    
    [Header("Settings")]
    [SerializeField] private float insideIntensity = 0.15f;
    [SerializeField] private float fadeSpeed = 2f;
    
    private float target = 1f;

    void OnTriggerEnter2D(Collider2D o)
    {
        if (o.CompareTag("Player")) target = insideIntensity;
    }

    void OnTriggerExit2D(Collider2D o)
    {
        if (o.CompareTag("Player")) target = 1f;
    }

    void Update()
    {
        if (globalLight != null)
        {
            globalLight.intensity = Mathf.MoveTowards(
                globalLight.intensity, target,
                fadeSpeed * Time.deltaTime);
        }
        
        float dark = 1f - target;
        
        if (playerLight != null)
        {
            playerLight.intensity = dark;
        }
        
        foreach (Light2D l in extraLights)
        {
            if (l != null) l.intensity = dark;
        }
    }
}