using System.Collections;
using UnityEngine;
using TMPro;

public class ToastView : MonoBehaviour
{
    [Header("Referensi UI")]
    [SerializeField] private GameObject box;        // 🟢 Panel/Box toast (UI_Panel_Gold)
    [SerializeField] private TextMeshProUGUI text;  // 🟢 Teks pesan

    [Header("Pengaturan")]
    [SerializeField] private float displayTime = 2.5f;

    private Coroutine showCoroutine;

    void Awake()
    {
        // 🟢 Pastikan box tersembunyi saat game mulai
        if (box != null)
        {
            box.SetActive(false);
        }
    }

    public void Show(string message)
    {
        Debug.Log($"[ToastView] Show dipanggil: '{message}'");
        
        if (text == null)
        {
            Debug.LogError("[ToastView] Field 'Text' kosong! Drag TextMeshProUGUI ke Inspector.");
            return;
        }

        if (box == null)
        {
            Debug.LogError("[ToastView] Field 'Box' kosong! Drag GameObject box toast ke Inspector.");
            return;
        }

        // 1. Set teks
        text.text = message;

        // 2. Tampilkan box
        box.SetActive(true);
        Debug.Log("[ToastView] Box diaktifkan");

        // 3. Jalankan timer untuk menyembunyikan
        if (showCoroutine != null) StopCoroutine(showCoroutine);
        showCoroutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        yield return new WaitForSeconds(displayTime);

        // Sembunyikan box setelah waktu habis
        if (box != null)
        {
            box.SetActive(false);
            Debug.Log("[ToastView] Box dinonaktifkan");
        }
    }
}