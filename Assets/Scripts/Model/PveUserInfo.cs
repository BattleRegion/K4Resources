using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System;
using PomeloSocketCenter.PomeloLib;

public class PveUserInfo : UserInfo
{
	public const int ODDS_VALUE = 10000;

	public int PvpType;

	public int Pvp_PosX;
	public int Pvp_PosY;
	public int Pvp_DelayTime; 
	public int Pvp_Round;
	public int Pvp_Def;
	public int Pvp_Hp;
	public int _pvp_TotalHp;
	// 当前能量
	public int Pvp_SkillPower;
	public string Pvp_FinalSkill;

	/// <summary>
	/// 总能量
	/// </summary>
	public int Pvp_TotalPower;

	public List<PvpSkillHouseData> UserSkillList;
    public List<PvpSkillCdData> UserSkillCdList;

	public PvpPlayerBuff pvpPlayerBuff;

	public PvpPlayerSkill pvpPlayerSkill;

	private bool _initStatus;

	public PveUserInfo(JsonObject data=null, bool initStatus=false):base(data)
	{
		/**this._initStatus = initStatus;

		this.PvpType = int.Parse(data ["type"].ToString ());
		this.UserId = int.Parse (data ["user_id"].ToString ());
		this.Level = int.Parse (data ["level"].ToString ());

		this.Pvp_PosX = int.Parse (data ["x"].ToString ());
		this.Pvp_PosY = int.Parse (data ["y"].ToString ());
		this.Pvp_DelayTime = int.Parse (data ["delay_time"].ToString ());
		this.Pvp_Round = int.Parse (data ["round"].ToString ());

		this.Pvp_Def = int.Parse (data ["def"].ToString ());
		this.Pvp_Hp = int.Parse (data ["hp"].ToString ());
		this.Pvp_SkillPower = int.Parse (data ["skill_power"].ToString ());

		this.UserSkillList = new List<PvpSkillHouseData> ();
		
		foreach (JsonObject petData in (JsonArray)data["pets"])
		{
			UserPet userPet = new UserPet(petData);
			UserPets.Add(userPet);
		}
		
		foreach (JsonObject wareData in (JsonArray)data["weapons"])
		{
			UserWare userWare = new UserWare(wareData);
			UserWares.Add(userWare);

			if (userWare.IsWare == true)
			{
				if (userWare.IsWeapon())
				{
					CurWeapon = userWare;
				}
				else if (userWare.IsHelmet())
				{
					CurHelmet = userWare;
				}
				else if (userWare.IsArmor())
				{
					CurArmor = userWare;
				}
			}
		}*/

        this.UserSkillList = new List<PvpSkillHouseData>();
		// 初始化角色技能
		this.UserSkillList = this.InitSkillData ();
        // 初始化角色技能 CD
        this.UserSkillCdList = this.InitSkillCdData();
	}

	public void ResetSkillList(){
		UserSkillList = InitSkillData ();
        // 初始化角色技能 CD
        this.UserSkillCdList = this.InitSkillCdData();
	}

    private List<UserPet> pets;
    public override List<UserPet> CurPets
    {
        get
        {
            pets = new List<UserPet>();
            foreach(UserPet u in UserManager.CurUserInfo.GetCurrentParty().pets)
            {
                pets.Add(u);
            }
            if(PveGameControl.CurFriend != null)
            {
                pets.Add(PveGameControl.CurFriend.FriendLeader);
            }
            return pets;
        }
        set { pets = value; }
    }

	/// <summary>
	/// 获得当前值
	/// </summary>
	/// <value>The current hp.</value>
	public float CurrentHp
	{
		get
		{
			if(this._initStatus) return this.curHp;
			return this.Pvp_Hp;
		}
	}
    /// <summary>
    /// 初始化技能 cd
    /// </summary>
    /// <returns>The skill cd data.</returns>
    protected List<PvpSkillCdData> InitSkillCdData()
    {
        List<PvpSkillCdData> resultList = new List<PvpSkillCdData>();
        foreach (UserPet p in this.CurPets)
        {
            if (p.CurPetData.PetSkillData != null)
            {
                resultList.Add(new PvpSkillCdData(p.CurPetData.PetSkillData, p.UserPetId));
            }
            if (p.CurPetData.PetSkillData2 != null)
            {
                resultList.Add(new PvpSkillCdData(p.CurPetData.PetSkillData2, p.UserPetId));
            }
        }
        return resultList;
    }

