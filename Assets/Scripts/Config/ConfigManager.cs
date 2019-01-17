using UnityEngine;
using System.Collections;
using SimpleJson;
using System;
using PomeloSocketCenter.PomeloLib;
public class ConfigManager 
{
    /// <summary>
    /// 客户端与服务器同步的时间
    /// </summary>
    public static TimeConfig LocalTime;

    /// <summary>
    /// 错误码配置
    /// </summary>
    private static ErrorConfig errorConfig;

    public static ErrorConfig ErrorConfig
    {
        get
        {
            if (ConfigManager.errorConfig == null)
            {
                ConfigManager.errorConfig = new ErrorConfig();
            }
            return ConfigManager.errorConfig;
        }
        set { ConfigManager.errorConfig = value; }
    }

    /// <summary>
    /// 角色配置
    /// </summary>
    private static CharacterConfig characterConfig;

    public static CharacterConfig CharacterConfig
    {
        get 
        {
            if (ConfigManager.characterConfig == null)
            {
                ConfigManager.characterConfig = new CharacterConfig();
            }
            return ConfigManager.characterConfig; 
        }
        set { ConfigManager.characterConfig = value; }
    }

    /// <summary>
    /// 障碍物配置
    /// </summary>
    private static BarrierConfig barrierConfig;

    public static BarrierConfig BarrierConfig
    {
        get 
        {
            if (ConfigManager.barrierConfig == null)
            {
                ConfigManager.barrierConfig = new BarrierConfig();
            }
            return ConfigManager.barrierConfig; 
        }
        set { ConfigManager.barrierConfig = value; }
    }

    /// <summary>
    /// 怪物配置
    /// </summary>
    private static MonsterConfig monsterConfig;

    public static MonsterConfig MonsterConfig
    {
        get 
        {
            if (ConfigManager.monsterConfig == null)
            {
                ConfigManager.monsterConfig = new MonsterConfig();
            }
            return ConfigManager.monsterConfig; 
        }
        set { ConfigManager.monsterConfig = value; }
    }

    /// <summary>
    /// 副本配置
    /// </summary>
    private static DungeonConfig dungeonConfig;
    public static DungeonConfig DungeonConfig
    {
        get 
        {
            if (ConfigManager.dungeonConfig == null)
            {
                ConfigManager.dungeonConfig = new DungeonConfig();
            }
            return ConfigManager.dungeonConfig; 
        }
        set { ConfigManager.dungeonConfig = value; }
    }

    /// <summary>
    /// 地图配置
    /// </summary>
    private static MapConfig mapConfig;

    public static MapConfig MapConfig
    {
        get 
        { 
            if(ConfigManager.mapConfig == null)
            {
                ConfigManager.mapConfig = new MapConfig();
            }
            return ConfigManager.mapConfig; 
        }
        set { ConfigManager.mapConfig = value; }
    }

    /// <summary>
    /// 皮肤配置
    /// </summary>
    private static SkinConfig skinConfig;

    public static SkinConfig SkinConfig
    {
        get 
        {
            if (ConfigManager.skinConfig == null)
            {
                ConfigManager.skinConfig = new SkinConfig();
            }
            return ConfigManager.skinConfig;
        }
        set { ConfigManager.skinConfig = value; }
    }

    /// <summary>
    /// 道具配置
    /// </summary>
    private static ItemConfig itemConfig;

    public static ItemConfig ItemConfig
    {
        get { 
            if(ConfigManager.itemConfig== null)
            {
                ConfigManager.itemConfig = new ItemConfig();
            }
            return ConfigManager.itemConfig;
        }
        set { ConfigManager.itemConfig = value; }
    }


    private static TrapConfig trapConfig;

    public static TrapConfig TrapConfig
    {
        get {
            if( ConfigManager.trapConfig==null)
            {
                ConfigManager.trapConfig = new TrapConfig();
            }
            return ConfigManager.trapConfig;
        }
        set { ConfigManager.trapConfig = value; }
    }

    /// <summary>
    /// BOSS配置
    /// </summary>
    private static BossConfig bossConfig;
    public static BossConfig BossConfig
    {
        get {
            if (ConfigManager.bossConfig == null)
            {
                ConfigManager.bossConfig = new BossConfig();
            }
            return ConfigManager.bossConfig; 
        }
        set { ConfigManager.bossConfig = value; }
    }

    /// <summary>
    /// 技能配置
    /// </summary>
    private static SkillConfig skillConfig;

    public static SkillConfig SkillConfig
    {
        get {
            if(ConfigManager.skillConfig == null)
            {
                ConfigManager.skillConfig = new SkillConfig();
            }
            return ConfigManager.skillConfig; 
        }
        set { ConfigManager.skillConfig = value; }
    }

