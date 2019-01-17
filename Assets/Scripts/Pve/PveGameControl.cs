using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;
public class PveGameControl : MonoBehaviour
{
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
    public SpriteRenderer BackGroundSprite;
    public SpriteRenderer BottomSprite;
    public FightDoor Door;
    int GameWidth = 7;
    int GameHeight = 9;

    public GameObject TilesArea;
    public List<PveTile> AllPveTiles = new List<PveTile>();

    public GameObject EliminateArea;
    public List<PveEliminate> AllEliminates = new List<PveEliminate>();

    public GameObject FightUnitArea;
    public List<PveEnemyUnit> AllEnemies = new List<PveEnemyUnit>();
    public List<PveOwnUnit> AllOwns = new List<PveOwnUnit>();
    public List<PveTrap> AllTraps = new List<PveTrap>();
    public static string CurDungeonId;
    public static string CurChapterId;
    public static int CurTotalGode;
    public static int CurEggNum;
    public static int CurBoxNum;

    public int CurFloor;

    public int CurRound; //总回合数

    public PveCharacter CurCharacter;

    public NewFloorNotice FloorNotice;

    public PveEliminate CurCharacterEliminate;

    public bool UserInputLock = true;

    public PlayerInfo PvePlayerInfo;

    public AttackComboLabel AttackCL;

    public GameObject FloorChange;

    public GameObject BossWarningResource;

    public Confirm SkillConfirm;

    public GameObject SkillBack;


    public GameObject CurResurrenction;

    public static BossSkillController.TriggerType CurSkillTime; //当前技能使用环境

    public static GameObject CurShakObj_gold;
    public static GameObject CurShakObj_egg;
    public static GameObject CurShakObj_box;

    #endregion

    #region 资源指针
    public GameObject PveTileResource;

    public GameObject PveEliminateResource;

    public GameObject PveMonsterResource;

    public GameObject CharacterResource;

    public GameObject PetResource;

    public GameObject BossResource;

    public GameObject Line;

    public GameObject Conner;

    public GameObject BarrierResource;

    public GameObject TrapResource;

    static public JsonObject CurPveData;

    static public FriendInfo CurFriend;

    public GameObject FightUnitEffect;

    public SkillFlyItem SkillFlyObject;

	public  JsonArray CurFloorGetJA = new JsonArray();

	public GameObject singalItem;

	public bool initStatus;

    #endregion

    #region  重写MONO
    void Start()
    {
		this.initStatus = false;
        CurFloor = 1;
        CurRound = 0;
        PveGameControl.CurTotalGode = 0;
        PveGameControl.CurBoxNum = 0;
        PveGameControl.CurEggNum = 0;

        RenderPveSceneStyle();
        CreateFightUnits();
        InitCharacter();
        InitPets();
        CloudRender();
        Invoke("CharacterMoveIn", 1f);

        ExitEvent();

        //if (CurDungeonId == "D1_2") PlayerPrefs.SetString("secondPveGuide", "over");

        PveGameControl.CurShakObj_gold=GameObject.Find("GoldTitle");
        PveGameControl.CurShakObj_egg=GameObject.Find("EggTitle");
        PveGameControl.CurShakObj_box = GameObject.Find("BoxTitle");
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

   
    #endregion

    #region 层个过渡
    bool first = true;
    public void BeginFloorLoad()
    {
        CurFloor++;
        FloorChange.SetActive(true);
        AnimationHelper.AnimationFadeTo(0, FloorChange, iTween.EaseType.linear, gameObject, "firstEnd", 0);       
    }
    public DungeonData curDungeonData;
    void firstEnd()
    {
        Hashtable args = new Hashtable();
        args.Add("alpha", 1);
        args.Add("time", 0.8f);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "FadeInEnd");
        iTween.FadeTo(FloorChange, args);
        PvePlayerInfo.SetFloorShow(CurFloor, ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId).FloorCount);
        
