using UnityEngine;
using UnityEngine.UI;

public class PartnerHud : MonoBehaviour
{
    [Header("Refs - Potret & Bar")]
    [SerializeField] private Image portrait;
    [SerializeField] private Image hpFill;
    [SerializeField] private Text hpText;
    [SerializeField] private Image expFill;

    [Header("Refs - Nama & Level")]
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;

    [Header("Refs - Ramuan")]
    [SerializeField] private Text potionText;

    private PlayerParty party;
    private string lastPartnerId;

    public void Construct(PlayerParty party)
    {
        this.party = party;
        if (party != null)
        {
            party.PotionsChanged -= Refresh;
            party.PotionsChanged += Refresh;
            
            party.PartnerChanged -= OnPartnerChanged;
            party.PartnerChanged += OnPartnerChanged;
            
            Refresh();
            OnPartnerChanged(); 
        }
    }

    void OnDestroy()
    {
        if (party != null)
        {
            party.PotionsChanged -= Refresh;
            party.PartnerChanged -= OnPartnerChanged;
        }
    }

    private void OnPartnerChanged()
    {
        if (party != null && party.Partner != null)
        {
            gameObject.SetActive(true);
            lastPartnerId = "";
        }
    }

    void LateUpdate()
    {
        if (party == null) return;

        bool hasPartner = party.Partner != null;
        gameObject.SetActive(hasPartner);
        
        if (!hasPartner) return;

        var m = party.Partner;

        if (m.Data.Id != lastPartnerId)
        {
            lastPartnerId = m.Data.Id;
            if (nameText != null) nameText.text = m.Data.DisplayName;
        }
        if (levelText != null) levelText.text = $"Lv{m.Level}";

        if (portrait != null && m.Data.Frames.Length > 0)
            portrait.sprite = m.Data.Frames[0];

        if (hpFill != null)
        {
            float ratio = m.MaxHP > 0 ? (float)m.CurrentHP / m.MaxHP : 0f;
            hpFill.fillAmount = ratio;
            hpFill.color = HpColors.For(ratio);
        }
        if (hpText != null)
            hpText.text = $"{m.CurrentHP}/{m.MaxHP}";

        if (expFill != null && m.ExpToNext > 0)
        {
            expFill.fillAmount = (float)m.Exp / m.ExpToNext;
        }
    }

    void Refresh()
    {
        if (potionText != null && party != null)
            potionText.text = $"Ramuan \u00D7{party.Potions}";
    }
}