    /// <summary>
    /// 宠物配置
    /// </summary>
    private static PetConfig petConfig;

    public static PetConfig PetConfig
    {
        get {
            if (ConfigManager.petConfig == null)
            {
                ConfigManager.petConfig = new PetConfig();
            }
            return ConfigManager.petConfig; 
        }
        set { ConfigManager.petConfig = value; }
    }

    /// <summary>
    /// 章节配置
    /// </summary>
    private static ChapterConfig chapterConfig;

    public static ChapterConfig ChapterConfig
    {
        get 
        {
            if (ConfigManager.chapterConfig == null)
            {
                ConfigManager.chapterConfig = new ChapterConfig();
            }
            return ConfigManager.chapterConfig;
        }
        set { chapterConfig = value; }
    }

    /// <summary>
    /// 英雄配置
    /// </summary>
    private static HeroConfig heroConfig;

    public static HeroConfig HeroConfig
    {
        get {
            if (ConfigManager.heroConfig == null)
            {
                ConfigManager.heroConfig = new HeroConfig();
            }
            return ConfigManager.heroConfig; 
        }
        set { ConfigManager.heroConfig = value; }
    }

    private static ParamConfig paramConfig;

    public static ParamConfig ParamConfig
    {
        get
        {
            if (ConfigManager.paramConfig == null)
            {
                ConfigManager.paramConfig = new ParamConfig();
            }
            return ConfigManager.paramConfig;
        }
        set { ConfigManager.paramConfig = value; }
    }

    /// <summary>
    /// 装备配置
    /// </summary>
    private static HardWareConfig hardWareConfig;

    public static HardWareConfig HardWareConfig
    {
        get
        {
            if (ConfigManager.hardWareConfig == null)
            {
                ConfigManager.hardWareConfig = new HardWareConfig();
            }
            return ConfigManager.hardWareConfig;
        }
        set { ConfigManager.hardWareConfig = value; }
    }

    /// <summary>
    /// 装备合成素材配置
    /// </summary>
    private static HardwareMaterialConfig hardwareMaterialConfig;

    public static HardwareMaterialConfig HardwareMaterialConfig
    {
        get
        {
            if (ConfigManager.hardwareMaterialConfig == null)
            {
                ConfigManager.hardwareMaterialConfig = new HardwareMaterialConfig();
            }
            return ConfigManager.hardwareMaterialConfig;
        }
        set { ConfigManager.hardwareMaterialConfig = value; }
    }

    /// <summary>
    /// 装备等级配置
    /// </summary>
    private static HardwareLevelConfig hardwareLevelConfig;

    public static HardwareLevelConfig HardwareLevelConfig
    {
        get
        {
            if (ConfigManager.hardwareLevelConfig == null)
            {
                ConfigManager.hardwareLevelConfig = new HardwareLevelConfig();
            }
            return ConfigManager.hardwareLevelConfig;
        }
        set
        {
            ConfigManager.hardwareLevelConfig = value;
        }
    }
    /// <summary>
    /// 新手引导内的消除块配置
    /// </summary>
    private static EliminateConfig eliminateConfig;

    public static EliminateConfig EliminateConfig
    {
        get
        {
            if (ConfigManager.eliminateConfig == null)
            {
                ConfigManager.eliminateConfig = new EliminateConfig();
            }
            return ConfigManager.eliminateConfig;
        }
        set { ConfigManager.eliminateConfig = value; }
    }
    /// <summary>
    /// 怪物等级配置
    /// </summary>
    private static PetLevelConfig petLevelConfig;

    public static PetLevelConfig PetLevelConfig
    {
        get
        {
            if (ConfigManager.petLevelConfig == null)
            {
                ConfigManager.petLevelConfig = new PetLevelConfig();
            }
            return ConfigManager.petLevelConfig;
        }
        set { ConfigManager.petLevelConfig = value; }
    }

    private static TipsConfig tipsConfig;

    /// <summary>
    /// 提示信息配置
    /// </summary>
    public static TipsConfig TipsConfig
    {
        get
        {
            if(ConfigManager.tipsConfig == null)
            {
                ConfigManager.tipsConfig = new TipsConfig();
            }
            return ConfigManager.tipsConfig;
        }
        set { ConfigManager.tipsConfig = value; }
    }

    /// <summary>
    /// 每日任务配置
    /// </summary>
    private static MissionConfig missionConfig;

    public static MissionConfig MissionConfig
    {
        get 
        {
            if(ConfigManager.missionConfig == null)
            {
                ConfigManager.missionConfig = new MissionConfig();
            }
            return ConfigManager.missionConfig;
        }
        set { ConfigManager.missionConfig = value; }
    }

