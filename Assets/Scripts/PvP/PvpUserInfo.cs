using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System;
using PomeloSocketCenter.PomeloLib;

public class PvpUserInfo
{
	public const int ODDS_VALUE = 10000;

	/// <summary>
	/// 用户ID
	/// </summary>
	public int UserId;
	
	/// <summary>
	/// 昵称
	/// </summary>
	public string NickName;
	
	/// <summary>
	/// 竞技场星级
	/// </summary>
	public int ArenaStarLevel;
	
	/// <summary>
	/// 竞技场经验
	/// </summary>
	public int ArenaStarExp;
	
	/// <summary>
	/// 竞技场排名
	/// </summary>
	public int ArenaRank;
	
	/// <summary>
	/// 用户拥有的宠物
	/// </summary>
	public List<UserPet> UserPets = new List<UserPet>();
	
	/// <summary>
	/// 用户装备
	/// </summary>
	public List<PvpUserWare> UserWares = new List<PvpUserWare>();

	public int PvpType;

	public int Pvp_PosX;
	public int Pvp_PosY;
	public int Pvp_DelayTime; 
	public int Pvp_Round;
	public int Pvp_Level;
	public int Pvp_Hp;
	public int Pvp_TotalHp;
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

	public PvpUserInfo(JsonObject data, bool initStatus)
	{
		this._initStatus = initStatus;

		this.ArenaStarExp = int.Parse (data ["star_exp"].ToString ());
		this.ArenaStarLevel = ConfigManager.ArenaConfig.GetLevelByExp (this.ArenaStarExp);

		this.PvpType = int.Parse(data ["type"].ToString ());
		this.UserId = int.Parse (data ["user_id"].ToString ());
		this.NickName = data ["name"].ToString ();
		this.Pvp_Level = int.Parse (data ["level"].ToString ());

		this.Pvp_PosX = int.Parse (data ["x"].ToString ());
		this.Pvp_PosY = int.Parse (data ["y"].ToString ());
		this.Pvp_DelayTime = int.Parse (data ["delay_time"].ToString ());
		this.Pvp_Round = int.Parse (data ["round"].ToString ());

		this.Pvp_Hp = int.Parse (data ["hp"].ToString ());
		this.Pvp_TotalHp = int.Parse (data ["hp_max"].ToString ());
		this.Pvp_SkillPower = int.Parse (data ["skill_power"].ToString ());

		this.UserSkillList = new List<PvpSkillHouseData> ();
		
		foreach (JsonObject petData in (JsonArray)data["pets"])
		{
			PvpUserPet userPet = new PvpUserPet(petData);
			// 设置宠物等级
			userPet.Level = int.Parse(petData["level"].ToString());
			UserPets.Add(userPet);
		}

		JsonArray weaponsDataList = data ["weapons"] as JsonArray;
		foreach (JsonObject wareData in weaponsDataList)
		{
			PvpUserWare userWare = new PvpUserWare(wareData);
			UserWares.Add(userWare);

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

		// 初始化角色技能
		this.UserSkillList = this.InitSkillData ();
		// 初始化角色技能 CD
		this.UserSkillCdList = this.InitSkillCdData ();
	}

	public int SeedUserID
	{
		get 
		{
			if(this.UserId == 0) return 0;
			return this.UserId % 1600000;
		}
	}

	private HeroData curHero;
	public HeroData CurHero
	{
		get
		{
			return ConfigManager.HeroConfig.GetHeroByLvl(Level);
		}
		set
		{
			curHero = value;
		}
	}

	private int curWarefare;
	public int CurWarfare
	{
		get 
		{
			int Value = 0;
			
			foreach(UserPet u in this.UserPets)
			{
				Value += u.CurWarefare;
			}
			if (this.CurWeapon != null)
			{
				Value += this.CurWeapon.CurWarefare;
			}
			if(this.CurHelmet != null)
			{
				Value += this.CurHelmet.CurWarefare;
			}
			if(this.CurArmor != null)
			{
				Value += this.CurArmor.CurWarefare;
			}
			return Value;
		}
		set { this.curWarefare = value; }
	}

	private UserWare curWeapon;
	public UserWare CurWeapon
	{
		get
		{
			return this.curWeapon;
		}
		set
		{
			curWeapon = value;
		}
	}

	private UserWare curHelmet;
	public UserWare CurHelmet
	{
		get
		{
			return this.curHelmet;
		}
		set
		{
			curHelmet = value;
		}
	}

	private UserWare curArmor;
	public UserWare CurArmor
	{
		get
		{
			return this.curArmor;
		}
		set
		{
			curArmor = value;
		}
	}

	/// <summary>
	/// 初始化技能 cd
	/// </summary>
	/// <returns>The skill cd data.</returns>
	protected List<PvpSkillCdData> InitSkillCdData()
	{
		List<PvpSkillCdData> resultList = new List<PvpSkillCdData> ();
		foreach (UserPet p in this.UserPets)
		{
			if(p.CurPetData.PetSkillData != null)
			{
				resultList.Add(new PvpSkillCdData(p.CurPetData.PetSkillData, p.UserPetId));
			}
			if(p.CurPetData.PetSkillData2 != null)
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
	protected List<PvpSkillHouseData> InitSkillData()
	{
		List<PvpSkillHouseData> dataList = new List<PvpSkillHouseData> ();

		List<UserPet> petList = new List<UserPet> ();
		foreach(UserPet userPet in this.UserPets)
		{
			petList.Add(userPet);
		}
		
		petList.Sort (delegate(UserPet x, UserPet y) 
		{
			if(x.UserPetId < y.UserPetId) return -1;
			if(x.UserPetId > y.UserPetId) return 1;
			return 0;
		});
		
		foreach (UserPet p in petList)
		{
			//if (p.MapIndex != -1)
			//{
				if(p.CurPetData.PetSkillData != null)
				{
					dataList.Add(new PvpSkillHouseData(p.CurPetData.PetSkillData, p.UserPetId));
				}
				if(p.CurPetData.PetSkillData2 != null)
				{
					dataList.Add(new PvpSkillHouseData(p.CurPetData.PetSkillData2, p.UserPetId));
				}
			//}
		}

		if(this.curWeapon != null)
		{
			// 处理武器数据
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

			skillData = ConfigManager.SkillConfig.GetSkillById(this.CurWeapon.CurHardWareData.SkillAffix3);
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

	public List<UserPet> CurPets
	{
		get
		{
			return this.UserPets;
		}
	}

	public int Level
	{
		get
		{
			return this.Pvp_Level;
		}
		set
		{
			this.Pvp_Level = value;
		}
	}

	private float curHp;
	public float CurHp
	{
		get 
		{
			PvpBuffValueData buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass (true);
			
			float Value = 0f;
			foreach(UserPet u in CurPets)
			{
				Value += u.CurHp *( 1 + buffValueData.hp_limit_odds / ODDS_VALUE);
			}
			
			buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass ();

			return CurHero.Hp + buffValueData.hp_limit + Value;
		}
		set { this.curHp = value; }
	}

	public float OriHp
	{
		get
		{
			float Value = 0f;
			foreach(UserPet u in CurPets)
			{
				Value += u.CurHp;
			}

			return CurHero.Hp + Value;
		}
	}

	private float curAtk;
	public float CurAtk
	{
		get 
		{
			PvpBuffValueData buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass ();

			float atk = CurHero.Attack;
			if (CurWeapon != null)
			{
				atk = atk + CurWeapon.CurAtk;
			}
			return atk * (1 + buffValueData.attack_odds / ODDS_VALUE) + buffValueData.attack;
		}
		set { this.curAtk = value; }
	}

	public float OriAtk
	{
		get 
		{
			float atk = CurHero.Attack;
			if (CurWeapon != null)
			{
				atk = atk + CurWeapon.CurAtk;
			}
			return atk;
		}
	}

	private float curDef;
	public float CurDef
	{
		get 
		{
			PvpBuffValueData buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass ();

			float def = CurHero.Def;
			if (CurArmor != null)
			{
				def = def + CurArmor.CurDef;
			}
			if (CurHelmet != null)
			{
				def = def + CurHelmet.CurDef;
			}
			return def * (1 + buffValueData.defense_odds / ODDS_VALUE) + buffValueData.defense;
		}
		set { this.curDef = value; }
	}

	public float OriDef
	{
		get 
		{
			float def = CurHero.Def;
			if (CurArmor != null)
			{
				def = def + CurArmor.CurDef;
			}
			if (CurHelmet != null)
			{
				def = def + CurHelmet.CurDef;
			}
			return def;
		}
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

	public float CurCrit
	{
		get
		{
			PvpBuffValueData buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass(false);
			return buffValueData.crit_odds;
		}
	}

	public float OriCrit
	{
		get
		{
			return ConfigManager.ParamConfig.GetParam ().GlobalCrit;
		}
	}

	public float CurCritHurt
	{
		get
		{
			PvpBuffValueData buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass(false);
			return buffValueData.crit_hurt_odds;
		}
	}

	public float OriCritHurt
	{
		get
		{
			return ConfigManager.ParamConfig.GetParam ().GlobalCritDamageGrowthRate;
		}
	}

	public float CurAvoid
	{
		get
		{
			PvpBuffValueData buffValueData = this.pvpPlayerBuff.GetValueByBuffTypeToClass(false);
			return buffValueData.avoid_odds;
		}
	}

	public float OriAvoid
	{
		get
		{
			return ConfigManager.ParamConfig.GetParam ().GlobalMiss;
		}
	}
}
