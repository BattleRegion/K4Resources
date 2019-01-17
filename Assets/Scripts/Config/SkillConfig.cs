using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System;

public class SkillConfig : GameConfig 
{
    public SkillConfig()
    {
        this.ConfigName = "Skill";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            SkillData s = new SkillData(data);
            Configs.Add(s);
        }
    }

    public SkillData GetSkillById(string Id)
    {
		if(string.IsNullOrEmpty(Id)) return null;

        foreach (SkillData s in Configs)
        {
            if (s.Id == Id)
            {
                return s;
            }
        }
        return null;
    }

    /// <summary>
    /// 概率参数的固定分母
    /// </summary>
    const float paramterDenominator = 10000;
    public float GetParameterPercent(float pValue)
    {
        return pValue / paramterDenominator;
    }

    //public SkillData GetSkillByIdLv(string Id, int Lv)
    //{
    //    foreach(SkillData s in Configs)
    //    {
    //        if(s.Id == Id && s.SkillLv == Lv)
    //        {
    //            return s;
    //        }
    //    }
    //    return null;
    //}
}

public class SkillData : ConfigData
{
    public string Id;
    public string Name;
    public string SkillFX;
    public string Description;
    public string Xparameter;//范围
    public string Yparameter;//Y属性
    public float Aparameter;//伤害
    public float Bparameter;//消除值
    public float Cparameter;//击退值
	public float Dparameter;//
	public float Nparameter;//
    public int SkillPower;//消耗
    public float CurNum;
    public int Type;
    public string SkillPrefab;//对应资源名字
    public string SkillIcon;
    public int UpCoin; //升级
    public string UpN1;
    public string UpN2;
    public string UpN3;
    public string UpN4;
    public int SkillCd;
	public int FXType1;
	public int FXType2;
	public int FXType3;

	public string FXPrefab1;
	public string FXPrefab2;
	public string FXPrefab3;

    public int OkPve;

	public string HardwareNeed;
    public List<string> SuitSkillHardwareIds = new List<string>();

    public SkillData(JsonObject data = null)
    {
		if(data == null) return;
        try
        {
            Id = data["SkillId"].ToString();
            Name = data["Name"].ToString();
            SkillFX = data["SkillFXId"].ToString();
            Description = data["Description"].ToString();
            Xparameter = data["Xparameter"].ToString();
            Yparameter = data["Yparameter"].ToString();
            Aparameter = float.Parse(data["Aparameter"].ToString());
            Bparameter = float.Parse(data["Bparameter"].ToString());//----
			Cparameter = float.Parse(data["Cparameter"].ToString());
			Dparameter = float.Parse(data["Dparameter"].ToString());
			Nparameter = float.Parse(data["Nparameter"].ToString());
            CurNum = Cparameter;
            Type = int.Parse(data["Type"].ToString());
            SkillIcon = data["SkillIcon"].ToString();
            UpCoin = int.Parse(data["UpCoin"].ToString());
            UpN1 = data["UpN1"].ToString();
            UpN2 = data["UpN2"].ToString();
            UpN3 = data["UpN3"].ToString();
            UpN4 = data["UpN4"].ToString();
            SkillCd = int.Parse(data["SkillCd"].ToString());

            FXType1 = int.Parse(data["FXType1"].ToString());
            FXType2 = int.Parse(data["FXType2"].ToString());
            FXType3 = int.Parse(data["FXType3"].ToString());

			FXPrefab1 = data["FXPrefab1"].ToString();
			FXPrefab2 = data["FXPrefab2"].ToString();
			FXPrefab3 = data["FXPrefab3"].ToString();
            OkPve = int.Parse(data["OKPVE"].ToString());
			this.HardwareNeed = data["HardwareNeed"].ToString();
            foreach(string s in HardwareNeed.Split(','))
            {
                SuitSkillHardwareIds.Add(s);
            }
            //Debug.Log("okpve= "+ OkPve);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
        try
        {
            //Debug.Log("ceshi  -30 .");
            SkillPower = int.Parse(data["SkillPower"].ToString()); //-500
        }
        catch
        {

        }
    }


    public SkillData(BossSkillData bSkill)
    {
        Xparameter = bSkill.Xparameter;
        Yparameter = bSkill.Yparameter;
        if (!string.IsNullOrEmpty(bSkill.Aparameter))
            Aparameter = float.Parse(bSkill.Aparameter);
        if (!string.IsNullOrEmpty(bSkill.Bparameter))
            Bparameter = float.Parse(bSkill.Bparameter);
        if (!string.IsNullOrEmpty(bSkill.Cparameter))
            Cparameter = float.Parse(bSkill.Cparameter);
        if (!string.IsNullOrEmpty(bSkill.Dparameter))
            Dparameter = float.Parse(bSkill.Dparameter);
        if (!string.IsNullOrEmpty(bSkill.Nparameter))
            Nparameter = float.Parse(bSkill.Nparameter);

        FXType1 = bSkill.FXType1;
        FXType2 = bSkill.FXType2;
        FXType3 = bSkill.FXType3;

        FXPrefab1 = bSkill.FXPrefab1;
        FXPrefab2 = bSkill.FXPrefab2;
        FXPrefab3 = bSkill.FXPrefab3;
    }
}

