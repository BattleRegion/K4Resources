using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System.Text.RegularExpressions;
public class DungeonScene : MonoBehaviour, OwnUnitInterFace, ObjectInterface,EnemyInterface
{
    #region 枚举
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
    #endregion

    #region 属性
    public DungeonTheme CutTheme;
    /// <summary>
    /// 当前层数
    /// </summary>
    public int CurFloor = 1;

    /// <summary>
    /// 游戏宽度
    /// </summary>
    int GameWidth = 7;

    /// <summary>
    /// 游戏高度
    /// </summary>
    int GameHeight = 9;

    /// <summary>
    /// 副本区域
    /// </summary>
    public GameObject DungeonArea;

    /// <summary>
    /// 所有地砖集合
    /// </summary>
    public List<TileBlock> FloorTiles = new List<TileBlock>();

    /// <summary>
    /// 当前所有消除块
    /// </summary>
    public List<EliminateBlock> CurAllEliminateBlock = new List<EliminateBlock>();

    /// <summary>
    /// 所有敌方单位
    /// </summary>
    public List<EnemyUnit> AllEnemyUnits = new List<EnemyUnit>();

    /// <summary>
    /// 所有我方单位
    /// </summary>
    public List<OwnUnit> AllOwnUnits = new List<OwnUnit>();

    /// <summary>
    /// 当前副本Id
    /// </summary>
    public string curDid;

    /// <summary>
    /// 用于记录当前是否为BOSS层
    /// </summary>
    bool hasBoss;

    /// <summary>
    /// 用户输入锁，只有在玩家回合的时候才能输入
    /// </summary>
    public bool userInputLock = true;

    /// <summary>
    /// 正在连接
    /// </summary>
    bool isLinking = false;

    /// <summary>
    /// 当前玩家对象所处的消除块
    /// </summary>
    public EliminateBlock curPlayerEliminateBlock;

    /// <summary>
    /// 前一次触摸的消除块
    /// </summary>
    EliminateBlock lastTouchEliminate;

    /// <summary>
    /// 当前消除块连接路径
    /// </summary>
    public List<TileBlock> curLinkPath = new List<TileBlock>();

    /// <summary>
    /// 当前连接的属性
    /// </summary>
    DungeonEnum.ElementAttributes BasicLinkAttributes = DungeonEnum.ElementAttributes.None;

    /// <summary>
    /// 用户初始位置
    /// </summary>
    static public int InitXPosition = 3;
    static public int InitYPosition = 1;

    /// <summary>
    /// 副本UI对象
    /// </summary>
    public DungeonUI CurDungeonUI;

    /// <summary>
    /// 当前回合我方行动队列
    /// </summary>
    public List<OwnUnit> CurOwnRoundActionQueue = new List<OwnUnit>();

    /// <summary>
    /// 当前可能的路径
    /// </summary>
    List<EliminateBlock> curPossiblePath = new List<EliminateBlock>();

    /// <summary>
    /// 当前我放行动单位结束数量；
    /// </summary>
    int curOwnActionEndCount = 0;

    /// <summary>
    /// 当前玩家指针
    /// </summary>
    public Player CurPlayer;

    /// <summary>
    /// 当前填充数量
    /// </summary>
    int fillMoveCount = 0;

    /// <summary>
    /// 回合可行动单位数量
    /// </summary>
    int canActionEnemyCount = 0;

    /// <summary>
    /// 是否在回合行动中
    /// </summary>
    bool isCurRoundAction = false;

    /// <summary>
    /// 门
    /// </summary>
    public FightDoor CurDoor;

    /// <summary>
    /// 攻击连接
    /// </summary>
    public AttackComboLabel AttackCL;

    /// <summary>
    /// 层过渡对象
    /// </summary>
    public GameObject FloorChange;

    /// <summary>
    /// 当前范围
    /// </summary>
    public List<TileBlock> AllRangesTile = new List<TileBlock>();

    public Confirm SkillConfirm;

    public GameObject SkillBack;

    public bool IsSkilling = false;

    static public string CurDId;
    #endregion

    #region 资源指针
    /// <summary>
    /// 地砖资源
    /// </summary>
    public GameObject TileResource;

    /// <summary>
    /// 消除块资源
    /// </summary>
    public GameObject EliminateResource;

    /// <summary>
    /// 怪物资源
    /// </summary>
    public GameObject MonsterResource;

    /// <summary>
    /// 障碍物资源
    /// </summary>
    public GameObject BarrierResource;

    /// <summary>
    /// 钥匙对象
    /// </summary>
    public GameObject KeyResource;

    /// <summary>
    /// 玩家资源
    /// </summary>
    public GameObject PlayerResource;

    /// <summary>
    /// 玩家宠物资源
    /// </summary>
    public GameObject PetResource;

    /// <summary>
    /// BOSS 出现警告资源
    /// </summary>
    public GameObject BossWarningResource;

    /// <summary>
    /// 技能范围资源指针
    /// </summary>
    public GameObject Line;

    public GameObject Conner;

    public GameObject Ranges;

    /// <summary>
    /// BOSS资源指针
    /// </summary>
    public GameObject BossResources;

    /// <summary>
    /// 胜负资源
    /// </summary>
    public GameObject WinResources;

    public GameObject LoseResources;

    public GameObject FloorInitCloudResources;

    public NewFloorNotice FloorNotice;

    public OwnUnit CurReadySkillUnit;
    #endregion

