using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;
    [SerializeField] private GameObject clickCatcher;
    
    [Header("Box")]
    [SerializeField] private Image boxImage;
    [SerializeField] private Text bodyText;
    [SerializeField] private Image continueHint;
    
    [Header("Blink")]
    [SerializeField] private float blinkInterval = 0.3f;
    private float blinkTimer;
    private bool showContinueHint; 

    [Header("Speaker")]
    [SerializeField] private GameObject speakerTag;
    [SerializeField] private Text speakerText;
    [SerializeField] private RectTransform speakerTagRect;
    
    [Header("Choices")]
    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private TextMenu choicesMenu;

    private Coroutine typing;
    private bool isNarration;
    private Action<int> onChoicePicked;
    private AudioService audio;

    void Awake()
    {
        audio = FindAnyObjectByType<AudioService>();
        if (continueHint != null) continueHint.gameObject.SetActive(false);
    }

    public void SetChoiceCallback(Action<int> callback) => onChoicePicked = callback;

    void Update()
    {
        if (showContinueHint && continueHint != null)
        {
            blinkTimer += Time.unscaledDeltaTime;
            
            float phase = (blinkTimer % (blinkInterval * 2f)) / blinkInterval;
            float alpha = phase < 1f ? phase : 2f - phase;  // segitiga: naik lalu turun
            
            Color c = continueHint.color;
            c.a = alpha;
            continueHint.color = c;
        }
    }

    public void Show(bool narration)
    {
        isNarration = narration;
        root.SetActive(true);
        if (choicesPanel != null) choicesPanel.SetActive(false);
        if (speakerTag != null) speakerTag.SetActive(!narration);
        
        showContinueHint = false;
        if (continueHint != null) continueHint.gameObject.SetActive(false);
    }

    public void Hide()
    {
        root.SetActive(false);
        showContinueHint = false;
        if (typing != null) StopCoroutine(typing);
    }

    public void ShowChoices(string[] options)
    {
        if (choicesPanel != null) choicesPanel.SetActive(true);
        if (choicesMenu != null)
        {
            choicesMenu.Picked -= HandleChoicePicked;
            choicesMenu.Picked += HandleChoicePicked;
            bool[] enabled = new bool[options.Length];
            for (int i = 0; i < options.Length; i++) enabled[i] = true;
            choicesMenu.Open(options, enabled, 0);
        }
    }

    private void HandleChoicePicked(int index) => onChoicePicked?.Invoke(index);

    public void HideChoices()
    {
        if (choicesPanel != null) choicesPanel.SetActive(false);
        if (choicesMenu != null) choicesMenu.Close();
    }

    public void SetSpeaker(string name)
    {
        if (speakerText != null) speakerText.text = name;
        if (speakerTag != null)
        {
            speakerTag.SetActive(!string.IsNullOrEmpty(name));
            if (speakerTagRect != null && speakerText != null)
            {
                float width = Mathf.Min(488f, name.Length * 20f + 64f);
                speakerTagRect.sizeDelta = new Vector2(width, speakerTagRect.sizeDelta.y);
            }
        }
    }

    public IEnumerator TypeLine(string speaker, string text, bool showContinue)
    {
        SetSpeaker(speaker);
        if (bodyText != null) bodyText.text = "";
        
        showContinueHint = false;
        if (continueHint != null) continueHint.gameObject.SetActive(false);
        
        blinkTimer = 0f;

        float perChar = 1f / 55f;
        for (int i = 0; i < text.Length; i++)
        {
            if (bodyText != null) bodyText.text += text[i];
            
            if (i % 2 == 0 && audio != null) 
                audio.PlayDialogBlip();
            
            yield return new WaitForSeconds(perChar);
        }

        if (showContinue && continueHint != null)
        {
            continueHint.gameObject.SetActive(true);
            continueHint.color = new Color(1, 1, 1, 1);
            showContinueHint = true;
            blinkTimer = 0f;
        }
    }
}