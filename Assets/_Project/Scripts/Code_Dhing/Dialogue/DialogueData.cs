using UnityEngine;

[CreateAssetMenu(fileName = "Dlg_", menuName = "Pulau Monster/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private string speaker;
    [SerializeField] private string[] lines;
    [SerializeField] private bool isNarration;

    public string Speaker => speaker;
    public string[] Lines => lines;
    public bool IsNarration => isNarration;
}