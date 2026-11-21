using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterPanel : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text elementText;
    [SerializeField] private Image hpFill;
    [SerializeField] private Text hpText;
    [SerializeField] private Image expFill;
    [SerializeField] private SpriteRenderer body;

    [Header("Idle")]
    [SerializeField] private float idleSpeed = 0.35f;

    public MonsterInstance Shown { get; private set; }
    private MonsterData shownData;
    private float hpShow;
    private bool hurting, evolving;
    private Vector3 home;
    private float lungeStart = -1f;
    private bool faceRight = true;

    public void Construct(bool faceRight)
    {
        this.faceRight = faceRight;
    }

    void Awake()
    {
        home = body != null ? body.transform.localPosition : Vector3.zero;
    }

    public void Show(MonsterInstance m, bool withExp = true, MonsterData displayAs = null)
    {
        Shown = m;
        shownData = displayAs ?? m.Data;
        hpShow = m.CurrentHP;

        if (nameText) nameText.text = shownData.DisplayName;
        if (levelText) levelText.text = "Lv" + m.Level;
        if (elementText) elementText.text = m.Data.Element.ToString();
        if (body && shownData.Frames.Length > 0) body.sprite = shownData.Frames[0];
        if (expFill) expFill.gameObject.SetActive(withExp);

        if (withExp && expFill != null && m.ExpToNext > 0)
        expFill.fillAmount = (float)m.Exp / m.ExpToNext;

        UpdateBars();
        if (withExp && expFill != null && m.ExpToNext > 0)
        {
            expFill.fillAmount = (float)m.Exp / m.ExpToNext;
        }
    }

    public void UpdateBars()
    {
        if (Shown == null) return;

        // HP Bar
        if (hpFill != null)
        {
            float ratio = Shown.MaxHP > 0 ? (float)Shown.CurrentHP / Shown.MaxHP : 0f;
            hpFill.fillAmount = ratio;
            hpFill.color = HpColors.For(ratio);
        }
        if (hpText != null) hpText.text = $"{Shown.CurrentHP}/{Shown.MaxHP}";

        if (expFill != null)
        {
            if (Shown.ExpToNext > 0)
            {
                expFill.fillAmount = (float)Shown.Exp / Shown.ExpToNext;
            }
            else
            {
                expFill.fillAmount = 0;
            }
        }
    }

    public void PlayExpBar(float ratio)
    {
        if (expFill) expFill.fillAmount = ratio;
    }

    void Update()
    {
        if (Shown == null) return;

        if (!hurting && !evolving && body != null && shownData != null && shownData.Frames.Length >= 2)
        {
            int frame = Mathf.FloorToInt(Time.time / idleSpeed) % 2;
            body.sprite = shownData.Frames[frame];
        }

        float k = 1f - Mathf.Pow(0.85f, Time.deltaTime * 60f);
        hpShow = Mathf.Lerp(hpShow, Shown.CurrentHP, k);
        if (hpFill)
        {
            hpFill.fillAmount = Shown.CurrentHP <= 0 ? 0f : hpShow / Shown.MaxHP;
            hpFill.color = HpColors.For(hpFill.fillAmount);
        }
        if (hpText) hpText.text = $"{Mathf.RoundToInt(hpShow)}/{Shown.MaxHP}";

        if (expFill != null && Shown.ExpToNext > 0)
        {
            expFill.fillAmount = (float)Shown.Exp / Shown.ExpToNext;
        }


        if (lungeStart >= 0f)
        {
            float p = (Time.time - lungeStart) / 0.26f;
            if (p < 1f)
            {
                float dx = Mathf.Sin(p * Mathf.PI) * 0.875f * (faceRight ? 1 : -1);
                if (body != null) body.transform.localPosition = home + new Vector3(dx, 0f, 0f);
            }
            else
            {
                if (body != null) body.transform.localPosition = home;
                lungeStart = -1f;
            }
        }
    }

    public void PlayLunge() => lungeStart = Time.time;

    public void PlayHurt() => StartCoroutine(HurtRoutine());

    IEnumerator HurtRoutine()
    {
        hurting = true;
        float start = Time.time;
        Color original = body != null ? body.color : Color.white;
        while (Time.time - start < 0.4f)
        {
            float t = (Time.time - start) / 0.08f;
            if (body != null) body.color = Mathf.FloorToInt(t) % 2 == 0 ? Color.white : Color.red;
            yield return null;
        }
        if (body != null) body.color = original;
        hurting = false;
    }

    public void PlayFaint() => StartCoroutine(FaintRoutine());

    IEnumerator FaintRoutine()
    {
        float start = Time.time;
        Vector3 startPos = body != null ? body.transform.localPosition : Vector3.zero;
        while (Time.time - start < 0.5f)
        {
            float p = (Time.time - start) / 0.5f;
            if (body != null)
            {
                body.transform.localPosition = startPos + new Vector3(0, -1.25f * p, 0);
                body.color = new Color(1, 1, 1, 1 - p);
            }
            yield return null;
        }
    }

    public void PlayEvolve(MonsterData from, MonsterData to, float duration)
    {
        StartCoroutine(EvolveRoutine(from, to, duration));
    }

    IEnumerator EvolveRoutine(MonsterData from, MonsterData to, float duration)
    {
        evolving = true;
        float start = Time.time;
        float interval = 0.25f;
        bool alt = false;
        while (Time.time - start < duration)
        {
            if (body != null)
            {
                var frames = (alt ? to : from).Frames;
                if (frames.Length > 0) body.sprite = frames[0];
                body.color = Mathf.FloorToInt((Time.time - start) / 0.15f) % 3 == 0 ? Color.white : Color.yellow;
            }
            alt = !alt;
            yield return new WaitForSeconds(interval);
            interval = Mathf.Max(0.05f, interval * 0.85f);
        }
        shownData = to;
        if (body != null) body.color = Color.white;
        evolving = false;
    }
}