    #region 重写MONO
    void Awake () 
    {
        CreateDungeon(CurDId);
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

    #region 初始化副本
    /// <summary>
    /// 初始化副本
    /// </summary>
    /// <param name="dungeonId"></param>
    public void CreateDungeon(string dungeonId)
    {
        CurFloor = 1;
        curDid = dungeonId;
        DungeonData dd = ConfigManager.DungeonConfig.GetDungeonById(curDid);
        CutTheme.SetRender(dd.Scene,this);
        CreateCurFloorMap();
        //刷新游戏UI界面
        RefreshPlayerInfo();
        //进入游戏循环
        //StartLoop();
    }

    /// <summary>
    /// 刷新用户信息
    /// </summary>
    void RefreshPlayerInfo()
    {
        CurDungeonUI.CurPlayerUIInfo.SetFloorShow(CurFloor, ConfigManager.DungeonConfig.GetDungeonById(curDid).FloorCount);
        CurDungeonUI.CurPlayerUIInfo.HpUI.SetCurHpShow(CurPlayer.CurHp, CurPlayer.Hp);
    }

    /// <summary>
    /// 根据配置创建当前层
    /// </summary>
    void CreateCurFloorMap()
    {
        //清理上一层缓存
        ClearLastFloorMap();
        //创建基础元素 
        CreateBasicObject();
        //创建敌方单位
        CreateEnemyUnits();
        //创建己方单位
        CreateOwnUnits();
        if (CurFloor == 1)
        {
            //云
            GameObject cloud = Instantiate(FloorInitCloudResources) as GameObject;
        }
        Invoke("PlayerMoveIn", 1);
        Invoke("FloorNotic", 0.5f);
    }

    void FloorNotic()
    {
        FloorNotice.ShowNewFloor(CurFloor, ConfigManager.DungeonConfig.GetDungeonById(curDid).FloorCount);
    }

    /// <summary>
    /// 角色进入
    /// </summary>
    void PlayerMoveIn()
    {
        CurPlayer.gameObject.SetActive(true);
        CurPlayer.SetPosition(3, 0);
        CurPlayer.UnitMove(DungeonEnum.FaceDirection.Up, FindTileByPosition(3, 1), "moveEnd", gameObject);
    }

    /// <summary>
    /// 角色进入结束
    /// </summary>
    int initCount = 63;
    void moveEnd()
    {
        initCount = CurAllEliminateBlock.Count;
        CurPlayer.UnitWaiting(DungeonEnum.FaceDirection.Up);
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
            StartCoroutine(EliminateBlockInitFallIn(i, waitingTimes[i]/2));
        }
    }

    IEnumerator EliminateBlockInitFallIn(int col,float waiting)
    {
        yield return new WaitForSeconds(waiting);

        List<EliminateBlock> columnBlocks = FindBlocksByColumn(col);
        for (int m = 0; m < columnBlocks.Count; m++)
        {
            columnBlocks[m].YPosition = columnBlocks[m].YPosition - 9;
            StartCoroutine(columnBlocks[m].ResertPositionAnimation(650, 0.15f * m,"InitResertEnd"));
        }
    }

    void InitResertEnd(GameObject blockObj)
    {
        initCount--;
        if (initCount == 0)
        {
            FloorNotice.MoveOut();
            curPlayerEliminateBlock = FindEliminateByPosition(InitXPosition, InitYPosition);
            curPlayerEliminateBlock.SetToPlayerRender();
            StartLoop();
        }
    }

    /// <summary>
    /// 清理上一层数据缓存
    /// </summary>
    void ClearLastFloorMap()
    {
        if (CurFloor > 1)
        {
            CurPlayer.HasFloorKey = false;
        }
        userInputLock = true;
        hasBoss = false;
        foreach (EliminateBlock b in CurAllEliminateBlock)
        {
            Destroy(b.gameObject);
        }
        CurAllEliminateBlock.Clear();
        foreach (EnemyUnit eu in AllEnemyUnits)
        {
            Destroy(eu.gameObject);
        }
        AllEnemyUnits.Clear();
        CurDoor.CloseDoor();
    }
    #endregion

    #region 创建地图元素的方法
    /// <summary>
    /// 创建基础元素，地砖和消除块
    /// </summary>
    void CreateBasicObject()
    {
        for (int i = 0; i < GameHeight; i++)
        {
            for (int j = 0; j < GameWidth; j++)
            {
                //只有第一层才需要创建地砖
                if (CurFloor == 1)
                {
                    //初始化地块
                    StartCoroutine(InitTile(j, i));
                }
                //初始化消除块
                StartCoroutine(InitEliminateBlock(j, i+9));
            }
        }
    }

    /// <summary>
    /// 创建当前层的所有敌方单位
    /// </summary>
    void CreateEnemyUnits()
    {
        List<MapElementData> elementsData = ConfigManager.MapConfig.GetElementsData(curDid, CurFloor);
        foreach (MapElementData elementData in elementsData)
        {
            if (elementData.Type == MapElementData.ElementType.Barrier)
            {
                BarrierData barrierData = ConfigManager.BarrierConfig.GetBarrierById(elementData.ElementId);
                StartCoroutine(InitBarrier(barrierData, elementData.XPosition, elementData.YPosition, elementData.Xrange, elementData.Yrange));
            }
            else if (elementData.Type == MapElementData.ElementType.Monster)
            {
                MonsterData monsterData = ConfigManager.MonsterConfig.GetMonsterById(elementData.ElementId);
                Debug.Log(elementData.ElementId);
                StartCoroutine(InitMonster(monsterData, elementData.XPosition, elementData.YPosition, elementData.Xrange, elementData.Yrange));
            }
            else if (elementData.Type == MapElementData.ElementType.Item)
            {
                ItemData itemData = ConfigManager.ItemConfig.GetItemById(elementData.ElementId);
                StartCoroutine(InitItem(itemData, elementData.XPosition, elementData.YPosition));
            }
            else if (elementData.Type == MapElementData.ElementType.Boss)
            {
                hasBoss = true;
                BossData bossData = ConfigManager.BossConfig.GetBossById(elementData.ElementId);
                StartCoroutine(InitBoss(bossData, elementData.XPosition, elementData.YPosition, elementData.Xrange, elementData.Yrange));
            }
        }
    }

    /// <summary>
    /// 创建己方单位
    /// </summary>
    void CreateOwnUnits()
    {
        if (CurFloor == 1)
        {
            //创建玩家
            StartCoroutine(InitPlayer());
            //创建玩家宠物
            foreach (UserPet p in UserManager.CurUserInfo.UserPets)
            {
                StartCoroutine(InitPet(p));
            }
        }
        else
        {
            curPlayerEliminateBlock = FindEliminateByPosition(InitXPosition, InitYPosition);
            foreach (OwnUnit u in AllOwnUnits)
            {
                u.SetPosition(InitXPosition, InitYPosition);
                u.SetObjectName();
            }
        }
    }

    /// <summary>
    /// 创建地砖
    /// </summary>
    /// <returns></returns>
    IEnumerator InitTile(int xPosition,int yPosition)
    {
        GameObject tileObject = NGUITools.AddChild(DungeonArea, TileResource);
        TileBlock tile = tileObject.GetComponent<TileBlock>();
        DungeonData dd = ConfigManager.DungeonConfig.GetDungeonById(curDid);
        tile.DungeonScene = this;
        FloorTiles.Add(tile);
        tile.RenderObjectSprite(xPosition, yPosition);
        tile.ObjectSprite.sprite = Resources.Load<Sprite>("OrginPic/FightBackGround/" + dd.Scene + "/" + "tile" + dd.Scene);
        yield return null;
    }

