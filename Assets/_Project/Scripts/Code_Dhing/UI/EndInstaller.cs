using UnityEngine;
using UnityEngine.UI;

public class EndInstaller : SceneInstaller
{
    [SerializeField] private Text partnerText;
    [SerializeField] private Text timeText;
    [SerializeField] private Image partnerImage;
    [SerializeField] private Button clickCatcher;
    
    private GameServices services;
    private MonsterInstance partner;
    private float ready;
    private bool leaving;

    public override void Install(GameServices s)
    {
        services = s;
        partner = s.Party.Partner;
        
        int total = Mathf.FloorToInt(s.Party.Playtime);
        partnerText.text = partner != null ? $"{partner.Data.DisplayName} · Lv{partner.Level}" : "";
        timeText.text = $"Waktu bermain {total / 60} menit {total % 60} detik";
        
        clickCatcher.onClick.AddListener(Leave);
        ready = Time.unscaledTime + 0.5f;
    }

    void Update()
    {
        if (partner != null && partnerImage != null)
        {
            int frame = Mathf.FloorToInt(Time.unscaledTime / 0.35f) % 2;
            if (partner.Data.Frames.Length > frame)
                partnerImage.sprite = partner.Data.Frames[frame];
        }
        
        if (MenuInput.Confirm) Leave();
    }

    private void Leave()
    {
        if (leaving || Time.unscaledTime < ready) return;
        leaving = true;
        services.Scenes.GoTo("Scn_Title");
    }
}