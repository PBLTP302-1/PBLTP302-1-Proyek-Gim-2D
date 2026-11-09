using UnityEngine;

public class AreaZoneI: MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip areaMusic;

    [Header("Debug")]
    [SerializeField] private string areaName = "Area";

    private GameServicesI services;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (services == null)
            {
                services = GameBootstrapI.CurrentServices;
            }

            if (services.Audio != null && areaMusic != null)
            {
                services.Audio.PlayWorldMusic(areaMusic);
                Debug.Log($"[AreaZone] Masuk area: {areaName}, musik: {areaMusic.name}");
            }
        }
    }
}