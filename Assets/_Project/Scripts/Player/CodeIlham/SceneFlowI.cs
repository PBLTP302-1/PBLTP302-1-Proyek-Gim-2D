using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowI : MonoBehaviour
{
    private GameBootstrapI bootstrap;
    private ScreenFader fader;
    private GameServicesI services;
    private bool holdFade = false;

    public bool HoldFade
    {
        get => holdFade;
        set => holdFade = value;
    }

    public const string WorldScene = "Scn_WorldI";
    public const string BattleScene = "Scn_BattleI";

    // 🟢 TAMBAHKAN METHOD INI
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log($"[SceneFlowI] Awake: {gameObject.name} akan persisten antar scene");
    }

    public void Construct(GameBootstrapI bootstrap, ScreenFader fader, GameServicesI services)
    {
        this.bootstrap = bootstrap;
        this.fader = fader;
        this.services = services;
        Debug.Log($"[SceneFlowI] Construct: bootstrap={bootstrap != null}, fader={fader != null}, services={services != null}");
    }

    public void GoTo(string sceneName)
    {
        bootstrap.StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (fader != null && !holdFade) yield return fader.FadeOut();

        Debug.Log($"[SceneFlow] Memuat scene: {sceneName}");

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        
        if (loadOp == null)
        {
            Debug.LogError($"[SceneFlow] GAGAL memuat scene '{sceneName}'! Cek Build Settings.");
            yield break;
        }

        while (!loadOp.isDone) yield return null;

        Debug.Log($"[SceneFlow] Berhasil load scene: {sceneName}");

        if (sceneName == WorldScene)
        {
            WorldInstallerI installer = FindAnyObjectByType<WorldInstallerI>();
            if (installer != null)
            {
                Debug.Log("[SceneFlow] WorldInstallerI ditemukan, memanggil Install()");
                installer.Install(services);
            }
        }

        if (sceneName == BattleScene)
        {
            BattleInstaller installer = FindAnyObjectByType<BattleInstaller>();
            if (installer != null)
            {
                Debug.Log("[SceneFlow] BattleInstaller ditemukan, memanggil Install()");
                installer.Install(services);
            }
        }

        if (fader != null && !holdFade) yield return fader.FadeIn();
    }

    public void LoadBattle()
    {
        Debug.Log("[SceneFlow] LoadBattle - Locking input");
        
        if (services != null && services.Input != null)
        {
            services.Input.Lock();
        }

        if (fader != null)
        {
            bootstrap.StartCoroutine(FadeOutThenLoadBattle());
        }
        else
        {
            bootstrap.StartCoroutine(LoadBattleAdditive());
        }
    }

    private IEnumerator FadeOutThenLoadBattle()
    {
        yield return fader.FadeOut();
        yield return LoadBattleAdditive();
    }

    private IEnumerator LoadBattleAdditive()
    {
        Debug.Log($"[SceneFlow] Loading battle scene additive: {BattleScene}");

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(BattleScene, LoadSceneMode.Additive);
        
        if (loadOp == null)
        {
            Debug.LogError($"[SceneFlow] GAGAL memuat battle scene!");
            yield break;
        }

        while (!loadOp.isDone) yield return null;

        Scene battleScene = SceneManager.GetSceneByName(BattleScene);
        if (battleScene.IsValid())
        {
            SceneManager.SetActiveScene(battleScene);
        }

        Debug.Log("[SceneFlow] Battle scene loaded additive");

        BattleInstaller installer = FindAnyObjectByType<BattleInstaller>();
        if (installer != null)
        {
            Debug.Log("[SceneFlow] BattleInstaller ditemukan, memanggil Install()");
            installer.Install(services);
        }

        if (services != null && services.Battle != null)
        {
            services.Battle.SetBattleLoading(false);
        }

        if (fader != null) yield return fader.FadeIn();
    }

    public void EndBattle(BattleResultI result)
    {
        Debug.Log($"[SceneFlow] EndBattle({result})");
        bootstrap.StartCoroutine(UnloadBattleAsync(result));
    }

    private IEnumerator UnloadBattleAsync(BattleResultI result)
    {
        BattleSystem battle = FindAnyObjectByType<BattleSystem>();
        if (battle != null)
        {
            battle.EndBattle();
            Debug.Log("[SceneFlow] BattleSystem dihentikan");
        }

        if (fader != null) yield return fader.FadeOut();

        if (services != null && services.Input != null)
        {
            services.Input.Unlock();
            Debug.Log("[SceneFlow] Input unlocked");
        }

        Scene battleScene = SceneManager.GetSceneByName(BattleScene);
        
        if (!battleScene.isLoaded)
        {
            Debug.LogWarning($"[SceneFlow] Battle scene sudah tidak aktif");
        }
        else
        {
            Debug.Log($"[SceneFlow] Unloading battle scene");
            
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(BattleScene);
            
            if (unloadOp == null)
            {
                Debug.LogError($"[SceneFlow] GAGAL unload battle scene!");
            }
            else
            {
                while (!unloadOp.isDone) yield return null;
                Debug.Log("[SceneFlow] Battle scene unloaded");
            }
        }

        Scene worldScene = SceneManager.GetSceneByName(WorldScene);
        if (worldScene.IsValid() && worldScene.isLoaded)
        {
            SceneManager.SetActiveScene(worldScene);
        }

        if (fader != null) yield return fader.FadeIn();
    }

    public void TeleportToSaveStone()
    {
        Debug.Log("[SceneFlow] TeleportToSaveStone triggered!");
        
        GameObject saveStoneObj = GameObject.FindWithTag("SaveStone");
        
        if (services != null && services.Party != null && services.Party.PlayerTransform != null)
        {
            if (saveStoneObj != null)
            {
                services.Party.PlayerTransform.position = saveStoneObj.transform.position;
                Debug.Log($"[SceneFlow] Player teleported to SaveStone at {saveStoneObj.transform.position}");
            }
            else
            {
                services.Party.PlayerTransform.position = new Vector3(24f, 7f, 0f);
                Debug.LogWarning("[SceneFlow] Tag 'SaveStone' tidak ditemukan! Menggunakan posisi fallback (24, 7).");
            }
        }

        WildMonsterI[] allMonsters = FindObjectsByType<WildMonsterI>();
        int resetCount = 0;
        
        foreach (WildMonsterI monster in allMonsters)
        {
            if (monster != null)
            {
                monster.ForceResetToHome();
                resetCount++;
            }
        }
        
        Debug.Log($"[SceneFlow] Successfully reset {resetCount} monsters to their original positions.");
    }

    public IEnumerator ReleaseFade()
    {
        Debug.Log("[SceneFlow] ReleaseFade() dipanggil");
        holdFade = false;
        if (fader != null) yield return fader.FadeIn();
    }
}