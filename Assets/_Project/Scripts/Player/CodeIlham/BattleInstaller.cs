using UnityEngine;
using UnityEngine.UI;

public class BattleInstaller : SceneInstaller
{
    [Header("Background")]
    [SerializeField] private Sprite forestBackground;
    [SerializeField] private Sprite caveBackground;
    
    // Bisa SpriteRenderer ATAU Image
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Image backgroundImage;

    [Header("Battle")]
    [SerializeField] private BattleSystem battle;
    [SerializeField] private BattleView view;

    // 🟢 BARU: Field khusus Audio (sesuai Modul Praktikum 7)
    [Header("Audio")]
    [SerializeField] private AudioClip battleMusic;
    [SerializeField] private AudioClip bossMusic;

    public override void Install(GameServicesI s)
    {
        Debug.Log("[BattleInstaller] Install() dipanggil!");

        // Set background - coba SpriteRenderer dulu
        if (backgroundRenderer != null)
        {
            backgroundRenderer.sprite = s.Battle.IsBoss && caveBackground != null
                ? caveBackground : forestBackground;
            Debug.Log($"[BattleInstaller] Background set via SpriteRenderer: {backgroundRenderer.sprite.name}");
        }
        // Fallback ke Image jika SpriteRenderer kosong
        else if (backgroundImage != null)
        {
            backgroundImage.sprite = s.Battle.IsBoss && caveBackground != null
                ? caveBackground : forestBackground;
            Debug.Log($"[BattleInstaller] Background set via Image: {backgroundImage.sprite.name}");
        }
        else
        {
            Debug.LogWarning("[BattleInstaller] Tidak ada background renderer! Battle akan tanpa background.");
        }

        // Construct BattleSystem
        if (battle != null && view != null)
        {
            battle.Construct(s.Party, s.Battle, s.Scenes);
            new BattlePresenter(battle, view);
            battle.Begin();
            Debug.Log("[BattleInstaller] Battle dimulai");
        }
        else
        {
            Debug.LogError($"[BattleInstaller] battle={battle != null}, view={view != null}");
        }

        // 🟢 BARU: Logika Putar Musik Battle (sesuai Modul Praktikum 7)
        if (s.Audio != null)
        {
            // Pilih bossMusic jika IsBoss true, jika tidak pilih battleMusic
            AudioClip musicToPlay = (s.Battle.IsBoss && bossMusic != null) ? bossMusic : battleMusic;
            
            if (musicToPlay != null)
            {
                s.Audio.PlayBattleMusic(musicToPlay);
                Debug.Log($"[BattleInstaller] 🎵 Memutar musik battle: {musicToPlay.name}");
            }
        }
    }
}