using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue_", menuName = "Pulau Monster/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Serializable]
    public struct Line
    {
        public string speaker;  // Kosong = narasi
        [TextArea(2, 4)]
        public string text;
    }

    public Line[] lines;
}