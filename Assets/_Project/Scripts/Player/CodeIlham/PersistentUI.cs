using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log($"[PersistentUI] {gameObject.name} akan persisten antar scene");
    }
}