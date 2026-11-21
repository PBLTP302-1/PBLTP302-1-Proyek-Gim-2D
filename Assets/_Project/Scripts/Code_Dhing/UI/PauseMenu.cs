using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMenu menu;
    [SerializeField] private ToastView toast;

    private GameServices services;
    private string[] options = { "Lanjut", "Simpan", "Ke Judul" };
    private bool isOpen;

    public void Construct(GameServices services)
    {
        this.services = services;
        if (menu != null) menu.Picked += OnPicked;
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (MenuInput.Cancel)
        {
            if (isOpen) Close();
            else TryOpen();
        }
    }

    void TryOpen()
    {
        if (services == null) return;
        if (services.Scenes.CurrentScene != SceneFlow.WorldScene) return;
        if (services.Scenes.IsBusy) return;
        if (services.Scenes.InBattle) return;
        if (services.Input.IsLocked) return;
        Open();
    }

    void Open()
    {
        if (panel != null) panel.SetActive(true);
        isOpen = true;
        services.Input.Lock();
        if (menu != null) menu.Open(options);
    }

    void Close()
    {
        if (menu != null) menu.Close();
        services.Input.Unlock();
        if (panel != null) panel.SetActive(false);
        isOpen = false;
    }

    void OnPicked(int index)
    {
        if (index == 0) Close();
        else if (index == 1) SaveNow();
        else if (index == 2) GoToTitle();
    }

    void SaveNow()
    {
        var player = FindAnyObjectByType<PlayerMovement>();
        Vector2 pos = player != null ? (Vector2)player.transform.position : Vector2.zero;

        if (player == null)
        {
            Debug.LogWarning("[PauseMenu] Player tidak ditemukan! Save di posisi (0,0).");
        }

        if (services.Audio != null) services.Audio.PlaySave();
        bool saved = services.Save.Save(SaveMapper.Capture(services, pos));
        if (toast != null) toast.Show(saved ? "Tersimpan!" : "Gagal menyimpan");
        Close();
    }

    void GoToTitle()
    {
        Close();
        services.Scenes.GoTo("Scn_Title");
    }
}