using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : MonoBehaviour
{
    [Header("Panels")]
    public MonsterPanel EnemyPanel;
    public MonsterPanel PlayerPanel;

    [Header("Message")]
    [SerializeField] private Text messageText;
    [SerializeField] private Text hintText;

    [Header("Menu")]
    [SerializeField] private TextMenu menu;

    [Header("FX")]
    [SerializeField] private Image flash;
    [SerializeField] private Image wipeTop;
    [SerializeField] private Image wipeBottom;
    [SerializeField] private GameObject[] evolveRings; 

    public event Action<int> ActionClicked;

    private Coroutine typing;
    private int lastPick;

    void Awake()
    {
        if (menu != null) menu.Picked += OnPicked;
    }

    void OnPicked(int i)
    {
        lastPick = i;
        ActionClicked?.Invoke(i);
    }

    public void ShowMessage(string text)
    {
        if (hintText != null) hintText.enabled = false;
        if (typing != null) StopCoroutine(typing);
        typing = StartCoroutine(Type(text));
    }

    IEnumerator Type(string text)
    {
        if (messageText == null) yield break;
        messageText.text = "";
        float perChar = 1f / 55f;
        for (int i = 0; i < text.Length; i++)
        {
            messageText.text += text[i];
            yield return new WaitForSeconds(perChar);
        }
    }

    public void ShowMenu(IReadOnlyList<string> labels, IReadOnlyList<bool> usable, string prompt, string hint)
    {
        if (typing != null) StopCoroutine(typing);
        if (messageText != null) messageText.text = prompt;
        if (hintText != null)
        {
            hintText.text = hint;
            hintText.enabled = !string.IsNullOrEmpty(hint);
        }
        int start = (usable != null && lastPick < usable.Count && usable[lastPick]) ? lastPick : 0;
        if (menu != null)
            menu.Open(ToArray(labels), ToArray(usable), start);
    }

    public void HideMenu()
    {
        if (menu != null) menu.Close();
    }

    public void PlayIntro() => StartCoroutine(IntroRoutine());

    IEnumerator IntroRoutine()
    {
        float dur = 0.48f;
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float p = t / dur;
            if (wipeTop != null) wipeTop.rectTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(180 * 4, 0, p));
            if (wipeBottom != null) wipeBottom.rectTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(-180 * 4, 0, p));
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float p = t / dur;
            if (wipeTop != null) wipeTop.rectTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(0, 180 * 4, p));
            if (wipeBottom != null) wipeBottom.rectTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(0, -180 * 4, p));
            yield return null;
        }
    }

    public void PlayFlash() => StartCoroutine(FlashRoutine());

    IEnumerator FlashRoutine()
    {
        if (flash == null) yield break;
        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float a = 1f - t / 0.5f;
            flash.color = new Color(1, 1, 1, a);
            yield return null;
        }
        flash.color = new Color(1, 1, 1, 0);
    }

    public void PlayEvolveRings(float duration)
    {
        if (evolveRings != null && evolveRings.Length > 0)
        {
            StartCoroutine(EvolveRingsRoutine(duration));
        }
        else
        {
            Debug.LogWarning("[BattleView] Array evolveRings kosong atau null! Cek Inspector.");
        }
    }

    IEnumerator EvolveRingsRoutine(float duration)
    {
        if (evolveRings == null || evolveRings.Length == 0) yield break;

        for (int i = 0; i < evolveRings.Length; i++)
        {
            if (evolveRings[i] != null)
            {
                evolveRings[i].SetActive(true);
                
                var anim = evolveRings[i].GetComponent<Animator>();
                if (anim != null)
                {
                    anim.Play("RingAnim", 0, 0f); 
                    anim.Update(0f);
                }
            }

            if (i < evolveRings.Length - 1)
            {
                yield return new WaitForSeconds(0.15f);
            }
        }

        float totalDelay = (evolveRings.Length - 1) * 0.15f;
        float remainingTime = Mathf.Max(0f, duration - totalDelay);
        yield return new WaitForSeconds(remainingTime);

        foreach (var ring in evolveRings)
        {
            if (ring != null) ring.SetActive(false);
        }
    }    static T[] ToArray<T>(IReadOnlyList<T> list)
    {
        if (list == null) return null;
        var arr = new T[list.Count];
        for (int i = 0; i < list.Count; i++) arr[i] = list[i];
        return arr;
    }
}