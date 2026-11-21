using UnityEngine;
using UnityEngine.UI;

public class PartnerNameUpdater : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;

    private PlayerParty party;

    public void Construct(PlayerParty party)
    {
        this.party = party;
    }

    void LateUpdate()
    {
        if (party == null || party.Partner == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        var m = party.Partner;
        if (nameText != null)
            nameText.text = m.Data.DisplayName;
        if (levelText != null)
            levelText.text = $"Lv{m.Level}";
    }
}