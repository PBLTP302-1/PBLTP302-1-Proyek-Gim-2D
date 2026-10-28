using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MonsterDatabase", menuName = "Pulau Monster/Monster Database")]
public class MonsterDatabase : ScriptableObject
{
    [SerializeField] private List<MonsterData> allMonsters = new List<MonsterData>();
    
    public List<MonsterData> AllMonsters => allMonsters;

    public MonsterData GetById(string id)
    {
        return allMonsters.Find(m => m.Id == id);
    }
}