using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MonsterDatabase", menuName = "Pulau Monster/Monster Database")]
public class MonsterDatabaseI : ScriptableObject
{
    [SerializeField] private List<MonsterDataI> allMonsters = new List<MonsterDataI>();
    
    public List<MonsterDataI> AllMonsters => allMonsters;

    public MonsterDataI GetById(string id)
    {
        return allMonsters.Find(m => m.Id == id);
    }
}