    /// <summary>
    /// 新手引导
    /// </summary>
    /// 
    private static GuiderConfig guiderConfig;

    public static GuiderConfig GuiderConfig
    {
        get
        {
            if (ConfigManager.guiderConfig == null)
            {
                ConfigManager.guiderConfig = new GuiderConfig();
            }
            return ConfigManager.guiderConfig;
        }
        set { ConfigManager.guiderConfig = value; }
    }

	
	
	/// <summary>
	/// 竞技场配置表
	/// </summary>
	private static ArenaConfig arenaConfig;
	public static ArenaConfig ArenaConfig
	{
		get 
		{
			if (ConfigManager.arenaConfig == null)
			{
				ConfigManager.arenaConfig = new ArenaConfig();
			}
			return ConfigManager.arenaConfig; 
		}
		set { ConfigManager.arenaConfig = value; }
	}

	private static EggConfig eggConfig;
	public static EggConfig EggConfig
	{
		get 
		{
			if (ConfigManager.eggConfig == null)
			{
				ConfigManager.eggConfig = new EggConfig();
			}
			return ConfigManager.eggConfig; 
		}
		set { ConfigManager.eggConfig = value; }
	}

	/// <summary>
	/// Pve 表情
	/// </summary>
	private static PveFaceConfig pveFaceConfig;
	public static PveFaceConfig PveFaceConfig
	{
		get 
		{
			if (ConfigManager.pveFaceConfig == null)
			{
				ConfigManager.pveFaceConfig = new PveFaceConfig();
			}
			return ConfigManager.pveFaceConfig; 
		}
		set { ConfigManager.pveFaceConfig = value; }
	}

    /// <summary>
    /// 随机种子文件
    /// </summary>
    private static RandomDataConfig randomDataConfig;

    public static RandomDataConfig RandomDataConfig
    {
        get
        {
            if (ConfigManager.randomDataConfig == null)
            {
                ConfigManager.randomDataConfig = new RandomDataConfig();
            }
            return ConfigManager.randomDataConfig;
        }
        set { ConfigManager.randomDataConfig = value; }
    }


	private static int CurVersion=0;
    /// <summary>
    /// 更新配置表
    /// </summary>
    public static void RefreshConfig(Action<bool> callback, Action<int,int> configProgress=null)
    {
        JsonObject args = new JsonObject();
        Loom.QueueOnMainThread(() =>
              {
                  
			      ConfigManager.CurVersion=0;
                  //PlayerPrefs.DeleteKey("version");
                  //if(!PlayerPrefs.HasKey("version"))
                       PlayerPrefs.SetInt("version", 0);
               
                  args.Add("version", PlayerPrefs.GetInt("version"));
                  args.Add("language", "chs");
                  
                 HttpHelper.GETRequest("http://" + LoginControl.LoginHost + "/json?version=" + PlayerPrefs.GetInt("version"),(hResult) =>
                 {
               
                    if (!hResult.HasError())
                    {
                        Loom.QueueOnMainThread(() =>
                        {
                            Debug.Log("----- " + hResult.ResponseString());
                            JsonArray configInfolist = (JsonArray)SimpleJson.SimpleJson.DeserializeObject(hResult.ResponseString());
                            int count = configInfolist.Count;

                          if (count == 0)
                          {
                              //无配置更新
                              callback(true);
                          }
                          else
                          {
                              foreach (JsonObject configUrl in configInfolist)
                              {
                                  string url = configUrl["url"].ToString();
                                  string name = configUrl["name"].ToString();
                                  int version = int.Parse(configUrl["version"].ToString());

                                  if (ConfigManager.CurVersion < version)
                                  {
                                      ConfigManager.CurVersion = version;
                                  }

                                  HttpHelper.GETRequest(url, (hr) =>
                                  {
                                      if (hr.HasError() == false)
                                      {
                                          FileHelper.StoreBytesToLocal(JsonDbHelper.GetRealJsonDbPath(name), hr.ResponseBytes());
                                          count--;
                                          Debug.Log("config:   " + name + "   " + version + "    " + count);
                                          configProgress(configInfolist.Count - count, configInfolist.Count);

                                      }
                                      if (count == 0)
                                      {
                                          //配置更新完成
                                          if (PlayerPrefs.GetInt("version") < ConfigManager.CurVersion)
                                          {
                                              PlayerPrefs.SetInt("version", ConfigManager.CurVersion);
                                          }
                                          callback(true);

                                      }
                                  });

                              }
                          }                        

                           });
                      }
                      else
                      {
                          callback(false);
                      }
                  });
              });
    }
    
}
