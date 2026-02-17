using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AllCharacterData
{
    public List<CharacterData> lockedCharacters = new List<CharacterData>();
    public List<CharacterData> unlockedCharacters = new List<CharacterData>();

    /// <summary> Master分のCharacterDataを作成し全てlockedCharactersに格納 </summary>
    public void Init(CharacterTable table)
    {
        lockedCharacters.Clear();
        unlockedCharacters.Clear();

        for (int i = 0; i < table.Count; i++)
        {
            lockedCharacters.Add(new CharacterData(table[i].index));
        }
    }

    /// <summary> locked→unlockedへ移動 </summary>
    public void Unlock(int masterId)
    {
        if (unlockedCharacters.Exists(c => c.masterId == masterId))
        {
            Debug.LogWarning($"[AllCharacterData] masterId={masterId} は既にアンロック済みです");
            return;
        }

        var data = lockedCharacters.Find(c => c.masterId == masterId);
        if (data == null)
        {
            Debug.LogWarning($"[AllCharacterData] masterId={masterId} がlockedCharactersに見つかりません");
            return;
        }

        lockedCharacters.Remove(data);
        data.Unlock();
        unlockedCharacters.Add(data);
    }

    /// <summary> unlocked→lockedへ戻す </summary>
    public void Lock(int masterId)
    {
        var data = unlockedCharacters.Find(c => c.masterId == masterId);
        if (data == null)
        {
            Debug.LogWarning($"[AllCharacterData] masterId={masterId} がunlockedCharactersに見つかりません");
            return;
        }

        unlockedCharacters.Remove(data);
        data.unlockedAt = null;
        lockedCharacters.Add(data);
    }

    /// <summary> locked/unlocked問わずmasterIdで検索 </summary>
    public CharacterData Get(int masterId)
    {
        return unlockedCharacters.Find(c => c.masterId == masterId)
            ?? lockedCharacters.Find(c => c.masterId == masterId);
    }

    /// <summary> アンロック済みか判定 </summary>
    public bool IsUnlocked(int masterId)
    {
        return unlockedCharacters.Exists(c => c.masterId == masterId);
    }
}
