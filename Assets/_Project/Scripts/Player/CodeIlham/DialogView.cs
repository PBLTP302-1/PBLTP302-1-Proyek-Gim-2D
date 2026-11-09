using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueView : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject dialogueRoot;
    [SerializeField] private GameObject clickCatcher; // WAJIB punya Image (alpha 0) & Button

    [Header("Box Dialog")]
    [SerializeField] private Image boxImage;
    [SerializeField] private TextMeshProUGUI bodyText;

    [Header("Speaker")]
    [SerializeField] private GameObject speakerTag; // WAJIB punya Image (UI_Panel_Gold)
    [SerializeField] private TextMeshProUGUI speakerText;

    [Header("Continue Hint")]
    [SerializeField] private GameObject continueHint; // Teks "▼"

    [Header("Choices")]
    [SerializeField] private GameObject choicesPanel; // WAJIB punya Image (UI_Panel_Navy)
    [SerializeField] private TextInput choicesMenu;   // Sesuai nama class Anda

    [Header("Dialogue Runner")]
    [SerializeField] private DialogueRunner runner;

    [Header("Sprite Box")]
    [SerializeField] private Sprite spriteNavy;   // UI_Panel_Navy
    [SerializeField] private Sprite spriteNarr;   // UI_Panel_Narr

    public bool IsTyping { get; private set; }

    private Coroutine typingCoroutine;
    private Coroutine blinkCoroutine;

    void Awake()
    {
        if (dialogueRoot != null) dialogueRoot.SetActive(false);
        if (choicesPanel != null) choicesPanel.SetActive(false);
        if (continueHint != null) continueHint.SetActive(false);

        // WAJIB: Set warna box ke PUTIH agar sprite tampil warna asli
        if (boxImage != null) boxImage.color = Color.white;

        // Hubungkan ClickCatcher untuk advance dialog (klik mouse di area dialog)
        if (clickCatcher != null)
        {
            Button btn = clickCatcher.GetComponent<Button>();
            if (btn == null) btn = clickCatcher.AddComponent<Button>();
            
            // Hapus listener lama untuk menghindari duplikasi
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { 
                if (runner != null) runner.OnClicked(); 
            });
        }

        // Hubungkan TextInput.Picked ke DialogueRunner.OnPicked
        if (choicesMenu != null && runner != null)
        {
            choicesMenu.Picked += (index) => runner.OnPicked(index);
        }
    }

    public void Show(bool isNarration)
    {
        if (dialogueRoot != null) dialogueRoot.SetActive(true);

        if (boxImage != null)
        {
            boxImage.color = Color.white; // Selalu putih!
            if (isNarration && spriteNarr != null)
                boxImage.sprite = spriteNarr; // Sprite narasi (biru gelap)
            else if (!isNarration && spriteNavy != null)
                boxImage.sprite = spriteNavy; // Sprite dialog (navy)
        }

        if (speakerTag != null) speakerTag.SetActive(false);
        if (continueHint != null) continueHint.SetActive(false);
        if (choicesPanel != null) choicesPanel.SetActive(false);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
    }

    public void Hide()
    {
        if (dialogueRoot != null) dialogueRoot.SetActive(false);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        IsTyping = false;
    }

    public IEnumerator TypeText(string speaker, string text, bool showContinue, float secondsPerChar)
    {
        IsTyping = true;

        // Handle Speaker Tag dengan Lebar Dinamis (Modul P6 Slide 8)
        if (!string.IsNullOrEmpty(speaker))
        {
            if (speakerTag != null)
            {
                speakerTag.SetActive(true);
                // Lebar min(488, huruf × 20 + 64)
                
            }
            if (speakerText != null) speakerText.text = speaker;
        }
        else
        {
            if (speakerTag != null) speakerTag.SetActive(false);
        }

        if (bodyText != null) bodyText.text = "";
        if (continueHint != null) continueHint.SetActive(false);

        if (bodyText != null && !string.IsNullOrEmpty(text))
        {
            foreach (char c in text)
            {
                bodyText.text += c;
                yield return new WaitForSeconds(secondsPerChar);
            }
        }

        IsTyping = false;

        if (showContinue && continueHint != null)
        {
            continueHint.SetActive(true);
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkHint(0.3f));
        }
    }

    public void ShowChoices(string[] options)
    {
        if (choicesPanel != null) choicesPanel.SetActive(true);
        if (continueHint != null) continueHint.SetActive(false);

        if (choicesMenu != null && options != null)
        {
            choicesMenu.Open(options);
        }
    }

    public void HideChoices()
    {
        if (choicesPanel != null) choicesPanel.SetActive(false);
    }

    private IEnumerator BlinkHint(float interval)
    {
        while (continueHint != null && continueHint.activeSelf)
        {
            continueHint.SetActive(false);
            yield return new WaitForSeconds(interval);
            if (continueHint != null) continueHint.SetActive(true);
            yield return new WaitForSeconds(interval);
        }
    }
}