        curDungeonData = ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId);
        //CurCharacter.InitBoutNu(curDungeonData.Round);

       
    }

    void FadeInEnd()
    {
        CreateNextCurFloor();
        EndFloorLoad();
    }

    void EndFloorLoad()
    {
        Hashtable args = new Hashtable();
        args.Add("alpha", 0);
        args.Add("time", 0.8f);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "FadeOutEnd");
        iTween.FadeTo(FloorChange, args);
    }

    void FadeOutEnd()
    {
        FloorChange.SetActive(false);
        
        CharacterMoveIn();
        
    }
    #endregion

    #region 设置场景风格
    void CreateNextCurFloor()
    {
        hasBoss = false;
        ClearLastFloor();
        CreateFightUnits();
        CurCharacter.SetPosition(3, 0);
        foreach (PveOwnUnit po in AllOwns)
        {
            if (po.GetType() == typeof(PvePet))
            {
                po.SetPosition(InitXposition, InitYposition);
            }
        }

        for (int i = 0; i < GameWidth; i++)
        {
            for (int j = 0; j < GameHeight; j++)
            {
                CreateEliminate(i, j + 9);
            }
        }
    }

    void ClearLastFloor()
    {
        UserInputLock = true;
        for (int i = AllTraps.Count - 1; i >= 0; i--)
        {
            PveTrap pt = AllTraps[i];
            Destroy(pt.gameObject);
        }
        AllTraps.Clear();

        foreach (PveEliminate b in AllEliminates)
        {
            Destroy(b.gameObject);
        }
        AllEliminates.Clear();

        foreach (PveEnemyUnit eu in AllEnemies)
        {
            Destroy(eu.gameObject);
        }
        AllEnemies.Clear();
        Door.CloseDoor();
    }

    void RenderPveSceneStyle()
    {
        DungeonData dungeonData = ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId);
		//初始化成就
		AchievementManager.Init (dungeonData, this);

		PvePlayerInfo.Exit_btn.SetActive(false);
        string basicPath = "OrginPic/FightBackGround/";
        basicPath = basicPath + dungeonData.Scene + "/";
        string backGround = basicPath + "background";
        string door = basicPath + "Door" + dungeonData.Scene;
        string tile = basicPath + "tile" + dungeonData.Scene;
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
        GameObject doorObject = Instantiate(Resources.Load(door)) as GameObject;
        doorObject.layer = LayerHelper.BasicFX;
        Door = doorObject.GetComponent<FightDoor>();
        for (int i = 0; i < GameWidth; i++)
        {
            for (int j = 0; j < GameHeight; j++)
            {
                GameObject PveTileObject = Instantiate(PveTileResource) as GameObject;
                PveTile pveTile = PveTileObject.GetComponent<PveTile>();
                SpriteRenderer sr = pveTile.RenderObject.GetComponent<SpriteRenderer>();
                //sr.sprite = Resources.Load<Sprite>(tile);
                pveTile.transform.parent = TilesArea.transform;
                pveTile.transform.localScale = new Vector3(1, 1, 1);
                pveTile.SetPosition(i, j);
                AllPveTiles.Add(pveTile);
                CreateEliminate(i, j+9);
            }
        }
        PvePlayerInfo.SetFloorShow(CurFloor, ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId).FloorCount);
    }

    

    #endregion

    #region 生成消除块
    void CreateEliminate(int xPosition,int yPosition)
    {
        GameObject PveEliminateObject = Instantiate(PveEliminateResource) as GameObject;
        PveEliminate pveEliminate = PveEliminateObject.GetComponent<PveEliminate>();
        
        pveEliminate.transform.parent = EliminateArea.transform;
        pveEliminate.transform.localScale = new Vector3(1, 1, 1);
        pveEliminate.SetPosition(xPosition, yPosition);
        pveEliminate.GameControl = this;
        AllEliminates.Add(pveEliminate);
        pveEliminate.AttrubuteToRender();
    }
    #endregion

    #region 生成战斗对象
    
    void CreateFightUnits()
    {

        JsonArray floorData = (JsonArray)CurPveData[CurFloor.ToString()];
        foreach (JsonObject data in floorData)
        {
            MapElementData.ElementType type = (MapElementData.ElementType)(int.Parse(data["type"].ToString()));
            string position = data["xy"].ToString();
            string[] positions = position.Split('_');
            int x = int.Parse(positions[0]);
            int y = int.Parse(positions[1]);
            int xR = int.Parse(positions[2]);
            int yR = int.Parse(positions[3]);
            JsonArray Drop = (JsonArray)data["elements"];

			if(Drop.Count>0){			 

				CurFloorGetJA.AddRange(Drop);
			}
			//Debug.Log("''''"+Drop.Count);
			//Debug.Log("''''"+CurFloorGetJA.Count);

            string eleId = data["id"].ToString();
            if (type == MapElementData.ElementType.Barrier)
            {
                BarrierData barrierData = ConfigManager.BarrierConfig.GetBarrierById(eleId);
                InitBarrier(barrierData, x, y, xR, yR, Drop);
            }
            else if (type == MapElementData.ElementType.Monster)
            {
                MonsterData monsterData = ConfigManager.MonsterConfig.GetMonsterById(eleId);
                InitMonster(monsterData, x, y, xR, yR, Drop);
            }
            else if (type == MapElementData.ElementType.DontDe)
            {
                MonsterData monsterData = ConfigManager.MonsterConfig.GetMonsterById(eleId);
                InitMonster(monsterData, x, y, xR, yR, Drop ,(int)type);
            }
            else if (type == MapElementData.ElementType.Item)
            {               
                //ItemData itemData = ConfigManager.ItemConfig.GetItemById(elementData.ElementId);
                //StartCoroutine(InitItem(itemData, elementData.XPosition, elementData.YPosition));
            }
            else if (type == MapElementData.ElementType.Boss)
            {
                hasBoss = true;
                BossData bossData = ConfigManager.BossConfig.GetBossById(eleId);
                Debug.Log(bossData.BossAI);
                InitBoss(bossData, x, y, xR, yR, Drop);

            }
            else if (type == MapElementData.ElementType.Trap)
            {
                TrapData trapData = ConfigManager.TrapConfig.GetTrapById(eleId);
                InitTrap(trapData, x, y, xR, yR);
            }
        }
    }

    public void InitTrap(TrapData trapData, int xPosition, int yPosition, int xRange, int yRange)
    {
        GameObject trapObject = NGUITools.AddChild(FightUnitArea, TrapResource);
        PveTrap trap = trapObject.GetComponent<PveTrap>();
        trap.GameControl = this;
        trap.XRange = xRange;
        trap.YRange = yRange;
        trap.DamagePersent = trapData.DamagePersent;
        //trap.DamagePersent =10000;
        trap.SetPosition(xPosition, yPosition);
        AllTraps.Add(trap);
    }

    public PveBarrier InitBarrier(BarrierData barrierData, int xPosition, int yPosition, int xRange, int yRange,JsonArray drop)
    {
        GameObject barrierObject = NGUITools.AddChild(FightUnitArea, BarrierResource);
        PveBarrier barrier = barrierObject.GetComponent<PveBarrier>();
        barrier.DropList = drop;
        barrier.GameControl = this;
        barrier.CurHp = barrierData.Hp;
        barrier.Hp = barrierData.Hp;
        barrier.XRange = xRange;
        barrier.YRange = yRange;
        barrier.SetPosition(xPosition, yPosition);



        barrier.SkinData = ConfigManager.SkinConfig.GetSkinDataById(barrierData.SkinId);
        string skinPath = "PreFabs/Dungeon/Barriers/" + barrierData.SkinId;
        GameObject rg = Resources.Load(skinPath) as GameObject;
        //Debug.Log(barrierData.SkinId+"    "+rg);
        if (rg)
        {
            GameObject bObject = NGUITools.AddChild(barrier.RenderObject, rg);
            bObject.transform.localPosition = new Vector3(0f,65f,0f);
            //barrier.RenderObject = bObject;
            //barrier.RenderObject.transform.localScale = new Vector3(1 / 568, 1 / 568, 1);
            barrier.RenderBarrier();
        }   
        

        AllEnemies.Add(barrier);
        return barrier;
    }

    bool hasBoss = false;
    public void InitBoss(BossData bossData, int xPosition, int yPosition, int xRange, int yRange, JsonArray drop)
    {
        GameObject bossObject = Instantiate(BossResource) as GameObject;
        PveBoss pveBoss = bossObject.GetComponent<PveBoss>();

        pveBoss.CurBoss = bossData;

        pveBoss.DropList = drop;
        pveBoss.BaseHp = bossData.Hp;
        pveBoss.CurHp = bossData.Hp;
        pveBoss.BaseAtk = bossData.Attack;
        pveBoss.BaseDef = bossData.Def;
        pveBoss.XRange = xRange;
        pveBoss.YRange = yRange;
        pveBoss.FightOff = bossData.FightOff;
        pveBoss.SkinData = ConfigManager.SkinConfig.GetSkinDataById(bossData.SkinId);
        pveBoss.Range = bossData.Range;
        pveBoss.Element = bossData.Element;

        pveBoss.RunPower = bossData.RunPower;
        pveBoss.curRunPower = pveBoss.RunPower;
        pveBoss.GameControl = this;
        pveBoss.ShowElement();

        pveBoss.AnalyzeBuff();  //初始化无buff各项属性
        pveBoss.CurUnitHp.curHpType = UnitHp.HpType.Boss;
        if (xRange > 1 && yRange > 1)
        {
            pveBoss.CurUnitHp.transform.localScale= new Vector3(1.8f,2,1);
            pveBoss.Element_monster.transform.localScale = pveBoss.Element_monster.transform.localScale*1.8f;
            Vector3 vc=pveBoss.Element_monster.transform.localPosition;
            pveBoss.Element_monster.transform.localPosition = new Vector3(-0.155f,vc.y,vc.z);
        }
       
        string skinPath = "PreFabs/Characters/" + bossData.SkinId;
        int j = 0;
        for (int i = 10; i <= 80; i = i + 10)
        {
            string lastPath = skinPath + i.ToString();
            GameObject rg = Resources.Load(lastPath) as GameObject;
            if (rg)
            {
                pveBoss.AllAnimations[j] = rg;
            }
            j++;
        }
        pveBoss.UnitWaiting(DungeonEnum.FaceDirection.Down);
        pveBoss.gameObject.SetActive(false);
        bossObject.transform.parent = FightUnitArea.transform;
        pveBoss.SetPosition(xPosition, yPosition);
        AllEnemies.Add(pveBoss);
        hasBoss = true;
    }

    public void InitMonster(MonsterData monsterData, int xPosition, int yPosition, int xRange, int yRange, JsonArray drop,int Types=0)
    {
        GameObject monsterObject = Instantiate(PveMonsterResource) as GameObject;
        PveMonster pveMonster = monsterObject.GetComponent<PveMonster>();
        pveMonster.DropList = drop;
        pveMonster.CurMonsterType = monsterData.CurMonsterType;
       // pveMonster.CurMonsterType = MonsterData.MonsterType.RunAway;
        pveMonster.BaseHp = monsterData.Hp;
        pveMonster.CurHp = monsterData.Hp;
        pveMonster.BaseAtk = monsterData.Attack;
        pveMonster.BaseDef = monsterData.Def;
        pveMonster.Element = monsterData.Element;
        pveMonster.XRange = xRange;
        pveMonster.YRange = yRange;
        pveMonster.SkinData = ConfigManager.SkinConfig.GetSkinDataById(monsterData.SkinId);
        pveMonster.Range = monsterData.Range;
        pveMonster.RunPower = monsterData.RunPower;
        pveMonster.curRunPower = pveMonster.RunPower;
        pveMonster.ShowElement();

        pveMonster.AnalyzeBuff(); //初始化无buff各项属性

        if (Types == 7)
        {
            pveMonster.CurState= PveFightUnit.UnitState.guard;

        }

        if (xRange > 1 && yRange > 1)
        {
            pveMonster.CurUnitHp.transform.localScale = new Vector3(2, 2, 1);
        }

        string skinPath = "PreFabs/Characters/" + monsterData.SkinId;
        int j = 0;
        for (int i = 10; i <= 80; i = i + 10)
        {
            string lastPath = skinPath + i.ToString();
            GameObject rg = Resources.Load(lastPath) as GameObject;
            if (rg)
            {
                pveMonster.AllAnimations[j] = rg;
            }
            j++;
        }
        pveMonster.UnitWaiting(DungeonEnum.FaceDirection.Down);
        pveMonster.GameControl = this;
        monsterObject.transform.parent = FightUnitArea.transform;

		pveMonster.InitXPosition = xPosition;
		pveMonster.InitYPosition = yPosition;

        pveMonster.SetPosition(xPosition, yPosition);
        if (pveMonster.CurMonsterType == MonsterData.MonsterType.RunAway)
        {
			pveMonster.runAwayIcon.SetActive(true);
            pveMonster.CurUnitHp.gameObject.SetActive(false);
            pveMonster.RunAwayNum = 3;
			pveMonster.CurInfoLabel.SetNum(pveMonster.RunAwayNum.ToString(), LayerHelper.UnitFX);
            pveMonster.CurInfoLabel.transform.localPosition = new Vector3(0.08f, -0.032f, 0);
        }else
		{
			pveMonster.runAwayIcon.SetActive(false);
		}
        AllEnemies.Add(pveMonster);
    }

    /// <summary>
    /// 创建怪物 (召唤 skill)
    /// </summary>
    /// <param name="monsterID">Monster I.</param>
    /// <param name="pvpFightUnit">Pvp fight unit.</param>
    /// <param name="round">Round.</param>
    /// <param name="skill">If set to <c>true</c> skill.</param>
    public PveMonster CreateMonster(BaseSkillItem skillItem, Vector3 monsterPosition, string monsterID, PveFightUnit pvpFightUnit, int round, bool hiddenStatus = false)
    {
        MonsterData monsterConfig = ConfigManager.MonsterConfig.GetMonsterById(monsterID);

        GameObject monsterObject = Instantiate(PveMonsterResource) as GameObject;
        PveMonster pvpMonster = monsterObject.GetComponent<PveMonster>();
        //pvpMonster.MonsterID = monsterID;
        //pvpMonster.UserType = pvpFightUnit.UserType;
        pvpMonster.pvpPlayerBuff = new PvpPlayerBuff();
        pvpMonster.pvpPlayerSkill = new PvpPlayerSkill();
        //pvpMonster.SkillItem = skillItem;
        pvpMonster.Hp = monsterConfig.Hp;
        pvpMonster.CurHp = pvpMonster.Hp;
        pvpMonster.Atk = monsterConfig.Attack;
        pvpMonster.Def = monsterConfig.Def;
        pvpMonster.Element = monsterConfig.Element;

        pvpMonster.XRange = monsterConfig.Range;
        pvpMonster.YRange = monsterConfig.Range;
        //pvpMonster.FightRound = round; //战斗回合
        //pvpMonster.FightRunPower = monsterConfig.RunPower; //行走能量
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

        if (!hiddenStatus)
        {
            //pvpMonster.ChangeHiddenStatus(true, 0f);
            //this.CreateMonsterCallback(pvpMonster);
        }
        else
        {
            //pvpMonster.ChangeHiddenStatus(true, 0f);
        }

        //this.AllBarrier.Add(pvpMonster);
        // 设置怪脚下的格子
        //this.ChangeMonsterStatus(pvpMonster, false);
        // 设置 A * 障碍点
        //this.aStarUtils.GetNode(pvpMonster.XPosition, pvpMonster.YPosition).walkable = false;

        return pvpMonster;
    }


    #endregion

    #region 生成玩家
    int InitXposition = 3;
    int InitYposition = 1;
    void InitPets()
    {
        foreach (UserPet p in UserManager.CurUserInfo.CurPets)
        {
            GameObject petObject = Instantiate(PetResource) as GameObject;
            PvePet pvePet = petObject.GetComponent<PvePet>();
            pvePet.CurUserPet = p;
            pvePet.Atk = p.CurAtk;
            pvePet.SkinData = ConfigManager.SkinConfig.GetSkinDataById(p.CurPetData.SkinId);
            string skinPath = "PreFabs/Characters/" + p.CurPetData.SkinId;
            int j = 0;
            for (int i = 10; i <= 80; i = i + 10)
            {
                string lastPath = skinPath + i.ToString();
                GameObject rg = Resources.Load(lastPath) as GameObject;
                if (rg)
                {
                    pvePet.AllAnimations[j] = rg;
                }
                j++;
            }
            pvePet.UnitWaiting(DungeonEnum.FaceDirection.Up);

            if (p.CurPetData.PetSkillData != null)
            {
                //Debug.Log(p.CurPetData.PetSkillData + "    "+ p.CurPetData.PetSkillData.OkPve);
                //if (p.CurPetData.PetSkillData.OkPve == 1)
                //{
                pvePet.MainSkill = p.CurPetData.PetSkillData;
                //}                  
            }
            if (p.CurPetData.PetSkillData2 != null)
            {
                //Debug.Log(p.CurPetData.PetSkillData2 + "    "+ p.CurPetData.PetSkillData.OkPve);                    
                //if (p.CurPetData.PetSkillData2.OkPve == 1)
                //{
                pvePet.MainSkill2 = p.CurPetData.PetSkillData2;
                //}
            }

            petObject.transform.parent = FightUnitArea.transform;
            pvePet.GameControl = this;
            pvePet.SetPosition(InitXposition, InitYposition);
            AllOwns.Add(pvePet);
            pvePet.gameObject.SetActive(false);
            AnimationHelper.AnimationFadeTo(0, petObject, iTween.EaseType.linear, null, null, 0);
        }
    }
    void InitCharacter()
    {
        GameObject playerObject = Instantiate(CharacterResource) as GameObject;
        PveCharacter pveCharacter = playerObject.GetComponent<PveCharacter>();
        if (UserManager.CurUserInfo.CurWeapon!=null)
        { 
            //Debug.Log(UserManager.CurUserInfo.CurWeapon.CurHardWareData.SkillAffix1);
            pveCharacter.ChainSkill = ConfigManager.SkillConfig.GetSkillById(UserManager.CurUserInfo.CurWeapon.CurHardWareData.SkillAffix1);
        }
		UserManager.pveUserInfo.ResetSkillList();
        PveSkillManager.SkillInit(UserManager.pveUserInfo, UserManager.pveUserInfo.UserSkillList, UserManager.pveUserInfo.UserSkillCdList);
        PveSkillManager.BuffListInit(UserManager.pveUserInfo);

        pveCharacter.pvpPlayerBuff = UserManager.pveUserInfo.pvpPlayerBuff;
        pveCharacter.pvpPlayerSkill = UserManager.pveUserInfo.pvpPlayerSkill;
        pveCharacter.SkinData = ConfigManager.SkinConfig.GetSkinDataById("S100");

        if (GuiderLocal.TriggerPve() && PveGameControl.CurDungeonId == "D1_1")
        {
            pveCharacter.BaseHp = tempHP;
            pveCharacter.CurHp = tempHP;
            pveCharacter.Hp = tempHP;
        }
        else
        {
            pveCharacter.BaseHp = UserManager.pveUserInfo.CurHp;
            pveCharacter.CurHp = UserManager.pveUserInfo.CurHp;
        }
        
        pveCharacter.BaseAtk = UserManager.pveUserInfo.CurAtk ;
        pveCharacter.BaseDef =(int)UserManager.pveUserInfo.CurDef;
        CurCharacter = pveCharacter;
        pveCharacter.XRange = 1;
        pveCharacter.YRange = 1;

        pveCharacter.AnalyzeBuff(); //初始化无buff各项属性

        string skinPath = "PreFabs/Characters/S100";
        int j = 0;
        for (int i = 10; i <= 80; i = i + 10)
        {
            string lastPath = skinPath + i.ToString();
            GameObject rg = Resources.Load(lastPath) as GameObject;
            if (rg)
            {
                //pveCharacter.AllAnimations[j] = Instantiate(rg) as GameObject;
                pveCharacter.AllAnimations[j] = rg ;//Instantiate(rg) as GameObject;
            }
            j++;
        }

        pveCharacter.UnitWaiting(DungeonEnum.FaceDirection.Up);
        playerObject.transform.parent = FightUnitArea.transform;
        pveCharacter.GameControl = this;
        pveCharacter.SetPosition(3, 0);
        AnimationHelper.AnimationFadeTo(0, playerObject, iTween.EaseType.linear, null, null, 0);
        SetHpUIHpShow();
        
        DungeonData dungeonData = ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId);
        CurCharacter.InitBoutNu(dungeonData.Round);

     
    }
    #endregion

    #region 角色走路控制
    void CharacterMoveIn()
    {
        

        PveSkillEffectManager.TriggerFixed(CurCharacter, false);
        CurCharacter.UnitMove(3, 1, () =>
        {
            CurCharacter.UnitWaiting(CurCharacter.CurFaceDirection);
            FloorInfoMoveIn();
            AllEliminatesDropIn();
            
        },CurCharacter.CurFaceDirection);
        
        AnimationHelper.AnimationFadeTo(1, CurCharacter.gameObject, iTween.EaseType.linear, null, null, 1);

        Resources.UnloadUnusedAssets();//2015-04-07         
    }
    #endregion

    #region 开场渲染
    void FloorInfoMoveIn()
    {
        FloorNotice.ShowNewFloor(CurFloor, ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId).FloorCount);
    }

    void CloudRender()
    {
        GameObject Cloud = Instantiate(Resources.Load("Prefabs/FX/Cloud")) as GameObject;
    }

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
        List<PveEliminate> columnBlocks = FindEliminateByColumn(col);
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
        if (DropNum == GameWidth*GameHeight)
        {
            FloorNotice.MoveOut();
			// 场景初始化完成
			this.initStatus = true;

           
            if (hasBoss)
            {
                Invoke("BossWarningResourcefun", 0.5f);
                Invoke("BossAppearRender", 2f);
                Invoke("BeginCharacterLoop", 2f);
				Invoke("ShowGuider", 0.5f);//2014-11-20  引导
            }
            else
            {
                Invoke("BeginCharacterLoop", 0.5f);
				Invoke("ShowGuider", 0.2f);//2014-11-20  引导
            }          
        }
    }
    #endregion

    #region 工具方法
    public List<PveTile> FindNeigherTileByRange(List<PveTile> rangTiles,PveEnemyUnit eu)
    {
        List<PveTile> neighbours = new List<PveTile>();
        foreach (PveTile rt in rangTiles)
        {
            List<PveTile> rtNeighbours = FindNeighbourTileIn(rt, AllPveTiles,eu);
            foreach(PveTile rtt in rtNeighbours)
            {
                if (!rangTiles.Contains(rtt) && !neighbours.Contains(rtt)&&rtt.CanMoveOn==true)
                {
                    neighbours.Add(rtt);
                }
                //四格
                //Debug.Log(rtt.CanMoveOnWithBoss+"  "+ rtt.name);
                if (!neighbours.Contains(rtt) && rtt.CanMoveOnWithBoss == true)
                {
                    neighbours.Add(rtt);
                }
            }
        }
        return neighbours;
    }

    /// <summary>
    /// 在某个集合里查找周围的格子
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="all"></param>
    /// <returns></returns>
    public List<PveTile> FindNeighbourTileIn(PveTile obj, List<PveTile> all,PveEnemyUnit eu=null)
    {
        List<PveTile> neighbours = new List<PveTile>();
        foreach (PveTile b in all)
        {           
            //怪物runpower<=1 不可斜走
            if ((obj.XPosition == b.XPosition - 1 && obj.YPosition == b.YPosition) ||
               (obj.XPosition == b.XPosition - 1 && obj.YPosition == b.YPosition + 1 && ((eu != null && eu.curRunPower > 1) || eu == null)) ||
               (obj.XPosition == b.XPosition - 1 && obj.YPosition == b.YPosition - 1 && ((eu != null && eu.curRunPower > 1) || eu == null)) ||
               (obj.XPosition == b.XPosition + 1 && obj.YPosition == b.YPosition) ||
               (obj.XPosition == b.XPosition + 1 && obj.YPosition == b.YPosition + 1 && ((eu != null && eu.curRunPower > 1) || eu == null)) ||
               (obj.XPosition == b.XPosition + 1 && obj.YPosition == b.YPosition - 1 && ((eu != null && eu.curRunPower > 1) || eu == null)) ||
               (obj.XPosition == b.XPosition && obj.YPosition == b.YPosition - 1) ||
                (obj.XPosition == b.XPosition && obj.YPosition == b.YPosition + 1))
            {
                neighbours.Add(b);
            }
        }
        return neighbours;
    }

    

    List<PveEliminate> FindEliminateByColumn(int col)
    {
        List<PveEliminate> eliminates = new List<PveEliminate>();
        foreach (PveEliminate pe in AllEliminates)
        {
            if (pe.XPosition == col)
            {
                eliminates.Add(pe);
            }
        }
        return eliminates;
    }


    public PveEliminate FineEliminate(int xPosition, int yPosition)
    {
        foreach (PveEliminate pe in AllEliminates)
        {
            if (pe.XPosition == xPosition && pe.YPosition == yPosition)
            {
                return pe;
            }
        }
        return null;
    }

    public PveTile FindPveTile(int xPosition, int yPosition)
    {
        foreach(PveTile pt in AllPveTiles)
        {
            if(pt.XPosition == xPosition&&pt.YPosition == yPosition)
            {
                return pt;
            }
        }
        return null;
    }

    /// <summary>
    /// 有单位方砖显示灰色覆盖
    /// </summary>
    public void EnemyUnitEliminateEnable(bool enable)
    {
        foreach (PveEnemyUnit pu in AllEnemies)
        {
            if (pu.CurState == PveFightUnit.UnitState.guard) continue;
            foreach (PveTile pt in pu.RangeTiles)
            {
                PveEliminate pe = FineEliminate(pt.XPosition, pt.YPosition);               
                if (pe)
                {
                    pe.SetEnabelRender(enable);
                }
            }
        }
    }

    bool HasEnemyOnEliminate(PveEliminate eliminate)
    {
        foreach (PveEnemyUnit pu in AllEnemies)
        {
            PveTile pt = FindPveTile(eliminate.XPosition, eliminate.YPosition);
            if (pu.RangeTiles.Contains(pt))
            {
                return true;
            }
        }
        return false;
    }
    public bool HasEnemyWithTile(PveTile t)
    {
        foreach (PveEnemyUnit pu in AllEnemies)
        {
            if (pu.XPosition == t.XPosition && pu.YPosition == t.YPosition)
            {
               return true;
            }
            if(t.CanMoveOnWithBoss == true)
            {
                return true;
            }
        }
        return false;
    }

    public PveTrap TryTrap(int xPosition, int yPosition)
    {
        foreach(PveTrap t in AllTraps)
        {
            if(t.XPosition == xPosition&&t.YPosition == yPosition)
            {
                return t;
            }
        }
        return null;
    }

    /// <summary>
    /// 判断所有非无敌怪
    /// </summary>
    public bool HasEnemyOnPosition(int xPosition, int yPosition,PveEnemyUnit e)
    {
        foreach (PveEnemyUnit pu in AllEnemies)
        {
            if (pu.CurState == PveFightUnit.UnitState.guard) continue;
            if (pu != e)
            {
                PveTile pt = FindPveTile(xPosition, yPosition);
                if (pu.RangeTiles.Contains(pt))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 判断所有怪
    /// </summary>
    public bool HasAllEnemyOnPosition(int xPosition, int yPosition, PveEnemyUnit e)
    {
        foreach (PveEnemyUnit pu in AllEnemies)
        {
            if (pu != e)
            {
                PveTile pt = FindPveTile(xPosition, yPosition);
                if (pu.RangeTiles.Contains(pt))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 判断非逃跑怪
    /// </summary>
    public bool HasNoRunEnemyOnPosition(int xPosition, int yPosition,PveEnemyUnit e)
    {
        foreach (PveEnemyUnit pu in AllEnemies)
        {
            if (pu.CurState == PveFightUnit.UnitState.guard) continue;
            if (pu != e)
            {
                PveTile pt = FindPveTile(xPosition, yPosition);
                if (pu.RangeTiles.Contains(pt))
                {
                    //过滤逃跑怪?
                    //if(pu.GetType()==typeof(PveMonster))
                    //  {
                        //  PveMonster pm = (PveMonster)pu;
                        //if(pm.CurMonsterType == MonsterData.MonsterType.RunAway)
                        //{
                        //    return false;
                        //}
                        //else
                        //{
                      //      return true;
                      //  }
                      //}
                      //else
                      //{
                         return true;
                      //}
                }
            }
        }
        return false;
    }

    public PveEnemyUnit FindEnemyOn(int xPosition, int yPosition)
    {
        foreach (PveEnemyUnit pu in AllEnemies)
        {
            if (pu.CurState == PveFightUnit.UnitState.guard) continue;
            PveTile pt = FindPveTile(xPosition, yPosition);
            if (pu.RangeTiles.Contains(pt))
            {
                return pu;
            }
        }
        return null;
    }
	public PveEnemyUnit FindEnemyOn_xy(int xPosition, int yPosition)
	{
		foreach (PveEnemyUnit pu in AllEnemies)
		{
			if (pu.CurState == PveFightUnit.UnitState.guard) continue;
			if(xPosition==pu.XPosition && yPosition==pu.YPosition) return pu;
		}
		return null;
	}



    public PveEnemyUnit FindNoRunEnemyOn(int xPosition, int yPosition)
    {
        foreach (PveEnemyUnit pu in AllEnemies)
        {
            if (pu.CurState == PveFightUnit.UnitState.guard) continue;
            PveTile pt = FindPveTile(xPosition, yPosition);


            //if (pu.RangeTiles.Contains(pt))
            //考虑2×2怪物，改
            if(pt.XPosition==pu.XPosition && pt.YPosition==pu.YPosition)
            {
                if (pu.GetType() == typeof(PveMonster))
                {
                    PveMonster pm = (PveMonster)pu;
                    if (pm.CurMonsterType == MonsterData.MonsterType.RunAway)
                    {
                        return null;
                    }
                    else
                    {
                        return pm;
                    }
                }
                else
                {
                    return pu;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 玩家角色是否在范围内
    /// </summary>
    public bool CharacterInRange(List<PveTile> tiles)
    {
        PveTile characterTile = FindPveTile(CurCharacter.XPosition, CurCharacter.YPosition);
        foreach(PveTile t in tiles)
        {
            if(characterTile == t)
            {
                return true;
            }
        }
        return false;
    }

    bool CanLink(PveEliminate eliminate)
    {
        return false;
    }
    #endregion

    
    #region 游戏循环
    int skilled = 0;
    void BeginCharacterLoop()
    {
        PveSkillEffectManager.TriggerFixed(CurCharacter,true);
        PvePlayerInfo.SetPowerData();
        PvePlayerInfo.RefreshCd(CurCharacter);
      
		if(this.PvePlayerInfo != null) this.PvePlayerInfo.SetGridValue(0);

		int roundPercent = this.CurCharacter.CurBoutNu; // 当前关卡剩余回合数
		float hpPercent = this.CurCharacter.CurHp / this.CurCharacter.Hp;
		// 如果回合小于一个回合或者血量小于 30%
		if(roundPercent <= 1 || hpPercent <= 0.3f)
		{
			if(this.singalItem == null)
			{
				this.singalItem = GameObject.Instantiate(Resources.Load("PreFabs/FX/Deading")) as GameObject;
				this.singalItem.SetActive(true);
			}else
			{
				this.singalItem.SetActive(true);
			}
		}else
		{
			if(this.singalItem != null) this.singalItem.SetActive(false);
		}

        if (CurCharacter.lastRoundCount <= 0)
        {
            CurCharacter.CurState = PveFightUnit.UnitState.normal;
        }

        if(CurCharacter.CurState == PveFightUnit.UnitState.normal)
        {
            CurRound++; //总回合数
            
            CurCharacterEliminate = FineEliminate(CurCharacter.XPosition, CurCharacter.YPosition);
            
            if (CurCharacterEliminate) CurCharacterEliminate.SetToPlayerRender();
            EnemyUnitEliminateEnable(false);
            CurCharacter.ShowArrow(true);

            CurCharacter.GetRevPowerWithSkill();
            CurCharacter.GetSkillProList();

            SkillReturnsReduce();
            //Debug.Log("  主角行动 CurRound= " + CurRound);

            if (skilled == 0)
            {
                CurCharacter.SetBoutNu(-1);
            }
            else
            {
                skilled = 0;
            }

            if (!CharacterDead())
            {         
                UserInputLock = false;
            }      
        }
        else if(CurCharacter.CurState == PveFightUnit.UnitState.vertigo)
        {
            Invoke("DelayEnemyRound", 2f);
        }
    }

    void DelayEnemyRound()
    {
        CurCharacter.lastRoundCount--;
        CurCharacter.SetBoutNu(-1);
        TryOwnsActionEnd();
        EnemyRound();
    }

    public void Relive()
    {
        Resurrenction.Type = 1;
        Resurrenction rs = CurResurrenction.GetComponent<Resurrenction>();
        Resurrenction.SysType = 1;
        rs.GameControl = this;
        CurResurrenction.SetActive(true);    
    }
    //复活
    public void GoOnWithRelive()
    {
        CurCharacter.CurState = PveFightUnit.UnitState.normal;
        CurCharacter.CurHp = CurCharacter.Hp;
        CurCharacter.UnitWaiting(CurCharacter.CurFaceDirection);
        GameObject reliveEff = Instantiate(Resources.Load("PreFabs/FX/UiFx_6")) as GameObject;
        reliveEff.transform.position = CurCharacter.transform.position;

        SetHpUIHpShow();        
        CurCharacter.CurUnitHp.RefreshUI(CurCharacter.CurHp, CurCharacter.Hp);
        BeginCharacterLoop();
        //CurCharacter.SetBoutNu(3);
    }
    public void SetHpUIHpShow(PveFightUnit pf=null)
    {
        float per = CurCharacter.CurHp / CurCharacter.Hp;
        if (per >0.4f)
        {
            CurCharacter.Weakness = false;
        }       
        else
        {
            CurCharacter.Weakness = true;
        }
        PvePlayerInfo.HpUI.SetCurHpShow(CurCharacter.CurHp, CurCharacter.Hp);
    }
   
    public void PlayerInfoChangePetPro()
    {
        PvePlayerInfo.ChangePetPro();
    }
    public void SkillChangePetPro()
    {
        PvePlayerInfo.SkillChangePetPro(CurCharacter.SkillProList);
    }

    public void SkillReturnsReduce()
    {
        CurCharacter.SkillReturnsClear();
        foreach (PveEnemyUnit pee in AllEnemies)
        {
            pee.SkillReturnsClear();
        }
    }
    void ResetAllEliminates()
    {
        foreach (PveEliminate pe in AllEliminates)
        {
            pe.ShowAddition(false);
            pe.UnLinkFromLastRender(false);
            pe.Square.gameObject.SetActive(false);
        }
    }
    void TryOwnsActionEnd()
    {
		actionOwnUnitCount--;
		//己方行动结束
		//Debug.Log("actionOwnUnitCount : " + actionOwnUnitCount);

        if (actionOwnUnitCount == 0)
        {
            ResetAllEliminates();

            CurCharacter.BeTrap = false;
            //判断陷阱伤害
            PveTrap t = TryTrap(CurCharacter.XPosition, CurCharacter.YPosition);
            if(t)
            {
                CurCharacter.BeTrapHurt(t);
            }

            if (!CharacterDead())
            {             
               //主角活着
                int hpCount = AllEnemies.Count;
                //扣血
                foreach (PveEnemyUnit pee in AllEnemies)
                {
                    pee.RealHpReduce(() =>
                    {
                        hpCount--;
                        if (hpCount == 0)
                        {
                            //敌方行动结束

                            GuideAction_TheBoutEnd_Id();
                            Invoke("GuideAction_TheBoutEnd_Action",2.3f);
                            List<PveEnemyUnit> allDead = new List<PveEnemyUnit>();
                            foreach (PveEnemyUnit pe in AllEnemies)
                            {
                                if (pe.CurHp <= 0)
                                {
                                    allDead.Add(pe);
                                }
                            }

                            if (allDead.Count == 0)
                            {
								//Debug.Log("角色条件达成调用了的次数");
                                //没怪死亡
                                AttackCL.Out();
                                FillEliminateBlocks();
                            }
                            else
                            {
                                //有怪死亡
                                int curHasDeadCount = 0;
                                for(int i = allDead.Count - 1; i >= 0 ; i--) //修改为倒序，因为需要遍历时remove
                                {
                                    PveEnemyUnit pe = allDead[i];
                                    if (pe.CurHp <= 0)
									{
										if(pe.GetType() == typeof(PveMonster) || pe.GetType() == typeof(PveBoss))
										{
											// 显示怪物表情 死亡
											PveFaceManager.Trigger(pe, PveFaceTypeEnum.DEAD);
										}
                                        //Trigger触发，不走死亡逻辑
                                        if(pe.GetType() == typeof(PveCannonTrigger))
                                        {
                                            pe.UnitDead((p) => 
                                            {
                                                allDead.Remove(pe);
                                                if(allDead.Count == 0)
                                                {
                                                    Debug.Log("只有trigger触发，无怪物死亡");
                                                    AttackCL.Out();
                                                    FillEliminateBlocks();
                                                }
                                            });
                                        }
                                        else
                                        {
                                            pe.UnitDead((p) =>
                                            {
                                                GameObject disObject = null;
                                                if (p.GetType() == typeof(PveBoss) || p.GetType() == typeof(PveMonster))
                                                {
                                                    disObject = Instantiate(Resources.Load("PreFabs/FX/Monster_D")) as GameObject;
                                                }
                                                else if (p.GetType() == typeof(PveBarrier))
                                                {
                                                    //disObject = Instantiate(Resources.Load("PreFabs/FX/Stone_D")) as GameObject;
                                                }
                                                if (disObject!=null) disObject.transform.position = p.transform.position;
                                                AllEnemies.Remove((PveEnemyUnit)p);

                                                //掉落物品
                                                foreach (JsonObject dropData in ((PveEnemyUnit)p).DropList)
                                                {
                                                    //Debug.Log(dropData);
                                                    string dropId = dropData["id"].ToString();
                                                    int dropCount = int.Parse(dropData["count"].ToString());
                                                    int type = int.Parse(dropData["type"].ToString());


                                                    if (type == 1)
                                                    {
                                                        if (dropId == "Currency2")
                                                        {
                                                            GameObject gold = Instantiate(Resources.Load("PreFabs/FX/Gold")) as GameObject;
                                                            gold.transform.position = p.transform.position;

                                                            GoldFlyAc gfa = gold.transform.GetComponent<GoldFlyAc>();
                                                            gold.SetActive(true);
                                                            gfa.gamecontrol = this;
                                                            gfa.SetVeAndNum(dropCount);
                                                            PveGameControl.CurTotalGode = PveGameControl.CurTotalGode + dropCount;

                                                        }
                                                    }
                                                    else if (type == 5)
                                                    {
                                                        int rank = ConfigManager.PetConfig.GetPetById(dropId).Rank;
                                                        GameObject gold = Instantiate(Resources.Load("PreFabs/Props/Egg_s_" + (rank - 1).ToString())) as GameObject;
                                                        PveGameControl.CurEggNum++;
                                                        gold.transform.position = p.transform.position;
                                                        gold.transform.localScale = new Vector3(0.8f, 0.8f, 1);

                                                        FlyBoxOrEgg fbe = gold.AddComponent<FlyBoxOrEgg>();
                                                        fbe.ResName = "egg";
                                                        fbe.StartFly(this);
                                                    }
                                                    else if (type == 4)
                                                    {
                                                        //装备
                                                        int rank = ConfigManager.HardWareConfig.GetHardWareById(dropId).Rank;

                                                        GameObject gold = Instantiate(Resources.Load("PreFabs/Props/Chest_s_" + (rank - 1).ToString())) as GameObject;
                                                        PveGameControl.CurBoxNum++;
                                                        gold.transform.position = p.transform.position;
                                                        gold.transform.localScale = new Vector3(0.8f, 0.8f, 1);

                                                        FlyBoxOrEgg fbe = gold.AddComponent<FlyBoxOrEgg>();
                                                        fbe.ResName = "box";
                                                        fbe.StartFly(this);
                                                    }
                                                    else
                                                    {
                                                        //物品
                                                        Debug.Log("pve 物品 dropId=" + dropId);
                                                        int rank = ConfigManager.ItemConfig.GetItemById(dropId).Rank;
                                                        GameObject gold = Instantiate(Resources.Load("PreFabs/Props/Chest_s_" + (rank - 1).ToString())) as GameObject;
                                                        PveGameControl.CurBoxNum++;
                                                        gold.transform.position = p.transform.position;
                                                        gold.transform.localScale = new Vector3(0.8f, 0.8f, 1);


                                                        FlyBoxOrEgg fbe = gold.AddComponent<FlyBoxOrEgg>();
                                                        fbe.ResName = "box";
                                                        fbe.StartFly(this);


                                                    }
                                                }

                                                Destroy(p.gameObject);

                                                curHasDeadCount++;

                                                int count = 0;
                                                foreach (PveEnemyUnit pu in AllEnemies)
                                                {
                                                    if (pu.GetType() != typeof(PveBarrier) && pu.GetType() != typeof(PveCannonTrigger) && pu.CurState != PveFightUnit.UnitState.guard)
                                                    {
                                                        count++; 
                                                     
                                                        //Debug.Log(" ----" + pu.name);
                                                    }

                                                    if (pu.GetType() == typeof(PveBoss) && pu.CurState == PveFightUnit.UnitState.guard)
                                                    {
                                                        //Debug.Log(" ----" + pu.name);
                                                        count++; 
                                                    }


                                                }
                                                //Debug.Log(" ----self0 -----count =  " + count);
                                                if (count != 0)
                                                {
                                                    if (curHasDeadCount == allDead.Count)
                                                    {
                                                        //Debug.Log("这儿条件调用了的次数" + curHasDeadCount);
                                                        AttackCL.Out();
                                                        FillEliminateBlocks();
                                                    }
                                                }
                                                else
                                                {
                                                    if (CurFloor < ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId).FloorCount)
                                                    {
                                                        Door.OpenDoor();
                                                        GameObject doorOpen = Instantiate(Resources.Load("PreFabs/FX/KEY_use")) as GameObject;
                                                        doorOpen.transform.position = new Vector3(0, 0.75f, 0);
                                                        doorOpen.SetActive(true);
                                                        PveTile endTile = FindPveTile(3, 8);
                                                        CurCharacter.UnitMove(endTile.XPosition, endTile.YPosition, () =>
                                                        {
                                                            AttackCL.Out();
                                                            BeginFloorLoad();
                                                        }, DungeonEnum.FaceDirection.Up);
                                                    }
                                                    else
                                                    {
                                                        RenderSuccess();
                                                    }
                                                }
                                            });
                                        }
                                        //--------------------------------------------                                   
                                        //---------------------------------------------
                                    }
                                }
                            }
                        }
                    });
                }
            }
        }
    }
  
    /// <summary>
    ///己方行动总次数，为0 己方行动结束
    /// </summary>
    public int actionOwnUnitCount = 1;
    void BeginCharacterAction()
    {
		// 清空表情
		PveFaceManager.Clear ();

        // 更新延时

        PveSkillManager.ExecuteRoundBegin(CurCharacter, null);
        // 执行回合开始
        PveSkillManager.ExecuteRoundBegin1(CurCharacter);
        // 触发行走 Buff 效果
        PveSkillManager.Trigger(SkillTriggerTypeEnum.Walk,  CurCharacter,this.AllEnemies);
		// 触发成就
		AchievementManager.Trigger (AchievementTypeEnum.Residue_Eliminate, this.CurPathEliminate.Count);

        actionOwnUnitCount = 1;
        //CurCharacter.SetBoutNu(-1);
            
        UserInputLock = true;
        foreach (PveEliminate pe in AllEliminates)
        {
            pe.SetEnabelRender(true);
        }
        foreach (PveEnemyUnit eu in AllEnemies)
        {
            eu.WillBeHitRender(0);
        }
        foreach (Transform t in Ranges.transform)
        {
            Destroy(t.gameObject);
        }
     
        PveEliminate endEliminate = CurPathEliminate[CurPathEliminate.Count - 1];
        endEliminate.SetChain(false);
        endEliminate.SelectScale(false);
        List<PveEliminate> curLosePathlist = curLosePath(CurPathEliminate);

        List<PetAvata> readyPetsAvata =  PvePlayerInfo.GetFocusPetAvata();
        List<PvePet> curActionPets = new List<PvePet>();
        foreach (PetAvata pa in readyPetsAvata)
        {
            foreach (PveOwnUnit po in AllOwns)
            {
                if (po.GetType() == typeof(PvePet))
                {
                    PvePet pp = (PvePet)po;
                    if (pp.CurUserPet == pa.CurPet)
                    {
                        curActionPets.Add(pp);
                    }
                }
            }
        }

        actionOwnUnitCount =actionOwnUnitCount + curActionPets.Count;
        CurCharacter.OwnAction(() =>
        {
            CurCharacter.UnitWaiting(CurCharacter.CurFaceDirection);
            CurCharacter.ChainSkillUse(() =>
            {
                TryOwnsActionEnd();
            });
        //}, curLosePathlist);
        }, CurPathEliminate);

        int i = 1;
        foreach (PvePet pp in curActionPets)
        {
            StartCoroutine(PetsAction(pp, i));
            i++;
        }
    }
    //去掉中间节点
    List<PveEliminate> curLosePath(List<PveEliminate> list)
    {
        if (list.Count < 3) return list;
        List<PveEliminate> li = new List<PveEliminate>();
        li.AddRange(list);
        List<PveEliminate> lose = new List<PveEliminate>();
        PveEliminate prep = null;
        
        int index=0;
        foreach(PveEliminate pe in li){
            
            if (index > (li.Count - 2)) break;
            PveEliminate nexp=li[index+1]  ;            
            if (prep != null && nexp!=null)
            {
                if (prep.XPosition == pe.XPosition && pe.XPosition == nexp.XPosition)
                {
                    lose.Add(pe);
                }else if(prep.YPosition == pe.YPosition && pe.YPosition == nexp.YPosition)
                {
                    lose.Add(pe);
                }
            }
            prep = pe;
            index++;
        }

        foreach (PveEliminate pe in lose)
        {
            li.Remove(pe);
        }
        //Debug.Log("li count= "+li.Count);
        return li;
    }
    //主角死亡判断
    public bool CharacterDead()
    {
        if (CurCharacter.CurHp <= 0f)
        {
            UserInputLock = true;
            CurCharacter.UnitDead((pu) =>
            {
                //复活
                Relive();
            });
            return true;
        }
        return false;
    }

    IEnumerator PetsAction(PvePet p,int delay)
    {
        yield return new WaitForSeconds(0.5f * delay);
        p.gameObject.SetActive(true);
        p.OwnAction(() =>
        {
            //回退
            int back = p.CurActionPath.Count - delay - 1;
            if(back<0)
            {
                back = 0;
            }
            PveEliminate pe = p.CurActionPath[back];
            PveTile curPt = FindPveTile(p.XPosition, p.YPosition);
            PveTile toPt = FindPveTile(pe.XPosition, pe.YPosition);
            DungeonEnum.FaceDirection direction = curPt.GetTargetDirection(toPt);
            p.UnitMove(pe.XPosition, pe.YPosition, () =>
            {
                p.UnitWaiting(ReverseDirection(p.CurFaceDirection));
                p.ChainSkillUse(() =>
                {
                    StartCoroutine(DelayFlyBack(p));
                });
            }, direction);
        }, CurCharacter.CurActionPath);
    }

    IEnumerator DelayFlyBack(PvePet p)
    {
        yield return new WaitForSeconds(0.5f);
        p.UnitWaiting(p.CurFaceDirection);


        p.PetBackToAvata(() =>
        {
           // p.gameObject.SetActive(false);
            TryOwnsActionEnd(); 
        });
    }

    int enemyRoundCount = 0;

	public JsonArray runAwayMonsterList;
    void EnemyRound()
    {
        PveBoss curBoss = null;

        foreach (PveTile cp in AllPveTiles)
        {
            cp.CanMoveOnWithBoss = false;
        }

        for(int i = 0; i < AllEnemies.Count; i++)
        {
            PveEnemyUnit pe = AllEnemies[i];
            if(pe.GetType() == typeof(PveBoss))
            {
                curBoss = (PveBoss)pe;
                foreach (PveTile ptt in pe.RangeTiles)
                {
                    if (ptt.XPosition == pe.XPosition && ptt.YPosition == pe.YPosition)
                    {
                        ptt.CanMoveOnWithBoss = false;
                    }
                    else
                    {
                        ptt.CanMoveOnWithBoss = true;
                    }
                }                
                break;
            }
        }
        if(curBoss != null)
        {
            //Debug.Log(curBoss.curRunPower);
            PveGameControl.CurSkillTime = BossSkillController.TriggerType.Round_Start;
            PveBoss boss = curBoss;
            if (boss.ReadyForBossSkill())
            {
                Debug.Log(boss.name + "回合前技能");
                boss.UseSkill(null, () =>
                {
                    EnemyRoundExcute();
                });
            }
            else
            {
                EnemyRoundExcute();
            }
        }
        else
        {
            EnemyRoundExcute();
        }
    }

    void EnemyRoundExcute()
    {
        foreach (PveEnemyUnit pe in AllEnemies) //判断并调整状态
        {
            if (pe.CurState == PveFightUnit.UnitState.guard)
            {
                if (pe.lastRoundCount == 0)
                {
                    //Debug.Log(pe.name);
                    pe.SetAppear();
                }
                pe.lastRoundCount--;
            }
        }

        foreach(PveEnemyUnit pe in AllEnemies)
        {
            pe.ClearTimeOutBuffs();


        }

        foreach (PveTile pt in AllPveTiles)
        {
            if (HasAllEnemyOnPosition(pt.XPosition, pt.YPosition, null))
            {
                pt.CanMoveOn = false;
            }
            else
            {
                pt.CanMoveOn = true;
            }
        }
        PveTile characterPt = FindPveTile(CurCharacter.XPosition, CurCharacter.YPosition);
        characterPt.CanMoveOn = false;

        foreach (PveEnemyUnit pe in AllEnemies)
        {
            pe.ActionPath.Clear();
            if (pe.CurState == PveFightUnit.UnitState.guard)
            {
                continue;
            }
            if (!pe.PlayerInRange() && (pe.GetType() == typeof(PveMonster) || pe.GetType() == typeof(PveBoss)))
            {
                pe.FindActionPath();
            }          
        }
        //2015-04-30

        //foreach (PveTile cp in AllPveTiles)
        //{
        //    cp.CanMoveOn = true;
        //}

        enemyRoundCount = 0;
        foreach (PveEnemyUnit pe in AllEnemies)
        {
            if(pe.GetType() == typeof(PveMonster))
            {
                enemyRoundCount++;
            }
            if (pe.CurState == PveFightUnit.UnitState.guard)
            {
                continue;
            }
            if (pe.GetType() == typeof(PveBoss))
            {
                enemyRoundCount++;
            }
        }

        foreach (PveEnemyUnit pe in AllEnemies)
        {

            if (pe.CurState == PveFightUnit.UnitState.guard)
            {
                if (AllEnemies.IndexOf(pe) == AllEnemies.Count - 1)
                {
                    BeginCharacterLoop();
                    continue;
                }
                else if(pe.GetType() != typeof(PveMonster)) continue;
            }


            if (pe.GetType() == typeof(PveMonster) || pe.GetType() == typeof(PveBoss))
            {
                pe.BeginAction(() =>
                {
                    //Debug.Log(pe.name);

                    enemyRoundCount--;
                    if (enemyRoundCount == 0)
                    {
                        if (!CharacterDead())
                        {
                            //主角活着
                            //判断死亡 敌方行动结束
                            List<PveEnemyUnit> allDead = new List<PveEnemyUnit>();
                            foreach (PveEnemyUnit ppe in AllEnemies)
                            {
                                if (ppe.GetType() == typeof(PveCannonTrigger)) continue;
                                if (ppe.CurHp <= 0 || ppe.RunAwayStatus)
                                {
                                    allDead.Add(ppe);
                                }
                            }

                            if (allDead.Count == 0)
                            {
                                //没怪死亡
                                PveBoss boss = null;
                                CurSkillTime = BossSkillController.TriggerType.Round_Over;
                                foreach (PveEnemyUnit pu in AllEnemies) //回合后技能
                                {
                                    if (pu.GetType() == typeof(PveBoss))
                                    {
                                        boss = (PveBoss)pu;
                                    }
                                }
                                if (boss != null && boss.ReadyForBossSkill())
                                {
                                    Debug.Log(boss.name + "回合后技能");
                                    boss.UseSkill(null, () =>
                                    {
                                        BeginCharacterLoop();
                                    });
                                }
                                else
                                {
                                    //Debug.Log("begin Character");
                                    BeginCharacterLoop();
                                }
                            }
                            else
                            {
                                //有怪死亡
                                int curHasDeadCount = 0;
                                foreach (PveEnemyUnit ppe in allDead)
                                {
                                    if (ppe.CurHp <= 0 || ppe.RunAwayStatus)
                                    {
                                        if (ppe.GetType() == typeof(PveMonster) || ppe.GetType() == typeof(PveBoss))
                                        {
                                            // 如果不是逃跑
                                            if (!ppe.RunAwayStatus)
                                            {
                                                // 显示怪物表情 死亡
                                                PveFaceManager.Trigger(ppe, PveFaceTypeEnum.DEAD);
                                            }
                                        }

                                        //--------------------------------------------                                   
                                        ppe.UnitDead((p) =>
                                        {
                                            bool deadEffectStatus = true;

                                            PveEnemyUnit pveEnemyUnit = p as PveEnemyUnit;
                                            if (pveEnemyUnit != null && pveEnemyUnit.RunAwayStatus) deadEffectStatus = false;
                                            // 如果需要播放死亡效果
                                            if (deadEffectStatus)
                                            {
                                                GameObject disObject = null;
                                                if (p.GetType() == typeof(PveBoss) || p.GetType() == typeof(PveMonster))
                                                {
                                                    disObject = Instantiate(Resources.Load("PreFabs/FX/Monster_D")) as GameObject;
                                                }
                                                else if (p.GetType() == typeof(PveBarrier))
                                                {
                                                    //disObject = Instantiate(Resources.Load("PreFabs/FX/Stone_D")) as GameObject;
                                                }
                                                if (disObject!=null) disObject.transform.position = p.transform.position;
                                            }
                                            AllEnemies.Remove((PveEnemyUnit)p);

                                            //掉落物品，如果不是逃跑
                                            if (!ppe.RunAwayStatus)
                                            {
                                                foreach (JsonObject dropData in ((PveEnemyUnit)p).DropList)
                                                {

                                                    string dropId = dropData["id"].ToString();
                                                    int dropCount = int.Parse(dropData["count"].ToString());
                                                    int type = int.Parse(dropData["type"].ToString());
                                                    if (type == 1)
                                                    {
                                                        if (dropId == "Currency2")
                                                        {
                                                            GameObject gold = Instantiate(Resources.Load("PreFabs/FX/Gold")) as GameObject;
                                                            gold.transform.position = p.transform.position;
                                                            PveGameControl.CurTotalGode = PveGameControl.CurTotalGode + dropCount;

                                                            GoldFlyAc gfa = gold.transform.GetComponent<GoldFlyAc>();
                                                            GameObject goldlabicon = GameObject.Find("GoldTitle");
                                                            gold.SetActive(true);
                                                            gfa.SetVeAndNum(dropCount);

                                                        }
                                                    }
                                                    else if (type == 5)
                                                    {
                                                        int rank = ConfigManager.PetConfig.GetPetById(dropId).Rank;

                                                        GameObject gold = Instantiate(Resources.Load("PreFabs/Props/Egg_s_" + (rank - 1).ToString())) as GameObject;
                                                        PveGameControl.CurEggNum++;
                                                        gold.transform.position = p.transform.position;
                                                        gold.transform.localScale = new Vector3(0.8f, 0.8f, 1);

                                                        FlyBoxOrEgg fbe = gold.AddComponent<FlyBoxOrEgg>();
                                                        fbe.ResName = "egg";
                                                        fbe.StartFly(this);

                                                    }
                                                    else if (type == 4)
                                                    {
                                                        //装备
                                                        int rank = ConfigManager.HardWareConfig.GetHardWareById(dropId).Rank;

                                                        GameObject gold = Instantiate(Resources.Load("PreFabs/Props/Chest_s_" + (rank - 1).ToString())) as GameObject;
                                                        PveGameControl.CurBoxNum++;
                                                        gold.transform.position = p.transform.position;
                                                        gold.transform.localScale = new Vector3(0.8f, 0.8f, 1);

                                                        FlyBoxOrEgg fbe = gold.AddComponent<FlyBoxOrEgg>();
                                                        fbe.ResName = "box";
                                                        fbe.StartFly(this);
                                                    }
                                                    else
                                                    {
                                                        //物品
                                                        int rank = ConfigManager.ItemConfig.GetItemById(dropId).Rank;

                                                        GameObject gold = Instantiate(Resources.Load("PreFabs/Props/Chest_s_" + (rank - 1).ToString())) as GameObject;
                                                        PveGameControl.CurBoxNum++;
                                                        gold.transform.position = p.transform.position;
                                                        gold.transform.localScale = new Vector3(0.8f, 0.8f, 1);

                                                        FlyBoxOrEgg fbe = gold.AddComponent<FlyBoxOrEgg>();
                                                        fbe.ResName = "box";
                                                        fbe.StartFly(this);
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                PveMonster runMonster = p as PveMonster;
                                                if (runMonster != null)
                                                {
                                                    if (this.runAwayMonsterList == null) this.runAwayMonsterList = new JsonArray();
                                                    this.runAwayMonsterList.Add(this.CurFloor + "" + runMonster.InitXPosition + "" + runMonster.InitYPosition);
                                                }
                                            }
                                            //PvePlayerInfo.setResNum();
                                            Destroy(p.gameObject);

                                            curHasDeadCount++;
                                            int count = 0;
                                            foreach (PveEnemyUnit pu in AllEnemies)
                                            {
                                                if (pu.GetType() != typeof(PveBarrier) && pu.GetType() != typeof(PveCannonTrigger) && pu.CurState != PveFightUnit.UnitState.guard)
                                                {
                                                    count++;

                                                    //Debug.Log(" ----" + pu.name);
                                                }

                                                if (pu.GetType() == typeof(PveBoss) && pu.CurState == PveFightUnit.UnitState.guard)
                                                {
                                                    //Debug.Log(" ----" + pu.name);
                                                    count++;
                                                }
                                            }
                                            Debug.Log(" ----self -----count =  " + count);
                                            if (count != 0)
                                            {
                                                if (curHasDeadCount == allDead.Count)
                                                {
                                                    //回合后技能
                                                    PveBoss boss = null;
                                                    CurSkillTime = BossSkillController.TriggerType.Round_Over;
                                                    foreach (PveEnemyUnit pu in AllEnemies)
                                                    {
                                                        if (pu.GetType() == typeof(PveBoss))
                                                        {
                                                            boss = (PveBoss)pu;
                                                        }
                                                    }
                                                    if (boss != null && boss.ReadyForBossSkill())
                                                    {
                                                        Debug.Log(boss.name + "回合后技能");
                                                        boss.UseSkill(null, () =>
                                                        {
                                                            BeginCharacterLoop();
                                                        });
                                                    }
                                                    else
                                                    {
                                                        BeginCharacterLoop();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (CurFloor < ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId).FloorCount)
                                                {
                                                    Door.OpenDoor();
                                                    GameObject doorOpen = Instantiate(Resources.Load("PreFabs/FX/KEY_use")) as GameObject;
                                                    doorOpen.transform.position = new Vector3(0, 0.75f, 0);
                                                    doorOpen.SetActive(true);
                                                    PveTile endTile = FindPveTile(3, 8);
                                                    CurCharacter.UnitMove(endTile.XPosition, endTile.YPosition, () =>
                                                    {
                                                        AttackCL.Out();
                                                        BeginFloorLoad();
                                                    }, DungeonEnum.FaceDirection.Up);
                                                }
                                                else
                                                {
                                                    RenderSuccess();
                                                }
                                            }
                                        });
                                    }
                                }
                            }

                        }
                    }
                });
            }
        }
    }
    #endregion

    #region 填充
    int fillMoveCount = 0;
    List<PveEliminate> readyFillBlocks = new List<PveEliminate>();
    void FillEliminateBlocks()
    {
        fillMoveCount = 0;
        readyFillBlocks.Clear();
        //逐列填充
        for (int i = 0; i < GameWidth; i++)
        {
            List<PveEliminate> columnBlocks = FindEliminateByColumn(i);
            //PveBarrier highestBarrier = FindHighestBarriarOnColomn(i);
            if (columnBlocks.Count < GameHeight)
            {
                //if (highestBarrier != null)
                //{
                    ///障碍物上方的块
                //    List<PveEliminate> onBarriar = new List<PveEliminate>();
                //    foreach (PveEliminate b in columnBlocks)
                //    {
                //        if (b.YPosition > highestBarrier.YPosition)
                //        {
                //            onBarriar.Add(b);
                //        }
                //    }
                //    CreateFills(highestBarrier.YPosition + 1, onBarriar, i);
                //}
                //else
                //{
                    CreateFills(0, columnBlocks, i);
                //}
            }
        }
		if(fillMoveCount > 0)
		{
        	AllFill();
		}else{
			fillMoveCount = 1;
			this.ResertPositionAnimationEnd(null);
		}
    }

    /// <summary>
    /// 排序某一列的消除块
    /// </summary>
    /// <param name="blocks"></param>
    /// <returns></returns>
    List<PveEliminate> SortColumnBlocks(List<PveEliminate> blocks)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            for (int j = 0; j < blocks.Count - i - 1; j++)
            {
                PveEliminate b1 = blocks[j];
                PveEliminate b2 = blocks[j + 1];
                if (b1.YPosition > b2.YPosition)
                {
                    PveEliminate temp = blocks[j];
                    blocks[j] = blocks[j + 1];
                    blocks[j + 1] = temp;
                }
            }
        }
        return blocks;
    }

    void CreateFills(int fillFrom, List<PveEliminate> curExistsBlock, int column)
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
        int creatIndex = 9;
        //创建填充
        for (int k = YIndex; k <= 8; k++)
        {
            GameObject PveEliminateObject = Instantiate(PveEliminateResource) as GameObject;
            PveEliminate pveEliminate = PveEliminateObject.GetComponent<PveEliminate>();
            
            pveEliminate.GameControl = this;
            AllEliminates.Add(pveEliminate);
            pveEliminate.transform.parent = EliminateArea.transform;
            pveEliminate.transform.localScale = new Vector3(1, 1, 1);
            pveEliminate.SetPosition(column, creatIndex);
            pveEliminate.XPosition = column;
            pveEliminate.YPosition = k;
            pveEliminate.AttrubuteToRender();
            pveEliminate.SetName();
            curExistsBlock.Add(pveEliminate);
            readyFillBlocks.Add(pveEliminate);
            creatIndex++;
        }

        //下落
        fillMoveCount = fillMoveCount + curExistsBlock.Count;
    }

    void AllFill()
    {
        for (int i = 0; i < 7; i++)
        {
            StartCoroutine(FillColoumn(i, i *0.01f));
        }
    }

    IEnumerator FillColoumn(int col, float delay)
    {
        yield return new WaitForSeconds(delay);
        //Debug.Log("delay -----"+delay);
        List<PveEliminate> columnB = new List<PveEliminate>();
        for (int i = 0; i < readyFillBlocks.Count; i++)
        {
            PveEliminate b = readyFillBlocks[i];
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

    void ResertPositionAnimationEnd(GameObject blockObj)
    {
        fillMoveCount--;
        if (fillMoveCount == 0)
        {
			//Debug.Log("远程武器攻击 ！！！！ 到这儿来了吗？？？？？");
            CurCharacterEliminate = FineEliminate(CurCharacter.XPosition, CurCharacter.YPosition);
			if(CurCharacterEliminate == null)
			{
				GameObject PveEliminateObject = Instantiate(PveEliminateResource) as GameObject;
				PveEliminate pveEliminate = PveEliminateObject.GetComponent<PveEliminate>();
				CurCharacterEliminate = pveEliminate;
				pveEliminate.CurEliminateAttribute = DungeonEnum.ElementAttributes.Player;
				pveEliminate.GameControl = this;
				AllEliminates.Add(pveEliminate);
				pveEliminate.transform.parent = EliminateArea.transform;
				pveEliminate.transform.localScale = new Vector3(1, 1, 1);
				pveEliminate.SetPosition(CurCharacter.XPosition, CurCharacter.YPosition);
				pveEliminate.ReturnToCommon();
			}
            CurCharacterEliminate.SetToPlayerRender();
            foreach (PveOwnUnit po in AllOwns)
            {
                if (po.GetType() == typeof(PvePet))
                {
                    po.SetPosition(CurCharacter.XPosition, CurCharacter.YPosition);
                }
            }
            EnemyRound();
            //curPlayerEliminateBlock = FindEliminateByPosition(CurPlayer.XPosition, CurPlayer.YPosition);
            //if (curPlayerEliminateBlock == null)
            //{
            //    GameObject EliminateObject = NGUITools.AddChild(DungeonArea, EliminateResource);
            //    curPlayerEliminateBlock = EliminateObject.GetComponent<EliminateBlock>();
            //    curPlayerEliminateBlock.DungeonScene = this;
            //    CurAllEliminateBlock.Add(curPlayerEliminateBlock);
            //    curPlayerEliminateBlock.RenderObjectSprite(CurPlayer.XPosition, CurPlayer.YPosition);
            //}
            //curPlayerEliminateBlock.SetToPlayerRender();
            //EnemyRound();
        }
    }

    /// <summary>
    /// 在某列上查找最高的障碍物
    /// </summary>
    /// <param name="column"></param>
    PveBarrier FindHighestBarriarOnColomn(int column)
    {
        List<PveBarrier> allColumnBarriers = new List<PveBarrier>();
        for (int i = 0; i < 9; i++)
        {
            PveBarrier barrier = FindBarrier(column, i);
            if (barrier)
            {
                allColumnBarriers.Add(barrier);
            }
        }
        //高度排序
        allColumnBarriers = SortColumnBarriers(allColumnBarriers);
        if (allColumnBarriers.Count > 0)
        {
            return allColumnBarriers[0];
        }
        return null;
    }
    /// <summary>
    /// 高度排序障碍物
    /// </summary>
    /// <param name="blocks"></param>
    /// <returns></returns>
    List<PveBarrier> SortColumnBarriers(List<PveBarrier> blocks)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            for (int j = 0; j < blocks.Count - i - 1; j++)
            {
                PveBarrier b1 = blocks[j];
                PveBarrier b2 = blocks[j + 1];
                if (b1.YPosition < b2.YPosition)
                {
                    PveBarrier temp = blocks[j];
                    blocks[j] = blocks[j + 1];
                    blocks[j + 1] = temp;
                }
            }
        }
        return blocks;
    }

    /// <summary>
    /// 判断是否有障碍物
    /// </summary>
    /// <param name="xPosition"></param>
    /// <param name="yPosition"></param>
    /// <returns></returns>
    PveBarrier FindBarrier(int xPosition, int yPosition)
    {
        PveEnemyUnit eventElement = FindEnemyOn(xPosition, yPosition);
        if (eventElement)
        {
            if (eventElement.GetType() == typeof(PveBarrier))
            {
                return (PveBarrier)eventElement;
            }
        }
        return null;
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
        if (Input.GetMouseButtonUp(0))
        {
            RayBumpObjectCheck(Input.mousePosition, TouchType.Out); return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            RayBumpObjectCheck(Input.mousePosition, TouchType.Enter); return;
        }
        if (Input.GetMouseButton(0))
        {
            RayBumpObjectCheck(Input.mousePosition, TouchType.Hold); return;
        }
    }
    //touch屏幕操作连线  开始 ..结束
    float LinePreTime = 0f;

    public List<PveEliminate> CurPathEliminate = new List<PveEliminate>();
    bool isLinking = false;
    int LinkCount = 0; //当前连线步数
    PveEliminate BasicEliminate = null;
    void RayBumpObjectCheck(Vector3 touchPosition, TouchType type)
    {
        if (UserInputLock == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchPosition), Vector2.zero);
            if (hit)
            {
                GameObject hitObject = hit.collider.gameObject;
                PveEliminate hitEliminateBlock = hitObject.GetComponent<PveEliminate>();
                if (hitEliminateBlock)
                {
                    if (type == TouchType.Enter)
                    {
                        LinkCount = 0;
                        PveEnemyUnit eu = FindEnemyOn(hitEliminateBlock.XPosition, hitEliminateBlock.YPosition);
                        if (eu!=null&&(eu.GetType()==typeof(PveMonster)||eu.GetType()==typeof(PveBoss)))
                        {
                            PveMonster m = (PveMonster)eu;
                            if(eu.CurState != PveFightUnit.UnitState.guard)
                            {
                                ShowMonsterInfo(true, m);
                            }
                        }
                        else
                        {
                            //开始
                            if (FirstLineOver)
                            {
                                CancelGuideAction_LineEnter();
                                TryBeginLink(hitEliminateBlock);
                            }
                            				
                        }
                    }
                    else if (type == TouchType.Hold)
                    {
                        LinkCount++;
                        if(LinkCount < CurCharacter.DistanceLimit || CurCharacter.DistanceLimit == -1) //步数限制判断
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
                    }
                    else if (type == TouchType.Out)
                    {
                        //结束
                        TryEndLink();
                        LinePreTime = Time.realtimeSinceStartup;
                        foreach (PveEnemyUnit enemy in AllEnemies)
                        {
                            if (enemy.GetType() == typeof(PveMonster) || enemy.GetType() == typeof(PveBoss))
                            {
                                PveMonster m = (PveMonster)enemy;
                                ShowMonsterInfo(false, m);
                            }
                        }
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


    //按住怪物  显示行走和攻击范围
    void ShowMonsterInfo(bool show,PveMonster m)
    {
        //Debug.Log(m.RunPower);
        //Debug.Log(m.Range);
        if(show)
        {
            List<PveEliminate> runEliminate = FindRunPowerEliminates(m);
            foreach(PveEliminate e in runEliminate)
            {
                e.ShowSpeed(true);
            }

            List<PveEliminate> attackEliminate = FindAttackEliminates(m, runEliminate);
            foreach (PveEliminate e in attackEliminate)
            {
                if (!runEliminate.Contains(e)) e.ShowAttack(true);
               
            }
        }
        else
        {
            foreach(PveEliminate e in AllEliminates)
            {
                e.ShowAttack(false);
                e.ShowSpeed(false);
            }
        }
    }


    public List<PveEliminate> FindRunPowerEliminates(PveMonster m)
    {
        List<PveEliminate> es = new List<PveEliminate>();
       
        //m.RangeTiles
        //Debug.Log("--- "+m.RangeTiles.Count);
        foreach (PveTile mt in m.RangeTiles)
        {
            for (int i = 1; i <= m.RunPower; i++)
            {
                foreach (PveEliminate pe in AllEliminates)
                {
                    if ((pe.XPosition == mt.XPosition - i && pe.YPosition == mt.YPosition) ||
                        (pe.XPosition == mt.XPosition + i && pe.YPosition == mt.YPosition) ||
                        (pe.XPosition == mt.XPosition && pe.YPosition == mt.YPosition - i) ||
                        (pe.XPosition == mt.XPosition && pe.YPosition == mt.YPosition + i) ||
                        (pe.XPosition == mt.XPosition && pe.YPosition == mt.YPosition))
                    {
                        if (!es.Contains(pe)) es.Add(pe);
                    }
                }
            }
        }

        if (m.RunPower == 1)
        {
            //Debug.Log("m.RunPowe=1 count= " + es.Count);

        }
        else if (m.RunPower == 2)
        {
            //斜走为2，Add斜走方砖
            foreach (PveTile mt in m.RangeTiles)
            {
                foreach (PveEliminate pe in AllEliminates)
                {
                    if ((pe.XPosition == mt.XPosition - 1 && pe.YPosition == mt.YPosition + 1) ||
                        (pe.XPosition == mt.XPosition - 1 && pe.YPosition == mt.YPosition - 1) ||
                        (pe.XPosition == mt.XPosition + 1 && pe.YPosition == mt.YPosition + 1) ||
                        (pe.XPosition == mt.XPosition + 1 && pe.YPosition == mt.YPosition - 1))
                    {
                        if (!es.Contains(pe)) es.Add(pe);
                    }
                }
                //Debug.Log("m.RunPowe=2 count= " + es.Count);
            }
        }

        return es;
    }


    public List<PveEliminate> FindAttackEliminates(PveMonster m,List<PveEliminate>curRunPowers)
    {
        List<PveEliminate> es = new List<PveEliminate>();
       
        if (m.Range == 1)
        {
            foreach (PveEliminate pe in curRunPowers)
            {
                for (int i = 1; i <= m.Range; i++)
                {
                    foreach (PveEliminate p in AllEliminates)
                    {
                        if ((p.XPosition == pe.XPosition - i && p.YPosition == pe.YPosition) ||
                            (p.XPosition == pe.XPosition + i && p.YPosition == pe.YPosition) ||
                            (p.XPosition == pe.XPosition && p.YPosition == pe.YPosition - i) ||
                            (p.XPosition == pe.XPosition && p.YPosition == pe.YPosition + i))
                        {
                            if (p.SpeedRange.gameObject.activeInHierarchy == false)
                            {
                                es.Add(p);
                            }
                        }
                    }
                }
            }
        }
        else
        {
           
            foreach (PveEliminate p in curRunPowers)
            {
                for (int i = 1; i <= m.Range; i++)
                {
                    foreach (PveEliminate pe in AllEliminates)
                    {
                        if ((pe.XPosition == p.XPosition - i && pe.YPosition == p.YPosition) ||
                            (pe.XPosition == p.XPosition - i && pe.YPosition == p.YPosition + i) ||
                            (pe.XPosition == p.XPosition - i && pe.YPosition == p.YPosition - i) ||
                            (pe.XPosition == p.XPosition + i && pe.YPosition == p.YPosition) ||
                            (pe.XPosition == p.XPosition + i && pe.YPosition == p.YPosition + i) ||
                            (pe.XPosition == p.XPosition + i && pe.YPosition == p.YPosition - i) ||
                            (pe.XPosition == p.XPosition && pe.YPosition == p.YPosition - i) ||
                            (pe.XPosition == p.XPosition && pe.YPosition == p.YPosition + i))
                        {
                            if (pe.SpeedRange.gameObject.activeInHierarchy == false)
                            {
                                es.Add(pe);
                            }
                        }
                    }
                }
            }
        }
        return es;
    }

    //开始连线
    void TryBeginLink(PveEliminate touchBlock)
    {
        if (SkillConfirm.gameObject.activeInHierarchy == false)
        {
            if (touchBlock.CurEliminateAttribute == DungeonEnum.ElementAttributes.Player)
            {
                //起始点击主角所在方砖
                CurCharacter.ShowArrow(false);
                isLinking = true;
            }
            else if (touchBlock.IsNeighbour(CurCharacterEliminate) && !HasEnemyOnEliminate(touchBlock))
            {
                //起始点击主角周围方砖
				//有同颜色且没敌人的相邻
                LinkCount++;
                if (LinkCount < CurCharacter.DistanceLimit || CurCharacter.DistanceLimit == -1) //步数限制判断
                {
                    CurCharacter.ShowArrow(false);
                    BasicEliminate = touchBlock;
                    CurPathEliminate.Add(CurCharacterEliminate);
                    CurCharacterEliminate.NextLinkEliminate = touchBlock;
                    CurPathEliminate.Add(touchBlock);
                    touchBlock.LastLinkEliminate = CurCharacterEliminate;
                    ShowPossibleLinkPath();
                    isLinking = true;
                    CurCharacterEliminate.LinkToNextRender();
                    PvePlayerInfo.FocusPetAvata(BasicEliminate.CurEliminateAttribute, CurPathEliminate.Count - 1);
                    EnemiesBeHitNumShow(touchBlock, true);
                    EnemiesBeHitNumShow(CurCharacterEliminate, true);
                }
            }
        }
        
    }

    void TryEndLink()
    {
        if (isLinking)
        {
            float curLineTime = Time.realtimeSinceStartup;
            float spaceTime = curLineTime - LinePreTime;
            if (CancelGuideAction_LineUp() == true && spaceTime < 0.3f && CurPathEliminate.Count <= 1)
            {
                CurPathEliminate.Add(CurCharacterEliminate);
                BeginCharacterAction();
            }
            else if (CurPathEliminate.Count > 1&& CancelGuideAction_LineUp()==true)
            {
                //连线结束  角色开始行动
                BeginCharacterAction();
            }
            else
            {
                //连线方块不足  复原
                CurCharacter.ShowArrow(true);
                foreach (PveEliminate pe in AllEliminates)
                {
                    pe.SetEnabelRender(true);
                }
                EnemyUnitEliminateEnable(false);
            }
            ClearLastLink();
        }
    }

    void TryBackLink(PveEliminate touchBlock)
    {
        if (CurPathEliminate.Count > 0)
        {
            PveEliminate endEliminate = CurPathEliminate[CurPathEliminate.Count - 1];
            if (touchBlock.NextLinkEliminate == endEliminate)
            {
                //倒着依次返回
                CurPathEliminate.RemoveAt(CurPathEliminate.Count - 1);
                touchBlock.UnLinkFromLastRender(true);
                if (touchBlock != CurCharacterEliminate)
                {
                    PvePlayerInfo.DisFocusPetAvata(BasicEliminate.CurEliminateAttribute, CurPathEliminate.Count - 1);
                }
                EnemiesBeHitNumShow(touchBlock.NextLinkEliminate, false);
                endEliminate.LastLinkEliminate = null;
                touchBlock.NextLinkEliminate = null;
                ShowCurRange(FindPveTile(touchBlock.XPosition, touchBlock.YPosition));
            }
            if (touchBlock == CurCharacterEliminate)
            {
                //直接返回
                foreach (PveEnemyUnit eu in AllEnemies)
                {
                    eu.WillBeHitRender(0);
                }
                PvePlayerInfo.DisFocusPetAvata(BasicEliminate.CurEliminateAttribute, -1);
                if (CurPathEliminate.Count > 2)
                {
                    foreach (PveEliminate pe in CurPathEliminate)
                    {
                        pe.UnLinkFromLastRender(true);
                        pe.LinkChainLabel.gameObject.SetActive(false);
                    }
                }
                foreach (PveEliminate pe in AllEliminates)
                {
                    pe.SetEnabelRender(true);
                }
                EnemyUnitEliminateEnable(false);
                ClearLastLink();
                isLinking = true;
                ShowCurRange(FindPveTile(touchBlock.XPosition, touchBlock.YPosition));
            }
        }
    }

    void TryLink(PveEliminate touchBlock)
    {
        if (isLinking && !HasEnemyOnEliminate(touchBlock)&&touchBlock.CurEliminateAttribute!=DungeonEnum.ElementAttributes.Player)
        {
            if (BasicEliminate == null&&touchBlock.IsNeighbour(CurCharacterEliminate))
            {
                BasicEliminate = touchBlock;
                CurPathEliminate.Add(CurCharacterEliminate);
                CurCharacterEliminate.NextLinkEliminate = touchBlock;
                CurPathEliminate.Add(touchBlock);
                touchBlock.LastLinkEliminate = CurCharacterEliminate;
                ShowPossibleLinkPath();
                CurCharacterEliminate.LinkToNextRender();
                PvePlayerInfo.FocusPetAvata(BasicEliminate.CurEliminateAttribute, CurPathEliminate.Count - 1);
                EnemiesBeHitNumShow(touchBlock,true);
                EnemiesBeHitNumShow(CurCharacterEliminate, true);
                ShowCurRange(FindPveTile(touchBlock.XPosition, touchBlock.YPosition));
            }
            else if (BasicEliminate!=null && BasicEliminate.CurEliminateAttribute == touchBlock.CurEliminateAttribute && touchBlock.IsNeighbour(CurPathEliminate[CurPathEliminate.Count - 1]))
            {
                CurPathEliminate[CurPathEliminate.Count - 1].NextLinkEliminate = touchBlock;
                touchBlock.LastLinkEliminate = CurPathEliminate[CurPathEliminate.Count - 1];
                CurPathEliminate.Add(touchBlock);
                touchBlock.LastLinkEliminate.LinkToNextRender();
                PvePlayerInfo.FocusPetAvata(BasicEliminate.CurEliminateAttribute, CurPathEliminate.Count - 1);
                EnemiesBeHitNumShow(touchBlock, true);
                ShowCurRange(FindPveTile(touchBlock.XPosition, touchBlock.YPosition));
            }
            
        }
    }

    void EnemiesBeHitNumShow(PveEliminate pe,bool add)
    {
        foreach (PveEnemyUnit pu in AllEnemies)
        {
            if (pu.CurState == PveFightUnit.UnitState.guard) continue;
            if(pu.GetType() == typeof(PveMonster))
            {
                if(pu.CurMonsterType != MonsterData.MonsterType.RunAway)
                {
                    foreach (PveTile pt in pu.RangeTiles)
                    {
                        if ((pt.XPosition == pe.XPosition - 1 && pt.YPosition == pe.YPosition) ||
                            (pt.XPosition == pe.XPosition + 1 && pt.YPosition == pe.YPosition) ||
                            (pt.XPosition == pe.XPosition && pt.YPosition == pe.YPosition - 1) ||
                            (pt.XPosition == pe.XPosition && pt.YPosition == pe.YPosition + 1))
                        {
                            //pu.CurHitNum++;
                            int hitNum;
                            List<PetAvata> readyPetsAvata = PvePlayerInfo.GetFocusPetAvata();
                            int addition = 1 + readyPetsAvata.Count;
                            if (add)
                            {
                                hitNum = pu.CurHitNum + addition;
                            }
                            else
                            {
                                hitNum = pu.CurHitNum - addition;
                            }
                            pu.WillBeHitRender(hitNum);
                        }
                    }
                }
            }
            else
            {
                foreach (PveTile pt in pu.RangeTiles)
                {
                    if ((pt.XPosition == pe.XPosition - 1 && pt.YPosition == pe.YPosition) ||
                        (pt.XPosition == pe.XPosition + 1 && pt.YPosition == pe.YPosition) ||
                        (pt.XPosition == pe.XPosition && pt.YPosition == pe.YPosition - 1) ||
                        (pt.XPosition == pe.XPosition && pt.YPosition == pe.YPosition + 1))
                    {
                        //pu.CurHitNum++;
                        int hitNum;
                        List<PetAvata> readyPetsAvata = PvePlayerInfo.GetFocusPetAvata();
                        int addition = 1 + readyPetsAvata.Count;
                        if (add)
                        {
                            hitNum = pu.CurHitNum + addition;
                        }
                        else
                        {
                            hitNum = pu.CurHitNum - addition;
                        }
                        pu.WillBeHitRender(hitNum);
                    }
                }
            }
        }
    }

    List<PveEliminate> CurPossiblePath = new List<PveEliminate>();
    void ShowPossibleLinkPath()
    {
        List<PveEliminate> beginBlocks = new List<PveEliminate>();
        // beginBlocks.Add(CurCharacterEliminate);
        beginBlocks.Add(BasicEliminate);
        FindPath(beginBlocks, true);
        foreach (PveEliminate pe in AllEliminates)
        {
            
           // pe.ReturnToCommon();           
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
    void FindPath(List<PveEliminate> blocks, bool isBegin)
    {
        foreach (PveEliminate b in blocks)
        {
            DungeonEnum.ElementAttributes type = b.CurEliminateAttribute;
            if (isBegin)
            {
                type = CurCharacterEliminate.NextLinkEliminate.CurEliminateAttribute;
            }
            List<PveEliminate> sameNeighbour = FindSameNeighbour(b, type);
            //过滤已经存在的
            List<PveEliminate> NotExists = new List<PveEliminate>();
            foreach (PveEliminate b1 in sameNeighbour)
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

    List<PveEliminate> FindSameNeighbour(PveEliminate block, DungeonEnum.ElementAttributes type)
    {
        List<PveEliminate> neighbours = new List<PveEliminate>();
        foreach (PveEliminate b in AllEliminates)
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
        foreach (PveEliminate pe in CurPathEliminate)
        {          
            pe.NextLinkEliminate = null;
            pe.LastLinkEliminate = null;           
        }
        //DisShowAllRange();//
        CurPathEliminate.Clear();
        CurPossiblePath.Clear();
        BasicEliminate = null;
        isLinking = false;
    }
    #endregion

    #region 清理
    public void ClearEliminate(PveEliminate pe)
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
    void BossAppearRender()
    {        
        foreach (PveEnemyUnit eu in AllEnemies)
        {
            if (eu.GetType() == typeof(PveBoss))
            {
                PveBoss b = (PveBoss)eu;
                b.BossAppearRender();
            }
        }
    }
    void BossWarningResourcefun()
    {
        GameObject BossWarning = Instantiate(BossWarningResource) as GameObject;
        Invoke("Bmg_Play", 1f);
    }
    void Bmg_Play()
    {
        //切换到boss战音乐
        AudioSource gm = GameObject.Find("UI Root").transform.GetComponent<AudioSource>();
        gm.clip = Resources.Load("Audio/BGM_war_boss") as AudioClip;
        gm.Play();
    }
    //胜利特效
    public void RenderSuccess()
    {
        CurCharacter.ChangeAnimation(DungeonEnum.FaceDirection.LeftDown, PveFightUnit.ActionState.Victor);
        WinUI_MissGuide();
        UserInputLock = true;
        Invoke("InvoRenderSuccess", 2f);
    }
    void InvoRenderSuccess()
    {
        //切换到胜利音乐
		AudioSource gm = GameObject.Find("UI Root").transform.GetComponent<AudioSource>();
        gm.Stop();

        GameObject gm_victor_music = new GameObject();
        gm_victor_music.name = "gm_victor_music";
        gm = gm_victor_music.AddComponent<AudioSource>();
        gm.clip = Resources.Load("Audio/BGM_war_victor") as AudioClip;
        DontDestroyOnLoad(gm_victor_music);
        gm.Play();

        GameObject resultGameObject = Instantiate(Resources.Load("PreFabs/UI/Win")) as GameObject;
        UIEventListener.Get(resultGameObject).onClick = (obj) =>
        {
            SendResultToServer(1);
            OverControl.isOver = true;
        };
    }

    void SendResultToServer(int result,string isExit="")
    {
		if(this.runAwayMonsterList == null) this.runAwayMonsterList = new JsonArray();

        JsonObject args = new JsonObject();
        args.Add("dungeon_id", CurDungeonId);
		args.Add("achieve", AchievementManager.GetJson (result));
		args.Add ("run_away", this.runAwayMonsterList);
        args.Add("is_pass", result);
        string api = GameRouteConfig.OverScene;
        //if(isExit=="exit")

        SocketCenter.Request(GameRouteConfig.OverScene, args, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
                    JsonObject data = new JsonObject();
                    data.Add("chapter_id", ConfigManager.DungeonConfig.GetDungeonById(CurDungeonId).ChapterId);
                    data.Add("dungeon_id", CurDungeonId);
					if(r.Data.ContainsKey("achieve_state"))
					{
						data.Add("achieve_state", r.Data["achieve_state"].ToString());
					}else{
						data.Add("achieve_state", -1);
					}

                    JsonObject taskData = (JsonObject)r.Data["daily_task"];
                    
                    if (result == 1)
                    {

                        JsonArray allDrop = (JsonArray)r.Data["elements"];
                        OverControl.AddElement(allDrop);
                        OverControl.TaskProgressUpdate(taskData);
                        OverControl.HelpFriend = CurFriend;

                        UserManager.CurView = ViewControl.Views.Dungeon;
                        UserManager.CurUserInfo.RefreashDailyTasks(taskData);
                        UserManager.CurUserInfo.AddElements(allDrop);
                        UserManager.CurUserInfo.AddNewPassDungeon(data);
                    }
                    else
                    {
                        UserManager.CurView = ViewControl.Views.Monster;
                    }
                    UserManager.CurUserInfo.NotEndDungeon = null;
                    Application.LoadLevel("MainScene");
                });
            }
        }, null, true,true);
    }
    //失败
    public void RenderFail()
    {
        UserInputLock = true;
        GameObject resultGameObject = Instantiate(Resources.Load("PreFabs/FX/Lose")) as GameObject;
        UIEventListener.Get(resultGameObject).onClick = (obj) =>
        {
            SendResultToServer(0);
            OverControl.isOver = false;
        };
    }
    #endregion
    
    #region 技能
    public PveOwnUnit CurReadySkillUnit;
    public bool IsSkilling = false;
    public PvePet FindPet(UserPet p)
    {
        foreach (PveOwnUnit ou in AllOwns)
        {
            if (ou.GetType() == typeof(PvePet))
            {
                PvePet pp = (PvePet)ou;
                if (pp.CurUserPet == p)
                {
                    return pp;
                }
            }
        }
        return null;
    }

    public int GetHouseID()
    {
        PvePet pvpPet = this.CurReadySkillUnit as PvePet;
        if (pvpPet != null)
        {
            return pvpPet.CurUserPet.UserPetId;
        }          
        return -1;
    }
    public PveTile tranTile;
    public void SetTranTile()
    {
        List<PveTile> vl = new List<PveTile>();
        for (int i = 0; i < GameWidth; i++)
        {
            for (int j = 0; j < GameHeight; j++)
            {
                PveTile pt = FindPveTile(i, j);
                if (pt != null && pt.CanMoveOn && !pt.CanMoveOnWithBoss)
                {
                    vl.Add(pt);
                }
            }
        }
        int ind = Tools.GetRandom_n(vl.Count);
        tranTile = vl[ind];
    }
    public void UseSkillTransmit()
    {      
        //FineEliminate(CurCharacter.XPosition, CurCharacter.YPosition).ReturnToCommon_tra();
        //CurCharacter.SetPosition(tranTile.XPosition, tranTile.YPosition);
        //this.FineEliminate(tranTile.XPosition, tranTile.YPosition).SetToPlayerRender();
        AnimationHelper.AnimationFadeTo(1, CurCharacter.gameObject, iTween.EaseType.linear, null, null, 0);
        ////CurCharacter.ShowArrow(true);
        ////// 设置当前所在格子
        //this.CurCharacterEliminate = this.FineEliminate(tranTile.XPosition, tranTile.YPosition);

    }

    public List<PveFightUnit> skillResultList=new List<PveFightUnit>();
    public void PowerSkillRenderBumpAttack(int skillType, string skillPrefab, PveCharacter targetItem, bool skillStatus = true)
    {
        if (skillType == SkillEffectTypeEnum.NONE) return;
        
        if(skillResultList!=null)skillResultList.Clear();;
        
		Debug.Log("chainskill..  skillType= "+skillType);
        // 如果需要逐个格子演示
        if (skillType == SkillEffectTypeEnum.ITEM)
        {
            this.PowerSkillRenderBumpAttackSingle(skillPrefab, targetItem, skillStatus);
        }
        else
        {
            this.PowerSkillRenderBumpAttackWhole(skillPrefab, targetItem, skillStatus);
        }
    }

    private void PowerSkillRenderBumpAttackSingle(string skillPrefab, PveCharacter targetItem, bool skillStatus)
    {
        foreach (PveTile t in this.AllSkillRangesTile)
        {
            if (t.XPosition != targetItem.XPosition || t.YPosition != targetItem.YPosition)
            {
                this.StartCoroutine(this.PowerSkillRenderBumpAttackSingleItem(t, skillPrefab, targetItem, skillStatus));
                // 如果需要调用函数
                if (skillStatus) targetItem.Invoke("BumpEnd", 1);
            }
        }
    }
    private IEnumerator PowerSkillRenderBumpAttackSingleItem(PveTile t, string skillPrefab, PveCharacter targetItem, bool skillStatus)
    {
        yield return new WaitForSeconds(targetItem.Distance(targetItem, t) * 0.15f);

        GameObject skillResource = Resources.Load(DungeonSpritePathManager.SkillBumpFX(skillPrefab)) as GameObject;
        GameObject skillBump = GameObject.Instantiate(skillResource) as GameObject;
        skillBump.transform.position = t.transform.position;
        skillBump.SetActive(true);
        CameraControl.ShakeCamera();

        if (skillStatus)
        {
            PveFightUnit e = this.FindEnemyOn(t.XPosition, t.YPosition);
           
                if (e)
                {
                    if (!skillResultList.Contains(e) && e.CurState != PveFightUnit.UnitState.guard)
                    { 
                        e.SkillHurtRender(targetItem.ChainSkill);
                        skillResultList.Add(e);
                    }                   
                }
        }
    }

    private void PowerSkillRenderBumpAttackWhole(string skillPrefab, PveCharacter targetItem, bool skillStatus)
    {
        GameObject skillResource = Resources.Load(DungeonSpritePathManager.SkillBumpFX(skillPrefab)) as GameObject;
        GameObject skillBump = GameObject.Instantiate(skillResource) as GameObject;
        skillBump.transform.position = targetItem.transform.position;
        skillBump.SetActive(true);
        CameraControl.ShakeCamera();
        if (skillStatus)
        {
            foreach (PveTile t in this.AllSkillRangesTile)
            {
                PveFightUnit e = this.FindEnemyOn(t.XPosition, t.YPosition);
                if (e)
                {
                    //e.SkillHurtRender(targetItem.ChainSkill);//, targetItem);
                    if (!skillResultList.Contains(e) && e.CurState != PveFightUnit.UnitState.guard)
                    {
                        e.SkillHurtRender(targetItem.ChainSkill);
                        skillResultList.Add(e);
                    }         
                }
                if (t.XPosition != targetItem.XPosition || t.YPosition != targetItem.YPosition)
                {
                    targetItem.Invoke("BumpEnd", 1);
                }
            }
        }
    }


    public void UseSkillEnd()
    {
        UserInputLock = false;
        IsSkilling = false;
        ShowSkillBack(false);
        foreach (PveEliminate eb in AllEliminates)
        {
            eb.ReturnToCommon();
        }
    }


    public List<PveTile> AllSkillRangesTile = new List<PveTile>();

    public List<PveTile> CaculateSkillRangeTile(PveTile block, SkillData skillData)
    {
        AllSkillRangesTile.Clear();
        AllSkillRangesTile.Add(block);
        int centerX = block.XPosition;
        int centerY = block.YPosition;

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
            PveTile b = FindPveTile(realX, realY);
            if (b != null)
            {
                AllSkillRangesTile.Add(b);
            }
        }
        return AllSkillRangesTile;
    }


    public int TotalPower = 0;
    public void RecoverAllPetEnergy(bool b=true)
    {
        int hpow = UserManager.CurUserInfo.CurHero.HPower;
        
        if(b)TotalPower = TotalPower + (int)CurCharacter.RevPowerWithSkill;
        if (TotalPower > hpow) TotalPower = hpow;
        if (TotalPower < 0) TotalPower = 0;

        //Debug.Log("111  "+TotalPower+"  "+ hpow);
        float pre = (float)TotalPower  / (float)hpow;
        //power 进度条 
        PvePlayerInfo.SetPowerValue(pre,TotalPower);
        foreach (PveOwnUnit o in AllOwns)
        {
            if (o.GetType() == typeof(PvePet))
            {
                PvePet p = (PvePet)o;
                p.RecoverPower();
            }
        }
    }
    //宠物使用技能
    public void UseSkillWithPetFly()
    {
        PvePet pt = CurReadySkillUnit as PvePet;
        pt.gameObject.SetActive(true);
        IsSkilling = true;
        foreach (PveEliminate eb in AllEliminates)
        {
            eb.ReturnToCommon();
        }
        Debug.Log(CurReadySkillUnit.MainSkill.SkillFX+" ---  ");
        if (CurReadySkillUnit.MainSkill.SkillFX == "SkFX42")
        {
            //传送技 初始宠物位置
            SetTranTile();
            pt.SetPosition(tranTile.XPosition, tranTile.YPosition);

            FineEliminate(CurCharacter.XPosition, CurCharacter.YPosition).ReturnToCommon_tra();
            CurCharacter.SetPosition(tranTile.XPosition, tranTile.YPosition);
            this.FineEliminate(tranTile.XPosition, tranTile.YPosition).SetToPlayerRender();
            this.CurCharacterEliminate = this.FineEliminate(tranTile.XPosition, tranTile.YPosition);

        }
               
        AnimationHelper.AnimationFadeTo(0, CurCharacter.gameObject, iTween.EaseType.linear, null, null, 0);
        pt.PetFlyToScene(() =>
        {           
            pt.UnitWaiting(DungeonEnum.FaceDirection.Down);
            pt.UnitAttack( pt.CurFaceDirection);
        });
    }
    public void EndUseSkillWithPetFly()
    {
        PvePet pt = CurReadySkillUnit as PvePet;     
        pt.PetBackToAvata(() =>
        {
            AnimationHelper.AnimationFadeTo(1, CurCharacter.gameObject, iTween.EaseType.linear, null, null, 0);
        });
    }
   

    SkillData curPetSkill;
    bool cdStatus;
    public bool skillSelfStatus;
    public void UseSkill(SkillData sd, bool selfStatus = true, bool cdStatus = true)
    {

        
        BaseSkillItem skillItem = null;
        skillItem = CurCharacter.pvpPlayerSkill.GetSkillItemBySkillID(sd.Id);

        if (skillItem == null) return;
        this.skillSelfStatus = selfStatus;

		// 触发成就
		AchievementManager.Trigger (AchievementTypeEnum.No_Skill, 1);
        // 显示技能名称
        SkillFlyManager.Run(sd.Name, this.SkillFlyObject);
        // 重置状态
        this.shineEndStatus = false;
        //this.fightStatus = true;

        this.cdStatus = cdStatus;
        curPetSkill = sd;
        TotalPower = TotalPower - sd.SkillPower;
        UserInputLock = true;
        //IsSkilling = true;
        ShowSkillBack(false);

        foreach (PveEliminate eb in AllEliminates)
        {
            eb.ReturnToCommon();
        }

        //宠物 立即释放技能      
        GameObject shine = Instantiate(Resources.Load("PreFabs/FX/before_skill")) as GameObject;
        shine.transform.position = CurCharacter.transform.position;

        shine.SetActive(true);
        Invoke("ShineEnd", 0.6f);
    }
    private bool shineEndStatus;

    void ShineEnd()
    {
        if (this.cdStatus)
        {
            //更新能量值         
            RecoverAllPetEnergy();
        }
        // 确保只执行一次
        if (!this.shineEndStatus)
        {
            this.shineEndStatus = true;
            UserInputLock = false;	
			IsSkilling = false;

            PveSkillManager.Trigger(curPetSkill, CurCharacter, this.cdStatus, CurCharacter.SkillAttackEnd_sys);
            skilled = 1;
            BeginCharacterLoop();

        }
    }

    #region 技能背景
    public void ShowSkillBack(bool show)
    {
        SkillBack.SetActive(show);
    }
    #endregion

    /// <summary>
    /// 显示当前技能范围
    /// </summary>
    /// <param name="block"></param>
    void ShowCurRange(PveTile block)
    {
        if (CurCharacter.ChainSkill != null && (CurCharacter.ChainSkill.Bparameter < CurPathEliminate.Count))
        {
            DisShowAllRange();
            CaculateSkillRangeTile(block, CurCharacter.ChainSkill);
            foreach (PveTile b in AllSkillRangesTile)
            {
                if (FindNeighbourTileIn(b, AllSkillRangesTile).Count != 8)
                {
                    CaculateRangeLine(b, AllSkillRangesTile);
                }
                //FineEliminate(b.XPosition, b.YPosition).SetSkillEnabel(true);
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

    /// <summary>
    /// 计算范围线
    /// </summary>
    /// <param name="b"></param>
    /// <param name="allInRange"></param>
    public void CaculateRangeLine(PveTile b, List<PveTile> allInRange)
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
    void CaculateLine(PveTile b, List<PveTile> allInRange)
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
    void DrawRangeLine(PveTile b, LineDirection direction, BlockConner blockConner)
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
    void DrawRangeConner(PveTile b, ConnerDirection direction, bool reverse)
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
    void CaculateConner(PveTile b, List<PveTile> allInRange)
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
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    /// <param name="all"></param>
    /// <param name="curBlock"></param>
    /// <returns></returns>
    public PveTile FindOffset(int offsetX, int offsetY, List<PveTile> all, PveTile curBlock)
    {
        foreach (PveTile b in all)
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
        foreach (PveTile b in AllSkillRangesTile)
        {          
            FineEliminate(b.XPosition, b.YPosition).ReturnToCommon();
        }
        AllSkillRangesTile.Clear();
        foreach (Transform t in Ranges.transform)
        {
            Destroy(t.gameObject);
        }
    }
    #endregion


    #region 新技能相关
	public List<PveFightUnit> GetSkillFightUnitTarget(SkillData skillData, PveFightUnit sourceItem)
    {
		List<PveFightUnit> resultList = new List<PveFightUnit>();

        BaseSkillItem skillItem = sourceItem.pvpPlayerSkill.GetSkillItemBySkillID(skillData.Id);
		if(skillItem.skillData.skillTarget == SkillTargetTypeEnum.Enemy_Player || skillItem.skillData.skillTarget == SkillTargetTypeEnum.Self_Player)
		{
			//PvpFightUnit targetItem = this.GetFightUnitByUserType(sourceItem);
			resultList.Add( sourceItem);
		}
        else if (skillItem.skillData.skillTarget == SkillTargetTypeEnum.Range)
        {
            this.CaculateSkillRangeTile(this.FindPveTile(sourceItem.XPosition, sourceItem.YPosition), skillData);
            foreach (PveTile t in this.AllPveTiles)
            {
                PveEliminate eb =  FineEliminate(t.XPosition, t.YPosition);
                if (eb)
                {
                    // 如果包含，
                    if (this.AllSkillRangesTile.Contains(t))
                    {
                        PveEnemyUnit pveFightUnit = this.FindEnemyOn(eb.XPosition, eb.YPosition);                     
						if (pveFightUnit != null&& resultList.IndexOf(pveFightUnit)==-1) resultList.Add(pveFightUnit);
                    }
                }
            }
        }
        return resultList;
    }


    #endregion

    #region 新手引导相关
    public GuiderLocal GuiderLocalMa;
    public List<PveEliminate> GuideEliList = new List<PveEliminate>();
    public void GuideLineEliminates(List<Vector2> list) {
        SetEliminatesCollider(false);
        GuideEliList.Clear();
		foreach (Vector2 v in list)
        {
			foreach (PveEliminate pe in AllEliminates)
            {
                if ((int)v.x == pe.XPosition && (int)v.y == pe.YPosition)
                {
                    pe.GetComponent<PolygonCollider2D>().enabled = true;
                    GuideEliList.Add(pe);
                }
            }
        }
        //CancelGuideAction_LineEnter();
        //FirstLineOver = false;
        HandAction();
    }

    //划线演示是否结束
   public bool FirstLineOver = false;

    void HandAction()
    {
        
		foreach (PveEliminate pe in GuideEliList)
		{
			pe.UnLinkFromLastRender(false);
			pe.NextLinkEliminate = null;
		}
		CurGuideEliId = 0;
                
        GuiderLocalMa.HandOb.transform.position = GuideEliList[0].transform.position + new Vector3(0.07f, 0.07f, 0f);      
        InvokeRepeating("EliminatesLineShow", 0.1f, 0.2f);
    }
    //0.04f  需调
    public int CurGuideEliId = 0;
    void EliminatesLineShow()
    {
        GuiderLocalMa.HandOb.SetActive(true);
        if (CurGuideEliId < GuideEliList.Count-1)
        {
            PveEliminate pe = GuideEliList[CurGuideEliId];
            PveEliminate pen = GuideEliList[CurGuideEliId + 1];
            pe.NextLinkEliminate=pen;
            pe.LinkToNextRender();

            Hashtable args = new Hashtable();
            //args.Add("easeType", iTween.EaseType.linear);
            args.Add("speed", 1f);
            Vector3 v3 =pen.transform.position +new Vector3(0.07f, 0.07f, 0f);
            args.Add("position", v3);
            args.Add("oncompletetarget", gameObject);
            iTween.MoveTo(GuiderLocalMa.HandOb, args);  
      
			CurGuideEliId++;
        }else{
            GuiderLocalMa.HandOb.SetActive(false);
            FirstLineOver=true;
            CancelInvoke();
            HandAction();
		}       
    }
    void GuideHandActionEnd(string e)
    {
        //?stop
        Debug.Log(e);
        CancelInvoke();
        Invoke("HandAction", 1.5f);
    }
    void WinUI_MissGuide() {
        if (GuiderLocalMa != null)
        {
            GuiderLocalMa.gameObject.SetActive(false);
        }
    }
    void CancelGuideAction_LineEnter()
    {
        if (GuiderLocalMa != null && GuiderLocalMa.CurGuiddata!=null)
        {
           
            if (GuiderLocalMa.CurGuiddata.Condition == "hand")
            {
                foreach (PveEliminate pe in GuideEliList)
                {
                    pe.UnLinkFromLastRender(false);
                    pe.NextLinkEliminate = null;
                }
				iTween.Stop(GuiderLocalMa.HandOb);
                GuiderLocalMa.HandOb.SetActive(false);
            }
            CurGuideEliId = 0;
            CancelInvoke();
        }
    }
    void ResetEnemyHitNum()
    {
        foreach (PveEnemyUnit pu in AllEnemies)
        {
            //pu.CurHitNum = 0;
            pu.WillBeHitRender(0);
        }
    }
    bool CancelGuideAction_LineUp()
    {
        bool b = true;
        if (GuiderLocalMa != null)
        {
            if (GuiderLocalMa.CurGuiddata.Condition == "hand")
            {
                //Debug.Log(GuideEliList.Count + "   " + CurPathEliminate.Count);
                if (GuideEliList.Count > CurPathEliminate.Count || GuideEliList[GuideEliList.Count-1].name!=CurPathEliminate[CurPathEliminate.Count-1].name)
                {
                    //未走满
                    b = false;
                    FirstLineOver = true;
                    DisShowAllRange();
                    ResetEnemyHitNum();
                    GuiderLocalMa.ShowCurGuider();
                }
                else
                {
                    b = true;
                    FirstLineOver = false;
                }
            }
        }
        //Debug.Log("----b "+b);
        return b;
    }
    void GuideAction_TheBoutEnd_Id()
    {
        if (GuiderLocalMa != null)
        {
            if (GuiderLocalMa.CurGuiddata.Condition == "hand")
            {
                UserManager.CurUserInfo.GuideId++;
            }
        }
    }
    void GuideAction_TheBoutEnd_Action()
    {
        if (GuiderLocalMa != null)
        {
            if (GuiderLocalMa.CurGuiddata.Condition == "hand")
            {
                iTween.Stop(GuiderLocalMa.HandOb);
                GuiderLocalMa.HandOb.SetActive(false);
                //GuiderLocalMa.ShowCurGuider();
                GuiderLocalMa.DelayShowNextStep();
            }
        }
    }
    //是否处在引导副本
    //public bool IsGuiderDunge()
    //{      
    //    //if (UserManager.CurUserInfo.GuideId == -1 || UserManager.CurUserInfo.GuideId >= 41 || CurDungeonId != "D1_1")
    //    //{
    //    //    PvePlayerInfo.Exit_btn.SetActive(true);
    //    //    FirstLineOver = true;
    //    //    return false;
    //    //}
    //    //else
    //    //{
    //    //int guideid = UserManager.CurUserInfo.GuideId;
    //    //    GuiderData gd = ConfigManager.GuiderConfig.GetAllData()[guideid - 1];
    //    //    if (gd.Scene == "Pve" && GuiderLocalMa == null && CurDungeonId == "D1_1")
    //    //    {
    //    //        return true;
    //    //    }
    //    //    else
    //    //    {
    //    //        return false;
    //    //    }
    //    //}
    //}

    float tempHP = 1896f;
    void ShowGuider()
    {
       // Debug.Log(PveGameControl.CurDungeonId);
        if (GuiderLocal.TriggerPve()) 
        {
            Debug.Log("pve guider  start !");           
            
            PvePlayerInfo.HpUI.SetCurHpShow(CurCharacter.CurHp, CurCharacter.Hp);
            if (PveGameControl.CurDungeonId == "D1_1")
            {
                PvePlayerInfo.Exit_btn.SetActive(false);
            }
            else
            {
                //第二次进入副本  可以退出
                PvePlayerInfo.Exit_btn.SetActive(true);
                
                //关卡掉落提示
                this.ShowTips(3);   
            }

            Transform guide =transform.FindChild("GuiderPanel");
            GuiderLocalMa = guide.GetComponent<GuiderLocal>();
            GuiderLocalMa.gamecontrol = this;
            GuiderLocalMa.girlBool = true;
            guide.gameObject.SetActive(true);
        }
        else
        {
            PvePlayerInfo.Exit_btn.SetActive(true);
            FirstLineOver = true;
        }
    }
    public void SetEliminatesCollider(bool b)
    {
        foreach (PveEliminate pe in AllEliminates)
        {
            pe.GetComponent<PolygonCollider2D>().enabled = b;
        }
    }
    public void SetEliminatesColliderWithList(List<Vector2> list){
        foreach (PveEliminate pe in AllEliminates)
        {
            foreach (Vector2 v in list)
            {
                if ((int)v.x == pe.XPosition && (int)v.y == pe.YPosition)
                {
                    pe.GetComponent<PolygonCollider2D>().enabled = true;
                }
            }           
        }
    }
    #endregion

    #region 退出副本
    public GameObject Exit_Tips_ui;

    public void ExitEvent()
    {       
        UIEventListener.Get(PvePlayerInfo.Exit_btn).onClick = (g) =>
         {
             //Exit_Tips_ui.SetActive(true);
             //PopTips pt = Exit_Tips_ui.transform.GetComponent<PopTips>();
             //pt.SetTips("确认退出副本？",()=>{
             //   this.RenderFail();
             //});

             ShowTips(2);
         };
    
    }
    void ShowTips(int t = 2)
    {
      
        Resurrenction rs = CurResurrenction.GetComponent<Resurrenction>();
        Resurrenction.SysType = t;
        rs.GameControl = this;
        CurResurrenction.SetActive(true); 
    }
    #endregion
}
