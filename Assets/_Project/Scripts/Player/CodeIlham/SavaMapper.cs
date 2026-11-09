using UnityEngine;
using System.Linq;

public static class SaveMapper
{
    // 1. Ambil data dari Game untuk disimpan
    public static SaveData Capture(GameServicesI services, Vector2 playerPosition)
    {
        SaveData data = new SaveData();
        data.playerPosition = playerPosition;
        data.flags = services.Flags.GetAll();
        
        if (services.Party != null)
        {
            data.potions = services.Party.Potions;
            
            if (services.Party.Partner != null)
            {
                data.partnerId = services.Party.Partner.Data.Id;
                data.partnerLevel = services.Party.Partner.Level;
                data.partnerExp = services.Party.Partner.CurrentEXP; // Sesuaikan dengan property EXP kamu
                data.partnerHp = services.Party.Partner.CurrentHP;
            }
        }
        return data;
    }

    // 2. Terapkan data dari Save ke Game
    public static void Apply(SaveData data, GameServicesI services)
    {
        if (data == null) return;

        // Restore Flags
        services.Flags.Load(data.flags);

        // Restore Party
        if (services.Party != null)
        {
            services.Party.Reset(); // Reset ke default dulu
            services.Party.SetPotions(data.potions); // Pastikan method ini ada di PlayerPartyI

            if (!string.IsNullOrEmpty(data.partnerId))
            {
                // Cari monster berdasarkan ID
                MonsterDataI partnerData = services.Database.GetById(data.partnerId);
                
                if (partnerData != null)
                {
                    MonsterInstanceI partner = new MonsterInstanceI(partnerData, data.partnerLevel);
                    services.Party.SetPartner(partner);
                    
                    // Restore HP (Opsional, karena SaveStone biasanya HealFull)
                    // partner.Heal(data.partnerHp); 
                }
            }
        }
    }
}