using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueView : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject dialogueRoot;
    [SerializeField] private GameObject clickCatcher;

    [Header("Box Dialog")]
    [SerializeField] private Image boxImage;
    [SerializeField] private TextMeshProUGUI bodyText;

    [Header("Speaker")]
    [SerializeField] private GameObject speakerTag;
    [SerializeField] private TextMeshProUGUI speakerText;

    [Header("Continue Hint")]
    [SerializeField] private GameObject continueHint;

    [Header("Choices")]
    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private TextInput choicesMenu;

    [Header("Dialogue Runner")]
    [SerializeField] private DialogueRunner runner;

    public bool IsTyping { get; private set; }

    private Coroutine typingCoroutine;
    private Coroutine blinkCoroutine;

    private static readonly Color ColorNavy = new Color(0.11f, 0.08f, 0.25f, 1f);
    private static readonly Color ColorNarr = new Color(0.07f, 0.06f, 0.14f, 1f);

    void Awake()
    {
        if (dialogueRoot != null) dialogueRoot.SetActive(false);
        if (choicesPanel != null) choicesPanel.SetActive(false);
        if (continueHint != null) continueHint.SetActive(false);

        // 🟢 WAJIB: Hubungkan TextMenu.Picked ke DialogueRunner.OnPicked
        if (choicesMenu != null && runner != null)
        {
            choicesMenu.Picked += (index) =>
            {
                Debug.Log($"[DialogueView] Pilihan dipilih: {index}");
                runner.OnPicked(index);
            };
            Debug.Log("[DialogueView] TextMenu.Picked terhubung ke DialogueRunner.OnPicked");
        }
        else
        {
            Debug.LogError($"[DialogueView] Koneksi gagal! choicesMenu={choicesMenu != null}, runner={runner != null}");
        }
    }

    public void Show(bool isNarration)
    {
        if (dialogueRoot != null) dialogueRoot.SetActive(true);

        if (boxImage != null)
        {
            boxImage.color = isNarration ? ColorNarr : ColorNavy;
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

        if (!string.IsNullOrEmpty(speaker))
        {
            if (speakerTag != null) speakerTag.SetActive(true);
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
            Debug.Log($"[DialogueView] ShowChoices: {options.Length} options");
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