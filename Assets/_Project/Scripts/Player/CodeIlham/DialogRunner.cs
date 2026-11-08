using System;
using System.Collections;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    [Header("Referensi")]
    [SerializeField] private DialogueView view;

    [Header("Kecepatan Ketik")]
    [SerializeField] private float secondsPerChar = 0.018f;

    public bool IsRunning { get; private set; }

    private InputGate gate;
    private bool clicked;
    private int picked;

    public void SetGate(InputGate newGate)
    {
        gate = newGate;
    }

    public void Run(IEnumerator sequence)
    {
        if (!IsRunning && sequence != null)
            StartCoroutine(RunRoutine(sequence));
    }

    private IEnumerator RunRoutine(IEnumerator sequence)
    {
        IsRunning = true;

        if (gate != null) gate.Lock();

        yield return null;
        clicked = false;

        yield return sequence;

        if (view != null) view.Hide();

        yield return null;

        if (gate != null) gate.Unlock();

        IsRunning = false;
    }

    public IEnumerator Say(string speaker, string text)
    {
        if (view != null) view.Show(false);
        yield return TypeLine(speaker, text, true);
        yield return WaitAdvance();
    }

    public IEnumerator Narrate(string text)
    {
        if (view != null) view.Show(true);
        yield return TypeLine("", text, true);
        yield return WaitAdvance();
    }

    public IEnumerator Choose(string speaker, string prompt,
        string[] options, Action<int> result)
    {
        if (view != null) view.Show(false);
        yield return TypeLine(speaker, prompt, false);
        picked = -1;
        if (view != null) view.ShowChoices(options);
        
        Debug.Log($"[DialogueRunner] Menunggu pilihan... picked={picked}");
        yield return new WaitUntil(() => picked >= 0);
        Debug.Log($"[DialogueRunner] Pilihan diterima: {picked}");
        
        if (view != null) view.HideChoices();
        result?.Invoke(picked);
    }

    private IEnumerator TypeLine(string speaker, string text, bool showContinue)
    {
        if (view != null)
        {
            yield return StartCoroutine(view.TypeText(speaker, text, showContinue, secondsPerChar));
        }
    }

    private IEnumerator WaitAdvance()
    {
        clicked = false;
        yield return new WaitUntil(() => AdvancePressed());
    }

    private bool AdvancePressed()
    {
        bool pressed = clicked || MenuInput.Confirm;
        clicked = false;
        return pressed;
    }

    public IEnumerator PlayData(DialogueData data)
    {
        if (data == null) yield break;
        foreach (var line in data.lines)
        {
            if (string.IsNullOrEmpty(line.speaker))
                yield return Narrate(line.text);
            else
                yield return Say(line.speaker, line.text);
        }
    }

    public void OnClicked()
    {
        clicked = true;
    }

    public void OnPicked(int index)
    {
        Debug.Log($"[DialogueRunner] OnPicked({index})");
        picked = index;
    }
}