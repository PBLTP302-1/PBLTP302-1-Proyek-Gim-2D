using UnityEngine;

public class PersistentDialogue : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}