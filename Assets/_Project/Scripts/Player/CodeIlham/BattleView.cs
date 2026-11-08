using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleView : MonoBehaviour
{
    [SerializeField] private MonsterPanel playerPanel;
    [SerializeField] private MonsterPanel enemyPanel;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private TextInput menu;
    [SerializeField] private Image flash;
    [SerializeField] private Image[] rings;
    [SerializeField] private Image wipeTop;
    [SerializeField] private Image wipeBottom;

    public MonsterPanel PlayerPanel => playerPanel;
    public MonsterPanel EnemyPanel => enemyPanel;

    public event System.Action<int> ActionClicked;

    private Coroutine typing;
    private int lastPick = 0;

    void Awake()
    {
        if (flash != null) flash.gameObject.SetActive(false);
        if (rings != null) foreach (var r in rings) if (r != null) r.gameObject.SetActive(false);
        if (wipeTop != null) wipeTop.gameObject.SetActive(false);
        if (wipeBottom != null) wipeBottom.gameObject.SetActive(false);
        if (hintText != null) hintText.enabled = false;

        if (menu != null) menu.Picked += index => ActionClicked?.Invoke(index);
    }

    public void ShowMessage(string text)
    {
        if (hintText != null) hintText.enabled = false;
        if (typing != null) StopCoroutine(typing);
        typing = StartCoroutine(Type(text));
    }

    private IEnumerator Type(string text)
    {
        if (messageText == null) yield break;
        messageText.text = "";
        float secondsPerChar = 0.018f;
        foreach (char c in text)
        {
            messageText.text += c;
            yield return new WaitForSeconds(secondsPerChar);
        }
    }

    public void ShowMenu(IReadOnlyList<string> labels, IReadOnlyList<bool> usable, string prompt, string hint)
    {
        if (typing != null) StopCoroutine(typing);
        if (messageText != null) messageText.text = prompt;
        if (hintText != null)
        {
            hintText.text = hint;
            hintText.enabled = true;
        }

        int start = usable.Count > lastPick && usable[lastPick] ? lastPick : 0;
        if (menu != null)
        {
            menu.Open(new List<string>(labels).ToArray(), new List<bool>(usable).ToArray(), start);
        }
    }

    public void PlayIntro()
    {
        StartCoroutine(IntroRoutine());
    }

    private IEnumerator IntroRoutine()
    {
        if (wipeTop != null) wipeTop.gameObject.SetActive(true);
        if (wipeBottom != null) wipeBottom.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.48f);
        if (wipeTop != null) wipeTop.gameObject.SetActive(false);
        if (wipeBottom != null) wipeBottom.gameObject.SetActive(false);
    }

    public void PlayFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        if (flash == null) yield break;
        flash.gameObject.SetActive(true);
        Color c = flash.color;
        c.a = 1f;
        flash.color = c;
        for (float t = 0f; t < 0.5f; t += Time.deltaTime)
        {
            c.a = 1f - (t / 0.5f);
            flash.color = c;
            yield return null;
        }
        flash.gameObject.SetActive(false);
    }

    public void PlayEvolveRings(float duration)
    {
        StartCoroutine(EvolveRingsRoutine(duration));
    }

    private IEnumerator EvolveRingsRoutine(float duration)
    {
        if (rings == null) yield break;
        foreach (var r in rings) if (r != null) r.gameObject.SetActive(true);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float p = t / duration;
            for (int i = 0; i < rings.Length; i++)
            {
                if (rings[i] == null) continue;
                float scale = 1f + p * 3f + i * 0.3f;
                rings[i].transform.localScale = new Vector3(scale, scale, 1f);
                Color c = rings[i].color;
                c.a = 1f - p;
                rings[i].color = c;
            }
            yield return null;
        }
        foreach (var r in rings) if (r != null) r.gameObject.SetActive(false);
    }
}