    /// <summary>
    /// 创建消除块
    /// </summary>
    /// <param name="xPosition"></param>
    /// <param name="yPosition"></param>
    /// <returns></returns>
    IEnumerator InitEliminateBlock(int xPosition, int yPosition)
    {
        GameObject EliminateObject = NGUITools.AddChild(DungeonArea, EliminateResource);
        EliminateBlock eliminateBlock = EliminateObject.GetComponent<EliminateBlock>();
        eliminateBlock.DungeonScene = this;
        CurAllEliminateBlock.Add(eliminateBlock);
        eliminateBlock.RenderObjectSprite(xPosition, yPosition);
        if (eliminateBlock.CurEliminateAttribute == DungeonEnum.ElementAttributes.Player)
        {
            curPlayerEliminateBlock = eliminateBlock;
        }
        yield return null;
    }

    /// <summary>
    /// 创建障碍物
    /// </summary>
    /// <returns></returns>
    IEnumerator InitBarrier(BarrierData barrierData, int xPosition, int yPosition,int xRange,int yRange)
    {
        GameObject barrierObject = NGUITools.AddChild(DungeonArea, BarrierResource);
        Barrier barrier = barrierObject.GetComponent<Barrier>();
        barrier.DungeonScene = this;
        barrier.CurHp = barrierData.Hp;
        barrier.Hp = barrierData.Hp;
        barrier.XRange = xRange;
        barrier.YRange = yRange;
        barrier.SetSkin(ConfigManager.SkinConfig.GetSkinDataById(barrierData.SkinId));
        barrier.RenderObjectSprite(xPosition, yPosition);
        barrier.EnemyHandler = this;
        AllEnemyUnits.Add(barrier);
        yield return null;
    }

    /// <summary>
    /// 创建怪物
    /// </summary>
    /// <param name="monsterData"></param>
    /// <returns></returns>
    IEnumerator InitMonster(MonsterData monsterData,int xPosition,int yPosition,int xRange,int yRange)
    {
        Debug.Log(monsterData);
        Debug.Log(monsterData.Id);
        GameObject monsterObject = NGUITools.AddChild(DungeonArea, MonsterResource);
        Monster monster = monsterObject.GetComponent<Monster>();
        monster.DungeonScene = this;
        monster.Range = monsterData.Range;
        monster.RunPower = monsterData.RunPower;
        monster.XRange = xRange;
        monster.YRange = yRange;
        //monster.CurAttackType = monsterData.Type;
        monster.EnemyHandler = this;
        monster.CurHp = monsterData.Hp;
        monster.Hp = monsterData.Hp;
        monster.Atk = monsterData.Attack;
        monster.SetSkin(ConfigManager.SkinConfig.GetSkinDataById(monsterData.SkinId));
        monster.RenderObjectSprite(xPosition, yPosition);
        AllEnemyUnits.Add(monster);
        yield return null;
    }

    /// <summary>
    /// 创建BOSS
    /// </summary>
    /// <param name="bossData"></param>
    /// <returns></returns>
    IEnumerator InitBoss(BossData bossData, int xPosition, int yPosition,int xRange,int yRange)
    {
        GameObject bossObject = NGUITools.AddChild(DungeonArea, BossResources);
        Boss boss = bossObject.GetComponent<Boss>();
        boss.FightOff = bossData.FightOff;
        boss.DungeonScene = this;
        boss.Range = bossData.Range;
        boss.RunPower = bossData.RunPower;
        boss.CurHp = bossData.Hp;
        boss.XRange = xRange;
        boss.YRange = yRange;
        boss.Hp = bossData.Hp;
        boss.Atk = bossData.Attack;
        boss.EnemyHandler = this;
        boss.SetSkin(ConfigManager.SkinConfig.GetSkinDataById(bossData.SkinId));
        boss.RenderObjectSprite(xPosition, yPosition);
        AllEnemyUnits.Add(boss);
        yield return null;
    }

    /// <summary>
    /// 创建道具
    /// </summary>
    /// <param name="itemData"></param>
    /// <returns></returns>
    IEnumerator InitItem(ItemData itemData, int xPosition, int yPosition)
    {
        /////钥匙
        //if (itemData.Id == "I1")
        //{
        //    //GameObject keyObject = NGUITools.AddChild(DungeonArea, KeyResource);
        //    //Key key = keyObject.GetComponent<Key>();
        //    //key.EnemyHandler = this;
        //    //key.DungeonScene = this;
        //    //key.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        //    //key.SetSkin(ConfigManager.SkinConfig.GetSkinDataById(itemData.SkinId));
        //    //key.RenderObjectSprite(xPosition, yPosition);
        //    //AllEnemyUnits.Add(key);
        //}
        yield return null;
    }

    /// <summary>
    /// 创建玩家对象
    /// </summary>
    /// <returns></returns>
    IEnumerator InitPlayer()
    {
        GameObject playerObject = NGUITools.AddChild(DungeonArea, PlayerResource);
        playerObject.SetActive(false);
        CurPlayer = playerObject.GetComponent<Player>();
        CurPlayer.Skill = ConfigManager.SkillConfig.GetSkillById("Sk1");
        CurPlayer.DungeonScene = this;
        CurPlayer.OwnUnitHandler = this;
        CharacterData characterData = ConfigManager.CharacterConfig.CharacterById(UserManager.CurUserInfo.CharacterId);
        CurPlayer.Hp = characterData.Hp;
        foreach (UserPet p in UserManager.CurUserInfo.UserPets)
        {
            CurPlayer.Hp = CurPlayer.Hp + p.CurPetData.Hp;
        }
        CurPlayer.CurHp = CurPlayer.Hp;
        CurPlayer.Atk = characterData.Attack;
        CurPlayer.SetSkin(ConfigManager.SkinConfig.GetSkinDataById(characterData.SkinId));
        CurPlayer.RenderObjectSprite(InitXPosition, InitYPosition);
        AllOwnUnits.Add(CurPlayer);
        yield return null; 
    }