	/// <summary>
	/// 初始化技能
	/// </summary>
	/// <returns>The skill data.</returns>
	public List<PvpSkillHouseData> InitSkillData()
	{
		List<PvpSkillHouseData> dataList = new List<PvpSkillHouseData> ();

        //List<UserPet> petList = new List<UserPet> ();
        //foreach(UserPet userPet in UserManager.CurUserInfo.UserPets)
        //{
        //    petList.Add(userPet);
        //}
		
        //petList.Sort (delegate(UserPet x, UserPet y) 
        //              {
        //    if(x.UserPetId < y.UserPetId) return -1;
        //    if(x.UserPetId > y.UserPetId) return 1;
        //    return 0;
        //});

        foreach (UserPet p in CurPets)
        {
            //Debug.Log(p.MapIndex+"     " + p.CurPetData.Id +"   "+p.CurPetData.PetSkillData);
            if (p.CurPetData.PetSkillData != null)
            {
                dataList.Add(new PvpSkillHouseData(p.CurPetData.PetSkillData, p.UserPetId));
            }
            if (p.CurPetData.PetSkillData2 != null)
            {
                dataList.Add(new PvpSkillHouseData(p.CurPetData.PetSkillData2, p.UserPetId));
            }
        }

		if(this.curWeapon != null)
		{
			SkillData skillData = ConfigManager.SkillConfig.GetSkillById(this.CurWeapon.CurHardWareData.SkillAffix1);
			if(skillData != null)
			{
				dataList.Add(new PvpSkillHouseData(skillData, this.CurWeapon.UserWareId));
			}
			
			skillData = ConfigManager.SkillConfig.GetSkillById(this.CurWeapon.CurHardWareData.SkillAffix2);
			if(skillData != null)
			{
				dataList.Add(new PvpSkillHouseData(skillData, this.CurWeapon.UserWareId));
			}
		}
		// 处理套装数据
		List<PvpSkillHouseData> suitSkillList = new List<PvpSkillHouseData> ();
		
		if(this.CurHelmet != null)
		{
			SkillData suitSkillData = ConfigManager.SkillConfig.GetSkillById (this.CurHelmet.CurHardWareData.SkillAffix1);
			if(suitSkillData != null)
			{
				// 如果达成套装条件
				if(this.SkillSuitCheck(suitSkillData))
				{
					if(!this.SkillSuitDataCheck(suitSkillList, suitSkillData))
					{
						suitSkillList.Add(new PvpSkillHouseData(suitSkillData, this.CurHelmet.UserWareId));
					}
				}
			}
			suitSkillData = ConfigManager.SkillConfig.GetSkillById (this.CurHelmet.CurHardWareData.SkillAffix2);
			if(suitSkillData != null)
			{
				// 如果达成套装条件
				if(this.SkillSuitCheck(suitSkillData))
				{
					if(!this.SkillSuitDataCheck(suitSkillList, suitSkillData))
					{
						suitSkillList.Add(new PvpSkillHouseData(suitSkillData, this.CurHelmet.UserWareId));
					}
				}
			}
			suitSkillData = ConfigManager.SkillConfig.GetSkillById (this.CurHelmet.CurHardWareData.SkillAffix3);
			if(suitSkillData != null)
			{
				// 如果达成套装条件
				if(this.SkillSuitCheck(suitSkillData))
				{
					if(!this.SkillSuitDataCheck(suitSkillList, suitSkillData))
					{
						suitSkillList.Add(new PvpSkillHouseData(suitSkillData, this.CurHelmet.UserWareId));
					}
				}
			}
		}
		
		if(this.CurArmor != null)
		{
			SkillData suitSkillData = ConfigManager.SkillConfig.GetSkillById (this.CurArmor.CurHardWareData.SkillAffix1);
			if(suitSkillData != null)
			{
				// 如果达成套装条件
				if(this.SkillSuitCheck(suitSkillData))
				{
					if(!this.SkillSuitDataCheck(suitSkillList, suitSkillData))
					{
						suitSkillList.Add(new PvpSkillHouseData(suitSkillData, this.CurArmor.UserWareId));
					}
				}
			}
			suitSkillData = ConfigManager.SkillConfig.GetSkillById (this.CurArmor.CurHardWareData.SkillAffix2);
			if(suitSkillData != null)
			{
				// 如果达成套装条件
				if(this.SkillSuitCheck(suitSkillData))
				{
					if(!this.SkillSuitDataCheck(suitSkillList, suitSkillData))
					{
						suitSkillList.Add(new PvpSkillHouseData(suitSkillData, this.CurArmor.UserWareId));
					}
				}
			}
			suitSkillData = ConfigManager.SkillConfig.GetSkillById (this.CurArmor.CurHardWareData.SkillAffix3);
			if(suitSkillData != null)
			{
				// 如果达成套装条件
				if(this.SkillSuitCheck(suitSkillData))
				{
					if(!this.SkillSuitDataCheck(suitSkillList, suitSkillData))
					{
						suitSkillList.Add(new PvpSkillHouseData(suitSkillData, this.CurArmor.UserWareId));
					}
				}
			}
		}
		
		if(suitSkillList != null && suitSkillList.Count > 0)
		{
			foreach(PvpSkillHouseData pvpSkillHouseData in suitSkillList)
			{
				dataList.Add(pvpSkillHouseData);
			}
		}
		return dataList;
	}

	private bool SkillSuitDataCheck(List<PvpSkillHouseData> dataList, SkillData skillData)
	{
		foreach(PvpSkillHouseData skillHouseData in dataList)
		{
			if(skillHouseData.skillData.Id == skillData.Id) return true;
		}
		return false;
	}
	
