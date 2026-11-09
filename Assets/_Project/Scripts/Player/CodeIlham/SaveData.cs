using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int version = SaveService.CurrentVersion;
    public string savedAt;
    public string scene = "Scn_WorldI";
    
    // Data Player
    public Vector2 playerPosition;
    
    // Data Partner
    public string partnerId;
    public int partnerLevel;
    public int partnerExp;
    public int partnerHp;
    
    // Data Inventory & Story
    public int potions;
    public float playtime;
    public List<string> flags = new List<string>();
}