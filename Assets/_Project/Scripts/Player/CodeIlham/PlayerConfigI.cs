using UnityEngine;
[CreateAssetMenu(fileName = "Cfg_PlayerI",
        menuName = "Pulau Monster/Player Config")]
public class PlayerConfigI : ScriptableObject
{
    [SerializeField] private float walkSpeed = 3.75f;
    [SerializeField] private float runSpeed = 6f;
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
}   