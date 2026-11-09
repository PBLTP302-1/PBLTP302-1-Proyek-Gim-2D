using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartnerHud : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI partnerNameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private RectTransform hpBarFill;
    [SerializeField] private RectTransform expBarFill;
    [SerializeField] private TextMeshProUGUI potionsText;
    [SerializeField] private GameObject panel;

    private PlayerPartyI party;
    private BattleSessionI battle;
    private MonsterInstanceI partner;
    private bool wasPartnerNull = true;
    private bool wasBattleActive = false;
    
    private Image hpBarImage;
    private Image expBarImage;

    void Start()
    {
        var services = GameBootstrapI.CurrentServices;
        party = services?.Party;
        battle = services?.Battle;
        
        // Konfigurasi HP Bar
        if (hpBarFill != null)
        {
            hpBarImage = hpBarFill.GetComponent<Image>();
            if (hpBarImage != null)
            {
                hpBarImage.type = Image.Type.Filled;
                hpBarImage.fillMethod = Image.FillMethod.Horizontal;
                hpBarImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            }
        }

        // Konfigurasi EXP Bar (Paksa Biru)
        if (expBarFill != null)
        {
            expBarImage = expBarFill.GetComponent<Image>();
            if (expBarImage != null)
            {
                expBarImage.type = Image.Type.Filled;
                expBarImage.fillMethod = Image.FillMethod.Horizontal;
                expBarImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                expBarImage.color = new Color(0.435f, 0.607f, 1f, 1f); // Biru #6F9BFF
            }
        }
        
        if (party != null)
        {
            party.PotionsChanged += OnPotionsChanged;
            UpdateHud();
        }
    }

    void Update()
    {
        if (party == null) return;

        // Deteksi perubahan status battle
        bool isBattleActiveNow = battle != null && battle.IsBattleActiveOrLoading;

        if (isBattleActiveNow != wasBattleActive)
        {
            wasBattleActive = isBattleActiveNow;

            if (isBattleActiveNow)
            {
                if (panel != null) panel.SetActive(false);
            }
            else
            {
                UpdateHud();
            }
        }

        // Deteksi partner baru didapat
        bool partnerJustObtained = wasPartnerNull && party.Partner != null && !isBattleActiveNow;
        
        if (partnerJustObtained)
        {
            wasPartnerNull = false;
            UpdateHud();
        }

        // Update bar setiap frame jika partner ada dan tidak dalam battle
        if (party.Partner != null && !isBattleActiveNow)
        {
            UpdateBars();
        }
    }

    private void UpdateHud()
    {
        bool isBattleActiveNow = battle != null && battle.IsBattleActiveOrLoading;
        
        if (party == null || party.Partner == null || isBattleActiveNow)
        {
            if (panel != null) panel.SetActive(false);
            if (party != null && party.Partner == null) wasPartnerNull = true;
            return;
        }

        if (panel != null) panel.SetActive(true);
        
        partner = party.Partner;

        if (partnerNameText != null) partnerNameText.text = partner.Data.DisplayName;
        
        // 🟢 PERUBAHAN: Format "Ramuan(N)" bukan "×N"
        if (potionsText != null)
        {
            potionsText.text = $"Ramuan({party.Potions})";
        }

        UpdateBars();
    }

    private void UpdateBars()
    {
        if (partner == null) return;

        // HP Bar dengan Warna Dinamis
        float hpPercent = (float)partner.CurrentHP / partner.MaxHP;
        
        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = hpPercent;
            hpBarImage.color = HpColors.For(hpPercent);
        }

        if (hpText != null)
        {
            hpText.text = $"{partner.CurrentHP}/{partner.MaxHP}";
        }

        // EXP Bar (Biru Tetap)
        float expPercent = Mathf.Clamp01((float)partner.CurrentEXP / partner.ExpToNext);
        
        if (expBarImage != null)
        {
            expBarImage.fillAmount = expPercent;
        }
    }

    // 🟢 PERUBAHAN: Format "Ramuan(N)" bukan "×N"
    private void OnPotionsChanged(int newCount)
    {
        if (potionsText != null)
        {
            potionsText.text = $"Ramuan({newCount})";
        }
    }

    void OnDestroy()
    {
        if (party != null) party.PotionsChanged -= OnPotionsChanged;
    }
}       