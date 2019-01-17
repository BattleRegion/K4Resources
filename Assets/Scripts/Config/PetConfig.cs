using UnityEngine;
using System.Collections;
using SimpleJson;
using System;

public class PetConfig : GameConfig {
    public PetConfig()
    {
        this.ConfigName = "Pet";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            PetData m = new PetData(data);
            Configs.Add(m);
            if(!PlayerPrefs.HasKey(AppMember.MemberId.ToString() + m.Id))
            {
                PlayerPrefs.SetInt(AppMember.MemberId.ToString() + m.Id, 0);
            }
        }
    }

    /// <summary>
    /// 判断是否为新获得
    /// </summary>
    public bool IsNew(string Id)
    {
        PetData pd = GetPetById(Id);
        if (pd == null) return false;
        else
        {
            switch (PlayerPrefs.GetInt(AppMember.MemberId.ToString() + pd.Id))
            {
                case 0: return true;
                case 1: return false;
                default: return false;
            }
        }
    }

    /// <summary>
    /// 标记已经获得过
    /// </summary>
    public bool SetNotNew(string Id)
    {
        PetData pd = GetPetById(Id);
        if (pd == null) return false;
        else
        {
            PlayerPrefs.SetInt(AppMember.MemberId.ToString() + pd.Id, 1);
            return true;
        }
    }

    /// <summary>
    /// 清除所有标记
    /// </summary>
    public void ClearAllKeys()
    {
        foreach (PetData p in Configs)
        {
            if (PlayerPrefs.HasKey(AppMember.MemberId.ToString() + p.Id))
            {
                PlayerPrefs.DeleteKey(AppMember.MemberId.ToString() + p.Id);
            }
        }
    }

    public PetData GetPetById(string Id)
    {
        foreach (PetData m in Configs)
        {
            if (m.Id == Id)
            {
                return m;
            }
        }
        return null;
    }
}

public class PetData : ConfigData
{
    public string SkinId;

    public string Id;

    public float Attack;

    public float Hp;

    public DungeonEnum.ElementAttributes PetPro;

    public DungeonEnum.ElementAttributes TempPetPro;

    public SkillData PetSkillData;

    public SkillData PetSkillData2;

    public int PCost;

    public string Name;

    public int Rank;

    public int MaxRank;

    public int Price;

    public int Exp;

    public int HpUp;

    public int AtkUp;

    public int MaxLevel;

    public int Warefare;

    public string Evo;

    public int EvoCoin;

    public string EvoN1;

    public string EvoN2;

    public string EvoN3;

    public string EvoN4;

    public string EvoN5;

    public PetData(JsonObject data)
    {
        try
        {
            Id = data["PetId"].ToString();
            Name = data["Name"].ToString();
            Evo = data["Evo"].ToString();
            EvoCoin = data["EvoCoin"].ToString() == "" ? 0 : int.Parse(data["EvoCoin"].ToString());
            EvoN1 = data["EvoN1"].ToString();
            EvoN2 = data["EvoN2"].ToString();
            EvoN3 = data["EvoN3"].ToString();
            EvoN4 = data["EvoN4"].ToString();
            EvoN5 = data["EvoN5"].ToString();
            Exp = int.Parse(data["EXP"].ToString());
            MaxLevel = int.Parse(data["LvMax"].ToString());
            Price = int.Parse(data["Price"].ToString());
            Rank = int.Parse(data["Rank"].ToString());
            MaxRank = int.Parse(data["RankMax"].ToString());
            HpUp = int.Parse(data["HpUp"].ToString());
            AtkUp = int.Parse(data["AttackUp"].ToString());
            SkinId = data["SkinId"].ToString();
            PCost = int.Parse(data["PCost"].ToString());
            Hp = float.Parse(data["Hp"].ToString());
            Attack = float.Parse(data["Attack"].ToString());
            PetPro = (DungeonEnum.ElementAttributes)int.Parse(data["Element"].ToString());
            Warefare = int.Parse(data["Warefare"].ToString());
            string skillId = data["SkillId1"].ToString();
            string skillId2 = data["SkillId2"].ToString();
         
            PetSkillData = ConfigManager.SkillConfig.GetSkillById(skillId);
            PetSkillData2 = ConfigManager.SkillConfig.GetSkillById(skillId2);
			
            TempPetPro = PetPro;
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    } 
}
