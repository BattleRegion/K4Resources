using UnityEngine;
using System.Collections;
using SimpleJson;
using System;

public class BossConfig : GameConfig {
    public BossConfig()
    {
        this.ConfigName = "Boss";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            BossData b = new BossData(data);
            Configs.Add(b);
        }
    }

    public BossData GetBossById(string Id)
    {
        foreach (BossData b in Configs)
        {
            if (b.Id == Id)
            {
                return b;
            }
        }
        return null;
    }
}

public class BossData : ConfigData
{
    public string Id;

    public string SkinId;

    public float Hp;

    public float Attack;

    public DungeonEnum.AttackType Type;

    public int Range;

    public int RunPower;

    public int FightOff;

    public int Def;

    public DungeonEnum.ElementAttributes Element;

    public string BossAI;

    public BossSkillController.SkillType AIType;

    public BossSkillData BossSkill = new BossSkillData();

    public BossData(JsonObject data)
    {
        try
        {
            Id = data["BossId"].ToString();
            SkinId = data["SkinId"].ToString();
            Hp = float.Parse(data["Hp"].ToString());
            Attack = float.Parse(data["Attack"].ToString());
            Range = int.Parse(data["Range"].ToString());
            RunPower = int.Parse(data["RunPower"].ToString());
            FightOff = int.Parse(data["FightOff"].ToString());
            Def = int.Parse(data["Def"].ToString());
            Element = (DungeonEnum.ElementAttributes)int.Parse(data["Element"].ToString());

            BossSkill.Xparameter = data["Xparameter"].ToString();
            BossSkill.Yparameter = data["Yparameter"].ToString();
            BossSkill.Aparameter = data["Aparameter"].ToString();
            BossSkill.Bparameter = data["Bparameter"].ToString();
            BossSkill.Cparameter = data["Cparameter"].ToString();
            BossSkill.Dparameter = data["Dparameter"].ToString();
            BossSkill.Nparameter = data["Nparameter"].ToString();

            BossSkill.FXType1 = int.Parse(data["FXTpye1"].ToString());
            BossSkill.FXType2 = int.Parse(data["FXTpye2"].ToString());
            BossSkill.FXType3 = int.Parse(data["FXTpye3"].ToString());

            BossSkill.FXPrefab1 = data["FXPrefab1"].ToString();
            BossSkill.FXPrefab2 = data["FXPrefab2"].ToString();
            BossSkill.FXPrefab3 = data["FXPrefab3"].ToString();

            BossAI = data["BossAI"].ToString();
            AIType = GetBossAIType(BossAI);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }

    BossSkillController.SkillType GetBossAIType(string ai)
    {
        if (ai == "BOSSAI1") return BossSkillController.SkillType.BossAI_1;
        if (ai == "BOSSAI2") return BossSkillController.SkillType.BossAI_2;
        if (ai == "BOSSAI3") return BossSkillController.SkillType.BossAI_3;
        if (ai == "BOSSAI4") return BossSkillController.SkillType.BossAI_4;
        if (ai == "BOSSAI5") return BossSkillController.SkillType.BossAI_5;
        if (ai == "BOSSAI6") return BossSkillController.SkillType.BossAI_6;
        if (ai == "BOSSAI7") return BossSkillController.SkillType.BossAI_7;
        if (ai == "BOSSAI8") return BossSkillController.SkillType.BossAI_8;
        if (ai == "BOSSAI9") return BossSkillController.SkillType.BossAI_9;
        if (ai == "BOSSAI10") return BossSkillController.SkillType.BossAI_10;
        if (ai == "BOSSAI11") return BossSkillController.SkillType.BossAI_11;
        if (ai == "BOSSAI12") return BossSkillController.SkillType.BossAI_12;
        if (ai == "BOSSAI13") return BossSkillController.SkillType.BossAI_13;
        if (ai == "BOSSAI14") return BossSkillController.SkillType.BossAI_14;
        if (ai == "BOSSAI15") return BossSkillController.SkillType.BossAI_15;
        if (ai == "BOSSAI16") return BossSkillController.SkillType.BossAI_16;
        if (ai == "BOSSAI17") return BossSkillController.SkillType.BossAI_17;
        if (ai == "BOSSAI18") return BossSkillController.SkillType.BossAI_18;
        else return BossSkillController.SkillType.None;
    }
}