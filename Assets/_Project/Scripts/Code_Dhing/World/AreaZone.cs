using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AreaZone : MonoBehaviour
{
    [SerializeField] private string areaName = "Desa Awal";
    [SerializeField] private AudioClip music;

    public string AreaName => areaName;
    private AudioService audio;

    void Awake() => audio = FindAnyObjectByType<AudioService>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log($"Masuk area: {areaName}");

        var hud = FindAnyObjectByType<AreaHud>();
        if (hud != null) hud.SetArea(areaName);

        if (audio != null && music != null)
            audio.PlayWorldMusic(music);
    }

    void OnTriggerExit2D(Collider2D other)
    {
    }
}