	private bool SkillSuitCheck(SkillData skillData)
	{
		if(string.IsNullOrEmpty(skillData.HardwareNeed)) return true;
		
		string[] itemList = skillData.HardwareNeed.Split (new string[]{","}, StringSplitOptions.RemoveEmptyEntries);
		foreach(string itemID in itemList)
		{
			if(this.CurHelmet != null && this.CurArmor != null)
			{
				if(itemID != this.CurHelmet.CurHardWareData.Id && itemID != this.CurArmor.CurHardWareData.Id) return false;
			}
		}
		
		return true;
	}

    public float GetPetHpUnderBuffs(UserPet u)
    {
        PveSkillManager.SkillInit(UserManager.pveUserInfo, UserManager.pveUserInfo.UserSkillList);
        PveSkillManager.BuffListInit(UserManager.pveUserInfo);
        ResetSkillList();

        float hp = 0f;
        PvpBuffValueData buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass(true);
        hp = u.CurHp * (1 + buffValueData.hp_limit_odds / ODDS_VALUE);
        return hp;
    }

    public float GetPetAtkUnderBuffs(UserPet u)
    {
        PveSkillManager.SkillInit(UserManager.pveUserInfo, UserManager.pveUserInfo.UserSkillList);
        PveSkillManager.BuffListInit(UserManager.pveUserInfo);
        ResetSkillList();

        float atk = 0f;
        PvpBuffValueData buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass(true);
        atk = u.CurAtk * (1 + buffValueData.attack_odds / ODDS_VALUE) + buffValueData.attack;
        return atk;
    }

	public override float CurHp
	{
		get 
		{
			PvpBuffValueData buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass (true);

			float Value = 0f;
			foreach(UserPet u in UserManager.CurUserInfo.CurPets)
			{
				Value += u.CurHp *( 1 + buffValueData.hp_limit_odds / ODDS_VALUE);
			}

			buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass ();

			
            //Debug.Log("---------------------------------------------------- 生命开始 -------------------------------------");
            //Debug.Log("原始生命值 " + CurHero.Hp);
            //Debug.Log("宠物生命加成 " + Value);
            //Debug.Log("当前生命上限 " + buffValueData.hp_limit);
            //Debug.Log("---------------------------------------------------- 生命结束 -------------------------------------");
			

			this.curHp = CurHero.Hp + buffValueData.hp_limit + Value;
			return this.curHp;
		}
		set { base.CurHp = value; }
	}

	public override float CurAtk
	{
		get 
		{
			PvpBuffValueData buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass ();

			float atk = CurHero.Attack;
			if (CurWeapon != null)
			{
				atk = atk +  UserManager.CurUserInfo.CurWeapon.CurAtk;
			}
			
            //Debug.Log("----------------------------------------------------------------------------------- 攻击");

            //Debug.Log("初始攻击值：：：：" + atk);
            //Debug.Log("攻击加成值：：：：" + buffValueData.attack);
            //Debug.Log("攻击加成值概率：：：：" + buffValueData.attack_odds);
            //Debug.Log("攻击最终值：：：：" + (atk * (1 + buffValueData.attack_odds / ODDS_VALUE) + buffValueData.attack));
			
			return (atk + buffValueData.attack) * (1 + buffValueData.attack_odds / ODDS_VALUE);
		}
		set { base.curAtk = value; }
	}

	public override float CurDef
	{
		get 
		{
			PvpBuffValueData buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass ();
           

			float def = CurHero.Def;
            if (UserManager.CurUserInfo.CurArmor != null)
			{
                def = def + UserManager.CurUserInfo.CurArmor.CurDef;
			}
            if (UserManager.CurUserInfo.CurHelmet != null)
			{
                def = def + UserManager.CurUserInfo.CurHelmet.CurDef;
			}

            //Debug.Log("----------------------------------------------------------------------------------- 防御 buff 开始");
            //Debug.Log("初始防御值：：：：" + def);
            //Debug.Log("攻击防御值：：：：" + buffValueData.defense);
            //Debug.Log("攻击防御值概率：：：：" + buffValueData.defense_odds);
            //Debug.Log("----------------------------------------------------------------------------------- 防御 buff 结束");
			
			return (def + buffValueData.defense) * (1 + buffValueData.defense_odds / ODDS_VALUE);
		}
		set { base.curDef = value; }
	}

	public int CurPower
	{
		get
		{
			HeroData heroData = ConfigManager.HeroConfig.GetHeroByLvl(this.Level);
			if(heroData == null) return 0;
			
			PvpBuffValueData buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass(true);
			int power = heroData.HPower;
			
			power = (int)((power + (int)buffValueData.energy_limit) * (1 + buffValueData.energy_limit_odds / ODDS_VALUE));
			
			if(power == 0) power = 1;
			
			return power;
		}
		set{this.Pvp_TotalPower = value;}
	}
}
