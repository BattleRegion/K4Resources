using UnityEngine;
using System.Collections;
using SimpleJson;
using System;

public class ParamConfig : GameConfig
{
    public ParamConfig()
    {
        this.ConfigName = "Parameter";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            ParamData m = new ParamData(data);
            Configs.Add(m);
        }
    }

    public ParamData GetParam()
    {
        return (ParamData)Configs[0];
    }
}

public class ParamData : ConfigData
{
    public float PetEXPGrowthRate;

    public float PetLvUpCoinGrowthRate;

    public float HardwareEXPGrowthRate;

    public float HardwareLvUpCoinGrowthRate;

    public float GlobalMiss;

    public float GlobalCrit;

    public float GlobalCritDamageGrowthRate;

    public float GlobalElementAddDamage;

    public float GlobalElemnetDeductDamage;

    public float GlobalSkillPowerGrowthRate;

    public string LuckPool;

    public string FriendsPool;

    public float MapRoundDotHp;

    public float PethouseConsume;

    public float WarehouseConsume;

    public float EnergyRecoveryTime;

    public float EnergyRecoveryRate;

    public float EnergyBuyRate;

    public float EnergyPrice;

    public float ResurrectionPrice;

    public float FriendsPoint;

    public float AdventurerPoint;

    public float YoungstersRate;

    public float YoungstersFriendsRate;

    public float YoungstersAdventurerRate;

    public float YoungstersFriendsCD;

    public float YoungstersFriendsLv;

    public float FriendsCardRate;

    public float WarefareRate;

	public int ArenaFreeRate;

	public string ArenaTime;

	public string ArenaName;

	public int CompensationPower;

	public int PVPRoundTime;

	public int PVPRoundFastTime;

	public int ArenaOpenLv;

	public string LegendReward;

	public string RewardTop1;
	
	public string RewardTop1Name;

	public string RewardTop2;
	
	public string RewardTop2Name;

	public string RewardTop3;
	
	public string RewardTop3Name;

	public string ArenaReTime;

    public float AdditionMax;

    public string ConfigVersion;

    public int WheelPrice;

    public int WheelResetPrice;


    public ParamData(JsonObject data)
    {
        try
        {
            PetEXPGrowthRate = float.Parse(data["PetEXPGrowthRate"].ToString());
            PetLvUpCoinGrowthRate = float.Parse(data["PetLvUpCoinGrowthRate"].ToString());
            HardwareEXPGrowthRate = float.Parse(data["HardwareEXPGrowthRate"].ToString());
            HardwareLvUpCoinGrowthRate = float.Parse(data["HardwareLvUpCoinGrowthRate"].ToString());
            GlobalMiss = float.Parse(data["GlobalMiss"].ToString());
            GlobalCrit = float.Parse(data["GlobalCrit"].ToString());
            //GlobalCrit = 0.2f;

            GlobalCritDamageGrowthRate = float.Parse(data["GlobalCritDamageGrowthRate"].ToString());
            GlobalElementAddDamage = float.Parse(data["GlobalElementAddDamage"].ToString());
            GlobalElemnetDeductDamage = float.Parse(data["GlobalElemnetDeductDamage"].ToString());
            GlobalSkillPowerGrowthRate = float.Parse(data["GlobalSkillPowerGrowthRate"].ToString());
            LuckPool = data["LuckPool"].ToString();
            FriendsPool = data["FriendsPool"].ToString();
            MapRoundDotHp = float.Parse(data["MapRoundDotHp"].ToString());
            PethouseConsume = float.Parse(data["PethouseConsume"].ToString());
            WarehouseConsume = float.Parse(data["WarehouseConsume"].ToString());
            EnergyRecoveryTime = float.Parse(data["EnergyRecoveryTime"].ToString());
            EnergyRecoveryRate = float.Parse(data["EnergyRecoveryRate"].ToString());
            EnergyBuyRate = float.Parse(data["EnergyBuyRate"].ToString());
            EnergyPrice = float.Parse(data["EnergyPrice"].ToString());
            ResurrectionPrice = float.Parse(data["ResurrectionPrice"].ToString());
            FriendsPoint = float.Parse(data["FriendsPoint"].ToString());
            AdventurerPoint = float.Parse(data["AdventurerPoint"].ToString());
            YoungstersRate = float.Parse(data["YoungstersRate"].ToString());
            YoungstersFriendsRate = float.Parse(data["YoungstersFriendsRate"].ToString());
            YoungstersAdventurerRate = float.Parse(data["YoungstersAdventurerRate"].ToString());
            YoungstersFriendsCD = float.Parse(data["YoungstersFriendsCD"].ToString());
            YoungstersFriendsLv = float.Parse(data["YoungstersFriendsLv"].ToString());
            FriendsCardRate = float.Parse(data["FriendsCardRate"].ToString());
            WarefareRate = float.Parse(data["WarefareRate"].ToString());

            WheelPrice = int.Parse(data["WheelPrice"].ToString());
            WheelResetPrice = int.Parse(data["WheelResetPrice"].ToString());

            ConfigVersion = data["ConfigVersion"].ToString();

            this.ArenaFreeRate = int.Parse(data["ArenaFreeRate"].ToString());
            this.ArenaTime = data["ArenaTime"].ToString();
            this.ArenaName = data["ArenaName"].ToString();
            this.CompensationPower = int.Parse(data["CompensationPower"].ToString());
            this.PVPRoundTime = int.Parse(data["PVPRoundTime"].ToString());
            this.PVPRoundFastTime = int.Parse(data["PVPRoundFastTime"].ToString());
            this.ArenaOpenLv = int.Parse(data["ArenaOpenLv"].ToString());
            this.LegendReward = data["LegendReward"].ToString();
            this.RewardTop1 = data["RewardTop1"].ToString();
            this.RewardTop2 = data["RewardTop2"].ToString();
			this.RewardTop3 = data["RewardTop3"].ToString();
			this.RewardTop1Name = data["RewardTop1Name"].ToString();
			this.RewardTop2Name = data["RewardTop2Name"].ToString();
			this.RewardTop3Name = data["RewardTop3Name"].ToString();
            this.ArenaReTime = data["ArenaReTime"].ToString();
            AdditionMax = float.Parse(data["AdditionMax"].ToString());
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
}
