using UnityEngine;

public class MenuTest : MonoBehaviour
{
    [SerializeField] private TextInput menu;

    void Start()
    {
        menu.Picked += OnPicked;
        menu.Open(new string[] { "Serang", "Jurus", "Ramuan", "Kabur" });
    }

    void OnPicked(int index)
    {
        string[] labels = { "Serang", "Jurus", "Ramuan", "Kabur" };
        Debug.Log($"Menu dipilih: {labels[index]}");
        
        // Buka lagi untuk test
        menu.Open(new string[] { "Serang", "Jurus", "Ramuan", "Kabur" });
    }
}