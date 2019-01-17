using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System;
using PomeloSocketCenter.PomeloLib;
public class UserInfo  
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public int UserId;

    /// <summary>
    /// 玩家CRE
    /// </summary>
    public string PlayerCre;

    /// <summary>
    /// 昵称
    /// </summary>
    public string NickName;

    /// <summary>
    /// 等级
    /// </summary>
    protected int level;
    public virtual int Level
    {
        get
        {
            level = ConfigManager.HeroConfig.GetLevelByHeroExp(UserManager.CurUserInfo.Exp);
            return level;
        }
        set
        {
            level = value;
        }
    }

    /// <summary>
    /// 当前队伍index
    /// </summary>
    public int CurPartyIndex;
    
    /// <summary>
    /// 当前组队最大HP
    /// </summary>
    public float CurTotalHp;

    /// <summary>
    /// 经验
    /// </summary>
    public int Exp;

    public int CurExp
    {
        get
        {
            return Exp - ConfigManager.HeroConfig.GetHeroByLvl(Level).Exp;
        }
        set
        {
            CurExp = value;
        }
    }

    public int CurLevelExp
    {
        get
        {
            if (Level == ConfigManager.HeroConfig.GetHeroMaxLevel()) return 0;
            else return ConfigManager.HeroConfig.GetHeroByLvl(Level + 1).Exp - ConfigManager.HeroConfig.GetHeroByLvl(Level).Exp;
        }
        set
        {
            CurExp = value;
        }
    }

    /// <summary>
    /// 钻石
    /// </summary>
    public int Diamond;

    /// <summary>
    /// 用户金币
    /// </summary>
    public int Gold;

    /// <summary>
    /// 体力
    /// </summary>
    public int Energy;

    /// <summary>
    /// 体力上限
    /// </summary>
    public int EnergyLimit;

    /// <summary>
    /// 装备栏上限
    /// </summary>
    public int WareLimit;

    /// <summary>
    /// 怪物栏上限
    /// </summary>
    public int PetHouseLimit;

    /// <summary>
    /// 剩余体力奖励
    /// </summary>
    public int EnergyPool;

    /// <summary>
    /// 用户的队伍
    /// </summary>
    public List<UserParty> UserPartys = new List<UserParty>();

    /// <summary>
    /// 用户的好友
    /// </summary>
    public List<FriendInfo> UserFriends = new List<FriendInfo>();

    /// <summary>
    /// 用户助战列表
    /// </summary>
    public List<HelpData> UserHelpers = new List<HelpData>();

    /// <summary>
    /// 用户的好友请求
    /// </summary>
    public List<RequestInfo> UserRequests = new List<RequestInfo>();

    /// <summary>
    /// 用户拥有的宠物
    /// </summary>
    public List<UserPet> UserPets = new List<UserPet>();

    /// <summary>
    /// 用户道具
    /// </summary>
    public List<UserItem> UserItems = new List<UserItem>();

    /// <summary>
    /// 用户装备
    /// </summary>
    public List<UserWare> UserWares = new List<UserWare>();

    /// <summary>
    /// 用户每日任务
    /// </summary>
    public List<UserTask> UserDailyTasks = new List<UserTask>();

    /// <summary>
    /// 用户抽奖奖池
    /// </summary>
    public List<LotteryItemData> UserLotteryItems = new List<LotteryItemData>();

    /// <summary>
    /// 打通的副本
    /// </summary>
    public List<JsonObject> PassDungeons = new List<JsonObject>();

	/// <summary>
	/// 竞技场报名数据
	/// </summary>
	public ArenaSeasonTicketInfo SeasonTicketInfo = new ArenaSeasonTicketInfo();

	/// <summary>
	/// 本赛季排名数据
	/// </summary>
	public List<ArenaThisRankInfo> SeasonRankInfoList = new List<ArenaThisRankInfo> ();

	/// <summary>
	/// 上赛季排名数据
	/// </summary>
	public List<ArenaPrevRankInfo> LastRankInfoList = new List<ArenaPrevRankInfo> ();

    /// <summary>
    /// 当前用户角色Id
    /// </summary>
    public string CharacterId;

    //体力恢复时间
    public DateTime StartRecoveryTime;

    public int Server_id=1;

    private TimeSpan recoveryCutdownTime;
    public TimeSpan RecoveryCutdownTime
    {
        get
        {
            TimeSpan config = new TimeSpan((long)ConfigManager.ParamConfig.GetParam().EnergyRecoveryTime * TimeSpan.TicksPerSecond);
            TimeSpan t = ConfigManager.LocalTime.LocalTime - StartRecoveryTime;
            return config - t;
        }
        set
        {
            recoveryCutdownTime = value;
        }
    }
    public bool RecoveryTag;

    protected HeroData curHero;
    public virtual HeroData CurHero
    {
        get
        {
            return ConfigManager.HeroConfig.GetHeroByLvl(ConfigManager.HeroConfig.GetLevelByHeroExp(UserManager.CurUserInfo.Exp));
        }
        set
        {
            curHero = value;
        }
    }

    public JsonObject NotEndDungeon;

    protected float curHp;
    public virtual float CurHp
    {
        get 
        {
            float Value = 0f;
            UserParty party = UserPartys[CurPartyIndex];

            foreach(UserPet u in party.pets)
            {
                Value += u.CurHp;
            }
            return UserManager.CurUserInfo.CurHero.Hp + Value;
        }
        set { curHp = value; }
    }

    /// <summary>
    /// 根据队伍ID获得生命值
    /// </summary>
    public float GetPartyHp(int pIndex)
    {
        float Value = (float)CurHero.Hp;
        UserParty party = UserPartys[pIndex];

        foreach(UserPet u in party.pets)
        {
            Value += u.CurHp;
        }
        return Value;
    }

    /// <summary>
    /// 新手引导
    /// </summary>
    public int GuideId ;//-1为结束

	protected float curAtk;
	public virtual float CurAtk
    {
        get 
        {
            float atk = CurHero.Attack;
            if (UserPartys[CurPartyIndex].weapon != null)
            {
                atk = atk + UserPartys[CurPartyIndex].weapon.CurAtk;
            }
            return atk;
            //return 1;
        }
        set { curAtk = value; }
    }

    /// <summary>
    /// 根据队伍ID获得攻击力
    /// </summary>
    public float GetPartyAtk(int pIndex)
    {
        float atk = CurHero.Attack;
        UserParty party = UserPartys[pIndex];
        if(party.weapon != null)
        {
            atk += party.weapon.CurAtk;
        }
        return atk;
    }

	protected float curDef;
	public virtual float CurDef
    {
        get 
        {
            float def = CurHero.Def;
            if (UserPartys[CurPartyIndex].armor != null)
            {
                def = def + UserPartys[CurPartyIndex].armor.CurDef;
            }
            if (UserPartys[CurPartyIndex].helmet != null)
            {
                def = def + UserPartys[CurPartyIndex].helmet.CurDef;
            }
            return def;
            //return 9999;
        }
        set { curDef = value; }
    }

    /// <summary>
    /// 根据队伍ID获得防御力
    /// </summary>
    public float GetPartyDef(int pIndex)
    {
        float def = CurHero.Def;
        UserParty party = UserPartys[pIndex];
        if(party.armor != null)
        {
            def += party.armor.CurDef;
        }
        if(party.helmet != null)
        {
            def += party.helmet.CurDef;
        }
        return def;
    }

    protected int curWarefare;
    public virtual int CurWarfare
    {
        get 
        {
            int Value = 0;
            UserParty party = UserPartys[CurPartyIndex];

            foreach(UserPet u in party.pets)
            {
                Value += u.CurWarefare;
            }
            if (party.weapon != null)
            {
                Value += party.weapon.CurWarefare;
            }
            if(party.helmet != null)
            {
                Value += party.helmet.CurWarefare;
            }
            if(party.armor != null)
            {
                Value += party.armor.CurWarefare;
            }
            return Value;
        }
        set { curWarefare = value; }
    }

    /// <summary>
    /// 当前武器，头盔，护甲，宠物
    /// </summary>
    protected UserWare curWeapon;
    public virtual UserWare CurWeapon
    {
        get
        {
            return UserPartys[CurPartyIndex].weapon;
        }
        set
        {
            curWeapon = value;
        }
    }

	protected UserWare curHelmet;
	public virtual UserWare CurHelmet
    {
        get
        {
            return UserPartys[CurPartyIndex].helmet;
        }
        set
        {
            curHelmet = value;
        }
    }

    protected UserWare curArmor;
	public virtual UserWare CurArmor
    {
        get
        {
            return UserPartys[CurPartyIndex].armor;
        }
        set
        {
            curArmor = value;
        }
    }

    private List<UserPet> curPets;
    public virtual  List<UserPet> CurPets
    {
        get
        {
            curPets = new List<UserPet>();
            UserParty party = UserPartys[CurPartyIndex];
            foreach(UserPet u in party.pets)
            {
                curPets.Add(u);
            }

            if(PveGameControl.CurFriend != null)
            {
                curPets.Add(PveGameControl.CurFriend.FriendLeader);
            }
            return curPets;
        }
        set { curPets = value; }
    }

	/// <summary>
	/// 竞技场免费次数
	/// </summary>
	private int _ArenaFreeTimes;
	public int ArenaFreeTimes
	{
		get
		{ 
			if(this._ArenaFreeTimes < 0) return 0;
			return this._ArenaFreeTimes;
		}
		set{ this._ArenaFreeTimes = value; }
	}

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
	/// PVP ticket
	/// </summary>
	public int ArenaPvpTicket;

	/// <summary>
	/// 报名状态
	/// </summary>
	public int ArenaPvpTicketStatus;

	/// <summary>
	/// 竞技场刷新
	/// </summary>
	public DateTime ArenaRefreshTime;

	/// <summary>
	/// 本赛季数据摘取时间间隔
	/// </summary>
	public int ArenaSeasonTimeDelay = 60;
	/// <summary>
	/// 上赛季数据摘取时间间隔
	/// </summary>
	public int ArenaLastTimeDelay = 600;
	
	public int ArenaSeasonTime;
	public int ArenaLastTime;

    public UserInfo(JsonObject data)
    {
		if(data == null) return;

        CharacterId = "S100";

        PlayerCre = data["credential"].ToString();

        JsonObject playerInfoData = (JsonObject)data["player_info"];

        UserId = int.Parse(playerInfoData["user_id"].ToString());

		JsonObject pvpTicketData = (JsonObject)data ["pvp_ticket"];
		if(pvpTicketData != null)
		{
			ArenaPvpTicket = int.Parse(pvpTicketData["ticket_id"].ToString());
			ArenaPvpTicketStatus = int.Parse(pvpTicketData["status"].ToString());
		}

        Exp = int.Parse(playerInfoData["exp"].ToString());

        Diamond = int.Parse(playerInfoData["diamond"].ToString());

        Gold = int.Parse(playerInfoData["gold"].ToString());

        Energy = int.Parse(playerInfoData["energy"].ToString());

        //Server_id = int.Parse(playerInfoData["server_id"].ToString());

        RecoveryTag = int.Parse(playerInfoData["energy_interval"].ToString()) == -1 ? false : true;

		this.ArenaFreeTimes = int.Parse (playerInfoData ["free_times"].ToString ());
		this.ArenaStarLevel = int.Parse (playerInfoData ["star_level"].ToString ());
		this.ArenaStarExp = int.Parse (playerInfoData ["star_exp"].ToString ());

        if(RecoveryTag)
        {
            RecoveryCutdownTime = new TimeSpan(long.Parse(playerInfoData["energy_interval"].ToString()) * TimeSpan.TicksPerSecond);
            StartRecoveryTime = new DateTime(0) + new TimeSpan(ConfigManager.LocalTime.LocalTime.Ticks - (((long)ConfigManager.ParamConfig.GetParam().EnergyRecoveryTime * TimeSpan.TicksPerSecond) - recoveryCutdownTime.Ticks));
        }


        if (playerInfoData["nickname"] == null)
        {
            NickName = playerInfoData["user_id"].ToString();
        }
        else
        {
            NickName = playerInfoData["nickname"].ToString();
        }

        JsonObject extendData = (JsonObject)data["extend"];
        EnergyLimit = int.Parse(extendData["energy_max"].ToString());
        WareLimit = int.Parse(extendData["warehouse_max"].ToString());
        PetHouseLimit = int.Parse(extendData["pethouse_max"].ToString());
        CurPartyIndex = int.Parse(extendData["queue_id"].ToString());

        //Debug.Log(PlayerPrefs.HasKey("GuideID")+" --");    	
        //GuideId = 60; //999;//67; //999;// 42;//999 15;49; ;61;
        GuideId = int.Parse(extendData["step"].ToString());
        //Debug.Log(GuideId);

        if (GuideId <= 1) {
            GuideId = 1;
        }else if(GuideId<42&&GuideId>15){
            GuideId = 16;
        }
        else if (GuideId >=70)
        {
            //guide end
            GuideId = -1;            
        }
        
        Debug.Log("引导ID=" + GuideId);
        PlayerPrefs.SetInt("GuideID",GuideId);

        EnergyPool = int.Parse(data["pve_energy"].ToString());

        foreach (JsonObject petData in (JsonArray)data["pets"])
        {
            UserPet userPet = new UserPet(petData);
            UserPets.Add(userPet);
            //Debug.Log(userPet.CurPetData);
            //Debug.Log(userPet.CurPetData.Id);
            if(ConfigManager.PetConfig.IsNew(userPet.CurPetData.Id))
            {
                ConfigManager.PetConfig.SetNotNew(userPet.CurPetData.Id);
            }
        }

        foreach (JsonObject itemData in (JsonArray)data["items"])
        {
            UserItem userItem = new UserItem(itemData);
            UserItems.Add(userItem);
            if(ConfigManager.ItemConfig.IsNew(userItem.CurItemData.Id))
            {
                ConfigManager.ItemConfig.SetNotNew(userItem.CurItemData.Id);
            }
        }

        foreach (JsonObject taskData in (JsonArray)data["daily_tasks"])
        {
            UserTask task = new UserTask(taskData);
            UserDailyTasks.Add(task);
        }

        foreach(JsonObject lotteryData in (JsonArray)data["pool_items"])
        {
            LotteryItemData lo = new LotteryItemData(lotteryData);
            UserLotteryItems.Add(lo);
        }

        CurWeapon = null;
        CurHelmet = null;
        CurArmor = null;
        foreach (JsonObject wareData in (JsonArray)data["weapons"])
        {
            UserWare userWare = new UserWare(wareData);
            UserWares.Add(userWare);
            if(ConfigManager.HardWareConfig.IsNew(userWare.CurHardWareData.Id))
            {
                ConfigManager.HardWareConfig.SetNotNew(userWare.CurHardWareData.Id);
            }
        }

        foreach (JsonObject passData in (JsonArray)data["passes"])
        {
            AddNewPassDungeon(passData);
        }

        JsonObject partys = (JsonObject)data["queues"];
        for(int i = 0; i < 5; i++)
        {
            UserParty up = new UserParty(i, (JsonObject)partys[i.ToString()], UserWares, UserPets);
            UserPartys.Add(up);
        }
    }
    //pve引导中断  不退出游戏  再次进入pve
    public int GetCurGuideID()
    {
        if (GuideId < 41 && GuideId > 14)
        {
            GuideId = 16;
        }
        else if (GuideId >= 100)
        {
            GuideId = -1;
        }
        PlayerPrefs.SetInt("GuideID", GuideId);
        return GuideId;
    }

    public void AddNewPassDungeon(JsonObject  data)
    {
        string cid = data["chapter_id"].ToString();
        string did = data["dungeon_id"].ToString();
		string achieveState = data ["achieve_state"].ToString ();

		JsonObject result = null;
		int length = PassDungeons.Count;
		for(int index = 0; index < length; index ++)
		{
			JsonObject d = PassDungeons[index];
			string tcid = d["chapter_id"].ToString();
			string tdid = d["dungeon_id"].ToString();
			if (cid == tcid && did == tdid)
			{
				result = d;
			}
		}
		// 如果数据存在，更新成就！
		if(result != null)
		{
			result["achieve_state"] = achieveState;
		}else
		{
			// 添加数据
			PassDungeons.Add(data);
		}

        /*bool has = false;
        foreach (JsonObject d in PassDungeons)
        {
            string tcid = d["chapter_id"].ToString();
            string tdid = d["dungeon_id"].ToString();
            if (cid == tcid && did == tdid)
            {
                has = true;
            }
        }
        if (has == false)
        {
            PassDungeons.Add(data);
        }*/
    }

    public void AddUserElements(JsonArray elements)
    {
		if(elements == null || elements.Count == 0) return;
        foreach (JsonObject data in elements)
        {
            string Id = data["id"].ToString();
            int count = int.Parse(data["count"].ToString());
            if (Id == "currency2")
            {
                Gold = Gold + count;
            }
        }
    }

    public void SetExtend(JsonObject data)
    {
        WareLimit = int.Parse(data["warehouse_max"].ToString());
        PetHouseLimit = int.Parse(data["pethouse_max"].ToString());
    }


    public void RefreashDailyTasks(JsonObject taskData)
    {
        if(taskData != null)
        {
            JsonArray tasks = (JsonArray)taskData["daily_tasks"];
            AddElements((JsonArray)taskData["elements"]);
            foreach (JsonObject data in tasks)
            {
                int id = int.Parse(data["daily_id"].ToString());
                int status = int.Parse(data["status"].ToString());
                for (int i = 0; i < UserDailyTasks.Count; i++)
                {
                    if (UserDailyTasks[i].UserTaskId == id)
                    {
                        if (status == 1)
                        {
                            UserDailyTasks.Remove(UserDailyTasks[i]);
                        }
                        else
                        {
                            UserDailyTasks[i] = new UserTask(data);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 根据服务端给的type判断类型并处理
    /// </summary>
    public void AddElements(JsonArray elements)
    {
        foreach (JsonObject data in elements)
        {
            int type = int.Parse(data["type"].ToString());
            switch(type)
            {
                case 0: break;
                case 1:
                    {
                        string id = data["id"].ToString();
                        int count = int.Parse(data["count"].ToString());
                        if (id == "currency2")
                        {
                            Gold += count;
                        }
                        else if (id == "exp")
                        {
                            if(Level < ConfigManager.HeroConfig.GetHeroMaxLevel())
                            {
                                Exp += count;
                                while (Exp >= ConfigManager.HeroConfig.GetHeroByLvl(Level + 1).Exp)
                                {
                                    Level++;
                                }
                            }
                        }
                        else if (id == "currency1")
                        {
                            Diamond += count;
                        }
                        else if (id == "energy")
                        {
                            if ( Energy == EnergyLimit && Energy + count < EnergyLimit)
                            {
                                StartRecoveryTime = new DateTime(ConfigManager.LocalTime.LocalTime.Ticks);
                                RecoveryTag = true;
                            }
                            Energy += count;
                            if (Energy > ConfigManager.ParamConfig.GetParam().EnergyBuyRate)
                            {
                                Energy = (int)ConfigManager.ParamConfig.GetParam().EnergyBuyRate;
                            }
                            if (Energy >= EnergyLimit) RecoveryTag = false;
                        }
                        break;
                    }
                case 2:
                    {
                        UserItem newUI = new UserItem(data);
                        UserManager.CurUserInfo.UserItems.Add(newUI);
                        if(ConfigManager.ItemConfig.IsNew(newUI.CurItemData.Id))
                        {
                            ConfigManager.ItemConfig.SetNotNew(newUI.CurItemData.Id);
                        }
                        break;
                    }
                case 3: break;
                case 4:
                    {
                        UserWare newUW = new UserWare(data);
                        UserManager.CurUserInfo.UserWares.Add(newUW);
                        if(ConfigManager.HardWareConfig.IsNew(newUW.CurHardWareData.Id))
                        {
                            ConfigManager.HardWareConfig.SetNotNew(newUW.CurHardWareData.Id);
                        }
                        break;
                    }
                case 5:
                    {
                        UserPet newP = new UserPet(data);
                        UserManager.CurUserInfo.UserPets.Add(newP);
                        if(ConfigManager.PetConfig.IsNew(newP.CurPetData.Id))
                        {
                            ConfigManager.PetConfig.SetNotNew(newP.CurPetData.Id);
                        }
                        break;
                    }
                case 6: break;
                case 7: break;
                case 8: break;
                case 9: break;
                case 10: break;
                case 11: break;
                case 12: break;
                case 13: break;
                case 14: break;
                case 15: break;
                case 16: break;
                case 17: break;
                default: Debug.Log("No fit type!"); break;
            }
        }
    }

    /// <summary>
    /// 刷新助战列表
    /// </summary>
    public void RefreshHelpList(Action callback)
    {
        UserHelpers.Clear();
        SocketCenter.Request(GameRouteConfig.GetHelpFriends, null, (result) =>
        {
            if (result.Code == SocketResult.ResultCode.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
                    JsonArray friends = new JsonArray();
                    friends = (JsonArray)result.Data["friends"];
                    foreach (JsonObject data in friends)
                    {
                        HelpData h = new HelpData(data);
                        UserHelpers.Add(h);
                    }

                    callback();
                });
            }
        }, null, true, true);
    }

    /// <summary>
    /// 刷新好友列表
    /// </summary>
    public void RefreshFriendList(JsonArray list)
    {
        UserFriends.Clear();
        foreach (JsonObject fData in list)
        {
            FriendInfo userFriend = new FriendInfo(fData);
            UserFriends.Add(userFriend);
        }
    }

    /// <summary>
    /// 刷新好友请求
    /// </summary>
    public void RefreshRequestList(JsonArray list)
    {
        UserRequests.Clear();
        foreach (JsonObject rData in list)
        {
            RequestInfo userRequest = new RequestInfo(rData);
            UserRequests.Add(userRequest);
        }
    }

    /// <summary>
    /// 获得当前队伍
    /// </summary>
    public UserParty GetCurrentParty()
    {
        return UserPartys[CurPartyIndex];
    }

    public bool HasClearDungeon(string dungeonId)
    {
        foreach (JsonObject pass in PassDungeons)
        {
            //string tcid = d["chapter_id"].ToString();
            string tdid = pass["dungeon_id"].ToString();
            bool achieved = int.Parse(pass["achieve_state"].ToString()) == 2 ? true : false;
            if (tdid == dungeonId)
            {
                return true;
            }
        }
        return false;
    }

    //判断是否完成副本成就
    public bool HasAchievedDungeon(string dungeonId)
    {
        foreach (JsonObject pass in PassDungeons)
        {
            string tdid = pass["dungeon_id"].ToString();
            if (tdid == dungeonId)
            {
                bool achieved = int.Parse(pass["achieve_state"].ToString()) == 2 ? true : false;
                return achieved;
            }
        }
        return false;
    }

    /// <summary>
    /// 判断是否打完所有当前chapterId关卡。  jc 2014-09-22
    /// </summary>
    /// <param name="chapterId"></param>
    /// <returns></returns>
    public bool HasClearChapter(string chapterId)
    {
        List<DungeonData> dungons = ConfigManager.DungeonConfig.GetChapterDungeons(chapterId);
        int count = dungons.Count;
        foreach (JsonObject pass in PassDungeons)
        {
            string tcid = pass["chapter_id"].ToString();
            if (tcid == chapterId)
            {
                count--;
            }
        }
        if (count == 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 扩展宠物栏
    /// </summary>
    public void ExpandPetHouse(int count)
    {
        JsonObject args = new JsonObject();
        args.Add("count", count);
        SocketCenter.Request(GameRouteConfig.ExpandPetHouse, args, (r) =>
        {
        }, null, true,true);
    }

    /// <summary>
    /// 扩展装备栏
    /// </summary>
    public void ExpandWareHouse(int count)
    {
        JsonObject args = new JsonObject();
        args.Add("count", count);
        SocketCenter.Request(GameRouteConfig.ExpandWareHouse, args, (r) =>
        {
        }, null, true,true);
    }

    /// <summary>
    /// 编辑队伍
    /// </summary>
    public void MakeTeam(Action callback,int userPetId,int position,int hasOnPetId, int partyIndex)
    {

        UserPet curP = FindPetById(hasOnPetId);
        JsonObject args = new JsonObject();
        args.Add("house_id",userPetId);
        args.Add("map_index", position);
        args.Add("queue_id", partyIndex);
        SocketCenter.Request(GameRouteConfig.MakeTeam, args, (result) =>
        {

            if (result.Code == SocketResult.ResultCode.Success)
            {
                UserPet p1 = FindPetById(userPetId);

                if(hasOnPetId == userPetId)
                {
                    UserPartys[partyIndex].petUids.Remove(p1.UserPetId);
                }
                else if(hasOnPetId == -1)
                {
                    UserPartys[partyIndex].petUids.Add(p1.UserPetId);
                }
                else
                {
                    UserPartys[partyIndex].petUids[position] = p1.UserPetId;
                }
                callback();
            }
        }, null, true,true);
    }

    /// <summary>
    /// 设定当前队伍
    /// </summary>
    public void ChangeTeam(Action callback, int index)
    {
        CurPartyIndex = index;
        JsonObject args = new JsonObject();
        args.Add("queue_id", index);
        SocketCenter.Request(GameRouteConfig.ChangeTeam, args, (r) =>
        {
            if(r.Code == SocketResult.ResultCode.Success)
            {
                if(callback != null) callback();
            }
        }, null, true, true);
    }

    public UserPet FindPetById(int Id)
    {
        foreach (UserPet up in UserPets)
        {
            if (up.UserPetId == Id)
            {
                return up;
            }
        }
        return null;
    }

    public UserWare FindUserWare(int Id)
    {
        foreach (UserWare uw in UserWares)
        {
            if (uw.UserWareId == Id)
            {
                return uw;
            }
        }
        return null;
    }

    public UserItem FindItemById(int Id)
    {
        foreach (UserItem ui in UserItems)
        {
            if (ui.UserItemId == Id)
            {
                return ui;
            }
        }
        return null;
    }

    public UserTask FindUserTask(int Id)
    {
        foreach(UserTask ut in UserDailyTasks)
        {
            if(ut.UserTaskId == Id)
            {
                return ut;
            }
        }
        return null;
    }

    /// <summary>
    /// 出售怪物
    /// </summary>
    public void SalePets(Action callback,List<UserPet> pets)
    {
        JsonObject args = new JsonObject();
        List<int> petIds = new List<int>();
        foreach (UserPet up in pets)
        {
            petIds.Add(up.UserPetId);
        }
        args.Add("house_ids", petIds);
        SocketCenter.Request(GameRouteConfig.SalePet, args, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                foreach (UserPet salePet in pets)
                {
                    for (int i = UserPets.Count - 1; i >= 0; i++)
                    {
                        UserPet up = UserPets[i];
                        if (salePet.UserPetId == up.UserPetId)
                        {
                            UserPets.RemoveAt(i); break;
                        }
                    }
                }
                callback();
            }
        }, null, true,true);
    }
}

public class UserManager
{
    public static bool AppStartTag = false;

    static public PveUserInfo pveUserInfo = null;//new skill use

    public static bool StartRecovery = false;

    static private UserInfo curUserInfo = null;
    public static UserInfo CurUserInfo
    {
        get 
        {
            if (UserManager.curUserInfo == null)
            {
                UserManager.curUserInfo = new UserInfo(null);
            }
            return UserManager.curUserInfo;
        }
        set { UserManager.curUserInfo = value; }
    }


    /// <summary>
    /// 当前View
    /// </summary>
    private static ViewControl.Views curView = ViewControl.Views.None;

    public static ViewControl.Views CurView
    {
        get
        {
            if (curView == ViewControl.Views.None)
            {
                UserManager.curView = ViewControl.Views.Monster;
            }
            return UserManager.curView;
        }
        set { UserManager.curView = value; }
    }

    public static void UserInit(Action callback)
    {
        JsonObject args = new JsonObject();
        args.Add("open_id", AppMember.MemberId);
        args.Add("credential", AppMember.Cre);
        args.Add("user_source", AppMember.AccountType.Normal);
        SocketCenter.Request(GameRouteConfig.PlayerInit, args, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
                    ConfigManager.LocalTime = new TimeConfig(r.Date);
                    CurUserInfo = new UserInfo(r.Data);

					UserManager.curUserInfo.ArenaSeasonTime = -1000;
					UserManager.curUserInfo.ArenaLastTime = -1000;

                    UserManager.pveUserInfo = new PveUserInfo(r.Data);

                    JsonObject data = (JsonObject)r.Data["scene"];
                    if (data!=null)
                    {
                        UserManager.curUserInfo.NotEndDungeon = (JsonObject)data["scene_context"];
                    }
                    AppStartTag = true;
                    callback();
                });
            }
        },null,true,false);
    }
}
