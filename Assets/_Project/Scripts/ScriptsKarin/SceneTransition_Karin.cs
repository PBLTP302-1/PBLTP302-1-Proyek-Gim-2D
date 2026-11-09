using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition_Karin : MonoBehaviour
{
    [SerializeField] private string targetBattleScene = "Scn_Battle";

    // Fungsi ini dipanggil saat monster menyentuh pemain
    public void TriggerBattle()
    {
        Debug.Log("Mempersiapkan perpindahan ke " + targetBattleScene);
        
        // Memuat scene battle secara additive (menumpuk di atas scene dunia)
        SceneManager.LoadScene(targetBattleScene, LoadSceneMode.Additive);
    }
}