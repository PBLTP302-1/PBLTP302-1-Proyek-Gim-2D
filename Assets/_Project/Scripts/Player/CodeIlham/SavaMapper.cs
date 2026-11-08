using UnityEngine;

public static class SaveMapper
{
    // 🟢 MENGAMBIL DATA DARI GAME UNTUK DISIMPAN
    public static SaveData Capture(GameServicesI services, Vector2 playerPosition)
    {
        SaveData data = new SaveData();
        
        data.playerPosition = playerPosition;
        data.potions = services.Party.Potions;
        data.flags = services.Flags.GetAll(); // Menggunakan method GetAll() dari StoryFlags Anda

        if (services.Party.Partner != null)
        {
            data.partnerId = services.Party.Partner.Data.Id;
            data.partnerLevel = services.Party.Partner.Level;
            data.partnerHp = services.Party.Partner.CurrentHP;
            // Catatan: Jika MonsterInstance Anda memiliki property Exp, tambahkan: data.partnerExp = services.Party.Partner.Exp;
        }

        return data;
    }

    // 🟢 MENERAPKAN DATA DARI FILE SAVE KE GAME
    public static void Apply(SaveData data, GameServicesI services)
    {
        // 1. Restore Flags
        services.Flags.Load(data.flags);

        // 2. Restore Party
        services.Party.Reset(); // Reset ke kondisi awal (tanpa partner, ramuan default)
        
        // Kembalikan jumlah ramuan sesuai save
        // (Asumsi: PlayerPartyI memiliki cara untuk mengatur ini. Jika tidak, Anda bisa menambah method SetPotions di PlayerPartyI)
        // services.Party.SetPotions(data.potions); 
        
        // 3. Restore Partner
        if (!string.IsNullOrEmpty(data.partnerId))
        {
            // Cari data monster berdasarkan ID yang tersimpan
            MonsterDataI partnerData = services.Database.GetById(data.partnerId); 
            
            if (partnerData != null)
            {
                MonsterInstanceI partner = new MonsterInstanceI(partnerData, data.partnerLevel);
                services.Party.SetPartner(partner);
                
                // Opsional: Kembalikan HP sesuai save (jika method Heal/TakeDamage tersedia)
                // int hpDiff = data.partnerHp - partner.CurrentHP;
                // if (hpDiff > 0) partner.Heal(hpDiff);
            }
        }
    }
}