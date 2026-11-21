using UnityEngine;

public static class SaveMapper
{
    public static SaveData Capture(GameServices s, Vector2 playerPos)
    {
        var data = new SaveData
        {
            playerPosition = playerPos,
            potions = s.Party.Potions,
            flags = s.Flags.GetAll(),
        };

        if (s.Party.Partner != null)
        {
            var m = s.Party.Partner;
            data.partnerId = m.Data.Id;
            data.partnerLevel = m.Level;
            data.partnerExp = m.Exp;
            data.partnerHp = m.CurrentHP;
        }

        return data;
    }

    public static void Apply(SaveData data, GameServices s)
    {
        if (data == null)
        {
            Debug.LogError("[SaveMapper] Apply dipanggil dengan data null!");
            return;
        }

        s.Flags.Load(data.flags);
        s.Party.Potions = data.potions;

        if (!string.IsNullOrEmpty(data.partnerId))
        {
            var monsterData = s.Database.GetById(data.partnerId);
            if (monsterData != null)
            {
                var partner = new MonsterInstance(
                    monsterData,
                    data.partnerLevel,
                    data.partnerExp,
                    data.partnerHp
                );
                s.Party.SetPartner(partner);
                Debug.Log($"[SaveMapper] Partner dimuat: {partner.Data.DisplayName} Lv{partner.Level}");
            }
            else
            {
                Debug.LogError($"[SaveMapper] Monster ID '{data.partnerId}' tidak ditemukan di Database!");
            }
        }
        else
        {
            Debug.LogWarning("[SaveMapper] Tidak ada partner di save file.");
        }
    }
}