    /// <summary>
    /// 创建玩家携带的宠物对象
    /// </summary>
    /// <returns></returns>
    IEnumerator InitPet(UserPet userPet)
    {
        int petInitX = InitXPosition;
        int petInitY = InitYPosition;
        GameObject petObject = NGUITools.AddChild(DungeonArea, PetResource);
        Pet pet = petObject.GetComponent<Pet>();
        pet.Skill = userPet.CurPetData.PetSkillData;
        pet.DungeonScene = this;
        pet.Atk = userPet.CurPetData.Attack;
        pet.OwnUnitHandler = this;
        pet.UserPet = userPet;
        PetData petData = ConfigManager.PetConfig.GetPetById(userPet.CurPetData.Id);
        pet.SetSkin(ConfigManager.SkinConfig.GetSkinDataById(petData.SkinId));
        pet.RenderObjectSprite(petInitX, petInitY);
        AllOwnUnits.Add(pet);
        yield return null; 
    }
    #endregion

    #region 游戏循环逻辑
    /// <summary>
    /// 进入当前游戏循环
    /// </summary>
    void GameLoop()
    {
        if (!TryLoseRender())
        {
            Debug.Log("进入游戏循环");
            ClearLastLoop();
        }
    }

    /// <summary>
    /// 清理上次循环的缓存数据
    /// </summary>
    void ClearLastLoop()
    {
        curOwnActionEndCount = 0;
        CurOwnRoundActionQueue.Clear();
        userInputLock = false;
        isCurRoundAction = false;
        foreach (EnemyUnit eu in AllEnemyUnits)
        {
            if (eu.GetType() == typeof(Monster))
            {
                Monster b = (Monster)eu;
                b.HasAttackInOwnRound = false;
            }
        }
        RefreshBlockRenderEnabel();
        Debug.Log("清理上一回合缓存数据,等待用户消除操作");
        CurPlayer.ShowArrow(true);
    }

    /// <summary>
    /// 开始循环
    /// </summary>
    void StartLoop()
    {
        //判断是否有BOSS
        if (hasBoss)
        {
            //渲染BOSS出现特效
            BossAppearRender();
        }
        else
        {
            //进入游戏循环
            GameLoop();
        }
    }
    #endregion

    #region 场景特效渲染
    void BossAppearRender()
    {
        GameObject BossWarning = Instantiate(BossWarningResource) as GameObject;
        foreach (EnemyUnit eu in AllEnemyUnits)
        {
            if (eu.GetType() == typeof(Boss))
            {
                Boss b = (Boss)eu;
                b.BossAppearRender();
            }
        }
        Invoke("GameLoop", 1.5f);
    }
    #endregion

    #region 用户输入监测
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
		if(Input.touchCount>=1)
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

