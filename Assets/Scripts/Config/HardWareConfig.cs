using UnityEngine;
using System.Collections;
using SimpleJson;
using System;
public class HardWareConfig : GameConfig
{
    public HardWareConfig()
    {
        this.ConfigName = "Hardware";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            HardWareData cd = new HardWareData(data);
            Configs.Add(cd);
            if (!PlayerPrefs.HasKey(AppMember.MemberId.ToString() + cd.Id))
            {
                PlayerPrefs.SetInt(AppMember.MemberId.ToString() + cd.Id, 0);
            }
        }
    }

    /// <summary>
    /// 判断是否为新获得
    /// </summary>
    public bool IsNew(string Id)
    {
        HardWareData hd = GetHardWareById(Id);
        if (hd == null) return false;
        else
        {
            switch (PlayerPrefs.GetInt(AppMember.MemberId.ToString() + hd.Id))
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
        HardWareData hd = GetHardWareById(Id);
        if (hd == null) return false;
        else
        {
            PlayerPrefs.SetInt(AppMember.MemberId.ToString() + hd.Id, 1);
            return true;
        }
    }

    /// <summary>
    /// 清除所有标记
    /// </summary>
    public void ClearAllKeys()
    {
        foreach (HardWareData h in Configs)
        {
            if (PlayerPrefs.HasKey(AppMember.MemberId.ToString() + h.Id))
            {
                PlayerPrefs.DeleteKey(AppMember.MemberId.ToString() + h.Id);
            }
        }
    }

    public HardWareData GetHardWareById(string Id)
    {
        foreach (HardWareData h in Configs)
        {
            if (h.Id == Id)
            {
                return h;
            }
        }
        return null;
    }

    public HardWareData GetHardWareBySkin(string Id)
    {
        foreach (HardWareData h in Configs)
        {
            if (h.SkinId == Id)
            {
                return h;
            }
        }
        return null;
    }
}

/// <summary>
/// 单个角色数据结构
/// </summary>
public class HardWareData : ConfigData
{
    public enum HardWareType
    {
        none = 0,//任意类型，用于任务
        Light = 1,//轻武器
        Heavy = 2,//重武器
        Far1 = 3,//远程弓
        Far2 = 4,//远程杖
        Head = 10,//头部
        Cuirass = 20,//胸甲
        Acc = 30//饰品
    }

    public string Id;

    //public string Icon;

    public string Name;

    public int Rank;

    public int MaxRank;

    public int HeroLevelLimit;

    public int LvlMax;

    public HardWareType Style;

    public int Hp;

    public int HpUp;

    public int Attack;

    public int AttackUp;

    public int Defend;

    public int DefendUp;

    public int Exp;

    public int Price;

    public string SkinId;

    public int Warefare;

    public string Evo;

    public int EvoCoin;

    public string EvoN1;

    public string EvoN2;

    public string EvoN3;

    public string EvoN4;

    public string EvoN5;

    public string SkillAffix1;

    public int SkillAffix1Amount;

    public string SkillAffix2;

    public int SkillAffix2Amount;

    public string SkillAffix3;

    public int SkillAffix3Amount;



    public DungeonEnum.ElementAttributes Element;

    public HardWareData(JsonObject data)
    {
        try
        {
            Id = data["HardwareId"].ToString();
            Name = data["Name"].ToString();
            Style = (HardWareType)(int.Parse(data["Style"].ToString()));
           // Debug.Log("sst= " + int.Parse(data["Style"].ToString()));
            SkinId = data["SkinId"].ToString();
            Element = (DungeonEnum.ElementAttributes)(int.Parse(data["Element"].ToString()));
            Rank = int.Parse(data["Rank"].ToString());
            MaxRank = int.Parse(data["MaxRank"].ToString());
            LvlMax = int.Parse(data["LvMax"].ToString());
            SkillAffix1 = data["SkillAffix1"].ToString();
            if (Style == HardWareType.Light || Style == HardWareType.Heavy || Style == HardWareType.Far1 || Style == HardWareType.Far2)
            {
                Attack = int.Parse(data["Attack"].ToString());
                AttackUp = int.Parse(data["AttackUp"].ToString());
            }
            else if (Style == HardWareType.Acc || Style == HardWareType.Head || Style == HardWareType.Cuirass)
            {
                Defend = int.Parse(data["Def"].ToString());
                DefendUp = int.Parse(data["DefUp"].ToString());
            }
            Exp = int.Parse(data["EXP"].ToString());
            Price = int.Parse(data["Price"].ToString());
            HeroLevelLimit = int.Parse(data["HeroLvN"].ToString());
            Warefare = int.Parse(data["Warefare"].ToString());
            Evo = data["Evo"].ToString();
            EvoCoin = int.Parse(data["EvoCoin"].ToString());
            EvoN1 = data["EvoN1"].ToString();
            EvoN2 = data["EvoN2"].ToString();
            EvoN3 = data["EvoN3"].ToString();
            EvoN4 = data["EvoN4"].ToString();
            EvoN5 = data["EvoN5"].ToString();
            SkillAffix1Amount = int.Parse(data["SkillAffix1Amount"].ToString());
            SkillAffix2 = data["SkillAffix2"].ToString();
            SkillAffix2Amount = int.Parse(data["SkillAffix2Amount"].ToString());
            SkillAffix3 = data["SkillAffix3"].ToString();
            SkillAffix3Amount = int.Parse(data["SkillAffix3Amount"].ToString());
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
}
