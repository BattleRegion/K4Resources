using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class PvpGameControl : MonoBehaviour
{
	private string message100 = "100";
	private string message101 = "101";
	private string message102 = "102";
	private string message103 = "103";
	private string message104 = "104";
	private string message105 = "105";
	private string message106 = "106";
	private string message107 = "107";
	private string message108 = "108";
	private string message109 = "109";

    #region 属性
    /// <summary>
    /// 触摸类型
    /// </summary>
    public enum TouchType
    {
        Enter = 1,
        Hold = 2,
        Out = 3
    }
    /// <summary>
    /// 技能范围角度
    /// </summary>
    public enum BlockConner
    {
        LeftUp = 1,
        LeftDown = 2,
        RightUp = 3,
        RightDown = 4
    }

    /// <summary>
    /// 技能范围直线方向
    /// </summary>
    public enum LineDirection
    {
        horizon = 1,
        vertical = 2
    }

    /// <summary>
    /// 转角方向
    /// </summary>
    public enum ConnerDirection
    {
        LeftUp = 1,
        LeftDown = 2,
        RightUp = 3,
        RightDown = 4
	}

	// 战斗结果
	public enum PvpFightResult
	{
		DEFAULT = -1,
		SUCCESS = 0,
		FAILED = 1,
		TIE = 2
	}

	public enum PvpFightStep
	{
		// 回合开始
		ROUND_BEGIN = 1,
		// 动作
		ACTION = 2,
		// 技能
		MAGIC = 3,
		// 回合结束
		ROUND_END = 4
	}

	/// <summary>
	/// 最大回合数
	/// </summary>
	private int MAX_ROUND = 8;

	/// <summary>
	/// 格子列数
	/// </summary>
	public int GameWidth = 7;

	/// <summary>
	/// 格子行数
	/// </summary>
	public int GameHeight = 9;

    public SpriteRenderer BackGroundSprite;
    public SpriteRenderer BottomSprite;

	/// <summary>
	/// 格子容器
	/// </summary>
	public GameObject TilesArea;
	/// <summary>
	/// 消除块容器
	/// </summary>
	public GameObject EliminateArea;
	/// <summary>
	/// 角色容器
	/// </summary>
	public GameObject FightUnitArea;

	/// <summary>
	/// 格子列表
	/// </summary>
	[HideInInspector]
    public List<PvpTile> AllPvpTiles = new List<PvpTile>();
	/// <summary>
	/// 消除块列表
	/// </summary>
	[HideInInspector]
    public List<PvpEliminate> AllEliminates = new List<PvpEliminate>();
	/// <summary>
	/// 宠物容器
	/// </summary>
	[HideInInspector]
	public List<List<PvpOwnUnit>> AllOwns = new List<List<PvpOwnUnit>> ();
	/// <summary>
	/// 所有障碍容器（角色、怪物）
	/// </summary>
	[HideInInspector]
	public List<PvpFightUnit> AllBarrier = new List<PvpFightUnit> ();

    public static string CurDungeonId;
	public static string CurSceneId;

    /// <summary>
    /// 自己
    /// </summary>
	[HideInInspector]
    public PvpCharacter PvpCharacterSelf;
	/// <summary>
	/// 敌人
	/// </summary>
	[HideInInspector]
	public PvpCharacter PvpCharacterEnemy;
	/// <summary>
	/// 当前消除格子
	/// </summary>
	[HideInInspector]
	public PvpEliminate CurCharacterEliminate;

	public bool UserInputLock = true;
	/// <summary>
	/// 双方数据 UI
	/// </summary>
    [HideInInspector]
	public PvpPlayerInfo PvpPlayerInfo;
	/// <summary>
	/// 自己数据
	/// </summary>
	[HideInInspector]
	public PvpUserInfo PvpUserInfoSelf;
	/// <summary>
	/// 敌人数据
	/// </summary>
	[HideInInspector]
	public PvpUserInfo PvpUserInfoEnemy;

    public GameObject FloorChange;

    public Confirm SkillConfirm;

    public GameObject SkillBack;

	public SkillFlyItem SkillFlyObject;

	public PvpFeelItemList PvpFeelItemObject;
	
	public UIWidget SelfAvatarCollider;
	public UIWidget EnemyAvatarCollider;
	public PvpRoundAction RoundAction;

    #endregion

    #region 资源指针
	/// <summary>
	/// 格子资源
	/// </summary>
    public GameObject PvpTileResource;

	/// <summary>
	/// 消除格子资源
	/// </summary>
	public GameObject PvpEliminateResource;

	/// <summary>
	/// 怪物资源
	/// </summary>
	public GameObject PvpMonsterResource;

	/// <summary>
	/// 角色资源
	/// </summary>
    public GameObject CharacterResource;

	/// <summary>
	/// 宠物资源
	/// </summary>
    public GameObject PetResource;

	/// <summary>
	/// 蛋资源
	/// </summary>
	public GameObject EggResource;

    public GameObject Line;

    public GameObject Conner;

	public GameObject FightUnitEffect;

	public GameObject PvpSurrender;
	public GameObject PvpDisconnect;

	public GameObject PowerObject;

	public PvpRoundSignItem RoundSignItem;

	public InfoLabel RoundTimerLabel;

	public PvpPlayerDetail EnemyPlayerDetail;
	public PvpPetDetail EnemyPetDetail;

	/// <summary>
	/// Pvp 数据
	/// </summary>
    static public JsonObject CurPvpData;

    public int CurFloor = 1;

	/// <summary>
	/// 当前操作的角色
	/// </summary>
	public PvpCharacter CurrentCharacter;

	/// <summary>
	/// 消息队列
	/// </summary>
	protected List<JsonObject> messageQueue;

	/// <summary>
	/// A 星寻路
	/// </summary>
	public AStarUtils aStarUtils;

    #endregion
	/// <summary>
	/// 初始化移动索引
	/// </summary>
	private int MoveInitIndex = 0;

	/// <summary>
	/// Pvp 格子数据
	/// </summary>
	public string PvpColors;

	/// <summary>
	/// Pvp 回合
	/// </summary>
    public int PvpRounds;

	/// <summary>
	/// 自己回合数
	/// </summary>
	public int SelfRoundsCount;

	/// <summary>
	/// 敌人回合数
	/// </summary>
	public int EnemyRoundsCount;
	public int PvpTableID;

	/// <summary>
	/// Pvp 地图数据
	/// </summary>
    public JsonArray MapJsonData;

	/// <summary>
	/// Pvp 蛋数据
	/// </summary>
	public JsonArray EggJsonData;

	/// <summary>
	/// 消除格子初始化状态
	/// </summary>
	public bool EliminateFirstStatus = false;

	/// <summary>
	/// 当前行走的 格子属性
	/// </summary>
	public DungeonEnum.ElementAttributes currentEliminateAttribute;

	/// <summary>
	/// 谁先出手
	/// </summary>
	private bool SelfStatus = true;

	/// <summary>
	/// 自己冷却时间
	/// </summary>
	private bool SelfCoolTimeStatus = false;

	/// <summary>
	/// 敌人冷却时间
	/// </summary>
	private bool EnemyCoolTimeStatus = false;

	/// <summary>
	/// 步骤数据
	/// </summary>
	private JsonArray stepDataList;

	/// <summary>
	/// 回合剩余时间
	/// </summary>
	private int RoundTimer;

	/// <summary>
	/// 怪物行走数据
	/// </summary>
	private JsonObject monsterDataList;
	private List<PvpEliminateData> pvpEliminateDataList;
	public PvpFightResult fightResult; // 战斗结果，延迟显示

	/// <summary>
	/// 初始化舞台状态
	/// </summary>
	private bool initStatus = false;
	private bool initUserInputLockStatus = false;

	/// <summary>
	/// 断线重连状态
	/// </summary>
	private bool reStopStatus = false;

	/// <summary>
	/// 危险提示
	/// </summary>
	private GameObject singalItem;

	public bool messageFightEndStatus = false;

	/// <summary>
	/// 消息处理回调
	/// </summary>
	public Action messageCallback_102;
	public Action messageCallback_103;

	public static bool PvpStatus = false;
	public static bool DeadStatus = false;

	private bool messageEndStatus = false;
	private bool fightPanelStatus = false;

    #region  重写MONO
    void Start()
	{
		this.messageEndStatus = false;
		this.fightPanelStatus = false;

		PvpGameControl.PvpStatus = true;
		PvpGameControl.DeadStatus = false;
		// 设置战斗结束消息为假
		this.messageFightEndStatus = false;
		// 设置消息列表初始化
		PvpMessageManager.gameControl = this;
		// 设置双方面板不显示
		this.PvpPlayerInfo.ChangeActiveStatus (false, true);
		// 初始化 A *
		this.aStarUtils = new AStarUtils (GameWidth, GameHeight);
		// 重置战斗结果
		this.fightResult = PvpFightResult.DEFAULT;
		// 重置消息队列
		this.messageQueue = new List<JsonObject> ();
		// 重置初始化状态
		this.initStatus = false;
		// 重置战斗结果显示状态
		this.resultRenderStatus = false;
		// 清空任务数据
		PvpOverUI.taskData = null;

		// 设置划线锁定
		this.UserInputLock = true;

		UIEventListener.Get(this.SelfAvatarCollider.gameObject).onClick = (g)=>
		{
			// 初始化表情
			this.PvpFeelItemObject.Show();
		};

		UIEventListener.Get(this.EnemyAvatarCollider.gameObject).onClick = (g)=>
		{
			if(this.EnemyPlayerDetail != null)
			{
				// 设置敌人信息
				this.EnemyPlayerDetail.SetPlayerInfo(this.PvpUserInfoEnemy, this);
			}
		};

		this.CreateGamePool (() =>
		{
			PvpConnect.PvpPushHandler = InitPvpPushHandler;
			PvpConnect.EntryWaiting(null, true);
			PvpConnect.CurDicCallback = (result) =>
			{
				Loom.QueueOnMainThread(() =>
				{
					if(result["route"].ToString() == GameRouteConfig.PvpEnterWaiting)
					{
						if(this.PvpDisconnect != null) this.PvpDisconnect.SetActive(true);
					}
				});
			};
		});
    }

	/// <summary>
	/// 投降关闭
	/// </summary>
	public void PvpSurrenderClose()
	{
		this.PvpSurrender.SetActive (false);
	}

	/// <summary>
	/// 投降确定
	/// </summary>
	public void PvpSurrenderSubmit()
	{
		this.PvpSurrender.SetActive (false);

		if(this.messageFightEndStatus) return;

		// 提交投降
		PvpConnect.Surrender (this.PvpTableID);
	}

	public void PvpDisconnectSubmit()
	{
		Application.LoadLevel ("test");
		//Application.LoadLevel(0); //强制登出
		//Application.LoadLevel ("test");
	}

	/// <summary>
	/// 发送表情
	/// </summary>
	/// <param name="faceID">Face I.</param>
	public void PvpFaceSubmit(string faceID)
	{
		// 如果场景没有初始化完成，不能发送表情
		if(!this.initStatus) return;

		// 自己显示表情
		this.PvpCharacterSelf.ShowFace (faceID);

		// 隐藏表情
		this.PvpFeelItemObject.Close ();

		// 提交表情
		PvpConnect.Face (this.PvpTableID, faceID);
	}

	/// <summary>
	/// 显示行走能量特效
	/// </summary>
	/// <param name="pvpFightUnit">Pvp fight unit.</param>
	public void ShowPowerEffect(PvpFightUnit pvpFightUnit)
	{
		// 如果是自己
		if(pvpFightUnit.UserType == this.PvpCharacterSelf.UserType)
		{
			// 设置能量效果
			PowerFlyTool.PowerEffect(pvpFightUnit.transform.localPosition, this.PowerObject, PvpUserInfoSelf.Pvp_SkillPower, PvpUserInfoSelf.CurPower);
		}
	}

	public void ShowTimerData(bool showStatus, int time)
	{
		if(this.RoundTimerLabel != null)
		{
			if(time > 0)
			{
				this.RoundTimerLabel.SetNum(time.ToString());	
				AnimationHelper.AnimationScaleTo(new Vector3(5f, 5f, 5f), this.RoundTimerLabel.gameObject, iTween.EaseType.linear, this.gameObject, "RoundTimerScaleEndCallback", 0.3f);
			}
			this.RoundTimerLabel.gameObject.SetActive(showStatus);
		}
	}

	private void RoundTimerScaleEndCallback()
	{
		AnimationHelper.AnimationScaleTo(new Vector3(4f, 4f, 4f), this.RoundTimerLabel.gameObject, iTween.EaseType.linear, null, null, 0.3f);
	}

	private void InitPvpPushHandler(JsonObject jsonData)
	{
		Loom.QueueOnMainThread(() =>
		{
			ApplicationControl.CurApp.StopPvpLoading ();

			string code = jsonData["code"].ToString();
			if(code == this.message104 || code == this.message101)
			{
				PvpConnect.PvpPushHandler = PvpPushHandler;
				this.StartCoroutine(this.SceneInitCallback(jsonData, 0f));
			}
		});
	}

	private IEnumerator SceneInitCallback(JsonObject jsonData, float delayTime)
	{
		yield return new WaitForSeconds (delayTime);

		// 设置双方面板显示
		this.PvpPlayerInfo.ChangeActiveStatus (true);
		
		string code = jsonData["code"].ToString();
		if(code == this.message104 || code == this.message101)
		{
			this.reStopStatus = false;
			if(code == this.message104) this.reStopStatus = true;
			
			this.EliminateFirstStatus = true;
			
			CurPvpData = jsonData ["data"] as JsonObject;
			
			this.MoveInitIndex = 0;
			
			this.PvpTableID = int.Parse(CurPvpData["table_id"].ToString());
			this.PvpColors = CurPvpData["colors"].ToString();
			//暂时不处理
			this.PvpRounds = int.Parse(CurPvpData["round"].ToString());
			
			this.MapJsonData = CurPvpData["maps"] as JsonArray;
			if(CurPvpData.ContainsKey("drop_eggs"))
			{
				this.EggJsonData = CurPvpData["drop_eggs"] as JsonArray;
			}
			this.RoundTimer = int.Parse(CurPvpData["timer"].ToString());
			
			PvpGameControl.CurDungeonId = CurPvpData ["dungeon_id"].ToString ();
			PvpGameControl.CurSceneId = CurPvpData["scene_id"].ToString();
			
			JsonArray userDataList = CurPvpData ["players"] as JsonArray;
			
			PvpUserInfo PvpUserInfoTop = null; 
			PvpUserInfo PvpUserInfoBottom = null; 
			
			if(code == this.message104)
			{
				PvpUserInfoTop = new PvpUserInfo (userDataList[1] as JsonObject, false);
				PvpUserInfoBottom = new PvpUserInfo (userDataList [0] as JsonObject, false);
			}
			else if(code == this.message101)
			{
				PvpUserInfoTop = new PvpUserInfo (userDataList[1] as JsonObject, true);
				PvpUserInfoBottom = new PvpUserInfo (userDataList [0] as JsonObject, true);
			}
			
			// 技能初始化
			SkillManager.SkillInit(PvpUserInfoTop, PvpUserInfoTop.UserSkillList, PvpUserInfoTop.UserSkillCdList);
			SkillManager.SkillInit(PvpUserInfoBottom, PvpUserInfoBottom.UserSkillList, PvpUserInfoBottom.UserSkillCdList);
			// 效果初始化
			SkillManager.BuffListInit(PvpUserInfoTop);
			SkillManager.BuffListInit(PvpUserInfoBottom);
			
			// 如果下面那个先手
			if(CurPvpData["current_index"].ToString() == "0")
			{
				// 如果下面那个角色 ID 与 当前 ID 不同，锁定
				if(PvpUserInfoBottom.UserId == UserManager.CurUserInfo.UserId)
				{
					this.SelfStatus = true;
					
					this.PvpUserInfoSelf = PvpUserInfoBottom;
					this.PvpUserInfoEnemy = PvpUserInfoTop;
					
					this.initUserInputLockStatus = false;
				}else{
					this.SelfStatus = false;
					
					this.PvpUserInfoSelf = PvpUserInfoTop;
					this.PvpUserInfoEnemy = PvpUserInfoBottom;
					
					this.initUserInputLockStatus = true;
				}
			}else
			{
				// 如果上面那个角色 ID 与 当前 ID 不同，锁定
				if(PvpUserInfoTop.UserId == UserManager.CurUserInfo.UserId)
				{
					this.SelfStatus = true;
					
					this.PvpUserInfoSelf = PvpUserInfoTop;
					this.PvpUserInfoEnemy = PvpUserInfoBottom;
					
					this.initUserInputLockStatus = false;
				}else{
					this.SelfStatus = false;
					
					this.PvpUserInfoSelf = PvpUserInfoBottom;
					this.PvpUserInfoEnemy = PvpUserInfoTop;
					
					this.initUserInputLockStatus = true;
				}
			}

			// 如果自己有延迟回合
			if(this.PvpUserInfoSelf.Pvp_DelayTime > 0)
			{
				this.SelfCoolTimeStatus = true;
			}else{
				this.SelfCoolTimeStatus = false;
			}
			// 如果敌人有延迟回合
			if(this.PvpUserInfoEnemy.Pvp_DelayTime > 0)
			{
				this.EnemyCoolTimeStatus = true;
			}else{
				this.EnemyCoolTimeStatus = false;
			}
			
			this.PvpRoundProgress(PvpUserInfoBottom.UserId);

			/// 渲染场景数据
			this.CreateSceneStyle(()=>
			                      {
				PvpGameObjectManager.Create("PreFabs/FX/Cloud", (GameObject pvpEnterObject)=>
				                            {
					pvpEnterObject.layer = LayerHelper.UI;
				});
				
				// 初始化角色数据，回调结束之后创建宠物数据
				this.InitCharacter(()=>
				                   {
					// 初始化宠物
					this.InitCharacterPets(()=>
					                       {
						this.CharacterMoveIn();
					});
				});
			});
		}else
		{
			if(!this.initStatus)
			{
				this.messageQueue.Add(jsonData);
			}
		}
	}
	
	/// <summary>
	/// 游戏回合处理
	/// </summary>
	/// <param name="userInfo">User info.</param>
	public void PvpRoundProgress(int userID)
	{
		if(this.PvpRounds > 0)
		{
			// 如果是自己先出手的
			if(userID == UserManager.CurUserInfo.UserId)
			{
				// 如果能被 2 整除
				if(this.PvpRounds % 2 == 0)
				{
					this.EnemyRoundsCount = (int)(this.PvpRounds * 0.5f);
					this.SelfRoundsCount = this.EnemyRoundsCount + 1;
				}else
				{
					this.SelfRoundsCount = this.EnemyRoundsCount = (int)(this.PvpRounds * 0.5f) + 1;
				}
			}else
			{
				// 如果能被 2 整除
				if(this.PvpRounds % 2 == 0)
				{
					this.SelfRoundsCount = (int)(this.PvpRounds * 0.5f);
					this.EnemyRoundsCount = this.SelfRoundsCount + 1;
				}else{
					this.EnemyRoundsCount = this.SelfRoundsCount = (int)(this.PvpRounds * 0.5f) + 1;
				}
			}
		}else{
			if(reStopStatus)
			{
				if(userID == UserManager.CurUserInfo.UserId)
				{
					this.SelfRoundsCount ++;
				}else{
					this.EnemyRoundsCount ++;
				}
			}
		}
	}

    /// <summary>
    /// 101 正常进入pvp 初始数据
    /// 104 pvp断线重连 初始数据
    /// 102 对方行走
    /// 103 倒计时回合开始
    /// 105 血值同步 及 战斗状态判断
    /// 106 对方技能使用
    /// </summary>

	private JsonArray xxObject;

	private void PvpPushHandler(JsonObject jsonData)
	{
		ApplicationControl.CurApp.StopLoading();
		Loom.QueueOnMainThread (() =>
		{
			if(!this.initStatus)
			{
				this.messageQueue.Add(jsonData);
				return;
			}

			string code = jsonData ["code"].ToString ();
			JsonObject pvpData = jsonData ["data"] as JsonObject;

			if (code == this.message102) 
			{
				// 添加到消息队列
				PvpMessageManager.InsertMessage(new PvpMessage102(this, pvpData));
			}else if(code == this.message107)
			{
				// 设置消息状态为真
				this.messageFightEndStatus = true;
				PvpMessageManager.InsertMessage(new PvpMessage107(this, pvpData));
			}else if(code == this.message103)
			{
				// 添加到消息队列
				PvpMessageManager.InsertMessage(new PvpMessage103(this, pvpData));
			}else if(code == this.message105)
			{
				PvpMessageManager.InsertMessage(new PvpMessage105(this, pvpData));
			}
			else if(code == this.message106)
			{
				SkillData skillData = PvpCharacterEnemy.pvpPlayerSkill.GetSkillDataBySkillID(pvpData["skill_id"].ToString());
				if(skillData != null)
				{
					// 调用技能展示
					this.UseSkill(skillData, false);
				}
			}
			else if(code == this.message108)
			{
				//通知敌人显示表情
				if(this.PvpCharacterEnemy != null) this.PvpCharacterEnemy.ShowFace(pvpData["face_id"].ToString());
			}
			else if(code == this.message100)
			{
				this.xxObject = pvpData["maps"] as JsonArray;
			}else if(code == this.message109)
			{
				this.messageEndStatus = true;
				if(this.fightPanelStatus)
				{
					// 隐藏 loading 条
					ApplicationControl.CurApp.StopLoading();
					this.SendResultToServer(this.fightResultValue);
				}
			}else if(code == this.message104)
			{
				this.InitPvpPushHandler(jsonData);
			}
		});
    }

	private int GetCoolTime(PvpFightUnit pvpFightUnit)
	{
		ParamData paramData = ConfigManager.ParamConfig.GetParam ();
		// 显示回合
		return paramData.PVPRoundTime;
	}

    void Update()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            MobileCheckTouch();
        }
        else
        {
            CheckTouch();
        }
    }

	/// <summary>
	/// 显示冷却时间
	/// </summary>
	private void StartCoolingTimer(int CoolingTime, bool self, int roundTime = -1)
	{
		this.PvpPlayerInfo.ChangeCoolingTime (CoolingTime, self, roundTime);
	}
	/// <summary>
	/// 停止冷却时间
	/// </summary>
	private void StopCoolingTimer()
	{
		if(this.PvpPlayerInfo != null) this.PvpPlayerInfo.StopCoolingTime ();
		if(this.RoundSignItem != null) this.RoundSignItem.Stop();
	}

    #endregion

    #region 设置场景风格

	private int poolIndex;
	private void CreateGamePool(Action callback)
	{
		this.poolIndex = 0;
		for (int i = 0; i < GameWidth; i++) 
		{
			for (int j = 0; j < GameHeight; j++) 
			{
				this.poolIndex ++;
				this.StartCoroutine(CreateGamePoolItem(callback));
			}
		}
	}

	private IEnumerator CreateGamePoolItem(Action callback)
	{
		yield return null;

		GameObject objectItem = new GameObject ();
		PvpGameObjectManager.Insert (objectItem);

		this.poolIndex --;

		if(this.poolIndex <= 0)
		{
			if(callback != null) callback();
		}
	}

    private void CreateSceneStyle(Action callback)
    {
        DungeonData dungeonData = ConfigManager.DungeonConfig.GetDungeonById(PvpGameControl.CurDungeonId);
		
		MAX_ROUND = dungeonData.Round;

        string basicPath = "OrginPic/FightBackGround/";
		basicPath = basicPath + PvpGameControl.CurSceneId + "/";
		string backGround = basicPath + "background";
		string tile = basicPath + "tile" + PvpGameControl.CurSceneId;

		Sprite[] backgrounds = Resources.LoadAll<Sprite>(backGround);
		foreach(Sprite s in backgrounds)
		{
			if(s.name == "background_up")
			{
				BackGroundSprite.sprite = s;
			}
			if(s.name == "background_down")
			{
				BottomSprite.sprite = s;
			}
		}

		// 创建 Tile
		this.CreateSceneTile (()=>
		{
			// 创建消除格子
			this.CreateSceneEliminate(()=>
			{
				// 创建蛋
				this.CreatePvpEgg(()=>
				{
					if(callback != null) callback();
				});
			});
		}, tile);
    }
	private int tileIndex;
	private void CreateSceneTile(Action callback, string tile)
	{
		this.tileIndex = 0;
		for (int i = 0; i < GameWidth; i++) 
		{
			for (int j = 0; j < GameHeight; j++) 
			{
				this.tileIndex ++;
				this.CreateSceneTileItem((PvpTile pvpTile)=>
				{
					this.tileIndex --;
					// 添加元素
					this.AllPvpTiles.Add(pvpTile);
					// 如果索引为 0
					if(this.tileIndex <= 0)
					{
						if(callback != null) callback();
					}
				}, i, j, tile);
			}
		}
	}
	private void CreateSceneTileItem(Action<PvpTile> callback, int i, int j, string tile)
	{
		PvpGameObjectManager.Create(this.PvpTileResource, (GameObject PvpTileObject)=>
		{
			PvpTile pvpTile = PvpTileObject.GetComponent<PvpTile>();
			//SpriteRenderer sr = pvpTile.RenderObject.GetComponent<SpriteRenderer>();
			//sr.sprite = Resources.Load<Sprite>(tile);
			pvpTile.transform.parent = TilesArea.transform;
			pvpTile.transform.localScale = new Vector3(1, 1, 1);
			pvpTile.SetPosition(i, j);

			if(callback != null) callback(pvpTile);
		});
	}
	#endregion
	
	#region 生成消除块
	private int eliminateIndex;
	private void CreateSceneEliminate(Action callback)
	{
		this.eliminateIndex = 0;
		for (int i = 0; i < GameWidth; i++) 
		{
			for (int j = 0; j < GameHeight; j++) 
			{
				this.eliminateIndex ++;
				this.CreateSceneEliminateItem((PvpEliminate pvpEliminate)=>
				{
					this.eliminateIndex --;
					// 添加元素
					this.AllEliminates.Add(pvpEliminate);
					// 如果索引为 0
					if(this.eliminateIndex <= 0)
					{
						if(callback != null) callback();
					}
				}, i, j + 9);
			}
		}
	}
	private void CreateSceneEliminateItem(Action<PvpEliminate> callback, int xPosition, int yPosition)
    {
		PvpGameObjectManager.Create(this.PvpEliminateResource, (GameObject PvpEliminateObject)=>
		{
			PvpEliminate pvpEliminate = PvpEliminateObject.GetComponent<PvpEliminate>();
			
			pvpEliminate.transform.parent = EliminateArea.transform;
			pvpEliminate.transform.localScale = new Vector3(1, 1, 1);
			pvpEliminate.SetPosition(xPosition, yPosition);
			pvpEliminate.GameControl = this;
			pvpEliminate.AttrubuteToRender();

			if(callback != null) callback(pvpEliminate);
		});
    }
    #endregion

    #region 生成战斗对象  
    #endregion

    #region 生成玩家
    
	int InitXposition = 3;
    int InitBYposition = 1;
	int InitTYposition = 7;

	void InitCharacterPets(Action callback)
    {
		// 创建自己的宠物
		this.CreatePvpPet ((List<PvpOwnUnit> selfPetList) =>
		{
			// 把自己宠物添加到列表中
			this.AllOwns.Add(selfPetList);
			// 创建敌人的宠物
			this.CreatePvpPet((List<PvpOwnUnit> enemyPetList) =>
			{
				// 把敌人宠物添加到列表中
				this.AllOwns.Add(enemyPetList);
				// 添加角色
				this.AllOwns [0].Add (this.PvpCharacterSelf);
				this.AllOwns [1].Add (this.PvpCharacterEnemy);
				// 调用回调
				if(callback != null) callback();

			}, this.PvpUserInfoEnemy, this.PvpCharacterEnemy);

		}, this.PvpUserInfoSelf, this.PvpCharacterSelf);
    }
    
	private int characterIndex;
	private int orderCharacterIndex;
	private int orderMonsterIndex;
	private int orderPetIndex;
    void InitCharacter(Action callback)
    {
		this.orderCharacterIndex = 0;
		int selfPos = 0;
		int enemyPos = 0;

		DungeonEnum.FaceDirection selfDirection = DungeonEnum.FaceDirection.Down;
		DungeonEnum.FaceDirection enemyDirection = DungeonEnum.FaceDirection.Down;

		if(this.PvpUserInfoSelf.Pvp_PosY > this.PvpUserInfoEnemy.Pvp_PosY)
		{
			selfPos = this.GameHeight - 1;
			enemyPos = 0;
			selfDirection = DungeonEnum.FaceDirection.Down;
			enemyDirection = DungeonEnum.FaceDirection.Up;
		}
		else
		{
			selfPos = 0;
			enemyPos = this.GameHeight - 1;
			selfDirection = DungeonEnum.FaceDirection.Up;
			enemyDirection = DungeonEnum.FaceDirection.Down;
		}

		// 创建自己
		this.CreatePvpCharacter ((PvpCharacter characterItem)=>
		{
			this.PvpCharacterSelf = characterItem;
			this.PvpCharacterSelf.gameObject.SetActive(false);
			// 调用创建完成回调
			this.InitCharacterActionCallback(callback);
		}, this.PvpUserInfoSelf, selfPos, selfDirection, true);

		// 创建敌人
		this.CreatePvpCharacter((PvpCharacter characterItem)=>
		{
			this.PvpCharacterEnemy = characterItem;
			this.PvpCharacterEnemy.gameObject.SetActive(false);
			// 调用创建完成回调
			this.InitCharacterActionCallback(callback);
		}, this.PvpUserInfoEnemy, enemyPos, enemyDirection, false);
    }

	void InitCharacterActionCallback(Action callback)
	{
		this.characterIndex ++;
		if(this.characterIndex >= 2)
		{
			// 显示 Avatar
			this.PvpPlayerInfo.ShowAvatar(this.PvpUserInfoSelf);
			this.PvpPlayerInfo.ShowAvatar (this.PvpUserInfoEnemy, true);

			if(callback != null) callback();
		}
	}

	/// <summary>
	/// 创建 PVP 角色数据
	/// </summary>
	/// <returns>The pvp character.</returns>
	/// <param name="pvpUserInfo">Pvp user info.</param>
	/// <param name="position">Position.</param>
	/// <param name="direction">Direction.</param>
	private void CreatePvpCharacter(Action<PvpCharacter> callback, PvpUserInfo pvpUserInfo, int position, DungeonEnum.FaceDirection direction, bool bottomStatus)
	{
		PvpGameObjectManager.Create (this.CharacterResource, (GameObject playerObject) =>
		{
			this.orderCharacterIndex ++;

			PvpCharacter pvpCharacter = playerObject.GetComponent<PvpCharacter>();
			
			playerObject.SetActive(true);
			
			pvpCharacter.pvpPlayerBuff = pvpUserInfo.pvpPlayerBuff;
			pvpCharacter.pvpPlayerSkill = pvpUserInfo.pvpPlayerSkill;
			
			if(pvpUserInfo.CurWeapon != null)
			{
				pvpCharacter.ChainSkill = ConfigManager.SkillConfig.GetSkillById(pvpUserInfo.CurWeapon.CurHardWareData.SkillAffix1);
				pvpCharacter.ChainSkill2 = ConfigManager.SkillConfig.GetSkillById(pvpUserInfo.CurWeapon.CurHardWareData.SkillAffix2);
			}
			
			pvpCharacter.SkinData = ConfigManager.SkinConfig.GetSkinDataById("S100");
			// 设置总血量
			pvpCharacter.Hp = pvpUserInfo.Pvp_TotalHp;
			// 设置当前血量
			pvpCharacter.CurHp = pvpUserInfo.Pvp_Hp;
			// 当前攻击力
			pvpCharacter.Atk = pvpUserInfo.CurAtk;
			// 当前防御力
			pvpCharacter.Def = pvpUserInfo.CurDef;
			
			pvpCharacter = pvpCharacter;
			pvpCharacter.XRange = 1;
			pvpCharacter.YRange = 1;
			
			string skinPath = "PreFabs/Characters/S100";
			int j = 0;
			for (int i = 10; i <= 80; i = i + 10)
			{
				string lastPath = skinPath + i.ToString();
				GameObject rg = Resources.Load(lastPath) as GameObject;
				if (rg)
				{
					pvpCharacter.AllAnimations[j] = rg;
				}
				j++;
			}
			
			pvpCharacter.PvpUserInfo = pvpUserInfo;
			
			pvpCharacter.UnitWaiting(direction);
			playerObject.transform.parent = FightUnitArea.transform;
			pvpCharacter.GameControl = this;
			if(!reStopStatus)
			{
				pvpCharacter.SetPosition(pvpUserInfo.Pvp_PosX, position);
			}else
			{
				pvpCharacter.SetPosition(pvpUserInfo.Pvp_PosX, pvpUserInfo.Pvp_PosY);
			}
			
			//AnimationHelper.AnimationFadeTo(0, playerObject, iTween.EaseType.linear, null, null, 0);
			
			DungeonData dungeonData = ConfigManager.DungeonConfig.GetDungeonById(PvpGameControl.CurDungeonId);
			
			if(bottomStatus)
			{
				pvpCharacter.UserType = PvpCharacter.BOTTOM;
				pvpCharacter.CurUnitHp.curHpType = UnitHp.HpType.OwnUnit;
				this.PvpPlayerInfo.BottomItem.SetBoutLabNum (MAX_ROUND - this.SelfRoundsCount);
				
			}else
			{
				pvpCharacter.UserType = PvpCharacter.TOP;
				pvpCharacter.CurUnitHp.curHpType = UnitHp.HpType.EnemyUnit;
				this.PvpPlayerInfo.TopItem.SetBoutLabNum (MAX_ROUND - this.EnemyRoundsCount);
				
				pvpCharacter.ShowElement();
			}
			// 设置缓存键
			pvpCharacter.CacheKey = "C_" + pvpCharacter.UserType + "_" + pvpCharacter.PvpUserInfo.UserId;

			pvpCharacter.RefreshHp(true);

			pvpCharacter.OrderIndex = this.orderCharacterIndex;
			
			// 重置图层
			pvpCharacter.ResertAllLayer ();
			
			this.AllBarrier.Add (pvpCharacter);

			pvpCharacter.InitAnimation(()=>
			{
				if(callback != null) callback(pvpCharacter);
			});
		});
	}

	public void RefreshHpAndPower(PvpCharacter pvpCharacter, float hp, float power)
	{
		pvpCharacter.CurHp = hp;
		this.SetHpUIHpShow (pvpCharacter);

		pvpCharacter.PvpUserInfo.Pvp_SkillPower = (int)power;

		//Debug.Log ("现在能量数据：：：：" + pvpCharacter.UserType + ":" + power);

		this.SetSkillPower (pvpCharacter);
	}

	/// <summary>
	/// 创建怪物
	/// </summary>
	/// <param name="monsterID">Monster I.</param>
	/// <param name="pvpFightUnit">Pvp fight unit.</param>
	/// <param name="round">Round.</param>
	/// <param name="skill">If set to <c>true</c> skill.</param>
	public PvpMonster CreateMonster(BaseSkillItem skillItem, Vector3 monsterPosition, string monsterID, PvpFightUnit pvpFightUnit, int round, bool hiddenStatus = false)
	{
		this.orderMonsterIndex ++;

		MonsterData monsterConfig = ConfigManager.MonsterConfig.GetMonsterById(monsterID);

		GameObject monsterObject = Instantiate(PvpMonsterResource) as GameObject;
		PvpMonster pvpMonster = monsterObject.GetComponent<PvpMonster>();
		pvpMonster.MonsterID = monsterID;
		pvpMonster.UserType = pvpFightUnit.UserType;

		pvpMonster.CacheKey = "M_" + pvpFightUnit.UserType + "_" + monsterID;

		pvpMonster.pvpPlayerBuff = new PvpPlayerBuff ();
		pvpMonster.pvpPlayerSkill = new PvpPlayerSkill ();
		pvpMonster.SkillItem = skillItem;
		pvpMonster.Hp = monsterConfig.Hp;
		pvpMonster.CurHp = pvpMonster.Hp;
		pvpMonster.Atk = monsterConfig.Attack;
		pvpMonster.Def = monsterConfig.Def;
		pvpMonster.Element = monsterConfig.Element;
		
		pvpMonster.XRange = monsterConfig.Range;
		pvpMonster.YRange = monsterConfig.Range;
		pvpMonster.FightRound = round; //战斗回合
		pvpMonster.FightRunPower = monsterConfig.RunPower; //行走能量
		pvpMonster.SkinData = ConfigManager.SkinConfig.GetSkinDataById(monsterConfig.SkinId);
		
		string skinPath = "PreFabs/Characters/" + monsterConfig.SkinId;
		int j = 0;
		for (int i = 10; i <= 80; i = i + 10)
		{
			string lastPath = skinPath + i.ToString();
			GameObject rg = Resources.Load(lastPath) as GameObject;
			if (rg)
			{
				pvpMonster.AllAnimations[j] = rg;
			}
			j++;
		}
		pvpMonster.UnitWaiting(DungeonEnum.FaceDirection.Down);
		pvpMonster.GameControl = this;
		monsterObject.transform.parent = FightUnitArea.transform;
		pvpMonster.SetPosition((int)monsterPosition.x, (int)monsterPosition.y);

		pvpMonster.ShowElement();

		if(!hiddenStatus)
		{
			pvpMonster.ChangeHiddenStatus (true, 0f);
			this.CreateMonsterCallback(pvpMonster);
		}
		else
		{
			pvpMonster.ChangeHiddenStatus (true, 0f);
		}

		pvpMonster.InitAnimation (null, false);

		this.AllBarrier.Add(pvpMonster);

		pvpMonster.OrderIndex = this.orderMonsterIndex;

		// 设置怪脚下的格子
		this.ChangeMonsterStatus (pvpMonster, false);

		// 设置 A * 障碍点
		this.aStarUtils.GetNode (pvpMonster.XPosition, pvpMonster.YPosition).walkable = false;

		return pvpMonster;
	}

	public void CreateMonsterCallback(PvpMonster pvpMonster, Action callback = null)
	{
		SkillEffectManager.Trigger (null, this.CurrentCharacter, pvpMonster.SkillItem, new Vector3 (pvpMonster.transform.localPosition.x, pvpMonster.transform.localPosition.y, 0f), (r) => 
		{
			pvpMonster.ChangeHiddenStatus(false, 1);
			// 重置所在图层
			pvpMonster.ResertAllLayer();
			if(callback != null) callback();
		});
	}

	/// <summary>
	/// 查找所有怪物
	/// </summary>
	/// <returns>The all monster list.</returns>
	public List<PvpFightUnit> GetAllMonsterList()
	{
		List<PvpFightUnit> monsterList = new List<PvpFightUnit> ();
		foreach(PvpFightUnit pvpFightUnit in this.AllBarrier)
		{
			if(pvpFightUnit.GetType() == typeof(PvpMonster)) monsterList.Add(pvpFightUnit);
		}
		return monsterList;
	}

	/// <summary>
	/// 设置怪底下显示
	/// </summary>
	/// <param name="status">If set to <c>true</c> status.</param>
	public void ChangeAllMonsterListStatus(bool status)
	{
		List<PvpFightUnit> monsterList = this.GetAllMonsterList ();

		foreach(PvpFightUnit pvpMonster in monsterList)
		{
			PvpEliminate eliminateItem = this.FineEliminate(pvpMonster.XPosition, pvpMonster.YPosition);
			if(eliminateItem != null) eliminateItem.ChangeMonsterStatus(status);
		}
	}

	public void ChangeMonsterStatus(PvpFightUnit pvpFightUnit, bool status)
	{
		this.FineEliminate (pvpFightUnit.XPosition, pvpFightUnit.YPosition).ChangeMonsterStatus (status);
	}

	/// <summary>
	/// 获取已方怪物
	/// </summary>
	/// <returns>The monster by user type.</returns>
	/// <param name="pvpFightUnit">Pvp fight unit.</param>
	public List<PvpMonster> GetMonsterByUserType(PvpFightUnit pvpFightUnit)
	{
		List<PvpMonster> resultList = new List<PvpMonster> ();

		foreach(PvpFightUnit fightItem in this.AllBarrier)
		{
			if(fightItem.UserType == pvpFightUnit.UserType && fightItem.GetType() == typeof(PvpMonster))
			{
				resultList.Add(fightItem as PvpMonster);
			}
		}

		return resultList;
	}

	/// <summary>
	/// 创建 PVP 宠物数据
	/// </summary>
	/// <param name="pvpUserInfo">Pvp user info.</param>
	private int petIndex;
	private void CreatePvpPet(Action<List<PvpOwnUnit>> callback, PvpUserInfo pvpUserInfo, PvpFightUnit pvpFightUnit)
	{
		this.petIndex = 0;

		List<PvpOwnUnit> petList = new List<PvpOwnUnit> ();

		bool actionStatus = false;
		foreach (UserPet p in pvpUserInfo.UserPets)
		{
			//if (p.MapIndex != -1)
			//{
				actionStatus = true;
				// 创建索引加 1
				this.petIndex ++;
				// 遍历创建宠物
				this.CreatePvpPetItem((PvpPet petItem)=>
				{
					// 创建索引减 1
					this.petIndex --;
					// 把宠物添加到列表中
					petList.Add(petItem);
					// 如果创建完成
					if(this.petIndex <= 0)
					{
						if(callback != null) callback(petList);
					}
				}, p, pvpFightUnit);
			//}
		}
		// 如果没有宠物
		if(!actionStatus)
		{
			if(callback != null) callback(petList);
		}
	}

	private void CreatePvpPetItem(Action<PvpPet> callback, UserPet p, PvpFightUnit pvpFightUnit)
	{
		PvpGameObjectManager.Create (this.PetResource, (GameObject petObject) =>
		{
			this.orderPetIndex ++;

			PvpPet pvpPet = petObject.GetComponent<PvpPet>();
			pvpPet.UserType = pvpFightUnit.UserType;

			// 设置缓存键
			pvpPet.CacheKey = "P_" + pvpFightUnit.UserType + "_" + p.UserPetId;

			pvpPet.CurUserPet = p;
			pvpPet.Atk = p.CurAtk;
			pvpPet.SkinData = ConfigManager.SkinConfig.GetSkinDataById(p.CurPetData.SkinId);
			pvpPet.XRange = 1;
			pvpPet.YRange = 1;
			string skinPath = "PreFabs/Characters/" + p.CurPetData.SkinId;
			int j = 0;
			for (int i = 10; i <= 80; i = i + 10)
			{
				string lastPath = skinPath + i.ToString();
				GameObject rg = Resources.Load(lastPath) as GameObject;
				if (rg)
				{
					pvpPet.AllAnimations[j] = rg;
				}
				j++;
			}
			pvpPet.UnitWaiting(DungeonEnum.FaceDirection.Up);
			pvpPet.MainSkill = p.CurPetData.PetSkillData;
			pvpPet.MainSkill2 = p.CurPetData.PetSkillData2;
			petObject.transform.parent = FightUnitArea.transform;
			pvpPet.GameControl = this;

			petObject.SetActive(false);

			pvpPet.OrderIndex = this.orderPetIndex;
			
			// 重置图层
			pvpPet.ResertAllLayer ();

			pvpPet.InitAnimation(()=>
			{
				if(callback != null) callback(pvpPet);
			});
		});
	}

	private int eggIndex;
	private void CreatePvpEgg(Action callback)
	{
		if(this.EggJsonData == null || this.EggJsonData.Count == 0)
		{
			if(callback != null) callback();
			return;
		}
		this.eggIndex = 0;
		foreach (JsonObject eggData in this.EggJsonData)
		{
			// 创建索引加 1
			this.eggIndex ++;
			// 遍历创建宠物
			this.CreatePvpEggItem((PvpEgg pvpEgg)=>
			{
				// 创建索引减 1
				this.eggIndex --;
				// 如果创建完成
				if(this.eggIndex <= 0)
				{
					if(callback != null) callback();
				}
			}, eggData);
		}
	}
	
	private void CreatePvpEggItem(Action<PvpEgg> callback, JsonObject eggData)
	{
		PvpGameObjectManager.Create (this.EggResource, (GameObject eggObject) =>
		{
			PvpEgg pvpEgg = eggObject.GetComponent<PvpEgg>();

			pvpEgg.eggType = int.Parse(eggData["type"].ToString());
			pvpEgg.eggID = eggData["egg_id"].ToString();
			pvpEgg.summonMonsterID = eggData["monster_id"].ToString();

			if(eggData.ContainsKey("hp") && eggData["hp"] != null)
			{
				pvpEgg.CurHp = float.Parse(eggData["hp"].ToString());
			}

			EggData configData = ConfigManager.EggConfig.GetEggById(pvpEgg.eggID);
			if(configData != null)
			{
				pvpEgg.Hp = configData.Hp;
			}

			pvpEgg.Def = float.Parse(eggData["def"].ToString());
			pvpEgg.XRange = 1;
			pvpEgg.YRange = 1;
			pvpEgg.transform.parent = FightUnitArea.transform;
			pvpEgg.GameControl = this;
			
			pvpEgg.SetPosition(int.Parse(eggData["x"].ToString()), int.Parse(eggData["y"].ToString()));

			pvpEgg.RefreshHp();

			// 把宠物添加到列表中
			this.AllBarrier.Add(pvpEgg);

			// 技能写死，主要是为了表现技能效果
			pvpEgg.SkillItem = SkillManager.GetSkillItem(ConfigManager.SkillConfig.GetSkillById("HSk3034_2"));

			this.aStarUtils.GetNode (pvpEgg.XPosition, pvpEgg.YPosition).walkable = false;

			if(callback != null) callback(pvpEgg);
		});
	}
	
	#endregion
	
	#region 角色走路控制
	void CharacterMoveIn()
	{
		/*PvpGameObjectManager.Create("PreFabs/FX/Cloud", (GameObject pvpEnterObject)=>
		{
				pvpEnterObject.layer = LayerHelper.UI;
		});*/
	
		this.PvpCharacterSelf.gameObject.SetActive (true);
		this.PvpCharacterEnemy.gameObject.SetActive (true);
	
		// 如果不是断线重连
		if(!this.reStopStatus)
		{
			this.PvpCharacterSelf.UnitMove(this.PvpUserInfoSelf.Pvp_PosX, this.PvpUserInfoSelf.Pvp_PosY, CharacterMoveInitCallback, this.PvpCharacterSelf.CurFaceDirection);
			//AnimationHelper.AnimationFadeTo(1, this.PvpCharacterSelf.gameObject, iTween.EaseType.linear, null, null, 1);
		
			this.PvpCharacterEnemy.UnitMove(this.PvpUserInfoEnemy.Pvp_PosX, this.PvpUserInfoEnemy.Pvp_PosY, CharacterMoveInitCallback, this.PvpCharacterEnemy.CurFaceDirection);
			//AnimationHelper.AnimationFadeTo(1, this.PvpCharacterEnemy.gameObject, iTween.EaseType.linear, null, null, 1);
		}
		else 
		{ 
			this.CharacterMoveInitCallback();
			this.CharacterMoveInitCallback();
			//AnimationHelper.AnimationFadeTo(1f, this.PvpCharacterSelf.gameObject, iTween.EaseType.linear, this.gameObject, "CharacterMoveInitCallback", 0.3f);
			//AnimationHelper.AnimationFadeTo(1f, this.PvpCharacterEnemy.gameObject, iTween.EaseType.linear, this.gameObject, "CharacterMoveInitCallback", 0.3f);
		}
    }

	private void CharacterMoveInitCallback()
	{
		this.MoveInitIndex ++;
		// 如果行动完，设置位置以及格子
		if(this.MoveInitIndex >= 2)
		{
			this.PvpCharacterSelf.UnitWaiting(this.PvpCharacterSelf.CurFaceDirection);
			this.PvpCharacterSelf.SetPosition(this.PvpCharacterSelf.XPosition, this.PvpCharacterSelf.YPosition);

			this.PvpCharacterEnemy.UnitWaiting(this.PvpCharacterEnemy.CurFaceDirection);
			this.PvpCharacterEnemy.SetPosition(this.PvpCharacterEnemy.XPosition, this.PvpCharacterEnemy.YPosition);

			this.aStarUtils.GetNode(this.PvpCharacterSelf.XPosition, this.PvpCharacterSelf.YPosition).walkable = false;
			this.aStarUtils.GetNode(this.PvpCharacterEnemy.XPosition, this.PvpCharacterEnemy.YPosition).walkable = false;

			AllEliminatesDropIn();
		}
	}

    #endregion

    #region 开场渲染

    void AllEliminatesDropIn()
    {
        DropNum = 0;
        List<float> waitingTimes = new List<float>();
        waitingTimes.Add(0.05f);
        waitingTimes.Add(0.1f);
        waitingTimes.Add(0.15f);
        waitingTimes.Add(0.2f);
        waitingTimes.Add(0.175f);
        waitingTimes.Add(0.125f);
        waitingTimes.Add(0.075f);
        for (int i = 0; i < 7; i++)
        {
            StartCoroutine(EliminateBlockInitFallIn(i, waitingTimes[i] / 2));
        }
    }


    IEnumerator EliminateBlockInitFallIn(int col, float waiting)
    {
        yield return new WaitForSeconds(waiting);
        List<PvpEliminate> columnBlocks = FindEliminateByColumn(col);
        for (int m = 0; m < columnBlocks.Count; m++)
        {
            columnBlocks[m].YPosition = columnBlocks[m].YPosition - 9;
            StartCoroutine(columnBlocks[m].ResertPositionAnimation(650, 0.15f * m, "InitResertEnd"));
        }
    }

    int DropNum = 0;
    void InitResertEnd()
    {
        DropNum++;
        if (DropNum == GameWidth * GameHeight)
        {
            //FloorNotice.MoveOut();
            Invoke("BeginCharacterLoop", 0.5f);
            Invoke("ShowGuider", 0.1f);
        }
    }
    #endregion

	/// <summary>
	/// 查找周围 8 个方向格子
	/// </summary>
	/// <returns>The eight grid list.</returns>
	/// <param name="pvpFightUnit">Pvp fight unit.</param>
	public List<PvpTile> FindEightGridList(Vector2 position)
	{
		List<Vector2> positionList = new List<Vector2> ();
		positionList.Add(new Vector2 (position.x, position.y + 1)); // x, y + 1
		positionList.Add(new Vector2 (position.x + 1, position.y + 1)); // x + 1, y + 1
		positionList.Add(new Vector2 (position.x + 1, position.y)); // x + 1, y
		positionList.Add(new Vector2 (position.x + 1, position.y - 1)); // x + 1, y - 1
		positionList.Add(new Vector2 (position.x, position.y - 1)); // x, y - 1
		positionList.Add(new Vector2 (position.x - 1, position.y - 1)); // x - 1, y - 1
		positionList.Add(new Vector2 (position.x - 1, position.y)); // x - 1, y
		positionList.Add(new Vector2 (position.x - 1, position.y + 1)); // x - 1, y + 1

		List<PvpTile> tileList = new List<PvpTile> ();
		foreach(Vector2 positionItem in positionList)
		{
			if(!FindEightGridCheck(positionItem)) tileList.Add(this.FindPvpTile((int)positionItem.x, (int)positionItem.y));
		}
		return tileList;

	}

	/// <summary>
	/// 检测格子
	/// </summary>
	/// <returns><c>true</c>, if eight grid check was found, <c>false</c> otherwise.</returns>
	/// <param name="position">Position.</param>
	public bool FindEightGridCheck(Vector2 position)
	{
		if(position.x >= this.GameWidth || position.y >= this.GameHeight) return true;
		if(position.x < 0 || position.y < 0) return true;

		foreach(PvpFightUnit pvpFightUnit in this.AllBarrier)
		{
			if(pvpFightUnit.XPosition == (int)position.x && pvpFightUnit.YPosition == (int)position.y) return true;
		}
		return false;
	}

	public bool FindEightGridCheckSelf(Vector2 position, PvpCharacter selfItem)
	{
		if(position.x >= this.GameWidth || position.y >= this.GameHeight) return true;
		if(position.x < 0 || position.y < 0) return true;
		
		foreach(PvpFightUnit pvpFightUnit in this.AllBarrier)
		{
			// 如果位置相同
			if(pvpFightUnit.XPosition == (int)position.x && pvpFightUnit.YPosition == (int)position.y)
			{
				if(pvpFightUnit.GetType() == typeof(PvpCharacter))
				{
					if(pvpFightUnit.PvpUserInfo.UserId == selfItem.PvpUserInfo.UserId) return false;
				}
				return true;
			}
		}
		return false;
	}

    #region 工具方法

    /// <summary>
    /// 在某个集合里查找周围的格子
    /// </summary>
    public List<PvpTile> FindNeighbourTileIn(PvpTile obj, List<PvpTile> all)
    {
        List<PvpTile> neighbours = new List<PvpTile>();
        foreach (PvpTile b in all)
        {
            //怪物runpower<=1 不可斜走
            if ((obj.XPosition == b.XPosition - 1 && obj.YPosition == b.YPosition) ||
               (obj.XPosition == b.XPosition - 1 && obj.YPosition == b.YPosition + 1) ||
               (obj.XPosition == b.XPosition - 1 && obj.YPosition == b.YPosition - 1) ||
               (obj.XPosition == b.XPosition + 1 && obj.YPosition == b.YPosition) ||
               (obj.XPosition == b.XPosition + 1 && obj.YPosition == b.YPosition + 1) ||
               (obj.XPosition == b.XPosition + 1 && obj.YPosition == b.YPosition - 1) ||
               (obj.XPosition == b.XPosition && obj.YPosition == b.YPosition - 1) ||
                (obj.XPosition == b.XPosition && obj.YPosition == b.YPosition + 1))
            {
                neighbours.Add(b);
            }
        }
        return neighbours;
    }

    List<PvpEliminate> FindEliminateByColumn(int col)
    {
        List<PvpEliminate> eliminates = new List<PvpEliminate>();
        foreach (PvpEliminate pe in AllEliminates)
        {
            if (pe.XPosition == col)
            {
                eliminates.Add(pe);
            }
        }
        return eliminates;
    }

    public PvpEliminate FineEliminate(int xPosition, int yPosition)
    {
        foreach (PvpEliminate pe in AllEliminates)
        {
            if (pe.XPosition == xPosition && pe.YPosition == yPosition)
            {
                return pe;
            }
        }
        return null;
    }

    public PvpTile FindPvpTile(int xPosition, int yPosition)
    {
        foreach (PvpTile pt in AllPvpTiles)
        {
            if (pt.XPosition == xPosition && pt.YPosition == yPosition)
            {
                return pt;
            }
        }
        return null;
    }

    public void EnemyUnitEliminateEnable(bool enable)
    {
        foreach (PvpFightUnit pu in this.AllBarrier)
        {
			if(pu.UserType != this.PvpCharacterSelf.UserType)
			{
	            foreach (PvpTile pt in pu.RangeTiles)
	            {
	                PvpEliminate pe = FineEliminate(pt.XPosition, pt.YPosition);
	                if (pe)
	                {
	                    pe.SetEnabelRender(enable);
	                }
	            }
			}
        }
    }

    bool HasEnemyOnEliminate(PvpEliminate eliminate)
    {
        foreach (PvpFightUnit pu in this.AllBarrier)
        {
            PvpTile pt = FindPvpTile(eliminate.XPosition, eliminate.YPosition);
            if (pu.RangeTiles.Contains(pt))
            {
                return true;
            }
        }
        return false;
    }

    public bool HasNoRunEnemyOnPosition(PvpFightUnit fightUnit, int xPosition, int yPosition)
    {
        foreach (PvpFightUnit pu in this.AllBarrier)
        {
			// 排除自己，并且没有在隐藏状态
			if(pu.UserType != fightUnit.UserType || pu.GetType() == typeof(PvpEgg))
			{
				PvpTile pt = FindPvpTile(xPosition, yPosition);
				if (pu.RangeTiles.Contains(pt))
				{
					return true;
				}
			}
        }
        return false;
    }

    public PvpFightUnit FindEnemyOn(int xPosition, int yPosition)
    {
		foreach (PvpFightUnit pu in this.AllBarrier)
        {
			if(pu.UserType != this.CurrentCharacter.UserType || pu.GetType() == typeof(PvpEgg))
			{
	            PvpTile pt = FindPvpTile(xPosition, yPosition);
	            if (pu.RangeTiles.Contains(pt))
	            {
	                return pu;
	            }
			}
        }
        return null;
    }
    #endregion

    #region 游戏循环
    void BeginCharacterLoop()
	{
		/*
		Debug.Log ("----------------------------------------------------战斗双方数据开始---------------------------------------");
		Debug.Log ("自己当前生命：" + this.PvpCharacterSelf.PvpUserInfo.CurHp);
		Debug.Log ("自己当前攻击力：" + this.PvpCharacterSelf.PvpUserInfo.CurAtk);
		Debug.Log ("自己当前防御力：" + this.PvpCharacterSelf.PvpUserInfo.CurDef);
		
		Debug.Log ("敌方当前生命：" + this.PvpCharacterEnemy.PvpUserInfo.CurHp);
		Debug.Log ("敌方当前攻击力：" + this.PvpCharacterEnemy.PvpUserInfo.CurAtk);
		Debug.Log ("敌方当前防御力：" + this.PvpCharacterEnemy.PvpUserInfo.CurDef);
		Debug.Log ("----------------------------------------------------战斗双方数据结束---------------------------------------");
		*/
		// 显示延迟显示数据
		this.PvpPlayerInfo.ChangeDelayActiveStatus (true);

		// 设置双方宠物的能量数据
		this.PvpPlayerInfo.SetPowerData ();

		// 如果是断线重链，设置断线重新连接的数据
		if(this.reStopStatus) PvpReboundManager.Progress(this, CurPvpData);

		// 设置玩家脚底的效果
		SkillEffectManager.TriggerFixed(this.PvpCharacterSelf);
		// 设置敌人脚底的效果
		SkillEffectManager.TriggerFixed (this.PvpCharacterEnemy);

		this.EliminateFirstStatus = false;

		// 如果是自己先出手，
		if(this.SelfStatus)
		{
			this.CurrentCharacter = this.PvpCharacterSelf;
			CurCharacterEliminate = FineEliminate(this.PvpCharacterSelf.XPosition, this.PvpCharacterSelf.YPosition);
		}else // 如果是敌人先出手
		{
			this.CurrentCharacter = this.PvpCharacterEnemy;
			CurCharacterEliminate = FineEliminate(this.PvpCharacterEnemy.XPosition, this.PvpCharacterEnemy.YPosition);
		}

		// 添加倒计时
		this.StartCoolingTimer (this.GetCoolTime(null), this.SelfStatus, this.RoundTimer);

		// 设置能量数据
		this.RecoverAllPetEnergy (this.PvpCharacterSelf, false);
		this.RecoverAllPetEnergy (this.PvpCharacterEnemy, false);

		// 如果是重登状态，设置回合开始处理
		if(this.reStopStatus)
		{
			this.GameRoundBegin (this.CurrentCharacter, false, false);
		}else
		{
			this.GameRoundBegin (this.CurrentCharacter, false, true);
		}

		this.initStatus = true;

		// 设置划线锁定状态
		this.UserInputLock = this.initUserInputLockStatus;

		// 如果是断线重连
		if(this.reStopStatus)
		{
			if(!this.PvpCharacterEnemy.pvpPlayerBuff.CanOrNotMoveCheck())
			{
				this.PvpCharacterEnemy.ChangeDizziness();
			}
		}

		// 如果存在消息队列，剩下消息队列中的信息
		if(this.messageQueue != null && this.messageQueue.Count > 0)
		{
			while(this.messageQueue.Count > 0)
			{
				this.PvpPushHandler(this.messageQueue[0]);
				this.messageQueue.RemoveAt(0);
			}
		}	
    }

	public void ShowOrHiddenArrow(bool status)
	{
		this.PvpCharacterSelf.ShowArrow (status);
	}

	public void SetHpUIHpShow(PvpFightUnit pvpCharacter, bool refreshStatus = false)
    {
		float per = pvpCharacter.CurHp / pvpCharacter.Hp;
        if (per > 0.4f)
        {
			pvpCharacter.Weakness = false;
        }
        else
        {
			pvpCharacter.Weakness = true;
        }

		if(pvpCharacter.UserType == PvpCharacter.BOTTOM)
		{
			// 设置血量
			PvpPlayerInfo.BottomItem.HpUI.SetCurHpShow(pvpCharacter.CurHp, pvpCharacter.Hp);
			// 设置宠物
			PvpPlayerInfo.BottomItem.RefreshPlayerInfo(pvpCharacter.UserInfo);
		}else if(pvpCharacter.UserType == PvpCharacter.TOP)
		{
			// 设置血量
			PvpPlayerInfo.TopItem.HpUI.SetCurHpShow(pvpCharacter.CurHp, pvpCharacter.Hp);
			// 设置宠物
			PvpPlayerInfo.TopItem.RefreshPlayerInfo(pvpCharacter.UserInfo);
		}
	}
	
	public void SetSkillPower(PvpFightUnit pvpFightUnit)
	{
		if (pvpFightUnit.UserType == this.PvpCharacterSelf.UserType)
		{
			PvpPlayerInfo.BottomItem.SetPowerValue((float)PvpUserInfoSelf.Pvp_SkillPower / (float)PvpUserInfoSelf.CurPower, PvpUserInfoSelf.Pvp_SkillPower);
		}else
		{
			PvpPlayerInfo.TopItem.SetPowerValue((float)PvpUserInfoEnemy.Pvp_SkillPower / (float)PvpUserInfoEnemy.CurPower, PvpUserInfoEnemy.Pvp_SkillPower);
		}
	}

	public void SetFocusPetAvata(DungeonEnum.ElementAttributes elementAttri, int count)
	{
		if (this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			PvpPlayerInfo.BottomItem.FocusPetAvata(elementAttri, count);
		}else
		{
			PvpPlayerInfo.TopItem.FocusPetAvata(elementAttri, count);
		}
	}

	public void SetDisFocusPetAvata(DungeonEnum.ElementAttributes elementAttri, int count)
	{
		if (this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			PvpPlayerInfo.BottomItem.DisFocusPetAvata(elementAttri, count);
		}else
		{
			PvpPlayerInfo.TopItem.DisFocusPetAvata(elementAttri, count);
		}
	}

	public List<PetAvata> GetFocusPetAvata()
	{
		if (this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			return PvpPlayerInfo.BottomItem.GetFocusPetAvata();
		}
		{
			return PvpPlayerInfo.TopItem.GetFocusPetAvata();
		}
		return null;
	}

	public PetAvata FindPetAvata(UserPet userPet)
	{
		if (this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			return PvpPlayerInfo.BottomItem.FindPetAvata(userPet);
		}else
		{
			return PvpPlayerInfo.TopItem.FindPetAvata(userPet);
		}
		return null;
	}

	public void SetBoutLabNum(int n)
	{
		if(n < 0) n = 0;

		if (this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			PvpPlayerInfo.BottomItem.SetBoutLabNum(n);
		}else
		{
			PvpPlayerInfo.TopItem.SetBoutLabNum(n);
		}
	}

	public void AvataEffectRender(bool render, UserPet userPet)
	{
		if(this.CurrentCharacter == null || this.PvpCharacterSelf == null) return;

		if (this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			PvpPlayerInfo.BottomItem.AvataEffectRender(render, userPet);
		}else
		{
			PvpPlayerInfo.TopItem.AvataEffectRender(render, userPet);
		}
	}

	public void ResetAvataEffectRender(bool render, UserPet userPet)
	{
		PvpPlayerInfo.BottomItem.AvataEffectRender(render, userPet);
	}

	public void AvataProgress(float pro, UserPet userPet)
	{
		if(this.CurrentCharacter == null || this.PvpCharacterSelf == null) return;

		if (this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			PvpPlayerInfo.BottomItem.AvataProgress(pro, userPet);
		}else
		{
			PvpPlayerInfo.TopItem.AvataProgress(pro, userPet);
		}
	}

	/// <summary>
	/// 战斗回合召唤怪物处理
	/// </summary>
	void FightMonsterRoundProgress()
	{
		List<PvpMonster> deadList = new List<PvpMonster> ();
		int index = 0;
		while(index < this.AllBarrier.Count)
		{
			PvpMonster pvpMonster = this.AllBarrier[index] as PvpMonster;
			// 如果是怪物，并且是当前行走角色召唤出来的
			if(pvpMonster != null && pvpMonster.UserType == this.CurrentCharacter.UserType)
			{
				// 如果怪物达到销毁条件
				if(pvpMonster.FightRound - 1 <= 0)
				{
					deadList.Add(pvpMonster);
				}else
				{
					pvpMonster.FightRound --;
				}
			}
			index ++;
		}

		if(deadList.Count > 0)
		{
			for(int deadIndex = 0; deadIndex < deadList.Count; deadIndex ++)
			{
				this.FightBarrierDeadProgressItem(deadList[deadIndex]);
			}
		}
	}

	/// <summary>
	/// 处理战斗怪物
	/// </summary>
	void FightBarrierDeadProgress(bool eggStatus = false)
	{
		List<PvpFightUnit> dieList = new List<PvpFightUnit> ();
		foreach(PvpFightUnit pvpFightUnit in this.AllBarrier)
		{
			if(pvpFightUnit.CurHp <= 0)
			{
				if(!eggStatus && pvpFightUnit.GetType() == typeof(PvpMonster))
				{
					dieList.Add(pvpFightUnit);
				}else if(eggStatus && pvpFightUnit.GetType() == typeof(PvpEgg))
				{
					dieList.Add(pvpFightUnit);
				}
			}
		}
		// 如果有死亡的怪物
		if(dieList.Count > 0)
		{
			for(int index = 0; index < dieList.Count; index ++)
			{
				this.FightBarrierDeadProgressItem(dieList[index]);
			}
		}
	}
	/// <summary>
	/// 处理死亡的怪物
	/// </summary>
	/// <param name="pvpFightUnit">Pvp fight unit.</param>
	public void FightBarrierDeadProgressItem(PvpFightUnit pvpFightUnit, bool deleteStatus = true, Action callback = null)
	{
		// 设置怪物脚下的格子颜色
		this.ChangeMonsterStatus (pvpFightUnit, true);
		// 死亡之后，设置为可以行走
		this.aStarUtils.GetNode (pvpFightUnit.XPosition, pvpFightUnit.YPosition).walkable = true;
		// 处理死亡
		pvpFightUnit.UnitDead((p) =>
		{
			GameObject disObject =  Instantiate(Resources.Load("PreFabs/FX/Monster_D")) as GameObject;
			disObject.transform.position = p.transform.position;
			this.AllBarrier.Remove(p);
			GameObject.Destroy(p.gameObject);

			if(callback != null) callback();
		});
	}

	/// <summary>
	/// 结果处理
	/// </summary>
	public void FightResultProgress()
	{
		Debug.Log ("处理战斗结果 ！！！！！！！！！" + this.fightResult);
		// 如果战斗失败
		if(this.fightResult == PvpFightResult.FAILED)
		{
			Debug.Log ("Failed 调用死亡动作 ！！！！！！！！！");
			this.PvpCharacterSelf.UnitDead((pu) =>
			{      
				PvpGameControl.DeadStatus = true;
				Debug.Log ("Failed End 调用死亡动作 ！！！！！！！！！");
				this.StopCoolingTimer();

				// 如果是失败，设置自己血为 0
				this.PvpCharacterSelf.CurHp = 0;
				this.PvpCharacterSelf.RefreshHp ();

				//失败特效
				RenderFail();
			});
			return;
		}
		// 如果战斗胜利
		if(this.fightResult == PvpFightResult.SUCCESS)
		{
			Debug.Log ("Success 调用死亡动作 ！！！！！！！！！");
			this.PvpCharacterEnemy.UnitDead((pu) =>
			                                {       
				PvpGameControl.DeadStatus = true;
				Debug.Log ("Success End 调用死亡动作 ！！！！！！！！！");
				this.StopCoolingTimer();

				// 如果是成功，设置对方血为 0
				this.PvpCharacterEnemy.CurHp = 0;
				this.PvpCharacterEnemy.RefreshHp ();

				//胜利特效
				this.RenderSuccess();
			});
			return;
		}
		// 如果是平局
		if(this.fightResult == PvpFightResult.TIE)
		{
			this.StopCoolingTimer();

			this.PvpCharacterSelf.UnitDead((pu) =>
			{           
				PvpGameControl.DeadStatus = true;
				// 如果是失败，设置自己血为 0
				this.PvpCharacterSelf.CurHp = 0;
				this.PvpCharacterSelf.RefreshHp ();
			});

			this.PvpCharacterEnemy.UnitDead((pu) =>
			{           
				PvpGameControl.DeadStatus = true;
				// 如果是失败，设置自己血为 0
				this.PvpCharacterEnemy.CurHp = 0;
				this.PvpCharacterEnemy.RefreshHp ();

				//失败特效
				RenderFail();
			});

			return;
		}
	}

    void TryOwnsActionEnd()
    {
        actionOwnUnitCount--;

		//Debug.Log ("action own unit count : " + actionOwnUnitCount);

        if (actionOwnUnitCount == 0)
        {
            //己方行动结束
            foreach (PvpEliminate pe in AllEliminates)
            {
                pe.ShowAddition(false);
                pe.UnLinkFromLastRender(false);
                pe.Square.gameObject.SetActive(false);
            }
          	
			// 重置角色格子下面的数据
			if(this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
			{
				this.FineEliminate (this.PvpCharacterEnemy.XPosition, this.PvpCharacterEnemy.YPosition).ResetRenderPlayerToElement();//ResetAttributeToRender (this.PvpCharacterEnemy.XPosition, this.PvpCharacterEnemy.YPosition);
			}else{
				this.FineEliminate (this.PvpCharacterSelf.XPosition, this.PvpCharacterSelf.YPosition).ResetRenderPlayerToElement();//ResetAttributeToRender (this.PvpCharacterSelf.XPosition, this.PvpCharacterSelf.YPosition);
			}
			
			this.FillEliminateBlocks();
        }
    }

	private List<PvpOwnUnit> GetFightPetList()
	{
		List<PvpOwnUnit> pvpOwnList = new List<PvpOwnUnit> ();
		if(this.CurrentCharacter.UserType == this.PvpCharacterEnemy.UserType)
		{
			pvpOwnList = this.AllOwns[1];
		}else if(this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			pvpOwnList = this.AllOwns[0];
		}
		return pvpOwnList;
	}

	/// <summary>
	/// 设置宠物位置
	/// </summary>
	/// <param name="petList">Pet list.</param>
	private void SetPetPosition(List<PvpOwnUnit> petList)
	{
		foreach(PvpOwnUnit ownUnit in petList)
		{
			if(ownUnit.GetType() == typeof(PvpPet))
			{
				ownUnit.SetPosition(this.CurrentCharacter.XPosition, this.CurrentCharacter.YPosition);
			}
		}
	}

	public void ResetGameControlStatus()
	{
		// 取消划线
		this.DisShowAllRange();

		foreach (PvpEliminate eb in this.AllEliminates)
		{
			eb.ReturnToCommon();

			eb.SetEnabelRender(true);

			eb.ShowAddition(false);
			eb.UnLinkFromLastRender(false);
			eb.Square.gameObject.SetActive(false);
		}
		// 只有是自己才切换回可以划线
		if(this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			this.EnemyUnitEliminateEnable(false);
			this.UserInputLock = false;
			this.CurrentCharacter.ShowArrow(true);
		}
		// 设置怪物脚底的颜色
		this.ChangeAllMonsterListStatus(false);

		// 重置宠物焦点
		if(this.PvpPlayerInfo != null) this.PvpPlayerInfo.ResetFocusPetAvata();

		// 设置技能确认面板
		if(this.SkillConfirm != null) this.SkillConfirm.gameObject.SetActive(false);
	}

	public PvpFightStep fightStep;
	/// <summary>
	/// 回合开始
	/// </summary>
	public void GameRoundBegin(PvpCharacter pvpCharacter, bool sendStatus = false, bool effectStatus = true)
	{
		// 显示开始回合
		if(this.RoundSignItem != null) this.RoundSignItem.Run(pvpCharacter.UserType == this.PvpCharacterSelf.UserType);
		if(this.RoundAction != null) this.RoundAction.ChangeData(pvpCharacter.UserType == this.PvpCharacterSelf.UserType);

		/*
		#if UNITY_STANDALONE_WIN
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.Append ("===> 第 ");
			stringBuilder.Append (this.PvpRounds);
			stringBuilder.Append (" 回合开始\n");
			stringBuilder.Append ("当前行动角色 ID：");
			stringBuilder.Append (pvpCharacter.PvpUserInfo.UserId);
			stringBuilder.Append ("，已方状态：");
			stringBuilder.Append (pvpCharacter.UserType == this.PvpCharacterSelf.UserType);
			stringBuilder.Append("\n");
			PvpLogManager.Log (this.PvpTableID, stringBuilder.ToString ());
		#endif
		*/
		// 回合开始阶段
		this.fightStep = PvpFightStep.ROUND_BEGIN;

		// 技能表现初始化
		SkillManager.Init ();

		// 如果锁定技能
		this.ResetGameControlStatus();
		
		this.FightMonsterList = new List<PvpMonster>();
		this.monsterPathDict = new Dictionary<int, List<PvpPathData>>();
		// 清除连线
		this.ClearLastLink ();

		// 如果是自己
		if(pvpCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			if(effectStatus)
			{
				this.SelfRoundsCount ++;
				pvpCharacter.SetRoundsData(this.SelfRoundsCount, MAX_ROUND);
			}else
			{
				this.SetBoutLabNum(MAX_ROUND - this.SelfRoundsCount);
			}
			this.SetPetPosition (this.AllOwns[0]);
		
			// 更新宠物状态
			this.ResetPowerAvatar(true);
		}else
		{
			if(effectStatus)
			{
				this.EnemyRoundsCount ++;
				pvpCharacter.SetRoundsData(this.EnemyRoundsCount, MAX_ROUND);
			}else
			{
				this.SetBoutLabNum(MAX_ROUND - this.EnemyRoundsCount);
			}
			this.SetPetPosition (this.AllOwns[1]);
			
			// 切换宠物状态
			this.ResetPowerAvatar(false);
		}

		if(effectStatus)
		{
			// 处理延时 Buff
			SkillManager.ExecuteRoundBegin (this.PvpCharacterSelf, this.PvpCharacterEnemy);
			// 更新 Buff 回合
			SkillManager.ExecuteRoundBegin (pvpCharacter);
			// 更新召唤 Round 回合
			this.FightMonsterRoundProgress ();
		}

		// 更新眩晕效果
		if(pvpCharacter.CurActionState == PvpFightUnit.ActionState.Vertigo)
		{
			if(pvpCharacter.pvpPlayerBuff.CanOrNotMoveCheck())
			{
				// 更新眩晕状态
				pvpCharacter.ChangeWaiting();
			}
		}

		// 如果是自己，显示血量与回合警告提示
		if(pvpCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			int roundPercent = MAX_ROUND - this.SelfRoundsCount;
			float hpPercent = this.PvpCharacterSelf.CurHp / this.PvpCharacterSelf.Hp;
			// 如果回合小于一个回合或者血量小于 30%
			if(roundPercent <= 1 || hpPercent <= 0.3f)
			{
				if(this.singalItem == null)
				{
					PvpGameObjectManager.Create("PreFabs/FX/Deading", (GameObject objectItem)=>
					{
						this.singalItem = objectItem;
						this.singalItem.SetActive(true);
					});
				}else
				{
					this.singalItem.SetActive(true);
				}
			}else
			{
				if(this.singalItem != null) this.singalItem.SetActive(false);
			}
		}
		else
		{
			if(this.singalItem != null) this.singalItem.SetActive(false);
		}

		// 如果是自己
		if(pvpCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			// 显示箭头
			this.ShowOrHiddenArrow(true);
			// 解锁划线状态
			this.UserInputLock = false;
			//
			this.isLinking = true;
		}else{
			// 隐藏箭头
			this.ShowOrHiddenArrow(false);
			// 锁定划线状态
			this.UserInputLock = true;
			//
			this.isLinking = false;
		}

		// 如果是自己
		//if(pvpCharacter.UserType == this.PvpCharacterSelf.UserType)
		//{
			this.PvpPlayerInfo.RefreshSkillCd(this.PvpCharacterSelf, this.PvpCharacterEnemy);
		//}

		FineEliminate(this.PvpCharacterSelf.XPosition, this.PvpCharacterSelf.YPosition).SetToPlayerRender();
		FineEliminate(this.PvpCharacterEnemy.XPosition, this.PvpCharacterEnemy.YPosition).SetToPlayerRender();
		
		// 如果不是机器人，机器人由服务器端处理
		if(pvpCharacter.PvpUserInfo.PvpType == 1)
		{
			// 如果不可以行走
			if(!pvpCharacter.pvpPlayerBuff.CanOrNotMoveCheck())
			{
				// 如果是自己
				if(pvpCharacter.UserType == this.PvpCharacterSelf.UserType)
				{
					this.StartCoroutine(this.CanNotMoveProgress(1f));
				}
			}
		}

		if(this.messageCallback_103 != null) this.messageCallback_103();
	}

	public void GameRoundCoolingTimerChange()
	{
		// 如果行走的格子为空，说明是回合时间到了，自动触发的
		if(this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			this.SelfCoolTimeStatus = true;
		}else{
			this.EnemyCoolTimeStatus = true;
		}
	}

    /// <summary>
    ///己方行动总次数，为0 己方行动结束
    /// </summary>
    public int actionOwnUnitCount = 1;
	private bool actionOwnUnitStatus = false;
	public void GameActionBegin(PvpCharacter pvpCharacter, bool sendStatus = false)
    {
		// 如果还在战斗状态，返回，不应该出现这样的情况
		if(this.fightStep == PvpFightStep.ACTION) return;

		//this.CurPathEliminate

		// 如果是自己
		if(pvpCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			this.PvpPlayerInfo.StopCoolingTime();
		}

		this.actionOwnUnitStatus = false;

		// 清空召唤出来的怪物
		SkillManager.CommonMonsterList.Clear ();

		// 回合阶段
		this.fightStep = PvpFightStep.ACTION;

		// 开始行走设置箭头状态
		this.ShowOrHiddenArrow(false);
		// 禁止划线
		this.UserInputLock = true;
		// 重置本回合行走触发的状态数据
		this.actionOwnUnitCount = 1;

		// 如果行走的格子数据不为空
		if (this.CurPathEliminate != null && this.CurPathEliminate.Count > 0) 
		{
			// 设置当前行走格子属性
			this.currentEliminateAttribute = this.CurPathEliminate [this.CurPathEliminate.Count - 1].CurEliminateAttribute;

			// 触发行走
			SkillManager.Trigger(SkillTriggerTypeEnum.Walk, 0, this.CurrentCharacter, null, null, null);
		}

		PvpBuffValueData pvpBuffValueDataSelf = this.PvpCharacterSelf.pvpPlayerBuff.GetValueByBuffTypeToClass ();
		/*
		#if UNITY_STANDALONE_WIN
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.Append ("+++> 已方数据开始\n");
			stringBuilder.Append ("当前血量：");
			stringBuilder.Append (this.PvpCharacterSelf.CurHp);
			stringBuilder.Append ("，当前能量：");
			stringBuilder.Append (this.PvpUserInfoSelf.CurPower);
			stringBuilder.Append ("，原始攻击：");
			stringBuilder.Append (this.PvpUserInfoSelf.OriAtk);
			stringBuilder.Append ("，当前攻击：");
			stringBuilder.Append (this.PvpUserInfoSelf.CurAtk);
			stringBuilder.Append ("，原始防御：");
			stringBuilder.Append (this.PvpUserInfoSelf.OriDef);
			stringBuilder.Append ("，当前防御：");
			stringBuilder.Append (this.PvpUserInfoSelf.CurDef);
			stringBuilder.Append ("，原始暴击：");
			stringBuilder.Append (this.PvpUserInfoSelf.OriCrit);
			stringBuilder.Append ("，当前暴击：");
			stringBuilder.Append (this.PvpUserInfoSelf.CurCrit);
			stringBuilder.Append ("，原始暴击伤害：");
			stringBuilder.Append (this.PvpUserInfoSelf.OriCritHurt);
			stringBuilder.Append ("，当前暴击伤害：");
			stringBuilder.Append (this.PvpUserInfoSelf.CurCritHurt);
			stringBuilder.Append ("，原始闪避：");
			stringBuilder.Append (this.PvpUserInfoSelf.OriAvoid);
			stringBuilder.Append ("，当前闪避：");
			stringBuilder.Append (this.PvpUserInfoSelf.CurAvoid);
			stringBuilder.Append ("\n+++> 敌方数据开始\n");
			stringBuilder.Append ("当前血量：");
			stringBuilder.Append (this.PvpCharacterEnemy.CurHp);
			stringBuilder.Append ("，当前能量：");
			stringBuilder.Append (this.PvpUserInfoEnemy.CurPower);
			stringBuilder.Append ("，原始攻击：");
			stringBuilder.Append (this.PvpUserInfoEnemy.OriAtk);
			stringBuilder.Append ("，当前攻击：");
			stringBuilder.Append (this.PvpUserInfoEnemy.CurAtk);
			stringBuilder.Append ("，原始防御：");
			stringBuilder.Append (this.PvpUserInfoEnemy.OriDef);
			stringBuilder.Append ("，当前防御：");
			stringBuilder.Append (this.PvpUserInfoEnemy.CurDef);
			stringBuilder.Append ("，原始暴击：");
			stringBuilder.Append (this.PvpUserInfoEnemy.OriCrit);
			stringBuilder.Append ("，当前暴击：");
			stringBuilder.Append (this.PvpUserInfoEnemy.CurCrit);
			stringBuilder.Append ("，原始暴击伤害：");
			stringBuilder.Append (this.PvpUserInfoEnemy.OriCritHurt);
			stringBuilder.Append ("，当前暴击伤害：");
			stringBuilder.Append (this.PvpUserInfoEnemy.CurCritHurt);
			stringBuilder.Append ("，原始闪避：");
			stringBuilder.Append (this.PvpUserInfoEnemy.OriAvoid);
			stringBuilder.Append ("，当前闪避：");
			stringBuilder.Append (this.PvpUserInfoEnemy.CurAvoid);
			stringBuilder.Append ("\n");
			PvpLogManager.Log (this.PvpTableID, stringBuilder.ToString ());
		#endif
		*/

		// 如果需要发消息给服务器端，计算召唤宠物的行走路径
		List<BaseSkillItem> prepareSkillList = pvpCharacter.pvpPlayerSkill.GetSkillDataBySkillTypeAndOddsType (SkillTypeEnum.Talent, SkillOddsTypeEnum.Summon);
		// 如果有技能
		if(prepareSkillList.Count > 0)
		{
			Dictionary<int, List<bool>> prepareList = this.PrepareFight (pvpCharacter);
			if(prepareList != null && prepareList.Count > 0)
			{
				Vector2 preparePosition = new Vector2();
				if(this.CurPathEliminate.Count > 0)
				{
					preparePosition = new Vector2(this.CurPathEliminate[this.CurPathEliminate.Count - 1].XPosition, this.CurPathEliminate[this.CurPathEliminate.Count - 1].YPosition);
					int index = 0;

					int dictCount = prepareList.Count;
					for(int dictIndex = 0; dictIndex < dictCount; dictIndex ++)
					{
						List<bool> dataList = prepareList[dictIndex];
						foreach(bool prepareData in dataList)
						{
							if(prepareData)
							{
								SkillManager.Trigger(index, pvpCharacter, this.GetFightUnitByUserType(pvpCharacter), preparePosition);
							}
							index ++;
						}
					}
				}
			}
		}

		List<PvpEgg> eggList = this.PrepareEgg (pvpCharacter);
		if(eggList != null && eggList.Count > 0)
		{
			foreach(PvpEgg pvpEgg in eggList)
			{
				this.CreateMonster(pvpEgg.SkillItem, new Vector3(pvpEgg.XPosition, pvpEgg.YPosition, 0f), pvpEgg.summonMonsterID, pvpCharacter, 999, true);
			}
		}
        
        foreach (PvpEliminate pe in this.AllEliminates)
        {
            pe.SetEnabelRender(true);
        }

        foreach (Transform t in this.Ranges.transform)
        {
            Destroy(t.gameObject);
		}

		// 如果是自己
		if(pvpCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			// 如果路径数大于 0
			if(this.CurPathEliminate.Count > 0 && sendStatus)
			{
				monsterDataList = new JsonObject ();
				// 设置战斗的怪物数据
				this.FightMonsterList = this.GetMonsterByUserType (pvpCharacter);
				// 设置怪物路径数据
				this.monsterPathDict = this.MonsterRoundPathList(pvpCharacter, this.FightMonsterList, this.CurPathEliminate[this.CurPathEliminate.Count - 1]);

				foreach(KeyValuePair<int, List<PvpPathData>> keyValuePair in this.monsterPathDict)
				{
					JsonObject monsterData = new JsonObject();
					JsonArray pathJsonList = new JsonArray();

					List<PvpPathData> pathList = keyValuePair.Value;
					foreach(PvpPathData pathDataItem in pathList)
					{
						JsonObject pathItemData = new JsonObject();
						pathItemData.Add("x", pathDataItem.x);
						pathItemData.Add("y", pathDataItem.y);
						pathJsonList.Add(pathItemData);
					}
					monsterDataList.Add(keyValuePair.Key.ToString(), pathJsonList);
				}
			}else
			{
				this.FightMonsterList = new List<PvpMonster>();
				this.monsterPathDict = new Dictionary<int, List<PvpPathData>>();
			}
		}else
		{
			// 设置战斗的怪物数据
			this.FightMonsterList = this.GetMonsterByUserType (pvpCharacter);
		}

		// 计算格子颜色用
		pvpEliminateDataList = new List<PvpEliminateData> ();		
		stepDataList = new JsonArray ();

		if(this.CurPathEliminate != null && this.CurPathEliminate.Count > 0)
		{
			// 设置当前行走格子属性
			this.currentEliminateAttribute = this.CurPathEliminate[0].CurEliminateAttribute;
			foreach(PvpEliminate pvpEliminateItem in this.CurPathEliminate)
			{
				// 如果不存在
				if(!this.PvpEliminateElementCheck(pvpEliminateDataList, pvpEliminateItem.XPosition, pvpEliminateItem.YPosition))
				{
					pvpEliminateDataList.Add(new PvpEliminateData(pvpEliminateItem.XPosition, pvpEliminateItem.YPosition));

					JsonObject pathItemData = new JsonObject();
					pathItemData.Add("x", pvpEliminateItem.XPosition);
					pathItemData.Add("y", pvpEliminateItem.YPosition);

					this.stepDataList.Add(pathItemData);
				}
			}

			if(pvpEliminateDataList != null && pvpEliminateDataList.Count > 0)
			{
				pvpEliminateDataList.RemoveAt(pvpEliminateDataList.Count - 1);
			}
		}

		if(this.CurPathEliminate.Count > 0)
		{
	        PvpEliminate endEliminate = this.CurPathEliminate[CurPathEliminate.Count - 1];
	        endEliminate.SetChain(false);
	        endEliminate.SelectScale(false);
		}
		
		List<PvpPet> curActionPets = this.GetActionPetList ();

        actionOwnUnitCount = actionOwnUnitCount + curActionPets.Count;
		pvpCharacter.OwnAction(() =>
		{
			if(pvpCharacter.CurActionState != PvpFightUnit.ActionState.Vertigo)
			{
				pvpCharacter.UnitWaiting(pvpCharacter.CurFaceDirection);
			}
			pvpCharacter.ChainSkillUse(() =>
			{
				TryOwnsActionEnd();
			});
		}, CurPathEliminate);

        int i = 1;
        foreach (PvpPet pp in curActionPets)
        {
            StartCoroutine(PetsAction(pp, i));
            i++;
        }
		// 如果是自己，并且需要发送消息
		if(pvpCharacter.UserType == this.PvpCharacterSelf.UserType && sendStatus)
		{
			// 向后端发送战斗数据
			this.SendMoveDataToServer();
		}
    }

	private List<PvpPet> GetActionPetList()
	{
		List<PvpPet> dataList = new List<PvpPet>();
		List<PetAvata> readyPetsAvata = this.GetFocusPetAvata();
		List<PvpOwnUnit> pvpOwnList = this.GetFightPetList ();
		
		foreach (PetAvata pa in readyPetsAvata)
		{
			foreach (PvpOwnUnit po in pvpOwnList)
			{
				if (po.GetType() == typeof(PvpPet))
				{
					PvpPet pp = (PvpPet)po;
					
					if (pp.CurUserPet.UserPetId == pa.CurPet.UserPetId)
					{
						dataList.Add(pp);
					}
				}
			}
		}

		return dataList;
	}
	
	private bool PvpEliminateElementCheck(List<PvpEliminateData> itemList, int x, int y)
	{
		foreach(PvpEliminateData eliminateItem in itemList)
		{
			if(eliminateItem.x == x && eliminateItem.y == y) return true;
		}
		return false;
	}

    IEnumerator PetsAction(PvpPet p, int delay)
    {
        yield return new WaitForSeconds(0.5f * delay);

		// 如果没有在显示状态
		if(!p.gameObject.activeSelf) p.gameObject.SetActive (true);

        p.OwnAction(() =>
		{
            //回退
            int back = p.CurActionPath.Count - delay - 1;
            if (back < 0)
            {
                back = 0;
            }

			List<PvpEliminate> eliminateList = new List<PvpEliminate>();
			eliminateList.Add(this.FineEliminate(p.XPosition, p.YPosition));
			eliminateList.Add(p.CurActionPath[back]);

			PvpMoveManager.Move(eliminateList, p, (PvpEliminate eliminateItem)=>
			{
			}, ()=>
			{
				p.UnitWaiting(ReverseDirection(p.CurFaceDirection));
				p.ChainSkillUse(() =>
				{
					StartCoroutine(DelayFlyBack(p));
				});
			});

            /*PvpEliminate pe = p.CurActionPath[back];
            PvpTile curPt = FindPvpTile(p.XPosition, p.YPosition);
            PvpTile toPt = FindPvpTile(pe.XPosition, pe.YPosition);
            DungeonEnum.FaceDirection direction = curPt.GetTargetDirection(toPt);
            p.UnitMove(pe.XPosition, pe.YPosition, () =>
            {
                p.UnitWaiting(ReverseDirection(p.CurFaceDirection));
                p.ChainSkillUse(() =>
                {
                    StartCoroutine(DelayFlyBack(p));
                });
            }, direction);*/
        }, this.CurrentCharacter.CurActionPath);
    }

    IEnumerator DelayFlyBack(PvpPet p)
    {
        yield return new WaitForSeconds(0.5f);
        p.UnitWaiting(p.CurFaceDirection);
        p.PetBackToAvata(() =>
        {
			p.gameObject.SetActive(false);

            TryOwnsActionEnd();
        });
    }

	// 敌人行走的数据
	public void EnemyRound(JsonArray dataList, JsonObject monsterList, Action callback)
	{
		this.messageCallback_102 = callback;
		if(dataList == null)
		{
			if(this.messageCallback_102 != null) this.messageCallback_102();
			return;
		}
		
		this.CurrentCharacter = this.PvpCharacterEnemy;

		foreach(PvpEliminate pvpEliminateItem in this.CurPathEliminate)
		{
			pvpEliminateItem.UnLinkFromLastRender(true);
			pvpEliminateItem.LinkChainLabel.gameObject.SetActive(false);
		}

		// 地方怪物行动列表
		this.monsterPathDict = new Dictionary<int, List<PvpPathData>> ();
		// 解析角色行动
		foreach (JsonObject jsonData in dataList) 
		{
			PvpEliminate pvpEliminate = FineEliminate (int.Parse(jsonData["x"].ToString()), int.Parse(jsonData["y"].ToString()));
			this.CurrentPathEliminateInsert(pvpEliminate);
			pvpEliminate.ShowAddition(true);
		}
		if(monsterList != null)
		{
			// 解析怪物行动
			foreach(var monsterData in monsterList)
			{
				JsonArray monsterPathData = monsterData.Value as JsonArray;

				List<PvpPathData> monsterPathList = new List<PvpPathData>();
				foreach(JsonObject monsterPathItem in monsterPathData)
				{
					monsterPathList.Add(new PvpPathData(int.Parse(monsterPathItem["x"].ToString()), int.Parse(monsterPathItem["y"].ToString())));
				}
				this.monsterPathDict.Add(int.Parse(monsterData.Key), monsterPathList);
			}
		}

		if(this.CurPathEliminate.Count > 0)
		{
			CurCharacterEliminate = this.CurPathEliminate[0];
			// 设置终结技能范围
			ShowCurRange(FindPvpTile(this.CurPathEliminate[this.CurPathEliminate.Count - 1].XPosition, this.CurPathEliminate[this.CurPathEliminate.Count - 1].YPosition));
			// 设置焦点数据
			this.SetFocusPetAvata (this.CurPathEliminate[this.CurPathEliminate.Count - 1].CurEliminateAttribute, this.CurPathEliminate.Count - 1);
		}

		// 调用行走函数
		this.GameActionBegin (this.CurrentCharacter, false);
    }
    #endregion

    #region 填充
    int fillMoveCount = 0;
    List<PvpEliminate> readyFillBlocks = new List<PvpEliminate>();
    void FillEliminateBlocks()
    {
		// 走完之后回合加 1
		this.PvpRounds ++;

		// 重置怪脚底下的颜色数据
		this.ChangeAllMonsterListStatus (true);

        fillMoveCount = 0;
        readyFillBlocks.Clear();
        //逐列填充

        for (int i = 0; i < GameWidth; i++)
        {
            List<PvpEliminate> columnBlocks = FindEliminateByColumn(i);
			CreateFills(0, columnBlocks, i);
        }
		if(fillMoveCount > 0)
		{
			AllFill();
		}else{
			this.ResertPositionAnimationEnd(null);
		}
    }

    /// <summary>
    /// 排序某一列的消除块
    /// </summary>
    /// <param name="blocks"></param>
    /// <returns></returns>
    List<PvpEliminate> SortColumnBlocks(List<PvpEliminate> blocks)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            for (int j = 0; j < blocks.Count - i - 1; j++)
            {
                PvpEliminate b1 = blocks[j];
                PvpEliminate b2 = blocks[j + 1];
                if (b1.YPosition > b2.YPosition)
                {
                    PvpEliminate temp = blocks[j];
                    blocks[j] = blocks[j + 1];
                    blocks[j + 1] = temp;
                }
            }
        }
        return blocks;
    }

	/// <summary>
	/// 缓存元素列表
	/// </summary>
	public List<PvpEliminate> CacheEliminateList = new List<PvpEliminate> ();
	/// <summary>
	/// 获得缓存对象
	/// </summary>
	/// <returns>The cache eliminate item.</returns>
	private GameObject GetCacheEliminateItem()
	{
		if(!PvpGameObjectManager.CACHE_STATUS) return null;

		if(this.CacheEliminateList == null || this.CacheEliminateList.Count == 0) return null;
		foreach(PvpEliminate objectItem in CacheEliminateList)
		{
			if(objectItem.destoryStatus)
			{
				objectItem.destoryStatus = false;
				return objectItem.gameObject;
			}
		}
		return null;
	}

    void CreateFills(int fillFrom, List<PvpEliminate> curExistsBlock, int column)
    {
        //没有障碍物全部填充
        //按当前Y坐标重新排序
        curExistsBlock = SortColumnBlocks(curExistsBlock);
        int YIndex = fillFrom;
        for (int j = 0; j < curExistsBlock.Count; j++)
        {
            curExistsBlock[j].YPosition = YIndex;
            curExistsBlock[j].SetName();
            readyFillBlocks.Add(curExistsBlock[j]);
            YIndex++;
        }

		List<PvpEliminateData> eliminateDataList = this.GetEliminateDataByColumn (column);

        int creatIndex = 9;
        //创建填充
        for (int k = YIndex, j = 0; k <= 8 && j <= 8; k++, j ++)
        {
			GameObject PvpEliminateObject = this.GetCacheEliminateItem();
			if(PvpEliminateObject == null)
			{
				PvpEliminateObject = Instantiate(PvpEliminateResource) as GameObject;
			}
			PvpEliminateObject.SetActive(true);

            PvpEliminate pveEliminate = PvpEliminateObject.GetComponent<PvpEliminate>();

            AllEliminates.Add(pveEliminate);
            pveEliminate.transform.parent = EliminateArea.transform;
            pveEliminate.transform.localScale = new Vector3(1, 1, 1);
			pveEliminate.GameControl = this;
            pveEliminate.SetPosition(column, creatIndex);
            pveEliminate.XPosition = column;
            pveEliminate.YPosition = k;
            pveEliminate.AttrubuteToRender(eliminateDataList[j].y);

            pveEliminate.SetName();
            curExistsBlock.Add(pveEliminate);
            readyFillBlocks.Add(pveEliminate);
            creatIndex++;
        }
        //下落
        fillMoveCount = fillMoveCount + curExistsBlock.Count;
    }

	private List<PvpEliminateData> GetEliminateDataByColumn(int col)
	{
		List<PvpEliminateData> resultList = new List<PvpEliminateData> ();
		foreach(PvpEliminateData eliminateData in this.pvpEliminateDataList)
		{
			if(eliminateData.x == col)
			{
				resultList.Add(eliminateData);
			}
		}

		resultList.Sort (delegate(PvpEliminateData dx, PvpEliminateData dy) {
			if(dx.y < dy.y) return -1;
			if(dx.y > dy.y) return 1;
			return 0;
				});

		return resultList;
	}

    void AllFill()
    {
        for (int i = 0; i < 7; i++)
        {
            StartCoroutine(FillColoumn(i, i * 0.01f));
        }
    }

    IEnumerator FillColoumn(int col, float delay)
    {
        yield return new WaitForSeconds(delay);
        List<PvpEliminate> columnB = new List<PvpEliminate>();
        for (int i = 0; i < readyFillBlocks.Count; i++)
        {
            PvpEliminate b = readyFillBlocks[i];
            if (b.XPosition == col)
            {
                columnB.Add(b);
            }
        }
        for (int m = 0; m < columnB.Count; m++)
        {
            columnB[m].NoDelayResertPositionAnimation(600, "ResertPositionAnimationEnd");
        }

    }

	public PvpFightUnit GetCurrentTargetItem()
	{
		PvpFightUnit targetItem = null;
		if(this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			targetItem = this.PvpCharacterEnemy;
		}else{
			targetItem = this.PvpCharacterSelf;
		}
		return targetItem;
	}

	private int GetXXColor(int x, int y)
	{
		for (int i = 0; i < this.xxObject.Count; i++) 
		{
			JsonObject jo = (JsonObject) this.xxObject[i];
			
			if (jo["x"].ToString() == x.ToString() && jo["y"].ToString() == y.ToString())
			{
				return int.Parse(jo["color"].ToString() == "0" ? jo["original"].ToString() : jo["color"].ToString());
			}
		}
		return -1;
	}

    void ResertPositionAnimationEnd(GameObject blockObj)
    {
        fillMoveCount--;
        if (fillMoveCount <= 0)
		{
			/*if(this.xxObject != null)
			{
				for(int ii = 0; ii < this.GameWidth; ii ++)
				{
					for(int ij = 0; ij < this.GameHeight; ij ++)
					{
						PvpEliminate pvpEliminate = this.FineEliminate(ii, ij);
						if((int)pvpEliminate.CurEliminateAttribute != this.GetXXColor(ii, ij))
						{
							Debug.Log("格子不同步数据：：：" + ii + ":" + ij + ":" + (int)pvpEliminate.CurEliminateAttribute + ":" + this.GetXXColor(ii, ij));
							pvpEliminate.SetToPlayerRender();
							//pvpEliminate.HiddenOperater();
						}
					}
				}
			}*/
			// 重置怪物脚下和颜色
			this.ChangeAllMonsterListStatus(false);

			CurCharacterEliminate = FineEliminate(this.CurrentCharacter.XPosition, this.CurrentCharacter.YPosition);
            if (CurCharacterEliminate == null)
            {
                GameObject PvpEliminateObject = Instantiate(PvpEliminateResource) as GameObject;
                PvpEliminate pveEliminate = PvpEliminateObject.GetComponent<PvpEliminate>();
                CurCharacterEliminate = pveEliminate;
                pveEliminate.CurEliminateAttribute = DungeonEnum.ElementAttributes.Player;
                AllEliminates.Add(pveEliminate);
                pveEliminate.transform.parent = EliminateArea.transform;
                pveEliminate.transform.localScale = new Vector3(1, 1, 1);
				pveEliminate.SetPosition(this.CurrentCharacter.XPosition, this.CurrentCharacter.YPosition);
                pveEliminate.ReturnToCommon();
            }

			List<PvpOwnUnit> ownUnitList = this.GetFightPetList ();
			foreach (PvpOwnUnit po in ownUnitList)
            {
                if (po.GetType() == typeof(PvpPet))
                {
					po.SetPosition(this.CurrentCharacter.XPosition, this.CurrentCharacter.YPosition);
                }
            }

			// 设置新的点为不可以行走
			this.aStarUtils.GetNode(this.CurrentCharacter.XPosition, this.CurrentCharacter.YPosition).walkable = false;
			
			// 处理怪物是否死亡
			this.FightBarrierDeadProgress();
			// 处理蛋是否死亡
			this.FightBarrierDeadProgress(true);

			bool monsterStatus = false;

			for(int monsterIndex = 0; monsterIndex < this.AllBarrier.Count; monsterIndex ++)
			{
				PvpMonster pvpMonsterItem = this.AllBarrier[monsterIndex] as PvpMonster;
				if(pvpMonsterItem != null && pvpMonsterItem.HiddenStatus)
				{
					Debug.Log("召唤怪物效果播放完才出现怪物 call !!!！！！！！！！！");
					monsterStatus = true;
					this.CreateMonsterCallback(pvpMonsterItem, ()=>
					{
						Debug.Log("召唤怪物效果播放完才出现怪物 ！！！！！！！！");
						this.CreateMonsterCallbackInvoke();
					});
				}
			}
			if(!monsterStatus)
			{
				// 这儿添加回调，执行怪物操作之后再调用回调函数执行后面的代码。
				this.MonsterRound(this.CurrentCharacter, ()=>
				{
					this.GameRoundEnd();
				});
			}
        }
    }

	public void CreateMonsterCallbackInvoke()
	{
		bool monsterStatus = false;
		for(int monsterIndex = 0; monsterIndex < this.AllBarrier.Count; monsterIndex ++)
		{
			PvpMonster pvpMonsterItem = this.AllBarrier[monsterIndex] as PvpMonster;
			if(pvpMonsterItem != null && pvpMonsterItem.HiddenStatus)
			{
				monsterStatus = true;
			}
		}
		if(monsterStatus) return;

		// 这儿添加回调，执行怪物操作之后再调用回调函数执行后面的代码。
		this.MonsterRound(this.CurrentCharacter, ()=>
		                  {
			this.GameRoundEnd();
		});
	}

	private void GameRoundEnd()
	{

		/*if(PvpMessageManager.pvpMessageResult != null)
		{
			PvpMessageManager.pvpMessageResult.Run(null);
			return;
		}*/

		FineEliminate(this.PvpCharacterSelf.XPosition, this.PvpCharacterSelf.YPosition).SetToPlayerRender(this.PvpCharacterSelf.HiddenAlpha);
		FineEliminate(this.PvpCharacterEnemy.XPosition, this.PvpCharacterEnemy.YPosition).SetToPlayerRender(this.PvpCharacterEnemy.HiddenAlpha);

		if(this.CurrentCharacter.UserType != this.PvpCharacterSelf.UserType)
		{
			Debug.Log ("战斗回合结束 ！！！！！！！！！");
			if(this.messageCallback_102 != null) this.messageCallback_102();
		}else
		{
			// 切换用户
			this.GameRoundCharacterChange();
			// 执行消息
			PvpMessageManager.Run();
		}

		// 切换用户
		//this.GameRoundCharacterChange();

		/*
		// 如果没有消息池数据
		if(PvpMessageManager.MessageCount <= 0)
		{
			// 切换用户
			this.GameRoundCharacterChange();
		}else
		{
			PvpMessageManager.Run();
		}*/
	}

	public void GameRoundCharacterChange(Action callback = null)
	{
		this.messageCallback_103 = callback;

		// 如果当前是自己，切换到敌人
		if(this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			this.StartCoolingTimer(this.GetCoolTime(this.PvpCharacterEnemy), false);
			
			// 如果是上面角色行动，切换到自己
			this.CurrentCharacter = this.PvpCharacterEnemy;
			this.CurCharacterEliminate = FineEliminate(this.PvpCharacterEnemy.XPosition, this.PvpCharacterEnemy.YPosition);
			
			this.GameRoundBegin(this.CurrentCharacter);
		}else
		{
			this.StartCoolingTimer(this.GetCoolTime(this.PvpCharacterSelf), true);
			
			// 如果是上面角色行动，切换到自己
			this.CurrentCharacter = this.PvpCharacterSelf;
			this.CurCharacterEliminate = FineEliminate(this.PvpCharacterSelf.XPosition, this.PvpCharacterSelf.YPosition);
			
			this.GameRoundBegin(this.CurrentCharacter);
		}
	}

	public void GameReboundCharacterChange()
	{
		// 轮到敌人行动了
		this.CurrentCharacter = this.PvpCharacterEnemy;
		
		this.StartCoolingTimer(this.GetCoolTime(this.CurrentCharacter), false);
		this.CurCharacterEliminate = FineEliminate(this.CurrentCharacter.XPosition, this.CurrentCharacter.YPosition);
		this.GameRoundBegin(this.CurrentCharacter);
	}

	private IEnumerator CanNotMoveProgress(float waitTime)
	{
		// 延迟 1 秒
		yield return new WaitForSeconds (waitTime);

		this.CurrentCharacter = this.PvpCharacterSelf;
		this.CurCharacterEliminate = FineEliminate(this.PvpCharacterSelf.XPosition, this.PvpCharacterSelf.YPosition);

		this.GameActionBegin(this.CurrentCharacter, true);
	}

	public List<PvpMonster> FightMonsterList;
	public int FightMonsterIndex;
	public Dictionary<int, List<PvpPathData>> monsterPathDict;
	public Action FightMonsterCallback;

	/// <summary>
	/// 计算行走格子
	/// </summary>
	/// <returns>The round path list.</returns>
	/// <param name="pvpFightUnit">Pvp fight unit.</param>
	public Dictionary<int, List<PvpPathData>> MonsterRoundPathList(PvpFightUnit pvpFightUnit, List<PvpMonster> fightMonsterList, PvpEliminate sourceItem)
	{
		PvpFightUnit pvpEnemyUnit = this.GetFightUnitByUserType (pvpFightUnit);
		
		AStarNode endNode = this.aStarUtils.GetNode(pvpEnemyUnit.XPosition, pvpEnemyUnit.YPosition);
		// 设置为可以行走
		endNode.walkable = true;

		// 设置角色当前所在格子可以行走，行走之后，这个格子是可以行走的，因为提前计算行走路径，所以需要事先更改状态
		this.aStarUtils.GetNode (this.PvpCharacterSelf.XPosition, this.PvpCharacterSelf.YPosition).walkable = true;

		// 设置自己的终点位置不可以行走，角色目标的位置需要设置为不可以行走，因为召唤怪物在角色行动之后，所以需要事先更改状态
		this.aStarUtils.GetNode (sourceItem.XPosition, sourceItem.YPosition).walkable = false;
		
		Dictionary<int, List<PvpPathData>> monsterPathData = new Dictionary<int, List<PvpPathData>> ();

		int index = 0;
		foreach(PvpMonster pvpMonster in fightMonsterList)
		{
			List<PvpPathData> pathList = this.MonsterPathList(pvpMonster, endNode, sourceItem);
			PvpPathData endPathData = pathList[pathList.Count - 1];
			// 把这个怪物的行走目标位置设置为不可以行走，这样可以保证怪物不穿透，怪物不重叠
			this.aStarUtils.GetNode(endPathData.x, endPathData.y).walkable = false;
			
			monsterPathData.Add(index, pathList);

			index ++;
		}
		
		// 设置为不可以行走
		endNode.walkable = false;

		return monsterPathData;
	}

	public void MonsterRound(PvpFightUnit pvpFightUnit, Action callback)
	{
		this.FightMonsterCallback = callback;
		this.FightMonsterIndex = 0;

		if(this.FightMonsterList == null || this.FightMonsterList.Count == 0)
		{
			if(callback != null) callback();
			return;
		}

		int index = 0;
		foreach(PvpMonster pvpMonster in this.FightMonsterList)
		{
			this.StartCoroutine(this.MonsterRoundEnumerator(pvpMonster, index));
			index ++;
		}
	}

	IEnumerator MonsterRoundEnumerator(PvpMonster pvpMonster, int index)
	{
		// 重置怪脚下的格子颜色
		this.ChangeMonsterStatus (pvpMonster, true);
		this.aStarUtils.GetNode(pvpMonster.XPosition, pvpMonster.YPosition).walkable = true;

		if(!this.monsterPathDict.ContainsKey(index))
		{
			// 调用回调
			this.MonsterRoundCallback();
			yield break;
		}

		List<PvpPathData> pathList = this.monsterPathDict [index];
		
		List<PvpEliminate> pathEliminateList = new List<PvpEliminate> ();
		foreach(PvpPathData pvpPathData in pathList)
		{
			pathEliminateList.Add(this.FineEliminate(pvpPathData.x, pvpPathData.y));
		}
		
		// 执行完之后，继续执行下一个
		pvpMonster.OwnAction(()=>
		{
			// 设置行走之后格子为不可以行走
			this.aStarUtils.GetNode(pvpMonster.XPosition, pvpMonster.YPosition).walkable = false;
			// 设置怪物脚下格子的颜色
			this.ChangeMonsterStatus(pvpMonster, false);
			// 设置怪物状态
			pvpMonster.UnitWaiting(pvpMonster.CurFaceDirection);
			// 调用回调
			this.MonsterRoundCallback();
		}, pathEliminateList);
		
		yield return null;
	}

	private void MonsterRoundCallback()
	{
		this.FightMonsterIndex ++;
		if(this.FightMonsterIndex >= this.FightMonsterList.Count)
		{
			if(this.FightMonsterCallback != null) this.FightMonsterCallback();
		}
	}

	/// <summary>
	/// 查找怪物行走路线
	/// </summary>
	/// <returns>The path list.</returns>
	/// <param name="pvpMonster">Pvp monster.</param>
	private List<PvpPathData> MonsterPathList(PvpMonster pvpMonster, AStarNode endNode, PvpEliminate sourceItem)
	{
		List<PvpPathData> resultList = new List<PvpPathData> ();
		// 如果不可以行走
		if(pvpMonster.FightRunPower == 0)
		{
			resultList.Add(new PvpPathData(pvpMonster.XPosition, pvpMonster.YPosition));
			return resultList;
		}
		else
		{
			AStarNode beginNode = this.aStarUtils.GetNode(pvpMonster.XPosition, pvpMonster.YPosition);
			PvpFightUnit pvpFightUnit = this.GetFightUnitByUserType (pvpMonster);

			IList<AStarNode> pathList = this.aStarUtils.FindPath (beginNode, endNode);
			if(pathList == null)
			{
				resultList.Add(new PvpPathData(pvpMonster.XPosition, pvpMonster.YPosition));
				return resultList;
			}

			for(int index = pathList.Count - 1; index >= 0; index --)
			{
				AStarNode aStarNode = pathList[index];
				if(resultList.Count <= pvpMonster.FightRunPower)
				{
					resultList.Add(new PvpPathData(aStarNode.nodeX, aStarNode.nodeY));
				}
				else
				{
					break;
				}
			}
			
			return resultList;
		}
	}

    public Dictionary<int, List<bool>> PrepareFight(PvpFightUnit pvpFightUnit)
    {
		Dictionary<int, List<bool>> dictList = new Dictionary<int, List<bool>> ();

		HardWareData.HardWareType ht = pvpFightUnit.PvpUserInfo.CurWeapon.CurHardWareData.Style;
		// 如果是远程武器
		if(ht == HardWareData.HardWareType.Far1 || ht == HardWareData.HardWareType.Far2)
		{
			PvpOwnUnit pvpOwnUnit = pvpFightUnit as PvpOwnUnit;
			// 如果走的值未达到条件。
			if(pvpOwnUnit == null || pvpOwnUnit.ChainSkill.Bparameter >= CurPathEliminate.Count) return dictList;

			List<bool> dataList = new List<bool>();
			// 远程攻击敌人列表
			List<PvpFightUnit> fightUnits = pvpOwnUnit.GetFarChainSkillAim();
			for(int index = 0; index < fightUnits.Count; index ++)
			{
				PvpFightUnit fightUnitItem = fightUnits[index];
				// 如果不是自己
				if(fightUnitItem.XPosition != pvpFightUnit.XPosition || fightUnitItem.YPosition != pvpFightUnit.YPosition)
				{
					// 添加闪避的状态
					dataList.Add(!fightUnitItem.CritAndAvoidStatusCheck(pvpFightUnit, CurPathEliminate.Count, index));
				}
			}
			// 添加数据
			dictList.Add(CurPathEliminate.Count - 1, dataList);
		}
		else
		{ // 单手、双手武器
			// 遍历步骤
			for (int index = 0; index < CurPathEliminate.Count; index ++)
	        {
				PvpEliminate pe = CurPathEliminate[index];
				// 设置虚拟步骤数据
	            //this.CurrentCharacter.SetPrePosition(pe.XPosition,pe.YPosition);
				// 添加闪避的状态
				dictList.Add(index, this.CurrentCharacter.FindPreFightEnemies(index, pe.XPosition, pe.YPosition));
	        }
		}
		return dictList;
    }

	public List<PvpEgg> PrepareEgg(PvpFightUnit pvpFightUnit)
	{
		// 重置临时数据
		int barrierCount = this.AllBarrier.Count;
		for(int barrierIndex = 0; barrierIndex < barrierCount; barrierIndex ++)
		{
			if(this.AllBarrier[barrierIndex].GetType() == typeof(PvpEgg))
			{
				this.AllBarrier[barrierIndex].VarHp = this.AllBarrier[barrierIndex].CurHp;
			}
		}

		// 处理角色伤害数据
		HardWareData.HardWareType ht = pvpFightUnit.PvpUserInfo.CurWeapon.CurHardWareData.Style;
		// 如果是远程武器
		if(ht == HardWareData.HardWareType.Far1 || ht == HardWareData.HardWareType.Far2)
		{
			PvpOwnUnit pvpOwnUnit = pvpFightUnit as PvpOwnUnit;
			// 如果走的值未达到条件。
			if(pvpOwnUnit != null && pvpOwnUnit.ChainSkill.Bparameter < CurPathEliminate.Count)
			{
				PvpEliminate endPe = this.CurPathEliminate[this.CurPathEliminate.Count - 1];

				List<bool> dataList = new List<bool>();
				// 远程攻击敌人列表
				List<PvpEgg> eggList = pvpOwnUnit.GetFarChainSkillAimEgg(endPe.XPosition, endPe.YPosition);

				for(int index = 0; index < eggList.Count; index ++)
				{
					PvpEgg eggItem = eggList[index];
					eggItem.VarHp -= 1f;
				}
			}
		}
		else
		{ // 单手、双手武器
			// 遍历步骤
			int pathCount = this.CurPathEliminate.Count;
			for (int index = 0; index < pathCount; index ++)
			{
				PvpEliminate pe = this.CurPathEliminate[index];
				List<PvpEgg> eggList = this.CurrentCharacter.FindPreFightEggs(index, pe.XPosition, pe.YPosition, index == pathCount - 1);
				for(int eggIndex = 0; eggIndex < eggList.Count; eggIndex ++)
				{
					PvpEgg eggItem = eggList[eggIndex];
					eggItem.VarHp -= 1f;
				}
			}
		}

		// 处理宠物伤害数据
		List<PvpPet> petList = this.GetActionPetList ();
		int petCount = petList.Count;
		for(int petIndex = 0; petIndex < petCount; petIndex ++)
		{
			int pathCount = this.CurPathEliminate.Count;
			for (int index = 0; index < pathCount; index ++)
			{
				PvpEliminate pe = this.CurPathEliminate[index];
				List<PvpEgg> eggList = petList[petIndex].FindPreFightEggs(index, pe.XPosition, pe.YPosition);
				for(int eggIndex = 0; eggIndex < eggList.Count; eggIndex ++)
				{
					PvpEgg eggItem = eggList[eggIndex];
					eggItem.VarHp -= 1f;
				}
			}
		}

		List<PvpEgg> resultList = new List<PvpEgg> ();
		// 检测蛋的 HP 值
		foreach(PvpFightUnit unitItem in this.AllBarrier)
		{
			if(unitItem.GetType() == typeof(PvpEgg))
			{
				if(unitItem.VarHp <= 0) resultList.Add(unitItem as PvpEgg);
			}
		}

		return resultList;
	}
    #endregion

    #region 消除
    /// <summary>
    /// 移动设备输入
    /// </summary>
    void MobileCheckTouch()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                RayBumpObjectCheck(Input.GetTouch(0).position, TouchType.Enter); return;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                RayBumpObjectCheck(Input.GetTouch(0).position, TouchType.Hold); return;
            }
        }
        if (Input.touchCount >= 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                RayBumpObjectCheck(Input.GetTouch(0).position, TouchType.Out); return;
            }
        }
    }

    /// <summary>
    /// 鼠标输入
    /// </summary>
    void CheckTouch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RayBumpObjectCheck(Input.mousePosition, TouchType.Enter); return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            RayBumpObjectCheck(Input.mousePosition, TouchType.Out); return;
        }
        if (Input.GetMouseButton(0))
        {
            RayBumpObjectCheck(Input.mousePosition, TouchType.Hold); return;
        }
    }
    //touch屏幕操作连线  开始 ..结束
    float LinePreTime = 0f;

    public List<PvpEliminate> CurPathEliminate = new List<PvpEliminate>();
    bool isLinking = false;
    PvpEliminate BasicEliminate = null;
	GameObject lastHitObject = null;
    void RayBumpObjectCheck(Vector3 touchPosition, TouchType type)
    {
		// 如果碰到了 UI 界面
		if(UICamera.hoveredObject != null) return;
        if (UserInputLock == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchPosition), Vector2.zero);
            if (hit)
            {
				GameObject hitObject = hit.collider.gameObject;
				/*
				if(type != TouchType.Out)
				{
					if(lastHitObject != null && hitObject == lastHitObject) return;
					lastHitObject = hitObject;
				}*/

                PvpEliminate hitEliminateBlock = hitObject.GetComponent<PvpEliminate>();
                if (hitEliminateBlock)
                {
                    if (type == TouchType.Enter)
                    {
						// 开始
						TryBeginLink(hitEliminateBlock);
                    }
                    else if (type == TouchType.Hold)
                    {
                        if (CurPathEliminate.Contains(hitEliminateBlock))
                        {
                            //如方块列表中含当前方块，后退到该方块
                            TryBackLink(hitEliminateBlock);
                        }
                        else
                        {
                            //连线
                            TryLink(hitEliminateBlock);
                        }
                    }
                    else if (type == TouchType.Out)
                    {
                        //结束
                        TryEndLink();
                        LinePreTime = Time.realtimeSinceStartup;
                    }
                }
            }
            else
            {
                if (type == TouchType.Out)
                {
                    TryEndLink();
                }
            }
        }
    }

    //开始连线
    void TryBeginLink(PvpEliminate touchBlock)
    {
        if (SkillConfirm.gameObject.activeInHierarchy == false)
        {
            if (touchBlock.CurEliminateAttribute == DungeonEnum.ElementAttributes.Player)
            {
                //起始点击主角所在方砖
                //CurCharacter.ShowArrow(false);
                isLinking = true;
            }
            else if (touchBlock.IsNeighbour(CurCharacterEliminate) && !HasEnemyOnEliminate(touchBlock))
            {
                //起始点击主角周围方砖
                //有同颜色且没敌人的相邻

                //CurCharacter.ShowArrow(false);
                BasicEliminate = touchBlock;
				this.CurrentPathEliminateInsert(CurCharacterEliminate);
                
                CurCharacterEliminate.NextLinkEliminate = touchBlock;
				this.CurrentPathEliminateInsert(touchBlock);
                
                touchBlock.LastLinkEliminate = CurCharacterEliminate;
                ShowPossibleLinkPath();
                isLinking = true;
                CurCharacterEliminate.LinkToNextRender();
				this.SetFocusPetAvata(BasicEliminate.CurEliminateAttribute, CurPathEliminate.Count - 1);
            }
        }
    }

    void TryEndLink()
    {
		//Debug.Log ("有没有到这儿来？？？？？？？？ ！！！！！！ ");
        if (isLinking)
        {
            float curLineTime = Time.realtimeSinceStartup;
            float spaceTime = curLineTime - LinePreTime;

            if (spaceTime < 0.3f && CurPathEliminate.Count <= 1)
            {
				this.CurrentPathEliminateInsert(CurCharacterEliminate);
                this.GameActionBegin(this.CurrentCharacter, true);
            }
            else if (CurPathEliminate.Count > 1)
            {
                //连线结束  角色开始行动
				this.GameActionBegin(this.CurrentCharacter, true);
            }
            else
            {
				// 判断是否点击在 NGUI 上面，
				if(UICamera.hoveredObject != null) return;
                //连线方块不足  复原
                //CurCharacter.ShowArrow(true);
                foreach (PvpEliminate pe in AllEliminates)
                {
                    pe.SetEnabelRender(true);
				}
				
				// 设置障碍物脚底下的颜色
				this.ResetMonsterAndEggRender();

				/*this.SetDisFocusPetAvata(BasicEliminate.CurEliminateAttribute, -1);
				if (CurPathEliminate.Count > 2)
				{
					foreach (PvpEliminate pe in CurPathEliminate)
					{
						pe.UnLinkFromLastRender(true);
						pe.LinkChainLabel.gameObject.SetActive(false);
					}
				}
				foreach (PvpEliminate pe in AllEliminates)
				{
					pe.SetEnabelRender(true);
				}*/

            }
            ClearLastLink();
        }
    }

	void ResetMonsterAndEggRender()
	{
		foreach(PvpFightUnit fightUnit in this.AllBarrier)
		{
			if(fightUnit.GetType() == typeof(PvpMonster) || fightUnit.GetType() == typeof(PvpEgg))
			{
				this.FineEliminate(fightUnit.XPosition, fightUnit.YPosition).SetEnabelRender(false);
			}
		}
	}

	void CurrentPathEliminateInsert(PvpEliminate eliminateItem)
	{
		// 如果没有添加过，添加
		if(CurPathEliminate.IndexOf(eliminateItem) == -1)
		{
			CurPathEliminate.Add(eliminateItem);
		}
	}

    void TryBackLink(PvpEliminate touchBlock)
    {
        if (CurPathEliminate.Count > 0)
        {
            PvpEliminate endEliminate = CurPathEliminate[CurPathEliminate.Count - 1];
            if (touchBlock.NextLinkEliminate == endEliminate)
            {
                //倒着依次返回
                CurPathEliminate.RemoveAt(CurPathEliminate.Count - 1);
                touchBlock.UnLinkFromLastRender(true);
                if (touchBlock != CurCharacterEliminate)
                {
					this.SetDisFocusPetAvata(BasicEliminate.CurEliminateAttribute, CurPathEliminate.Count - 1);
                }
                endEliminate.LastLinkEliminate = null;
                touchBlock.NextLinkEliminate = null;
                ShowCurRange(FindPvpTile(touchBlock.XPosition, touchBlock.YPosition));
            }
            if (touchBlock == CurCharacterEliminate)
            {
				this.SetDisFocusPetAvata(BasicEliminate.CurEliminateAttribute, -1);
				if (CurPathEliminate.Count > 2)
				{
					foreach (PvpEliminate pe in CurPathEliminate)
					{
						pe.UnLinkFromLastRender(true);
						pe.LinkChainLabel.gameObject.SetActive(false);
					}
				}
				foreach (PvpEliminate pe in AllEliminates)
				{
					pe.SetEnabelRender(true);
				}
				
				// 设置障碍物脚底下的颜色
				this.ResetMonsterAndEggRender();

				ClearLastLink();
				isLinking = true;
				ShowCurRange(FindPvpTile(touchBlock.XPosition, touchBlock.YPosition));
            }
        }
    }

    void TryLink(PvpEliminate touchBlock)
    {
		if (isLinking && !HasEnemyOnEliminate(touchBlock) && touchBlock.CurEliminateAttribute != DungeonEnum.ElementAttributes.Player)
        {
            if (BasicEliminate == null && touchBlock.IsNeighbour(CurCharacterEliminate))
            {
                BasicEliminate = touchBlock;
				this.CurrentPathEliminateInsert(CurCharacterEliminate);
                
                CurCharacterEliminate.NextLinkEliminate = touchBlock;
				this.CurrentPathEliminateInsert(touchBlock);
                
                touchBlock.LastLinkEliminate = CurCharacterEliminate;
                ShowPossibleLinkPath();
                CurCharacterEliminate.LinkToNextRender();
				this.SetFocusPetAvata(BasicEliminate.CurEliminateAttribute, CurPathEliminate.Count - 1);
                ShowCurRange(FindPvpTile(touchBlock.XPosition, touchBlock.YPosition));
            }
            else if (BasicEliminate != null && BasicEliminate.CurEliminateAttribute == touchBlock.CurEliminateAttribute && touchBlock.IsNeighbour(CurPathEliminate[CurPathEliminate.Count - 1]))
            {
                CurPathEliminate[CurPathEliminate.Count - 1].NextLinkEliminate = touchBlock;
                touchBlock.LastLinkEliminate = CurPathEliminate[CurPathEliminate.Count - 1];
				this.CurrentPathEliminateInsert(touchBlock);
                
                touchBlock.LastLinkEliminate.LinkToNextRender();
				this.SetFocusPetAvata(BasicEliminate.CurEliminateAttribute, CurPathEliminate.Count - 1);
                ShowCurRange(FindPvpTile(touchBlock.XPosition, touchBlock.YPosition));
            }

        }
    }

    List<PvpEliminate> CurPossiblePath = new List<PvpEliminate>();
    void ShowPossibleLinkPath()
    {
        List<PvpEliminate> beginBlocks = new List<PvpEliminate>();
        beginBlocks.Add(BasicEliminate);
        FindPath(beginBlocks, true);
        foreach (PvpEliminate pe in AllEliminates)
        {
            if (!CurPossiblePath.Contains(pe))
            {
                pe.SetEnabelRender(false);
            }
        }
        CurCharacterEliminate.SetEnabelRender(true);
    }

    /// <summary>
    /// 查找可以连接的路径
    /// </summary>
    void FindPath(List<PvpEliminate> blocks, bool isBegin)
    {
        foreach (PvpEliminate b in blocks)
        {
            DungeonEnum.ElementAttributes type = b.CurEliminateAttribute;
            if (isBegin)
            {
                type = CurCharacterEliminate.NextLinkEliminate.CurEliminateAttribute;
            }
            List<PvpEliminate> sameNeighbour = FindSameNeighbour(b, type);
            //过滤已经存在的
            List<PvpEliminate> NotExists = new List<PvpEliminate>();
            foreach (PvpEliminate b1 in sameNeighbour)
            {
                if (CurPossiblePath.Contains(b1) == false && !HasEnemyOnEliminate(b1))
                {
                    NotExists.Add(b1);
                    CurPossiblePath.Add(b1);
                }
            }
            if (NotExists.Count != 0)
            {
                // 继续寻路
                FindPath(NotExists, false);
            }
        }
    }

    List<PvpEliminate> FindSameNeighbour(PvpEliminate block, DungeonEnum.ElementAttributes type)
    {
		List<PvpEliminate> neighbours = new List<PvpEliminate>();
        foreach (PvpEliminate b in AllEliminates)
        {
            if ((block.XPosition == b.XPosition - 1 && block.YPosition == b.YPosition) ||
                (block.XPosition == b.XPosition - 1 && block.YPosition == b.YPosition + 1) ||
                (block.XPosition == b.XPosition - 1 && block.YPosition == b.YPosition - 1) ||
                (block.XPosition == b.XPosition + 1 && block.YPosition == b.YPosition) ||
                (block.XPosition == b.XPosition + 1 && block.YPosition == b.YPosition + 1) ||
                (block.XPosition == b.XPosition + 1 && block.YPosition == b.YPosition - 1) ||
                (block.XPosition == b.XPosition && block.YPosition == b.YPosition - 1) ||
                 (block.XPosition == b.XPosition && block.YPosition == b.YPosition + 1))
            {
                if (b.CurEliminateAttribute == type)
                {
                    neighbours.Add(b);
                }
            }
        }
        return neighbours;
    }

    void ClearLastLink()
    {
        foreach (PvpEliminate pe in CurPathEliminate)
        {
            pe.NextLinkEliminate = null;
            pe.LastLinkEliminate = null;
        }

        CurPathEliminate.Clear();
        CurPossiblePath.Clear();
        BasicEliminate = null;
    }
    #endregion

    #region 清理
    public void ClearEliminate(PvpEliminate pe)
    {
        AllEliminates.Remove(pe);
    }
    #endregion

    #region 工具方法
    DungeonEnum.FaceDirection ReverseDirection(DungeonEnum.FaceDirection direciton)
    {
        if (direciton == DungeonEnum.FaceDirection.Down)
        {
            return DungeonEnum.FaceDirection.Up;
        }
        else if (direciton == DungeonEnum.FaceDirection.Left)
        {
            return DungeonEnum.FaceDirection.Right;
        }
        else if (direciton == DungeonEnum.FaceDirection.LeftDown)
        {
            return DungeonEnum.FaceDirection.UpRight;
        }
        else if (direciton == DungeonEnum.FaceDirection.LeftUp)
        {
            return DungeonEnum.FaceDirection.RightDown;
        }
        else if (direciton == DungeonEnum.FaceDirection.Right)
        {
            return DungeonEnum.FaceDirection.Left;
        }
        else if (direciton == DungeonEnum.FaceDirection.RightDown)
        {
            return DungeonEnum.FaceDirection.LeftUp;
        }
        else if (direciton == DungeonEnum.FaceDirection.Up)
        {
            return DungeonEnum.FaceDirection.Down;
        }
        else if (direciton == DungeonEnum.FaceDirection.UpRight)
        {
            return DungeonEnum.FaceDirection.LeftDown;
        }
        return DungeonEnum.FaceDirection.None;
    }
    #endregion

    #region 胜负渲染

	private bool resultRenderStatus = false;
	private int fightResultValue = 0;

    //胜利特效
    public void RenderSuccess()
    {
		if(this.resultRenderStatus) return;

		if(this.PvpSurrender != null) this.PvpSurrender.SetActive (false);

		this.resultRenderStatus = true;
        PvpCharacterSelf.ChangeAnimation(DungeonEnum.FaceDirection.LeftDown, PvpFightUnit.ActionState.Victor);
		UserInputLock = true;
		
		//切换到胜利音乐
		AudioSource gm = GameObject.Find("UI Root").transform.GetComponent<AudioSource>();
		gm.Stop();
		
		GameObject gm_victor_music = new GameObject();
		gm_victor_music.name = "gm_victor_music";
		gm = gm_victor_music.AddComponent<AudioSource>();
		gm.clip = Resources.Load("Audio/BGM_war_victor") as AudioClip;
		DontDestroyOnLoad(gm_victor_music);
		gm.Play();

		PvpGameObjectManager.Create ("PreFabs/UI/Win", (GameObject resultGameObject) =>
		{
			//GameObject resultGameObject = Instantiate(Resources.Load("PreFabs/UI/Win")) as GameObject;
			UIEventListener.Get(resultGameObject).onClick = (obj) =>
			{
				AudioSource audioSourceItem = GameObject.Find("gm_victor_music").transform.GetComponent<AudioSource>();
				if(audioSourceItem != null)
				{
					audioSourceItem.Stop();
					GameObject.Destroy(audioSourceItem.gameObject);
				}
				// 如果消息没有返回
				if(!this.messageEndStatus)
				{
					// 设置面板状态
					this.fightPanelStatus = true;
					this.fightResultValue = 1;
					// 显示 loading 条
					ApplicationControl.CurApp.BeginLoading();
				}else
				{
					this.SendResultToServer(1);
				}
				//OverControl.isOver = true;
			};
		});

	}
	//失败
	public void RenderFail()
	{
		if(resultRenderStatus) return;

		if(this.PvpSurrender != null) this.PvpSurrender.SetActive (false);

		this.resultRenderStatus = true;

		AudioSource gm = GameObject.Find("UI Root").transform.GetComponent<AudioSource>();
		gm.Stop();

		UserInputLock = true;

		PvpGameObjectManager.Create("PreFabs/FX/Lose", (GameObject resultGameObject)=>
		{
			//GameObject resultGameObject = Instantiate(Resources.Load("PreFabs/FX/Lose")) as GameObject;
			UIEventListener.Get(resultGameObject).onClick = (obj) =>
			{
				// 如果消息没有返回
				if(!this.messageEndStatus)
				{
					// 设置面板状态
					this.fightPanelStatus = true;
					this.fightResultValue = 0;
					// 显示 loading 条
					ApplicationControl.CurApp.BeginLoading();
				}else
				{
					this.SendResultToServer(0);
				}
			};
		});
	}
	/// <summary>
	/// 向服务器端发送数据
	/// </summary>
	void SendMoveDataToServer()
	{
		// 添加 loading
		//this.StartCoroutine (this.SendMoveDataToServerCallback ());
		PvpConnect.MovePaths (this.PvpTableID, this.stepDataList, this.monsterDataList);
	}

	public static bool messageCallbackStatus;
	private IEnumerator SendMoveDataToServerCallback()
	{
		messageCallbackStatus = false;
		yield return new WaitForSeconds (0f);
		if(!messageCallbackStatus)
		{
			ApplicationControl.CurApp.BeginLoading();
		}
	}

    void SendResultToServer(int result)
    {
        JsonObject args = new JsonObject();
        args.Add("ticket_id", UserManager.CurUserInfo.ArenaPvpTicket);
        
        SocketCenter.Request(GameRouteConfig.PvpOver, args, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
					int starLevel = int.Parse(r.Data["star_level"].ToString());
					int starExp = int.Parse(r.Data["star_exp"].ToString());
					
					JsonArray fightData = (JsonArray)r.Data["elements"];

					// 任务信息
					JsonObject taskData = (JsonObject)r.Data["daily_task"];
					// 设置任务数据
					PvpOverUI.taskData = taskData;

					// 添加数据
					UserManager.CurUserInfo.AddElements(fightData);
					// 设置数据
					PvpOverUI.ChangeData(starLevel, starExp);

					// 设置场景跳转数据
					SceneDataManager.fromScene = SceneDataManager.PVP;

					// 销毁对象池
					this.SceneDispose();
					// 异步加载场景
					this.StartCoroutine(this.LoadMainScene());
                });
            }
        }, null, true, true);
    }

	private IEnumerator LoadMainScene()
	{
		// 设置 PVP 状态
		PvpGameControl.PvpStatus = false;

		AsyncOperation asyncOperation = Application.LoadLevelAsync ("MainScene");
		yield return asyncOperation;
	}

    #endregion

    #region 技能
    public PvpOwnUnit CurReadySkillUnit;
    public bool IsSkilling = false;
    public PvpPet FindPet(UserPet p)
	{
		for(int index = 0; index < this.AllOwns.Count; index ++)
		{
			List<PvpOwnUnit> unitList = this.AllOwns[index];

			foreach(PvpOwnUnit pvpOwnUnit in unitList)
			{
				if (pvpOwnUnit.GetType() == typeof(PvpPet))
				{
					PvpPet pp = (PvpPet)pvpOwnUnit;
					if (pp.CurUserPet == p)
					{
						return pp;
					}
				}
			}
		}
        return null;
    }

	public PvpFightUnit GetFightUnitByUserType(PvpFightUnit fightUnit)
	{
		if(fightUnit.UserType == this.PvpCharacterSelf.UserType) return this.PvpCharacterEnemy;
		return this.PvpCharacterSelf;
	}

	public List<PvpFightUnit> GetSkillFightUnitTarget(SkillData skillData, PvpFightUnit sourceItem)
	{
		List<PvpFightUnit> resultList = new List<PvpFightUnit> ();

		BaseSkillItem skillItem = sourceItem.pvpPlayerSkill.GetSkillItemBySkillID (skillData.Id);
		// 如果目标是地方主角
		if(skillItem.skillData.skillTarget == SkillTargetTypeEnum.Enemy_Player || skillItem.skillData.skillTarget == SkillTargetTypeEnum.Self_Player)
		{
			if(skillItem.skillData.skillTarget == SkillTargetTypeEnum.Enemy_Player)
			{
				PvpFightUnit targetItem = this.GetFightUnitByUserType(sourceItem);
				resultList.Add(targetItem);
			}else
			{
				resultList.Add(sourceItem);
			}
		}else if(skillItem.skillData.skillTarget == SkillTargetTypeEnum.Range)
		{
			this.CaculateSkillRangeTile(this.FindPvpTile(sourceItem.XPosition, sourceItem.YPosition), skillData);
			foreach (PvpTile t in this.AllPvpTiles)
			{
				PvpEliminate eb = this.FineEliminate(t.XPosition, t.YPosition);
				if (eb)
				{
					// 如果包含，
					if (this.AllSkillRangesTile.Contains(t))
					{
						PvpFightUnit pvpFightUnit = this.GetBarrierFightUnitByPosition(eb, sourceItem);
						// 这儿只判断地方主角
						if(pvpFightUnit != null) resultList.Add(pvpFightUnit);
					}
				}
			}
		}

		return resultList;
	}

	private PvpFightUnit GetBarrierFightUnitByPosition(PvpEliminate pvpEliminate, PvpFightUnit sourceItem)
	{
		foreach(PvpFightUnit fightUnit in this.AllBarrier)
		{
			if(fightUnit.UserType != sourceItem.UserType || fightUnit.GetType() == typeof(PvpEgg))
			{
				if(fightUnit.XPosition == pvpEliminate.XPosition && fightUnit.YPosition == pvpEliminate.YPosition) return fightUnit;
			}
		}

		return null;
	}

    public List<PvpTile> AllSkillRangesTile = new List<PvpTile>();

    public void CaculateSkillRangeTile(PvpTile block, SkillData skillData)
    {
        AllSkillRangesTile.Clear();
		this.AllSkillRangesTile = this.CaculateSkillRangeTileList (block.XPosition, block.YPosition, skillData);
    }

	public List<PvpTile> CaculateSkillRangeTileList(int xPosition, int yPosition, SkillData skillData)
	{
		List<PvpTile> resultList = new List<PvpTile> ();
		resultList.Add (this.FindPvpTile (xPosition, yPosition));

		int centerX = xPosition;
		int centerY = yPosition;
		//解析范围配置
		List<string> RangeConfigs = new List<string>();
		string pattern = "\\((.*?)\\)";
		MatchCollection mc = Regex.Matches(skillData.Xparameter, pattern);
		foreach (Match m in mc)
		{
			RangeConfigs.Add(m.Groups[1].Value);
		}
		foreach (string range in RangeConfigs)
		{
			string[] postionS = range.Split(',');
			int offX = int.Parse(postionS[0]);
			int offY = int.Parse(postionS[1]);
			int realX = centerX + offX;
			int realY = centerY + offY;
			PvpTile b = FindPvpTile(realX, realY);
			if (b != null)
			{
				resultList.Add(b);
			}
		}

		return resultList;
	}

    public int TotalPower
	{
		get{ return this.PvpUserInfoSelf.Pvp_SkillPower;}
		set{ this.PvpUserInfoSelf.Pvp_SkillPower = value; }
	}

    public void RecoverAllPetEnergy(PvpFightUnit fightUnit, bool b = true)
    {
		if (b)
		{
			ParamData paramData = ConfigManager.ParamConfig.GetParam();

			float powerAdd = fightUnit.pvpPlayerBuff.GetValueByBuffTypeToFloat(BuffTypeEnum.Recover_Energy, false);

			//Debug.Log("能量数据加成：：：：：" + powerAdd);

			fightUnit.PvpUserInfo.Pvp_SkillPower += (int)paramData.GlobalSkillPowerGrowthRate + (int)powerAdd;
		}

		// 更新自己的能量
		this.SetSkillPower (fightUnit);

		if(fightUnit.UserType == this.PvpCharacterSelf.UserType)
		{
	        foreach (PvpOwnUnit o in AllOwns[0])
	        {
	            if (o.GetType() == typeof(PvpPet))
	            {
	                PvpPet p = (PvpPet)o;
	                p.RecoverPower();
	            }
	        }
		}
    }

	private void ResetPowerAvatar(bool status)
	{
		foreach(PvpOwnUnit o in AllOwns[0])
		{
			if(o.GetType() == typeof(PvpPet))
			{
				PvpPet p = (PvpPet)o;
				if(status)
				{
					p.RecoverPower();
				}else{
					p.ResetPetPowerAvatar();
				}
			}
		}
	}

	public void UseSkillWithPetFly()
	{
		PvpPet pt = this.CurReadySkillUnit as PvpPet;
		pt.gameObject.SetActive(true);

		this.IsSkilling = true;
		foreach (PvpEliminate eb in AllEliminates)
		{
			eb.ReturnToCommon();
		}
		// 设置当前角色隐藏
		this.CurrentCharacter.gameObject.SetActive (false);
		pt.PetFlyToScene(true, () =>
		                 {
			pt.UnitWaiting(DungeonEnum.FaceDirection.Down);
			pt.UnitAttack( pt.CurFaceDirection);
		});
	}
	public void EndUseSkillWithPetFly()
	{
		PvpPet pt = this.CurReadySkillUnit as PvpPet;
		pt.PetBackToAvata(() =>
		{
			this.CurrentCharacter.gameObject.SetActive(true);
			pt.gameObject.SetActive(false);
		});
	}

	public void UseSkillTransmit(BaseSkillItem skillItem, PvpFightUnit fightUnit, int oriXPosition, int oriYPosition, int xPosition, int yPosition)
	{
		PvpGameObjectManager.Create ("PreFabs/FX/Skill_03", (GameObject objectItem) =>
		{
			fightUnit.SetPosition (xPosition, yPosition);

			objectItem.transform.parent = fightUnit.transform.parent;
			objectItem.transform.localPosition = fightUnit.transform.localPosition;

			this.FineEliminate (oriXPosition, oriYPosition).ResetRenderPlayerToElement();
			this.FineEliminate (xPosition, yPosition).SetToPlayerRender ();
			
			// 设置当前所在格子
			this.CurCharacterEliminate = this.FineEliminate (xPosition, yPosition);

			List<PvpOwnUnit> petList = this.AllOwns[0];
			if(this.CurrentCharacter.UserType != this.PvpCharacterSelf.UserType)
			{
				petList = this.AllOwns[1];
			}

			foreach(PvpOwnUnit petUnit in petList)
			{
				if(petUnit.GetType() == typeof(PvpPet))
				{
					petUnit.SetPosition(xPosition, yPosition);
				}
			}
		});
	}
	
	public void UseSkillEnd()
	{
		UserInputLock = false;
		IsSkilling = false;
		
		foreach (PvpEliminate eb in AllEliminates)
		{
			eb.ReturnToCommon();
		}
	}

    //宠物使用技能
    SkillData curPetSkill;
	public bool skillSelfStatus;

    public void UseSkill(SkillData sd, bool selfStatus = true, bool cdStatus = true)
	{
		this.fightStep = PvpFightStep.MAGIC;

		// 清除连线
		this.ClearLastLink();
		this.skillSelfStatus = selfStatus;

		// 显示技能名称
		SkillFlyManager.Run (sd.Name, this.SkillFlyObject);

		// 显示施法动作
		if(this.skillSelfStatus)
		{
			this.PvpCharacterSelf.Magic();
		}else{
			this.PvpCharacterEnemy.Magic();
		}

        curPetSkill = sd;
        UserInputLock = true;
        IsSkilling = true;

        foreach (PvpEliminate eb in AllEliminates)
        {
            eb.ReturnToCommon();
        }

		PvpGameObjectManager.Create("PreFabs/FX/before_skill", (GameObject shineObject)=>
		{
			//宠物 立即释放技能      
			//GameObject shine = Instantiate(Resources.Load("PreFabs/FX/before_skill")) as GameObject;
			if(this.skillSelfStatus)
			{
				shineObject.transform.position = this.PvpCharacterSelf.transform.position;
			}else{
				shineObject.transform.position = this.PvpCharacterEnemy.transform.position;
			}
			shineObject.SetActive(true);
			// 调用协程
			this.StartCoroutine (this.ShineEnd (cdStatus, this.skillSelfStatus));
		});
    }

    IEnumerator ShineEnd(bool cdStatus, bool skillSelfStatus)
    {
		yield return new WaitForSeconds (0.6f);

		if(cdStatus)
		{
			if(skillSelfStatus)
			{
				this.PvpCharacterSelf.PvpUserInfo.Pvp_SkillPower -= this.curPetSkill.SkillPower;
				RecoverAllPetEnergy(this.PvpCharacterSelf, false);
				this.SetSkillPower(this.PvpCharacterSelf);
			}else{
				this.PvpCharacterEnemy.PvpUserInfo.Pvp_SkillPower -= this.curPetSkill.SkillPower;
				RecoverAllPetEnergy(this.PvpCharacterEnemy, false);
				this.SetSkillPower(this.PvpCharacterEnemy);
			}
		}

		this.IsSkilling = false;
		this.ShowSkillBack(false);
		
		if(cdStatus && skillSelfStatus)
		{
			// 向后端请求数据
			PvpConnect.Skill(this.PvpTableID, this.GetHouseID());
		}
		
		// 如果是自己，
		if(skillSelfStatus)
		{
			SkillManager.Trigger(curPetSkill, 0, this.PvpCharacterSelf, cdStatus, this.UseSkillCallback);
		}else{
			SkillManager.Trigger(curPetSkill, 0, this.PvpCharacterEnemy, cdStatus, this.UseSkillCallback);
		}
    }

	public int GetHouseID()
	{
		if(this.CurrentCharacter.UserType == this.PvpCharacterSelf.UserType)
		{
			PvpPet pvpPet = this.CurReadySkillUnit as PvpPet;
			if(pvpPet != null)
			{
				return pvpPet.CurUserPet.UserPetId;
			}
		}else{

		}
		return -1;
	}

	private void UseSkillCallback()
	{
		// 只有最后技能执行完之后才进行回调
		if(SkillManager.RepeatMagicCount <= 0)
		{
			this.PvpPlayerInfo.RefreshSkillCd(this.PvpCharacterSelf, this.PvpCharacterEnemy);

			List<PvpFightUnit> deadList = new List<PvpFightUnit>();
			foreach(PvpFightUnit pvpFightUnit in this.AllBarrier)
			{
				if(pvpFightUnit.GetType() == typeof(PvpEgg) || pvpFightUnit.GetType() == typeof(PvpMonster))
				{
					if(pvpFightUnit.CurHp <= 0) deadList.Add(pvpFightUnit);
				}
			}

			if(deadList != null && deadList.Count > 0)
			{
				this.StartCoroutine(this.UseSkillCallbackEnumerator(deadList));
			}else
			{
				// 只有自己才解锁
				if(this.skillSelfStatus)
				{
					UserInputLock = false;
				}

				// 战斗状态
				this.fightStep = PvpFightStep.ROUND_BEGIN;
				Debug.Log("UserInputLock Status : " + this.UserInputLock);
				// 执行消息队列
				PvpMessageManager.Run();
			}
		}
	}

	private IEnumerator UseSkillCallbackEnumerator(List<PvpFightUnit> deadList)
	{
		yield return new WaitForSeconds (1f);

		int deadIndex = 0;
		foreach(PvpFightUnit pvpFightUnit in deadList)
		{
			deadIndex ++;
			if(pvpFightUnit.GetType() == typeof(PvpEgg))
			{
				PvpEgg pvpEgg = pvpFightUnit as PvpEgg;
				this.CreateMonster(pvpEgg.SkillItem, new Vector3(pvpEgg.XPosition, pvpEgg.YPosition, 0f), pvpEgg.summonMonsterID, this.skillSelfStatus ? this.PvpCharacterSelf : this.PvpCharacterEnemy, 999, true);
			}
			this.FightBarrierDeadProgressItem(pvpFightUnit, true, ()=>
			                                  {
				deadIndex --;
				// 如果是蛋，显示怪物
				if(deadIndex <= 0)
				{
					for(int monsterIndex = 0; monsterIndex < this.AllBarrier.Count; monsterIndex ++)
					{
						PvpMonster pvpMonsterItem = this.AllBarrier[monsterIndex] as PvpMonster;
						if(pvpMonsterItem != null && pvpMonsterItem.HiddenStatus)
						{
							this.CreateMonsterCallback(pvpMonsterItem, null);
						}
					}

					// 只有自己才解锁
					if(this.skillSelfStatus)
					{
						UserInputLock = false;
					}
					
					// 战斗状态
					this.fightStep = PvpFightStep.ROUND_BEGIN;
					Debug.Log("UserInputLock Status : " + this.UserInputLock);
					// 执行消息队列
					PvpMessageManager.Run();
				}
			});
		}
	}

	/// <summary>
	/// 弹回处理
	/// </summary>
	/// <param name="buffStatus">If set to <c>true</c> buff status.</param>
	public void ReboundCharacterAction(Action callback = null)
	{
		// 弹回从服务器端拉取数据
		PvpConnect.ReBound (this.PvpTableID, (r)=>
		{
			Loom.QueueOnMainThread(() =>
			{
				// 设置弹回数据
				PvpReboundManager.reboundData = r["table_info"] as JsonObject;
				// 这个时候只会在战斗状态，需要绘制格子
				PvpReboundManager.Progress(this, PvpReboundManager.reboundData, true);
				// 调用回调函数
				if(callback != null) callback();
			});
		});
	}

    #region 技能背景
    public void ShowSkillBack(bool show)
    {
		// 不需要了。
		return;
        SkillBack.SetActive(show);
    }
    #endregion

    /// <summary>
    /// 显示当前技能范围
    /// </summary>
    /// <param name="block"></param>
    void ShowCurRange(PvpTile block)
    {
		if (this.CurrentCharacter.ChainSkill != null && (this.CurrentCharacter.ChainSkill.Bparameter < CurPathEliminate.Count))
        {
			//Debug.Log("终结技能范围 ：：：" + this.CurrentCharacter.ChainSkill.Bparameter);
			//Debug.Log("终结技能范围 ：：：" + CurPathEliminate.Count);
            DisShowAllRange();
            CaculateSkillRangeTile(block, this.CurrentCharacter.ChainSkill);
            foreach (PvpTile b in AllSkillRangesTile)
            {
                if (FindNeighbourTileIn(b, AllSkillRangesTile).Count != 8)
                {
                    CaculateRangeLine(b, AllSkillRangesTile);
                }
            }

            /*Hashtable args = new Hashtable();
            args.Add("alpha", 0.4);
            args.Add("time", 0.3f);
            args.Add("easetype", iTween.EaseType.easeOutQuad);
            args.Add("looptype", iTween.LoopType.pingPong);
            iTween.FadeFrom(Ranges, args);*/
        }
        else
        {
            DisShowAllRange();
        }
    }

	public void PowerSkillRenderBumpAttack(int skillType, string skillPrefab, PvpCharacter targetItem, bool skillStatus = true)
	{
		if(skillType == SkillEffectTypeEnum.NONE) return;
		
		// 如果需要逐个格子演示
		if(skillType == SkillEffectTypeEnum.ITEM)
		{
			this.PowerSkillRenderBumpAttackSingle(skillPrefab, targetItem, skillStatus);
		}else{
			this.PowerSkillRenderBumpAttackWhole(skillPrefab, targetItem, skillStatus);
		}
	}
	
	private void PowerSkillRenderBumpAttackSingle(string skillPrefab, PvpCharacter targetItem, bool skillStatus)
	{
		foreach (PvpTile t in this.AllSkillRangesTile)
		{
			if (t.XPosition != targetItem.XPosition || t.YPosition != targetItem.YPosition)
			{
				this.StartCoroutine(this.PowerSkillRenderBumpAttackSingleItem(t, skillPrefab, targetItem, skillStatus));
				// 如果需要调用函数
				if(skillStatus) targetItem.Invoke("BumpEnd", 1);
			}
		}
	}
	
	private IEnumerator PowerSkillRenderBumpAttackSingleItem(PvpTile t, string skillPrefab, PvpCharacter targetItem, bool skillStatus)
	{
		yield return new WaitForSeconds(targetItem.Distance(targetItem, t) * 0.15f);

		// 创建效果
		PvpGameObjectManager.Create(DungeonSpritePathManager.SkillBumpFX(skillPrefab), (GameObject skillBump)=>
		{
			//GameObject skillResource = Resources.Load(DungeonSpritePathManager.SkillBumpFX(skillPrefab)) as GameObject;
			//GameObject skillBump = GameObject.Instantiate(skillResource) as GameObject;
			skillBump.transform.position = t.transform.position;
			skillBump.SetActive(true);
			
			if(skillStatus)
			{
				CameraControl.ShakeCamera();
				PvpFightUnit e = this.FindEnemyOn(t.XPosition, t.YPosition);
				if (e) e.SkillHurtRender(targetItem.ChainSkill, targetItem);
			}
		});
	}
	
	private void PowerSkillRenderBumpAttackWhole(string skillPrefab, PvpCharacter targetItem, bool skillStatus)
	{
		PvpGameObjectManager.Create(DungeonSpritePathManager.SkillBumpFX(skillPrefab), (GameObject skillBump)=>
		{
			//GameObject skillResource = Resources.Load(DungeonSpritePathManager.SkillBumpFX(skillPrefab)) as GameObject;
			//GameObject skillBump = GameObject.Instantiate(skillResource) as GameObject;
			skillBump.transform.position = targetItem.transform.position;
			skillBump.SetActive(true);
			if(skillStatus)
			{
				CameraControl.ShakeCamera();
				foreach (PvpTile t in this.AllSkillRangesTile)
				{
					PvpFightUnit e = this.FindEnemyOn(t.XPosition, t.YPosition);
					if (e)
					{
						e.SkillHurtRender(targetItem.ChainSkill, targetItem);
					}
					if (t.XPosition != targetItem.XPosition || t.YPosition != targetItem.YPosition)
					{
						targetItem.Invoke("BumpEnd", 1);
					}
				}
			}
		});
	}

    /// <summary>
    /// 计算范围线
    /// </summary>
    /// <param name="b"></param>
    /// <param name="allInRange"></param>
    public void CaculateRangeLine(PvpTile b, List<PvpTile> allInRange)
    {
        //计算直线
        CaculateLine(b, allInRange);
        CaculateConner(b, allInRange);
    }

    /// <summary>
    /// 计算直线
    /// </summary>
    /// <param name="b"></param>
    /// <param name="allInRange"></param>
    void CaculateLine(PvpTile b, List<PvpTile> allInRange)
    {
        if (FindOffset(0, 1, allInRange, b) && !FindOffset(-1, 0, allInRange, b) && !FindOffset(-1, 1, allInRange, b))
        {
            DrawRangeLine(b, LineDirection.vertical, BlockConner.LeftUp);
        }
        if (FindOffset(0, 1, allInRange, b) && !FindOffset(1, 0, allInRange, b) && !FindOffset(1, 1, allInRange, b))
        {
            DrawRangeLine(b, LineDirection.vertical, BlockConner.RightUp);
        }
        if (FindOffset(1, 0, allInRange, b) && !FindOffset(1, -1, allInRange, b) && !FindOffset(0, -1, allInRange, b))
        {
            DrawRangeLine(b, LineDirection.horizon, BlockConner.RightDown);
        }
        if (FindOffset(1, 0, allInRange, b) && !FindOffset(1, 1, allInRange, b) && !FindOffset(0, 1, allInRange, b))
        {
            DrawRangeLine(b, LineDirection.horizon, BlockConner.RightUp);
        }
    }

    /// <summary>
    /// 绘制直线范围
    /// </summary>
    /// <param name="b"></param>
    /// <param name="direction"></param>
    /// <param name="blockConner"></param>
    void DrawRangeLine(PvpTile b, LineDirection direction, BlockConner blockConner)
    {
        GameObject line = Instantiate(Line) as GameObject;
        line.transform.parent = Ranges.transform;
        line.transform.localPosition = b.transform.localPosition;
        line.transform.localScale = new Vector3(1, 1, 1);
        if (direction == LineDirection.horizon)
        {
            if (blockConner == BlockConner.RightDown)
            {
                line.transform.localPosition = new Vector3(line.transform.localPosition.x + 42, line.transform.localPosition.y - 42, line.transform.localPosition.z);
            }
            else if (blockConner == BlockConner.RightUp)
            {
                line.transform.localPosition = new Vector3(line.transform.localPosition.x + 42, line.transform.localPosition.y + 42, line.transform.localPosition.z);
            }
        }
        else
        {
            line.transform.localEulerAngles = new Vector3(0, 0, 90);
            if (blockConner == BlockConner.LeftUp)
            {
                line.transform.localPosition = new Vector3(line.transform.localPosition.x - 42, line.transform.localPosition.y + 42, line.transform.localPosition.z);
            }
            else if (blockConner == BlockConner.RightUp)
            {
                line.transform.localPosition = new Vector3(line.transform.localPosition.x + 42, line.transform.localPosition.y + 42, line.transform.localPosition.z);
            }
        }
    }

    /// <summary>
    /// 绘制曲线范围
    /// </summary>
    /// <param name="b"></param>
    /// <param name="direction"></param>
    /// <param name="reverse"></param>
    void DrawRangeConner(PvpTile b, ConnerDirection direction, bool reverse)
    {
        GameObject conner = Instantiate(Conner) as GameObject;
        conner.transform.parent = Ranges.transform;
        conner.transform.localPosition = b.transform.localPosition;
        conner.transform.localScale = new Vector3(1, 1, 1);
        if (direction == ConnerDirection.LeftDown)
        {
            if (reverse)
            {
                conner.transform.localPosition = new Vector3(conner.transform.localPosition.x - 84, conner.transform.localPosition.y - 84, conner.transform.localPosition.z);
                conner.transform.localEulerAngles = new Vector3(0, 0, -180);
            }
        }
        else if (direction == ConnerDirection.LeftUp)
        {
            if (reverse)
            {
                conner.transform.localPosition = new Vector3(conner.transform.localPosition.x - 84, conner.transform.localPosition.y + 84, conner.transform.localPosition.z);
                conner.transform.localEulerAngles = new Vector3(0, 0, 90);
            }
            else
            {
                conner.transform.localEulerAngles = new Vector3(0, 0, 270);
            }
        }
        else if (direction == ConnerDirection.RightDown)
        {
            if (reverse)
            {
                conner.transform.localPosition = new Vector3(conner.transform.localPosition.x + 84, conner.transform.localPosition.y - 84, conner.transform.localPosition.z);
                conner.transform.localEulerAngles = new Vector3(0, 0, 270);
            }
            else
            {
                conner.transform.localEulerAngles = new Vector3(0, 0, 90);
            }
        }
        else if (direction == ConnerDirection.RightUp)
        {
            if (reverse)
            {
                conner.transform.localPosition = new Vector3(conner.transform.localPosition.x + 84, conner.transform.localPosition.y + 84, conner.transform.localPosition.z);
                conner.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                conner.transform.localEulerAngles = new Vector3(0, 0, -180);
            }
        }
    }

    /// <summary>
    /// 绘制转角范围
    /// </summary>
    /// <param name="b"></param>
    /// <param name="allInRange"></param>
    void CaculateConner(PvpTile b, List<PvpTile> allInRange)
    {
        //DrawRangeConner(b, ConnerDirection.LeftDown, BlockConner.LeftUp);
        if (!FindOffset(-1, 0, allInRange, b) && !FindOffset(0, 1, allInRange, b) && !FindOffset(-1, 1, allInRange, b))
        {
            DrawRangeConner(b, ConnerDirection.LeftUp, false);
        }
        if (!FindOffset(-1, 0, allInRange, b) && !FindOffset(0, -1, allInRange, b) && !FindOffset(-1, -1, allInRange, b))
        {
            DrawRangeConner(b, ConnerDirection.LeftDown, false);
        }
        if (!FindOffset(1, 0, allInRange, b) && !FindOffset(0, 1, allInRange, b) && !FindOffset(1, 1, allInRange, b))
        {
            DrawRangeConner(b, ConnerDirection.RightUp, false);
        }
        if (!FindOffset(1, 0, allInRange, b) && !FindOffset(0, -1, allInRange, b) && !FindOffset(1, -1, allInRange, b))
        {
            DrawRangeConner(b, ConnerDirection.RightDown, false);
        }
        if (FindOffset(-1, 0, allInRange, b) && FindOffset(0, 1, allInRange, b) && !FindOffset(-1, 1, allInRange, b))
        {
            DrawRangeConner(b, ConnerDirection.LeftUp, true);
        }
        if (FindOffset(1, 0, allInRange, b) && FindOffset(0, 1, allInRange, b) && !FindOffset(1, 1, allInRange, b))
        {
            DrawRangeConner(b, ConnerDirection.RightUp, true);
        }
        if (FindOffset(-1, 0, allInRange, b) && FindOffset(0, -1, allInRange, b) && !FindOffset(-1, -1, allInRange, b))
        {
            DrawRangeConner(b, ConnerDirection.LeftDown, true);
        }
        if (FindOffset(1, 0, allInRange, b) && FindOffset(0, -1, allInRange, b) && !FindOffset(1, -1, allInRange, b))
        {
            DrawRangeConner(b, ConnerDirection.RightDown, true);
        }
    }

    /// <summary>
    /// 查找偏移
    /// </summary>
   
    public PvpTile FindOffset(int offsetX, int offsetY, List<PvpTile> all, PvpTile curBlock)
    {
        foreach (PvpTile b in all)
        {
            if (b.XPosition == curBlock.XPosition + offsetX && b.YPosition == curBlock.YPosition + offsetY)
            {
                return b;
            }
        }
        return null;
    }

    public GameObject Ranges;
    /// <summary>
    /// 移除所有的范围显示
    /// </summary>
    void DisShowAllRange()
    {
        AllSkillRangesTile.Clear();
        foreach (Transform t in Ranges.transform)
        {
            Destroy(t.gameObject);
        }
    }

	private void SceneDispose()
	{
		PvpGameObjectManager.Dispose ();
	}
    #endregion
}

