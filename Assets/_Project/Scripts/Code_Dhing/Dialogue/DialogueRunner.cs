using System;
using System.Collections;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    [SerializeField] private DialogueView view;
    private InputGate gate;
    private AudioService audio; // ✅ TAMBAH INI
    public bool IsRunning { get; private set; }
    private int picked = -1;
    private bool clicked;
    private bool wasConfirmPressed;

    public void Construct(InputGate gate)
    {
        this.gate = gate;
        audio = FindAnyObjectByType<AudioService>(); 

        if (view != null)
        {
            view.SetChoiceCallback(OnChoicePicked);
        }
    }

    public void Run(IEnumerator sequence)
    {
        if (!IsRunning && sequence != null)
            StartCoroutine(RunRoutine(sequence));
    }

    IEnumerator RunRoutine(IEnumerator sequence)
    {
        IsRunning = true;
        gate?.Lock();
        yield return null;
        clicked = false;
        yield return sequence;
        view.Hide();
        yield return null;
        gate?.Unlock();
        IsRunning = false;
    }

    public IEnumerator Say(string speaker, string text)
    {
        view.Show(false);
        yield return TypeLine(speaker, text, true);
    }

    public IEnumerator Narrate(string text)
    {
        view.Show(true);
        yield return TypeLine("", text, true);
    }

    public IEnumerator Choose(string speaker, string prompt, string[] options, Action<int> result)
    {
        view.Show(false);
        yield return TypeLine(speaker, prompt, false);
        picked = -1;
        view.ShowChoices(options);
        yield return new WaitUntil(() => picked >= 0);
        view.HideChoices();
        result?.Invoke(picked);
    }

    public IEnumerator PlayData(DialogueData data)
    {
        if (data == null) yield break;
        foreach (var line in data.Lines)
        {
            if (data.IsNarration)
                yield return Narrate(line);
            else
                yield return Say(data.Speaker, line);
        }
    }

    IEnumerator TypeLine(string speaker, string text, bool showContinue)
    {
        yield return view.TypeLine(speaker, text, showContinue);
        yield return new WaitUntil(AdvancePressed);
    }

    bool AdvancePressed()
    {
        bool confirm = MenuInput.Confirm;
        bool pressed = clicked || (gate != null && gate.InteractPressed) || MenuInput.Confirm;

        if (pressed && !wasConfirmPressed && audio != null)
        {
            audio.PlayConfirm();
        }
        wasConfirmPressed = confirm;

        clicked = false;
        return pressed;
    }

    public void OnClick() => clicked = true;
    public void OnChoicePicked(int index) => picked = index;
}