    /// <summary>
    /// 射线碰撞对象检测
    /// </summary>
    void RayBumpObjectCheck(Vector3 touchPosition, TouchType type)
    {
        if (userInputLock == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchPosition), Vector2.zero);
            if (hit)
            {
                GameObject hitObject = hit.collider.gameObject;
                EliminateBlock hitEliminateBlock = hitObject.GetComponent<EliminateBlock>();
                if (hitEliminateBlock)
                {
                    if (type == TouchType.Enter)
                    {
                        TryBeginLink(hitEliminateBlock);
                    }
                    else if (type == TouchType.Hold)
                    {
                        if (lastTouchEliminate != hitEliminateBlock)
                        {
                            TryLink(hitEliminateBlock);
                            TryBackLink(hitEliminateBlock);
                        }
                    }
                    else if (type == TouchType.Out)
                    {
                        TryEndLink();
                    }
                    lastTouchEliminate = hitEliminateBlock;
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
    #endregion

    #region 消除连接逻辑
    /// <summary>
    /// 尝试开始连接
    /// </summary>
    void TryBeginLink(EliminateBlock touchBlock)
    {
        if (touchBlock.CurEliminateAttribute == DungeonEnum.ElementAttributes.Player)
        {
            isLinking = true;
            CurPlayer.ShowArrow(false);
            //Debug.Log("开始连接");
        }
        else if (touchBlock.isNeighbour(curPlayerEliminateBlock) && !HasEnemyUnitOnBlock(touchBlock))
        {
            isLinking = true;
            TryLink(touchBlock);
            CurPlayer.ShowArrow(false);
           // Debug.Log("开始连接");
        }
    }

    /// <summary>
    /// 连接
    /// </summary>
    void TryLink(EliminateBlock touchBlock)
    {
        if (isLinking && !HasEnemyUnitOnBlock(touchBlock))
        {
            Debug.Log("try" + touchBlock);
            if (BasicLinkAttributes == DungeonEnum.ElementAttributes.None&&curPlayerEliminateBlock.isNeighbour(touchBlock))
            {
                BasicLinkAttributes = touchBlock.CurEliminateAttribute;
                curPlayerEliminateBlock.NextEliminateBlock = touchBlock;
                curLinkPath.Add(FindTileByPosition(curPlayerEliminateBlock.XPosition, curPlayerEliminateBlock.YPosition));
                curLinkPath.Add(FindTileByPosition(touchBlock.XPosition, touchBlock.YPosition));
                curPlayerEliminateBlock.LinkToRender();
                CurDungeonUI.FocusPetAvata(BasicLinkAttributes, curLinkPath.Count - 1);
                //显示路劲
                List<EliminateBlock> beginBlocks = new List<EliminateBlock>();
                beginBlocks.Add(curPlayerEliminateBlock);
                curPossiblePath.Add(curPlayerEliminateBlock);
                FindPath(beginBlocks, true);
                foreach (EliminateBlock b in CurAllEliminateBlock)
                {
                    if (curPossiblePath.Contains(b) == false)
                    {
                        b.SetEnabelRender(false);
                    }
                }
                ShowCurRange(FindTileByPosition(touchBlock.XPosition, touchBlock.YPosition));
                CheckLinkNeighbourEnemy();
                //Debug.Log("第一个连接块" + touchBlock);
            }
            else
            {
                if (curLinkPath.Count > 0)
                {
                    EliminateBlock lastEliminateBlock = FindEliminateByPosition(curLinkPath[curLinkPath.Count - 1].XPosition, curLinkPath[curLinkPath.Count - 1].YPosition);
                    if (touchBlock.CurEliminateAttribute == BasicLinkAttributes && touchBlock.isNeighbour(lastEliminateBlock) && !curLinkPath.Contains(FindTileByPosition(touchBlock.XPosition, touchBlock.YPosition)))
                    {
                        lastEliminateBlock.NextEliminateBlock = touchBlock;
                        touchBlock.LastEliminateBlock = lastEliminateBlock;
                        curLinkPath.Add(FindTileByPosition(touchBlock.XPosition, touchBlock.YPosition));
                        lastEliminateBlock.LinkToRender();
                        CurDungeonUI.FocusPetAvata(BasicLinkAttributes, curLinkPath.Count - 1);
                        ShowCurRange(FindTileByPosition(touchBlock.XPosition, touchBlock.YPosition));
                        CheckLinkNeighbourEnemy();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 尝试结束连接
    /// </summary>
    void TryEndLink()
    {
        if (isLinking)
        {
            if (curLinkPath.Count > 0)
            {
                foreach (EliminateBlock eb in CurAllEliminateBlock)
                {
                    eb.SetEnabelRender(true);
                }
                OwnRound();
            }
            else
            {
                CurPlayer.ShowArrow(true);
                ClearLastLink();
                RefreshBlockRenderEnabel();
            }
        }
    }

    /// <summary>
    /// 尝试回退连接
    /// </summary>
    void TryBackLink(EliminateBlock touchBlock)
    {
        if (isLinking && curLinkPath.Count>0)
        {
            EliminateBlock lastEliminateBlock = FindEliminateByPosition(curLinkPath[curLinkPath.Count - 1].XPosition, curLinkPath[curLinkPath.Count - 1].YPosition);
            if (touchBlock.CurEliminateAttribute == DungeonEnum.ElementAttributes.Player)
            {
                CurDungeonUI.DisFocusPetAvata(BasicLinkAttributes, -1);
                UnLinkRenderAll();
                ClearLastLink();
                isLinking = true;
                DisShowAllRange();
                RefreshBlockRenderEnabel();
            }
            else if (curLinkPath.Count > 2 && touchBlock.NextEliminateBlock == lastEliminateBlock)
            {
                curLinkPath.Remove(FindTileByPosition(lastEliminateBlock.XPosition, lastEliminateBlock.YPosition));
                touchBlock.UnLinkRender(true);
                touchBlock.NextEliminateBlock = null;
                lastEliminateBlock.ClearEliminateBlock();
                CurDungeonUI.DisFocusPetAvata(BasicLinkAttributes, curLinkPath.Count -1);
                ShowCurRange(FindTileByPosition(touchBlock.XPosition, touchBlock.YPosition));
                CheckLinkNeighbourEnemy();
            }
        }
    }

    /// <summary>
    /// 清理上次连接数据
    /// </summary>
    void ClearLastLink()
    {
        isLinking = false;
        BasicLinkAttributes = DungeonEnum.ElementAttributes.None;
        curLinkPath.Clear();
        curPossiblePath.Clear();
    }

    /// <summary>
    /// 取消所有连接渲染
    /// </summary>
    void UnLinkRenderAll()
    {
        foreach (TileBlock block in curLinkPath)
        {
            EliminateBlock eb = FindEliminateByPosition(block.XPosition, block.YPosition);
            eb.UnLinkRender(false);
            eb.SetChain(false);
        }
        foreach (EnemyUnit eu in AllEnemyUnits)
        {
            eu.WillBeHitRender(0);
        }
    }

    /// <summary>
    /// 查找可以连接的路径
    /// </summary>
    void FindPath(List<EliminateBlock> blocks, bool isBegin)
    {
        foreach (EliminateBlock b in blocks)
        {
            DungeonEnum.ElementAttributes type = b.CurEliminateAttribute;
            if (isBegin)
            {
                type = curPlayerEliminateBlock.NextEliminateBlock.CurEliminateAttribute;
            }
            List<EliminateBlock> sameNeighbour = FindSameNeighbour(b, type);
            //过滤已经存在的
            List<EliminateBlock> NotExists = new List<EliminateBlock>();
            foreach (EliminateBlock b1 in sameNeighbour)
            {
                if (curPossiblePath.Contains(b1) == false&&!HasEnemyUnitOnBlock(b1))
                {
                    NotExists.Add(b1);
                    curPossiblePath.Add(b1);
                }
            }
            if (NotExists.Count != 0)
            {
                // 继续寻路
                FindPath(NotExists, false);
            }
        }
    }

    /// <summary>
    /// 查找周围8个消除块里某个颜色类型的
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    List<EliminateBlock> FindSameNeighbour(EliminateBlock block, DungeonEnum.ElementAttributes type)
    {
        List<EliminateBlock> neighbours = new List<EliminateBlock>();
        foreach (EliminateBlock b in CurAllEliminateBlock)
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

    /// <summary>
    /// 填充消除块
    /// </summary>
    List<EliminateBlock> readyFillBlocks = new List<EliminateBlock>();
    void FillEliminateBlocks()
    {
        Debug.Log("填充消除块");
        fillMoveCount = 0;
        readyFillBlocks.Clear();
        //逐列填充
        for (int i = 0; i < GameWidth; i++)
        {
            List<EliminateBlock> columnBlocks = FindBlocksByColumn(i);
            Barrier highestBarrier = FindHighestBarriarOnColomn(i);
            if (columnBlocks.Count < GameHeight)
            {
                if (highestBarrier != null)
                {
                    ///障碍物上方的块
                    List<EliminateBlock> onBarriar = new List<EliminateBlock>();
                    foreach (EliminateBlock b in columnBlocks)
                    {
                        if (b.YPosition > highestBarrier.YPosition)
                        {
                            onBarriar.Add(b);
                        }
                    }
                    CreateFills(highestBarrier.YPosition + 1, onBarriar, i);
                }
                else
                {
                    CreateFills(0, columnBlocks, i);
                }
            }
        }
        AllFill();
    }

    /// <summary>
    /// 创建填充
    /// </summary>
    void CreateFills(int fillFrom, List<EliminateBlock> curExistsBlock, int column)
    {
        //没有障碍物全部填充
        //按当前Y坐标重新排序
        curExistsBlock = SortColumnBlocks(curExistsBlock);
        int YIndex = fillFrom;
        for (int j = 0; j < curExistsBlock.Count; j++)
        {
            curExistsBlock[j].YPosition = YIndex;
            curExistsBlock[j].SetObjectName();
            readyFillBlocks.Add(curExistsBlock[j]);
            YIndex++;
        }
        int creatIndex = 9;
        //创建填充
        for (int k = YIndex; k <= 8; k++)
        {
            GameObject rbGameObject = NGUITools.AddChild(DungeonArea, EliminateResource);
            EliminateBlock removeBlock = rbGameObject.GetComponent<EliminateBlock>();
            removeBlock.DungeonScene = this;
            CurAllEliminateBlock.Add(removeBlock);
            removeBlock.SetSprite();
            removeBlock.SetPosition(column, creatIndex);
            removeBlock.XPosition = column;
            removeBlock.YPosition = k;
            removeBlock.SetObjectName();
            curExistsBlock.Add(removeBlock);
            readyFillBlocks.Add(removeBlock);
            creatIndex++;
        }

        //下落
        fillMoveCount = fillMoveCount + curExistsBlock.Count;
    }

    void AllFill()
    {
        for (int i = 0; i < 7; i++)
        {
            StartCoroutine(FillColoumn(i,i*0.1f));
        }
    }

    IEnumerator FillColoumn(int col,float delay)
    {
        yield return new WaitForSeconds(delay);
        List<EliminateBlock> columnB = new List<EliminateBlock>();
        for (int i = 0; i < readyFillBlocks.Count; i++)
        {
            EliminateBlock b = readyFillBlocks[i];
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
    #endregion

    #region 工具方法
    public void RefreshBlockRenderEnabel()
    {
        foreach(EliminateBlock eb in CurAllEliminateBlock)
        {
            eb.SetEnabelRender(true);
        }
        
        foreach (EnemyUnit eu in AllEnemyUnits)
        {
            foreach (TileBlock tb in eu.PositionTiles)
            {
                EliminateBlock eb = FindEliminateByPosition(tb.XPosition, tb.YPosition);
                if (eb)
                {
                    eb.SetEnabelRender(false);
                }
            }
        }
    }

    /// <summary>
    /// 排序某一列的消除块
    /// </summary>
    /// <param name="blocks"></param>
    /// <returns></returns>
    List<EliminateBlock> SortColumnBlocks(List<EliminateBlock> blocks)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            for (int j = 0; j < blocks.Count - i - 1; j++)
            {
                EliminateBlock b1 = blocks[j];
                EliminateBlock b2 = blocks[j + 1];
                if (b1.YPosition > b2.YPosition)
                {
                    EliminateBlock temp = blocks[j];
                    blocks[j] = blocks[j + 1];
                    blocks[j + 1] = temp;
                }
            }
        }
        return blocks;
    }

    /// <summary>
    /// 是否有单位在某个消除块上
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    bool HasEnemyUnitOnBlock(EliminateBlock block)
    {
        foreach (EnemyUnit eu in AllEnemyUnits)
        {
            foreach (TileBlock tb in eu.PositionTiles)
            {
                EliminateBlock eb = FindEliminateByPosition(tb.XPosition, tb.YPosition);
                if (eb == block)
                {
                    return true;
                }
             }
        }
        return false;
    }

    /// <summary>
    /// 根据位置查找敌方单位
    /// </summary>
    /// <param name="xPosition"></param>
    /// <param name="yPosition"></param>
    /// <returns></returns>
    public EnemyUnit FindEnemyUnit(int xPosition, int yPosition)
    {
        foreach (EnemyUnit e in AllEnemyUnits)
        {
            foreach (TileBlock tb in e.PositionTiles)
            {
                if (tb.XPosition == xPosition && tb.YPosition == yPosition)
                {
                    return e;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 查找地块
    /// </summary>
    /// <param name="xPosition"></param>
    /// <param name="yPosition"></param>
    /// <returns></returns>
    public TileBlock FindTileByPosition(int xPosition, int yPosition)
    {
        foreach (TileBlock tb in FloorTiles)
        {
            if (tb.XPosition == xPosition && tb.YPosition == yPosition)
            {
                return tb;
            }
        }
        return null;
    }

    /// <summary>
    /// 位置查找消除块
    /// </summary>
    /// <param name="xPosition"></param>
    /// <param name="yPosition"></param>
    /// <returns></returns>
    public EliminateBlock FindEliminateByPosition(int xPosition, int yPosition)
    {
        foreach (EliminateBlock eb in CurAllEliminateBlock)
        {
            if (eb.XPosition == xPosition && eb.YPosition == yPosition)
            {
                return eb;
            }
        }
        return null;
    }

    /// <summary>
    /// 查找Tile
    /// </summary>
    /// <param name="xPosition"></param>
    /// <param name="yPosition"></param>
    /// <returns></returns>
    public TileBlock FindTile(int xPosition, int yPosition)
    {
        foreach (TileBlock t in FloorTiles)
        {
            if (t.XPosition == xPosition && t.YPosition == yPosition)
            {
                return t;
            }
        }
        return null;
    }

    /// <summary>
    /// 在某个集合里查找周围的格子
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="all"></param>
    /// <returns></returns>
    public List<TileBlock> FindNeighbourTileIn(TileBlock obj, List<TileBlock> all)
    {
        List<TileBlock> neighbours = new List<TileBlock>();
        foreach (TileBlock b in all)
        {
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

    /// <summary>
    /// 查找周围的敌人
    /// </summary>
    /// <returns></returns>
    public List<EnemyUnit> FindNeighbourEnemy(DungeonObject unit)
    {
        List<EnemyUnit> neighbourEnemies = new List<EnemyUnit>();
        foreach (EnemyUnit eu in AllEnemyUnits)
        {
            foreach (TileBlock t in eu.PositionTiles)
            {
                if (t.isNeighbour(unit)&&!neighbourEnemies.Contains(eu))
                {
                    neighbourEnemies.Add(eu); break;
                }
            }
        }
        return neighbourEnemies;
    }


    /// <summary>
    /// 查找某一列的所有消除块
    /// </summary>
    /// <param name="col"></param>
    /// <returns></returns>
    List<EliminateBlock> FindBlocksByColumn(int col)
    {
        List<EliminateBlock> columnBlocks = new List<EliminateBlock>();
        foreach (EliminateBlock b in CurAllEliminateBlock)
        {
            if (b.XPosition == col)
            {
                columnBlocks.Add(b);
            }
        }
        return columnBlocks;
    }


    /// <summary>
    /// 在某列上查找最高的障碍物
    /// </summary>
    /// <param name="column"></param>
    Barrier FindHighestBarriarOnColomn(int column)
    {
        List<Barrier> allColumnBarriers = new List<Barrier>();
        for (int i = 0; i < 9; i++)
        {
            Barrier barrier = FindBarrier(column, i);
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
    List<Barrier> SortColumnBarriers(List<Barrier> blocks)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            for (int j = 0; j < blocks.Count - i - 1; j++)
            {
                Barrier b1 = blocks[j];
                Barrier b2 = blocks[j + 1];
                if (b1.YPosition < b2.YPosition)
                {
                    Barrier temp = blocks[j];
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
    Barrier FindBarrier(int xPosition, int yPosition)
    {
        EnemyUnit eventElement = FindEnemyUnit(xPosition, yPosition);
        if (eventElement)
        {
            if (eventElement.GetType() == typeof(Barrier))
            {
                return (Barrier)eventElement;
            }
        }
        return null;
    }

    /// <summary>
    /// 查找宠物
    /// </summary>
    /// <param name="up"></param>
    public Pet FindPet(UserPet up)
    {
        foreach (OwnUnit ou in AllOwnUnits)
        {
            if (ou.GetType() == typeof(Pet))
            {
                Pet p = (Pet)ou;
                if (p.UserPet == up)
                {
                    return p;
                }
            }
        }
        return null;
    }
    #endregion

    #region 我方行动回合处理逻辑

    /// <summary>
    /// 己方回合
    /// </summary>
    void OwnRound()
    {
        isCurRoundAction = true;
        EliminateBlock.DesCount = 0;
        Debug.Log("消除操作完毕，进入己方行动回合");
        foreach (EnemyUnit eu in AllEnemyUnits)
        {
            eu.WillBeHitRender(0);
        }
        DisShowAllRange();
        //CurDungeonUI.AllAvataEffectiveRender(true);
        FindEliminateByPosition(curLinkPath[curLinkPath.Count - 1].XPosition, curLinkPath[curLinkPath.Count - 1].YPosition).SelectScale(false);
        FindEliminateByPosition(curLinkPath[curLinkPath.Count - 1].XPosition, curLinkPath[curLinkPath.Count - 1].YPosition).SetChain(false);
        userInputLock = true;
        CreateOwnActionQueue();
        CreateOwnQueueUnitsActionPath();
        QueueUnitsAction();
    }

    /// <summary>
    /// 创建该回合我方行动队列
    /// </summary>
    void CreateOwnActionQueue()
    {
        List<PetAvata> readyPetAvata = CurDungeonUI.GetFocusPetAvata();
        foreach (OwnUnit own in AllOwnUnits)
        {
            if (own.GetType() == typeof(Player))
            {
                CurOwnRoundActionQueue.Add(own);
            }
            else if (own.GetType() == typeof(Pet))
            {
                Pet ownPet = (Pet)own;
                foreach (PetAvata pa in readyPetAvata)
                {
                    if (pa.CurPet == ownPet.UserPet)
                    {
                        CurOwnRoundActionQueue.Add(ownPet); break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 创建单位行动路径
    /// </summary>
    void CreateOwnQueueUnitsActionPath()
    {
        for (int i = 0; i < CurOwnRoundActionQueue.Count; i++)
        {
            OwnUnit own = CurOwnRoundActionQueue[i];
            own.SetCurActionPath(curLinkPath);
            if (own.GetType() == typeof(Pet))
            {
                Pet p = (Pet)own;
                p.lastBeginBlock = curLinkPath[curLinkPath.Count - i - 1];
            }
        }
    }

    /// <summary>
    /// 按队列位置创建路径
    /// </summary>
    /// <param name="index"></param>
    List<TileBlock> CaculatePathByIndex(int index)
    {
        List<TileBlock> path = new List<TileBlock>();
        for (int i = 0; i < curLinkPath.Count - index; i++)
        {
            path.Add(curLinkPath[i]);
        }
         return path;
    }

    /// <summary>
    /// 队列单位行动
    /// </summary>
    void QueueUnitsAction()
    {
        for (int i = 0; i < CurOwnRoundActionQueue.Count; i++)
        {
            OwnUnit own = CurOwnRoundActionQueue[i];
            if (own.GetType() == typeof(Pet))
            {
                Pet p = (Pet)own;
                p.curActionIndex = i;
            }
            own.OwnBeginAction();
        }
    }
    #endregion

    #region 敌方行动回合处理
    /// <summary>
    /// 敌方回合
    /// </summary>
    void EnemyRound()
    {
        List<EnemyUnit> canActionEnemies = new List<EnemyUnit>();
        foreach (EnemyUnit eu in AllEnemyUnits)
        {
            if (EnemyUnit.CanRoundAction(eu))
            {
                canActionEnemies.Add(eu);
            }
        }

        canActionEnemyCount = canActionEnemies.Count;
        foreach (EnemyUnit eu in canActionEnemies)
        {
            eu.RoundAction();
        }
        if (canActionEnemies.Count == 0)
        {
            GameLoop();
        }
    }
    #endregion

    #region ownunit接口实现
    public void OwnUnitActionEnd(OwnUnit unit)
    {
        curOwnActionEndCount++;
        if (curOwnActionEndCount == CurOwnRoundActionQueue.Count)
        {
            Debug.Log("我方回合结束");
            ClearLastLink();
            foreach (OwnUnit o in AllOwnUnits)
            {
                if (o.GetType() == typeof(Pet))
                {
                    o.SetPosition(CurPlayer.XPosition, CurPlayer.YPosition);
                    o.SetObjectName();
                }
            }
            AttackCL.Out();
            FillEliminateBlocks();
            //Invoke("FillEliminateBlocks", 0.2f);
        }
    }
    #endregion

    #region object接口实现
    public void ObjectDestoryFromDungeon(DungeonObject dungeonObject)
    {
        if (dungeonObject.GetType() == typeof(EliminateBlock))
        {
            CurAllEliminateBlock.Remove((EliminateBlock)dungeonObject);
        }
        else if (dungeonObject.GetType() == typeof(Key))
        {
            AllEnemyUnits.Remove((EnemyUnit)dungeonObject);
        }
        else if (dungeonObject.GetType() == typeof(Barrier))
        {
            AllEnemyUnits.Remove((EnemyUnit)dungeonObject);
        }
        else if (dungeonObject.GetType() == typeof(Monster) || dungeonObject.GetType() == typeof(Boss))
        {
            Monster m = (Monster)dungeonObject;
            if (m.curAttackEndCallback!=null)
            {
                m.curAttackEndCallback();
            }
            AllEnemyUnits.Remove((EnemyUnit)dungeonObject);
        }
        dungeonObject.ObjectDisAppear();
    }
    #endregion

    #region 技能范围
    /// <summary>
    /// 移除所有的范围显示
    /// </summary>
    void DisShowAllRange()
    {
        foreach (Transform t in Ranges.transform)
        {
            Destroy(t.gameObject);
        }
    }

    public void CaculateSkillRangeTile(TileBlock block, SkillData skillData)
    {
        AllRangesTile.Clear();
        AllRangesTile.Add(block);
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
            TileBlock b = FindTileByPosition(realX, realY);
            if (b != null)
            {
                AllRangesTile.Add(b);
            }
        }
    }

    /// <summary>
    /// 显示当前技能范围
    /// </summary>
    /// <param name="block"></param>
    void ShowCurRange(TileBlock block)
    {
        if (CurPlayer.Skill != null && (CurPlayer.Skill.Bparameter < curLinkPath.Count))
        {
            DisShowAllRange();
            CaculateSkillRangeTile(block, CurPlayer.Skill);
            foreach (TileBlock b in AllRangesTile)
            {
                if (FindNeighbourTileIn(b, AllRangesTile).Count != 8)
                {
                    CaculateRangeLine(b, AllRangesTile);
                }
            }

            Hashtable args = new Hashtable();
            args.Add("alpha", 0);
            args.Add("time", 0.6f);
            args.Add("easetype", iTween.EaseType.easeOutQuad);
            args.Add("looptype", iTween.LoopType.pingPong);
            iTween.FadeFrom(Ranges, args);
        }
    }

    /// <summary>
    /// 计算范围线
    /// </summary>
    /// <param name="b"></param>
    /// <param name="allInRange"></param>
    public void CaculateRangeLine(TileBlock b, List<TileBlock> allInRange)
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
    void CaculateLine(TileBlock b, List<TileBlock> allInRange)
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
    /// 绘制转角范围
    /// </summary>
    /// <param name="b"></param>
    /// <param name="allInRange"></param>
    void CaculateConner(TileBlock b, List<TileBlock> allInRange)
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
    /// 绘制直线范围
    /// </summary>
    /// <param name="b"></param>
    /// <param name="direction"></param>
    /// <param name="blockConner"></param>
    void DrawRangeLine(TileBlock b, LineDirection direction, BlockConner blockConner)
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
    void DrawRangeConner(TileBlock b, ConnerDirection direction, bool reverse)
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
    /// 查找偏移
    /// </summary>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    /// <param name="all"></param>
    /// <param name="curBlock"></param>
    /// <returns></returns>
    public TileBlock FindOffset(int offsetX, int offsetY, List<TileBlock> all, TileBlock curBlock)
    {
        foreach (TileBlock b in all)
        {
            if (b.XPosition == curBlock.XPosition + offsetX && b.YPosition == curBlock.YPosition + offsetY)
            {
                return b;
            }
        }
        return null;
    }
    #endregion

    #region 消除块回调
    void ResertPositionAnimationEnd(GameObject blockObj)
    {
        fillMoveCount--;

        if (fillMoveCount == 0)
        {
            curPlayerEliminateBlock = FindEliminateByPosition(CurPlayer.XPosition, CurPlayer.YPosition);
            if (curPlayerEliminateBlock == null)
            {
                GameObject EliminateObject = NGUITools.AddChild(DungeonArea, EliminateResource);
                curPlayerEliminateBlock = EliminateObject.GetComponent<EliminateBlock>();
                curPlayerEliminateBlock.DungeonScene = this;
                CurAllEliminateBlock.Add(curPlayerEliminateBlock);
                curPlayerEliminateBlock.RenderObjectSprite(CurPlayer.XPosition, CurPlayer.YPosition);
            }
            curPlayerEliminateBlock.SetToPlayerRender();
            EnemyRound();
        }
    }
    #endregion

    #region EnemyUinit接口
    public void EnemyUnitRoundActionEnd()
    {
        canActionEnemyCount--;
        if (canActionEnemyCount == 0)
        {
            Debug.Log("所有敌方单位行动结束,进入下一回合");
            GameLoop();
        }
    }
    #endregion

    #region 进入下一层
    /// <summary>
    /// 尝试进入下一层
    /// </summary>
    public void TryGoToNextFloor()
    {
        if (CurDoor.HasOpen&& !isCurRoundAction)
        {
            CurDoor.HasOpen = false;
            CurFloor++;
            CurDungeonUI.CurPlayerUIInfo.SetFloorShow(CurFloor, ConfigManager.DungeonConfig.GetDungeonById(curDid).FloorCount);
            Debug.Log("尝试进入下一层");
            BeginFloorLoad();
        }
    }
    #endregion

    #region  监测周围敌人
    void CheckLinkNeighbourEnemy()
    {
        foreach (EnemyUnit eu in AllEnemyUnits)
        {
            int pathCount = 0;
            foreach (TileBlock b in curLinkPath)
            {
                if (b.isNeighbour(eu))
                {
                    pathCount++;
                }
            }

            int petCount = CurDungeonUI.GetFocusPetAvata().Count * pathCount;
            if (eu.GetType() == typeof(Barrier))
            {
                petCount = 0;
            }
            eu.WillBeHitRender(pathCount + petCount);
        }
    }
    #endregion

    #region 层个过渡
    void BeginFloorLoad()
    {
        CurPlayer.gameObject.SetActive(false);
        FloorChange.SetActive(true);
        Hashtable args = new Hashtable();
        args.Add("alpha", 0);
        args.Add("time", 0.8f);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "FadeInEnd");
        iTween.FadeFrom(FloorChange, args);
    }

    void FadeInEnd()
    {
        CreateCurFloorMap();
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
        //StartLoop();
    }
    #endregion

    #region 战斗结果渲染
    public void WinRender()
    {
        userInputLock = true;
        GameObject resultGameObject = Instantiate(WinResources) as GameObject;
        UIEventListener.Get(resultGameObject).onClick = (obj) =>
        {
            Application.LoadLevel("MainScene");
        };
    }

    bool TryLoseRender()
    {
        if (CurPlayer.CurHp <= 0)
        {
            userInputLock = true;
            GameObject resultGameObject = Instantiate(LoseResources) as GameObject;
			UIEventListener.Get(resultGameObject).onClick = (obj)=>
			{
                Application.LoadLevel("MainScene");
			};
            return true;
        }
        return false;
    }
    #endregion

    #region 回复所有宠物能量
    public void RecoverAllPetEnergy()
    {
        foreach (OwnUnit o in AllOwnUnits)
        {
            if (o.GetType() == typeof(Pet))
            {
                Pet p = (Pet)o;
                p.PetRecoverPower();
            }
        }
    }
    #endregion

    #region 技能背景
    public void ShowSkillBack(bool show)
    {
        SkillBack.SetActive(show);
    }
    #endregion

}
