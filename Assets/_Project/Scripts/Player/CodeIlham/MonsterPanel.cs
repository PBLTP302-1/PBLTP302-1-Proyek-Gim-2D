using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterPanel : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField] private SpriteRenderer body;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI elementText;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Bars")]
    [SerializeField] private Image hpFill;
    [SerializeField] private Image expFill;

    private MonsterInstanceI shown;
    private MonsterDataI displayData;
    private bool isEnemyPanel;
    private float hpShow;
    private Vector3 home;
    private float lungeStart;
    private bool hurting;
    private bool evolving;

    public void Show(MonsterInstanceI instance, bool isEnemy, MonsterDataI displayAs = null)
    {
        shown = instance;
        displayData = displayAs ?? instance.Data;
        isEnemyPanel = isEnemy;
        hpShow = instance.CurrentHP;

        if (nameText != null) nameText.text = displayData.DisplayName;
        if (levelText != null) levelText.text = $"Lv {instance.Level}";
        if (elementText != null)
        {
            elementText.text = instance.Data.Element.ToString();
            elementText.gameObject.SetActive(isEnemy);
        }

        if (body != null && displayData.Frames != null && displayData.Frames.Length > 0)
        {
            body.sprite = displayData.Frames[0];
            body.enabled = true;
            body.color = Color.white;
            home = body.transform.localPosition;
        }

        if (hpFill != null)
        {
            hpFill.fillAmount = instance.CurrentHP / instance.MaxHP;
            hpFill.color = HpColors.For(hpFill.fillAmount);
        }

        if (hpText != null)
        {
            if (isEnemy)
            {
                hpText.text = "";
            }
            else
            {
                hpText.text = $"{instance.CurrentHP}/{instance.MaxHP}";
            }
        }

        if (expFill != null && !isEnemy)
        {
            expFill.gameObject.SetActive(true);
            float expRatio = (float)instance.CurrentEXP / instance.ExpToNext;
            expFill.fillAmount = Mathf.Clamp01(expRatio);
        }
        else if (expFill != null)
        {
            expFill.gameObject.SetActive(false);
        }
    }

    public void PlayLunge(bool isPlayer)
    {
        lungeStart = Time.time;
    }

    public void PlayHurt()
    {
        StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        hurting = true;
        if (body != null) body.color = Color.white;
        
        for (float t = 0f; t < 0.4f; t += Time.deltaTime)
        {
            if (body != null)
            {
                body.color = Mathf.FloorToInt(t / 0.1f) % 2 == 0 ? Color.white : Color.red;
            }
            yield return null;
        }

        if (body != null) body.color = Color.white;
        hurting = false;
    }

    public void PlayFaint()
    {
        StartCoroutine(FaintRoutine());
    }

    private IEnumerator FaintRoutine()
    {
        float duration = 0.5f;
        Vector3 startPos = body != null ? body.transform.localPosition : Vector3.zero;
        Vector3 endPos = startPos + new Vector3(0, -20f / 16f, 0);

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float p = t / duration;
            if (body != null)
            {
                body.transform.localPosition = Vector3.Lerp(startPos, endPos, p);
                body.color = new Color(1, 1, 1, 1 - p);
            }
            yield return null;
        }
    }

    public IEnumerator PlayEvolve(MonsterDataI from, MonsterDataI to, float duration)
    {
        evolving = true;
        float swapRate = 0.1f;
        float timer = 0f;
        bool showNew = false;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            if (timer % swapRate < Time.deltaTime)
            {
                showNew = !showNew;
                if (body != null)
                {
                    Sprite[] frames = showNew ? to.Frames : from.Frames;
                    if (frames != null && frames.Length > 0)
                    {
                        body.sprite = frames[0];
                    }
                    body.color = Color.white;
                }
            }
            yield return null;
        }

        if (body != null && to.Frames != null && to.Frames.Length > 0)
        {
            body.sprite = to.Frames[0];
            body.color = Color.white;
        }
        evolving = false;
    }

    void Update()
    {
        if (shown == null) return;

        // Idle animation
        if (!hurting && !evolving && body != null && displayData.Frames != null && displayData.Frames.Length >= 2)
        {
            int frame = Mathf.FloorToInt(Time.time / 0.35f) % 2;
            body.sprite = displayData.Frames[frame];
        }

        // HP lerp
        if (hpFill != null)
        {
            float k = 1f - Mathf.Pow(0.85f, Time.deltaTime * 60f);
            hpShow = Mathf.Lerp(hpShow, shown.CurrentHP, k);
            hpFill.fillAmount = shown.CurrentHP <= 0 ? 0f : hpShow / shown.MaxHP;
            hpFill.color = HpColors.For(hpFill.fillAmount);
        }

        if (hpText != null && !isEnemyPanel)
        {
            hpText.text = $"{Mathf.RoundToInt(hpShow)}/{shown.MaxHP}";
        }

        // Lunge animation
        if (body != null && Time.time - lungeStart < 0.26f)
        {
            float p = (Time.time - lungeStart) / 0.26f;
            float dx = p < 1f ? Mathf.Sin(p * Mathf.PI) * 0.875f * (isEnemyPanel ? -1 : 1) : 0f;
            body.transform.localPosition = home + new Vector3(dx, 0f, 0f);
        }
    }
}