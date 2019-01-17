using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class PveFightUnit : PveGameObject
{
    #region 枚举
    public enum ActionState
    {
        None = 0,
        Waiting = 10,
        Run = 40,
        Attack = 30,
        Skill = 50,
        Hurt = 20,
        Dead = 70,
        HeavyAttack = 80,
        Far1Attack = 90,
        Far2Attack = 100,
        FinishSkill = 110,
        Vertigo = 120,
        Victor = 130
    }

    public enum UnitState
    {
        normal = 0,
        vertigo = 1,
        guard = 2
    }
    #endregion

    #region 属性
    public DungeonEnum.FaceDirection CurFaceDirection;

    public ActionState CurActionState;

    public UnitState CurState = UnitState.normal;

    public int lastRoundCount = -1; //buff回合数

    public bool AttackTag = false; //本回合是否进行过有效攻击

    public int DistanceLimit = -1; //移动步数限制

    public bool UsingSkill = false; //是否正在使用技能

    public List<GameObject> AllAnimations = new List<GameObject>();

    public int MoveSpeed = 140;

    public List<PveTile> RangeTiles = new List<PveTile>();

    /// Hp
    public float curHpvalue;
    public float CurHp
    {
        get
        {
            if (GetType() == typeof(PveCharacter))
            {
               
                //return UserManager.pveUserInfo.CurHp;
                return curHpvalue;
            }
            else
            {
                return curHpvalue;
            }
        }
        set 
		{
			curHpvalue = value; 
			// 如果是角色
			if(this.GetType() == typeof(PveCharacter))
			{
				// 触发成就
				AchievementManager.Trigger(AchievementTypeEnum.Residue_Hp, (int)((curHpvalue / Hp) * 10000));
			}
        }
    }

    public float Hp; //上限

    public float BaseHp; //基础上限
    ///

    /// atk
    public float BaseAtk;

    public float Atk;
    ///


    /// def
    public int BaseDef;

    public int Def;
    ///

    /// evade
    public float BaseEvade
    {
        get
        {
            if (GetType() == typeof(PveBoss) || GetType() == typeof(PveMonster)) return 0f;
            if(GetType() == typeof(PveCharacter)) return ConfigManager.SkillConfig.GetParameterPercent(ConfigManager.ParamConfig.GetParam().GlobalMiss); //闪避率
            return 0f;
        }
        set
        {
            BaseEvade = value;
        }
    }

    public float Evade;
    ///

    public DungeonEnum.ElementAttributes Element;

    public UnitHp CurUnitHp;

    public SkinConfigData SkinData;

    private PveEliminate curElim;

    public PveEliminate CurElim
    {
        get
        {
            if(GetType() != typeof(PveCharacter) && GetType() != typeof(PvePet))
                curElim = GameControl.FineEliminate(XPosition, YPosition);
            return curElim;
        }
        set
        {
            curElim = value;
        }
    }

    public bool IsChain = false;

    public bool Weakness = false;

    public Hashtable SHash = new Hashtable();

    public float DisFromeChar;

    public GameObject MissShow;

    public List<PveBuffData> CurBuffs = new List<PveBuffData>(); //buff列表



    // 反击状态
    public PvpFightUnit StrickeFightUnit;
    public bool StrickeStatus;
    // 反击被击状态
    public bool StrickeHurtStatus;
    public Action<string> StrikeCallback;

    /// <summary>
    /// 隐藏状态
    /// </summary>
    public bool HiddenStatus;

    /// <summary>
    /// 隐藏 alpha
    /// </summary>
    public float HiddenAlpha;

    //public List<SkillData> pvpSkillDataList;

    /// <summary>
    /// 角色 Buff 信息
    /// </summary>
    public PvpPlayerBuff pvpPlayerBuff;

    /// <summary>
    /// 角色技能信息
    /// </summary>
	public PvpPlayerSkill pvpPlayerSkill;

	/// <summary>
	/// 表情
	/// </summary>
	public List<string> faceList;


    #endregion

    #region 重写父类
    public override int GetLayer()
    {
        return LayerHelper.Unit;
    }

    public override void SetPosition(int xPosition, int yPosition)
    {
        base.SetPosition(xPosition, yPosition);
        CaculateRangeTiles();
        Vector3 basicPotisition = CaculateRealPosition(xPosition, yPosition);
        transform.localPosition = new Vector3(basicPotisition.x + XRange / 2 * 40, basicPotisition.y, basicPotisition.z);
        SetOrder();
    }

    public override Vector3 CaculateRealPosition(int xPosition, int yPosition)
    {
        Vector3 v = base.CaculateRealPosition(xPosition, yPosition);
        return new Vector3(v.x, v.y - 26, 0);
    }
    #endregion

    #region 公用方法,Action
    public void UnitChainAttack()
    {
        HardWareData.HardWareType ht= UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style;
        ActionState curActionState;
        if (ht == HardWareData.HardWareType.Far1)
        {
            curActionState = ActionState.Far1Attack;
        }
        else if (ht == HardWareData.HardWareType.Far2)
        {
            curActionState = ActionState.Far2Attack;
        }
        else
        {
            curActionState = ActionState.FinishSkill;
        }
         	
       ChangeAnimation(DungeonEnum.FaceDirection.LeftDown, curActionState);
    }

    public void UnitAttack(DungeonEnum.FaceDirection direction)
	{
        ChangeAnimation(direction, ActionState.Attack);
    }

    public virtual void AttackEnd()
	{		
        //Debug.Log("attackend      " + CurFaceDirection);
        UnitWaiting(CurFaceDirection);
    }
    public void BeHurtEnd()
    {
        IsChain = false;
        UnitWaiting(CurFaceDirection);
    }

    public Action<PveFightUnit> CurDeadAction;
    public virtual void UnitDead(Action<PveFightUnit> curDeadAction)
	{
		CurDeadAction = curDeadAction;

		PveEnemyUnit pveEnemyUnit = this as PveEnemyUnit;
		// 如果是敌人，并且逃跑状态为真
		if(pveEnemyUnit != null && pveEnemyUnit.RunAwayStatus)
		{
			GameObject disObject = Instantiate(Resources.Load("PreFabs/FX/skill_03")) as GameObject;
			disObject.transform.position = this.transform.position;
			AnimationHelper.AnimationFadeTo(0f, this.gameObject, iTween.EaseType.linear, this.gameObject, "DeadEnd", 1f);
        }
        else if (GetType() == typeof(PveBarrier ))
        {
            AnimationHelper.AnimationFadeTo(0f, this.gameObject, iTween.EaseType.linear, this.gameObject, "DeadEnd", 1f);
        }
        else
		{
	        ChangeAnimation(DungeonEnum.FaceDirection.LeftDown, ActionState.Dead);
		}
    }

    public void DeadEnd()
    {
        CurDeadAction(this);
    }

    public virtual void AttackBump()
    {

    }

    public virtual void SkillAttackBump()
    {
    }

    public virtual void SkillAttackEnd()
    {
    }

    public virtual void ChainAttackBump()
    {
    }

    public virtual void ChainAttackEnd()
    {
    }

    public void UnitWaiting(DungeonEnum.FaceDirection direction)
	{
        if(CurState == UnitState.normal || CurState == UnitState.guard)
        {
            ChangeAnimation(direction, ActionState.Waiting);
        }
        else if(CurState == UnitState.vertigo)
        {

            UnitVertigo(direction);
        }
    }

    Action MoveEndAction;
    public virtual void UnitMove(int xPosition, int yPosition, Action moveEndAction, DungeonEnum.FaceDirection direction)
    {
        MoveEndAction = moveEndAction;
        Vector3 toVec = CaculateRealPosition(xPosition, yPosition);
        Vector3 realVec = new Vector3(toVec.x + XRange / 2 * 40, toVec.y, toVec.z);
		
        ChangeAnimation(direction, ActionState.Run);
        CleanZorder(RenderObject);
        XPosition = xPosition;
        YPosition = yPosition;
        SetOrder();
        SetName();
        AnimationHelper.AnimationMoveToSpeed(realVec, gameObject, iTween.EaseType.linear, gameObject, "MoveEnd", MoveSpeed);
    }

    void MoveEnd()
    {
        if (MoveEndAction != null)
        {
            MoveEndAction();
        }
    }



    /// <summary>
    /// 眩晕
    /// </summary> 
    public virtual void UnitVertigo(DungeonEnum.FaceDirection direction)
    {
    }

    public virtual void SetVertigo(int round)
    {
        lastRoundCount = round;
        CurState = UnitState.vertigo;
    }

    public bool IsNeighbour(PveFightUnit fu)
    {
        //Debug.Log(UserManager.CurUserInfo.CurWeapon  );
        
        HardWareData.HardWareType ht = UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style;

        //正四方向
        foreach (PveTile pt in RangeTiles)
        {
            if ((pt.XPosition == fu.XPosition - 1 && pt.YPosition == fu.YPosition) ||
           (pt.XPosition == fu.XPosition + 1 && pt.YPosition == fu.YPosition) ||
           (pt.XPosition == fu.XPosition && pt.YPosition == fu.YPosition - 1) ||
            (pt.XPosition == fu.XPosition && pt.YPosition == fu.YPosition + 1))
            {
                return true;
            }
        }

        //Debug.Log("sty= "+ht);

        //主角 轻武器 补四个斜角
        if (fu.name.IndexOf("Character") == 0 && ht == HardWareData.HardWareType.Light)
        {            
            foreach (PveTile pt in RangeTiles)
            {
                if ((pt.XPosition == fu.XPosition - 1 && pt.YPosition == fu.YPosition + 1) ||
                (pt.XPosition == fu.XPosition + 1 && pt.YPosition == fu.YPosition + 1) ||
                (pt.XPosition == fu.XPosition + 1 && pt.YPosition == fu.YPosition - 1) ||
                (pt.XPosition == fu.XPosition - 1 && pt.YPosition == fu.YPosition - 1))
                {
                    return true;
                }
            }
        }

        return false;
    }
    #endregion

    #region 动画方法
    public void ChangeAnimation(DungeonEnum.FaceDirection faceDirection, ActionState actionState)
    {
        CurFaceDirection = faceDirection;
        GameObject tempObject = RenderObject;
        if (GetAnimationObject() != null)
        {
            //if (GetType() == typeof(PveCharacter))
            //{
            //    //Debug.Log(CurFaceDirection +"--- "+GetAnimationObject());
            //    RenderObject = GetAnimationObject() as GameObject;
            //    RenderObject.SetActive(true);
            //}
            //else
            //{
                RenderObject = Instantiate(GetAnimationObject()) as GameObject;
            //}
            
        }
        //Debug.Log(RenderObject);
       
        RenderObject.transform.parent = transform;
        RenderObject.transform.localPosition = new Vector3(0,0,0);
        if (tempObject && RenderObject)
        {
            RenderObject.transform.localPosition = tempObject.transform.localPosition;

            if (GetType() != typeof(PveCharacter))
            {
                Destroy(tempObject);
            }
            else
            {
                Animator ac = tempObject.GetComponent<Animator>();              
                if (RenderObject != tempObject)
                {
                    CleanZorder(tempObject);
                    ac.SetInteger("ActionState", (int)ActionState.Waiting);
                    tempObject.SetActive(false);
                }
                else
                {
                    ac.SetInteger("ActionState", (int)ActionState.Waiting);
                }
            }
            Destroy(tempObject);//----
        }
        SetOrder();
        if (GetType() == typeof(PveCharacter))
        {
            PlayerAvata pveAvata = RenderObject.GetComponent<PlayerAvata>();

            // 如果需要隐藏阴影
            if (!this.shadowStatus)
            {
                GameObject playerShadow = pveAvata.transform.FindChild("Shadow").gameObject;
                // 获取阴影对象
                if (playerShadow != null) playerShadow.SetActive(false);
            }

            if (UserManager.CurUserInfo.CurWeapon != null)
            {
                pveAvata.AddAvataWare(UserManager.CurUserInfo.CurWeapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
                pveAvata.WeaponEffectShow();
            }

            if (UserManager.CurUserInfo.CurHelmet != null)
            {
                pveAvata.AddAvataWare(UserManager.CurUserInfo.CurHelmet.CurHardWareData.SkinId, CurFaceDirection);
            }

            if (UserManager.CurUserInfo.CurArmor != null)
            {
                pveAvata.AddAvataWare(UserManager.CurUserInfo.CurArmor.CurHardWareData.SkinId, CurFaceDirection);
            }
            //虚弱
            PlayerAvata pa = RenderObject.transform.GetComponent<PlayerAvata>();
            if (pa.Weakness != null) pa.Weakness.SetActive(Weakness);
            Animator am = RenderObject.transform.GetComponent<Animator>();
            if (Weakness && ActionState.Waiting == actionState)
            {
                am.speed = 0.5f;
            }
            else
            {
				if(ActionState.Hurt != actionState)
				{
                	am.speed = 1f;
				}else{
					am.speed = 0.5f;
				}
            }
        }
        CurActionState = actionState;
        
        //Debug.Log(name+"  "+faceDirection+"  actionState="+actionState);
        Animator at = RenderObject.GetComponent<Animator>();        
        at.SetInteger("ActionState", (int)CurActionState);
        ResertAllLayer();
    }
    #endregion

    #region 工具方法
    public void CaculateRangeTiles()
    {
        RangeTiles.Clear();
        for (int i = XPosition; i < XPosition + XRange; i++)
        {
            for (int j = YPosition; j < YPosition + YRange; j++)
            {
				PveTile pt = GameControl.FindPveTile(i, j);
                if (pt)
                {
                    RangeTiles.Add(pt);
                }
            }
        }
    }

    public List<PveTile> GetRangeTiles(int xPosition, int yPosition)
    {
        List<PveTile> tiles = new List<PveTile>();
        for (int i = xPosition; i < xPosition + XRange; i++)
        {
            for (int j = yPosition; j < yPosition + YRange; j++)
            {
				PveTile pt = GameControl.FindPveTile(i, j);
                if (pt)
                {
                    tiles.Add(pt);
                }
            }
        }
        return tiles;
    }

    public void ResertAllLayer()
    {
        List<GameObject> childs = GetChild(RenderObject.transform);
        foreach (GameObject go in childs)
        {
            go.layer = GetLayer();

        }
    }

    public List<GameObject> GetChild(Transform trans)
    {
        List<GameObject> childs = new List<GameObject>();
        foreach (Transform child in trans)
        {
            childs.Add(child.gameObject);
            if (child.childCount > 0)
            {
                List<GameObject> temp = GetChild(child);
                foreach (GameObject tempT in temp)
                {
                    childs.Add(tempT);
                }
            }
        }
        return childs;
    }

    public GameObject GetAnimationObject()
    {
        if (CurFaceDirection == DungeonEnum.FaceDirection.Up)
        {
            return AllAnimations[0];
        }
        else if (CurFaceDirection == DungeonEnum.FaceDirection.UpRight)
        {
            return AllAnimations[1];
        }
        else if (CurFaceDirection == DungeonEnum.FaceDirection.Right)
        {
            return AllAnimations[2];
        }
        else if (CurFaceDirection == DungeonEnum.FaceDirection.RightDown)
        {
            return AllAnimations[3];
        }
        else if (CurFaceDirection == DungeonEnum.FaceDirection.Down)
        {
            return AllAnimations[4];
        }
        else if (CurFaceDirection == DungeonEnum.FaceDirection.LeftDown)
        {
            return AllAnimations[5];
        }
        else if (CurFaceDirection == DungeonEnum.FaceDirection.Left)
        {
            return AllAnimations[6];
        }
        else if (CurFaceDirection == DungeonEnum.FaceDirection.LeftUp)
        {
            return AllAnimations[7];
        }
        return null;
    }

    int highestOrder = 0;
    public override void CleanZorder(GameObject render)
    {
        if (render)
        {
            List<GameObject> childs = GetChild(render.transform);
            foreach (GameObject go in childs)
            {
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if (sr)
                {
                    sr.sortingOrder = sr.sortingOrder - GetBasicZorder();
                    if (highestOrder < sr.sortingOrder)
                    {
                        highestOrder = sr.sortingOrder;
                    }
                }
            }

            if (this.GetType() == typeof(PveMonster))
            {
                PveMonster pm = this as PveMonster;
                if (pm.runAwayIcon) pm.runAwayIcon.GetComponent<SpriteRenderer>().sortingOrder = highestOrder;
            }

            if (CurUnitHp)
            {
                CurUnitHp.frameSprite.sortingOrder = highestOrder + 3;
                CurUnitHp.hpSprite.sortingOrder = highestOrder + 2;
                CurUnitHp.backSprite.sortingOrder = highestOrder + 1;
            }
        }
    }


    public override void SetOrder()
    {
        highestOrder = -99999999;
        if (RenderObject)
        {
            RenderObject.transform.localPosition = new Vector3(RenderObject.transform.localPosition.x, RenderObject.transform.localPosition.y, (float)Tools.GetRandom_n(100)*-1f);
            List<GameObject> childs = GetChild(RenderObject.transform);
            childs.Add(RenderObject);
            foreach (GameObject go in childs)
            {
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if (sr)
                {
                    //sr.sortingOrder = sr.sortingOrder + GetBasicZorder();
                    sr.sortingOrder = GetBasicZorder();

                    //解决同层级宠物被主角帽子遮住问题
                    if (GetType() == typeof(PvePet)) sr.sortingOrder = sr.sortingOrder + 1;

                    if (highestOrder < sr.sortingOrder)
                    {
                        highestOrder = sr.sortingOrder;
                    }
                }
            }

           if( this.GetType() == typeof(PveMonster)){
               PveMonster pm = this as PveMonster;             
               if(pm.runAwayIcon) pm.runAwayIcon.GetComponent<SpriteRenderer>().sortingOrder = highestOrder + 5;
           }
            
            if (CurUnitHp)
            {
                CurUnitHp.frameSprite.sortingOrder = highestOrder + 3;
                CurUnitHp.hpSprite.sortingOrder = highestOrder + 2;
                if (CurUnitHp.backSprite) CurUnitHp.backSprite.sortingOrder = highestOrder + 1;
                Transform boss = CurUnitHp.transform.FindChild("boss");
                if (boss) boss.GetComponent<SpriteRenderer>().sortingOrder = highestOrder + 4;
                Transform ele = transform.FindChild("element_monster");
                if (ele) ele.GetComponent<SpriteRenderer>().sortingOrder = highestOrder + 5;
            }
        }
    }
    private bool shadowStatus = true;
    public void HiddenShadow(bool shadowStatus)
    {
        this.shadowStatus = shadowStatus;
        if (!this.shadowStatus) this.ChangeAnimation(this.CurFaceDirection, this.CurActionState);
    }
    int GetBasicZorder()
    {
        return 200 * (XPosition - YPosition);
    }

    //技能造成的伤害
    public void SkillHurtRender(SkillData skill)
    {
        float attack = skill.Aparameter;
        if (skill.SkillFX == "SkFX0")
        {
            //attack = UserManager.CurUserInfo.CurWeapon.CurAtk;
            //走普通伤害逻辑
            BeHurt(GameControl.CurCharacter);
            return;
        }

        //相克关系
        int fp = (int)UserManager.CurUserInfo.CurWeapon.CurHardWareData.Element;
        int sp = StP();
        float pr = GetElementParam(fp, sp);


        float at = (attack - Def)*pr;
        if (at < 1) at = 1;
        CurHp = CurHp - at;

		if(this.GetType() == typeof(PveBoss))
		{
			Debug.Log("终结技能 BOSS 受伤害 ");
			if(CurHp <= 0)
			{
				Debug.Log("终结技能 BOSS 死亡 ");
				// 触发成就
				AchievementManager.Trigger(AchievementTypeEnum.Chain_Kill_Boss, 1);
			}
		}


        
        HurtLabelShow(at, GameControl.CurCharacter,false,false,pr);
        //装备技能击退
        int id;
        int.TryParse(skill.Id.Substring(2).ToString(), out id);

        Debug.Log(" --- SkillHurtRender  id =" + id + " --- " + skill.SkillFX);

        //skill.Cparameter = 1;
        if (skill.SkillFX == "SkFX1" && skill.Cparameter > 0.01f)
        {
            //击退
            DungeonEnum.FaceDirection direction = GameControl.CurCharacter.GetTargetDirection(this);

            BeHitBackRender(skill.Cparameter, direction);
                        
            //PveEliminate eb = GameControl.FineEliminate(XPosition, YPosition);
            //eb.SetToPlayerRender();
        }

        HurtAnimation();
        if(CurUnitHp != null)
        {
            CurUnitHp.RefreshUI(CurHp, Hp);
        }
    }



    //属性相克  f 攻击 ， s 防守
    //风3>>水4>>火2>>地1>>风3
    public float GetElementParam(int f, int s)
    {
        float p = 1;

        if (s > 10)
        {
            int tss = s % 10;
            int tss1 = s / 10;
            if (tss == tss1) s = tss;
        }
        //Debug.Log("s="+s);
        float havekill = ConfigManager.ParamConfig.GetParam().GlobalElementAddDamage;
        float havnotkill = ConfigManager.ParamConfig.GetParam().GlobalElemnetDeductDamage;

        if (s == 0)
        {
        }
        else if (s == 1)
        {
            if (f == 2) p = havekill;
            if (f == 3) p = havnotkill;           
        }
        else if (s == 2)
        {
            if (f == 4) p = havekill;
            if (f == 1) p = havnotkill;          
        }
        else if (s == 3)
        {
            if (f == 1) p = havekill;
            if (f == 4) p = havnotkill;            
        }
        else if (s == 4)
        {
            if (f == 3) p = havekill;
            if (f == 2) p = havnotkill;           
        }
        else if (s >= 10)
        {
            //防守方为主角 且 双防装属性不一  为被克                        
            p = havekill;
        }
        return p;
    }
    //攻击方属性
    public int ActP(PveFightUnit fu)
    {
        int fp = 0;
        //Debug.Log(fu.name + "  " + this.name);

        if (fu.GetType() == typeof(PvePet))
        {
            PvePet p = (PvePet)fu;
            fp = (int)p.CurUserPet.CurPetData.PetPro;
        }
        if (fu.GetType() == typeof(PveCharacter))
        {
            fp = (int)UserManager.CurUserInfo.CurWeapon.CurHardWareData.Element;
        }
        if (fu.GetType() == typeof(PveBoss))
        {
            PveBoss p = (PveBoss)fu;
            fp = (int)p.Element;
        }
        if (fu.GetType() == typeof(PveMonster))
        {
            PveMonster p = (PveMonster)fu;
            fp = (int)p.Element;
        }
        return fp;

    }
    //防守方属性
    public int StP()
    {

        int sp = 0;
       
        if (name.IndexOf("Pet") == 0)
        {
            PvePet p = (PvePet)this;
            sp = (int)p.CurUserPet.CurPetData.PetPro;
        }
        if (name.IndexOf("Character") == 0)
        {
            //头盔 和护盾
            sp = 0;
            if (UserManager.CurUserInfo.CurHelmet != null && UserManager.CurUserInfo.CurArmor != null)
            {
                sp = (int)UserManager.CurUserInfo.CurHelmet.CurHardWareData.Element * 10 + (int)UserManager.CurUserInfo.CurArmor.CurHardWareData.Element;

            }
            else
            {
                if (UserManager.CurUserInfo.CurHelmet != null)
                {
                    sp = (int)UserManager.CurUserInfo.CurHelmet.CurHardWareData.Element;
                }
                if (UserManager.CurUserInfo.CurArmor != null)
                {
                    sp = (int)UserManager.CurUserInfo.CurArmor.CurHardWareData.Element;
                }
            }
        }
        if (name.IndexOf("Boss") == 0)
        {
            PveBoss p = (PveBoss)this;
            sp = (int)p.Element;
        }
        if (name.IndexOf("Monster") == 0)
        {
            PveMonster p = (PveMonster)this;
            sp = (int)p.Element;
        }
        return sp;
    }
    /// <summary>
    /// 更新血量
    /// </summary>
    /// <param name="addHp">Add hp.</param>
    public void ChangeHp(float addHp)
    {
        // 显示伤害
        HurtLabelShow(Math.Abs(addHp), this);

        //this.Hp = CurHp;
        this.CurHp += addHp;

        if (this.CurHp > this.Hp) this.CurHp = this.Hp;

        Debug.Log("技能结束之后最终剩余血量：" + this.CurHp);
        this.RefreshHp();
    }
    /// <summary>
    /// 换血
    /// </summary>
    /// <param name="hp">Hp.</param>
    public void TurnHp(float hp)
    {
        // 变化血量
        float changeHp = hp - this.CurHp;
        // 显示伤害
        HurtLabelShow(Math.Abs(changeHp), this);

        this.Hp =  CurHp;
        this.CurHp = hp;

        if (this.CurHp > this.Hp) this.CurHp = this.Hp;

        Debug.Log("技能结束之后最终剩余血量：" + this.CurHp);

        this.RefreshHp();
    }
    public void RefreshHp()
    {
        CurUnitHp.RefreshUI(CurHp, Hp);
        GameControl.SetHpUIHpShow(this);
    }
    /// <summary>
    /// 这个函数可以与下面那个函数合并
    /// </summary>
    /// <returns>The hurt value.</returns>
    /// <param name="fu">Fu.</param>
    public float BeHurtValue(PveFightUnit fu, ref bool critBool, ref bool missBool)
    {

        float addtion = 1f;
        // 行动步骤
        int actionStep = 0;
        if (!fu.StrickeStatus)
        {
            addtion = fu.CurElim.Addition; // 方砖伤害加成
            actionStep = fu.CurElim.CurStep;
        }
        else
        {
            addtion = this.CurElim.Addition;
            actionStep = this.CurElim.CurStep;
        }

        // 如果是反击，不计算伤害加成
        if (fu.StrickeStatus) addtion = 1.0f; // 如果是反击攻击，没有方砖伤害加成

        int sat = (int)fu.getAtk(); // 攻击方攻击力

        int fp = ActP(fu); // 攻击方属性
        int sp = StP();    // 防御方属性

        // 暴击数据计算
        float critData = 0.0f;
        float critPro = CritPre(fu);
        // 这儿写死
        critPro = 0;

        critBool = true;
        if ((int)critPro == 0) critBool = false;
        if (critBool) // 如果值不为 0 说明是暴击
        {
            critData = ConfigManager.ParamConfig.GetParam().GlobalCritDamageGrowthRate + this.pvpPlayerBuff.GetValueByBuffTypeToFloat(BuffTypeEnum.Recover_Crit_Hurt);
        }

        float lastDamage = 0;

        // 闪躲数据计算
        missBool = MissRandom();
        // 这儿写死
        missBool = false;

        // 如果眩晕，没有闪避
		if(this.GetType() == typeof(PveCharacter))
		{
        	if (!this.pvpPlayerBuff.CanOrNotMoveCheck()) missBool = false;
		}
        if (missBool && !critBool) // 如果闪避，并且没有触发暴击，先计算暴击，再计算闪避，暴击之后不会闪避
        {
            lastDamage = 0;
        }
        else
        {
            //普通攻击伤害计算公式 = (进攻方攻击力-防御方防御力) * (方砖伤害加成 +暴击伤害加成)
            lastDamage = (sat - this.Def) * (addtion + critData / 10000);  //最后伤害
            lastDamage = (int)lastDamage;
            if (lastDamage <= 0)
            {
                lastDamage = 1;
            }
        }
        return lastDamage;
    }





    public float GetRandomHurtPer()
    {
        float ra = 1;
        int jg = Tools.GetRandom_n(10) - 5;
        ra = 1 + jg * 0.01f;
        return ra;
    }
    int MissActionNum = 0;
    public bool MissActionWithSkill()
    {
        bool b = true;
        SkillData sd = GetSkillDataWithSkillFXId("SkFX9");
        if (sd != null)
        {
            //失去行动数回合          
            int bp = (int)sd.Bparameter;
            int jg = Tools.GetRandom_n(10000);
            if (jg <= bp)
            {
                b = false;
                MissActionNum = (int)sd.CurNum;

            }
            SHash.Remove(sd.Id);
        }
        return b;
    }
    public bool MissActionReturn()
    {
        if (MissActionNum > 0)
        {
            MissActionNum--;
            Debug.Log("怪物" + name + " 失去行动 " + MissActionNum);
            return false;
        }
        else
        {
            return true;
        }
    }

    public List<DungeonEnum.ElementAttributes> SkillProList = new List<DungeonEnum.ElementAttributes>();
    public void GetSkillProList()
    {

        SkillProList.Clear();
        SkillData sd = GetSkillDataWithSkillFXId("SkFX13");
        if (sd != null)
        {
            //主角                                 
            if (sd.Yparameter != "0")
            {
                SkillProList.Add((DungeonEnum.ElementAttributes)int.Parse(sd.Yparameter.ToString()));
            }

            if ((int)sd.Aparameter * 1000 != 0)
            {
                SkillProList.Add((DungeonEnum.ElementAttributes)int.Parse(sd.Aparameter.ToString()));
            }
            if ((int)sd.Bparameter * 1000 != 0)
            {
                SkillProList.Add((DungeonEnum.ElementAttributes)int.Parse(sd.Bparameter.ToString()));
            }
            if ((int)sd.Cparameter * 1000 != 0)
            {
                SkillProList.Add((DungeonEnum.ElementAttributes)int.Parse(sd.Cparameter.ToString()));
            }
            Debug.Log(":: "+SkillProList.Count);
			GameControl.SkillChangePetPro();
        }
        else
		{	GameControl.PlayerInfoChangePetPro();
        }

    }

    public float RevPowerWithSkill = 1f;
    public float GetRevPowerWithSkill()
    {
        float r = 1;      
        RevPowerWithSkill = r * ConfigManager.ParamConfig.GetParam().GlobalSkillPowerGrowthRate;
        //Debug.Log("RevPowerWithSkill=" + RevPowerWithSkill);
        return RevPowerWithSkill;
    }

   

    public virtual float getAtk()
    {
		float atk = Atk;
        // 如果是宠物
        if (this.GetType() == typeof(PvePet))
        {
            // 获取宠物攻击加成
            PvpBuffValueData pvpBuffValueData = GameControl.CurCharacter.pvpPlayerBuff.GetValueByBuffTypeToClass(true);
            // 加上宠物攻击数据
            atk = (int)(Atk * (1 + pvpBuffValueData.attack_odds / PvpUserInfo.ODDS_VALUE));

            //Debug.Log("原来的宠物攻击力：" + Atk + "，新的数据：" + atk);

            return atk;
        }
        return atk;
       
    }

    public void SkillReturnsClear()
    {
        List<string> del = new List<string>();
        foreach (SkillData sd in SHash.Values)
        {
            if (sd != null)
            {
                sd.CurNum--;
                if (sd.CurNum < 1) del.Add(sd.Id);
            }
        }
        if (del.Count == 0) return;
        foreach (string s in del)
        {
            SHash.Remove(s);
        }
    }
    public SkillData GetSkillDataWithSkillFXId(string fxid)
    {
        foreach (SkillData sd in SHash.Values)
        {
            if (sd != null && sd.SkillFX == fxid)
            {
                return sd;
            }
        }
        return null;
    }

    public void BeSpeciallyHurt(float damage)
    {
        float atk = damage;
        int def = Def;

        //根据方块位置，乘上系数计算伤害值
        //fu 攻击方
        float at = atk - def;
        if (gameObject == null) return;
        float sat = at;//* GetRandomHurtPer();
        if (sat < 0) sat = 1;


        //Debug.Log(fp + "    " + sp+"  atk=" + atk   + "  def=" + def+"   fu.name= " + fu.name);
        Debug.Log(" monster    BeHurt = " + sat);
        CurHp = CurHp - sat;
        HurtLabelShow(sat, null);
        //Debug.Log(name+"  "+CurHp);

        string hitSound = "FX8";//克
        GameObject g = Resources.Load("PreFabs/FX/" + hitSound) as GameObject;
        if (g)
        {
            GameObject beHitObject = Instantiate(g) as GameObject;
            beHitObject.transform.parent = transform;
            beHitObject.transform.localPosition = new Vector3(0, 0.05f, 0);
        }

        HurtAnimation();

        if (CurUnitHp != null)
        {
            CurUnitHp.RefreshUI(CurHp, Hp);
        }
    }

    public virtual void BeHurt(PveFightUnit fu)
    {
        float atk = fu.getAtk();
        int def = Def;
        
        if (fu.pvpPlayerBuff != null && fu.pvpPlayerBuff.roundBuffList.Count > 0)
        {
            //Debug.Log("-------------------------------------敌方 buff 数据开始---------------------------------------------");
            foreach (PvpBuffData pvpBuffData in fu.pvpPlayerBuff.roundBuffList)
            {
                Debug.Log("buff data : " + pvpBuffData.buffType + ", round : " + pvpBuffData.roundValue);
            }
            //Debug.Log("-------------------------------------敌方 buff 数据结束---------------------------------------------");
        }
       
        //根据方块位置，乘上系数计算伤害值
        //fu 攻击方
        float at = atk - def;
        float addtion = 0f;
        if (fu.GetType() == typeof(PveCharacter) || fu.GetType() == typeof(PvePet))
        {
            addtion = fu.CurElim.Addition - 1f;
        }
        if (gameObject == null) return;
        //根据双方属性 再次计算伤害值at
        int fp = ActP(fu);
        int sp = StP();
        float pr=GetElementParam(fp, sp);
        float sat = at * (addtion + pr);//* GetRandomHurtPer();
        if (sat < 0) sat = 1;

        //暴击
        bool critBool = false;
       

        if (fu.GetType() == typeof(PveCharacter))
        {
            float tempAt = sat;           
            float crit = ConfigManager.ParamConfig.GetParam().GlobalCrit;
            int jg = Tools.GetRandom_n(10000);
            if (crit > jg )
            {
                CameraControl.ShakeCamera();
                tempAt = sat * (1f + ConfigManager.ParamConfig.GetParam().GlobalCritDamageGrowthRate/10000);
                critBool = true;
            }
            sat = tempAt;          
        }

        float miss = Evade;
        bool missBool = false;
        if(!critBool)
        {          
            double jg1 =Tools.GetRandom();
            if (jg1 < (double)miss)
            {
                //CameraControl.ShakeCamera();
                sat = 0;
                missBool = true;
                Debug.Log(jg1);
                Debug.Log(miss);
                ShowMissEff();
            }
        }
        //Debug.Log(fp + "    " + sp+"  atk=" + atk   + "  def=" + def+"   fu.name= " + fu.name);
        //Debug.Log(" monster    BeHurt = " + sat);
        CurHp = CurHp - sat;
        HurtLabelShow(sat, fu, critBool, missBool,pr);
        //Debug.Log(name+"  "+CurHp);

        string sId = fu.SkinData.Id;
        if (fu.GetType() == typeof(PveCharacter))
        {
            if (UserManager.CurUserInfo.CurWeapon != null)
            {
                sId = UserManager.CurUserInfo.CurWeapon.CurHardWareData.SkinId;
            }
        }
        //攻击音效       
        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(sId);
        string hitSound = skinData.FireFXName;
        if (critBool) hitSound = "FX6";
        if (missBool) hitSound = "FX7";
        if (pr < 0.99f) hitSound = "FX8";//克
        GameObject g = Resources.Load("PreFabs/FX/" + hitSound) as GameObject;
        if (g)
        {
            GameObject beHitObject = Instantiate(g) as GameObject;
            beHitObject.transform.parent = transform;
            beHitObject.transform.localPosition = new Vector3(0, 0.05f, 0);
        }

		// 如果是怪物
		if(this.GetType() == typeof(PveMonster) || this.GetType() == typeof(PveBoss))
		{
			if(!critBool)
			{
				// 显示怪物表情 受伤，无暴击
				PveFaceManager.Trigger(this, PveFaceTypeEnum.HURT);
			}else{
				// 显示怪物表情 受伤，有暴击
				PveFaceManager.Trigger(this, PveFaceTypeEnum.HURT_CRIT);
			}

			//Debug.Log("攻击之后当前的怪物血量为：" + this.CurHp + ":" + this.Hp);
		}

        HurtAnimation();

        if(CurUnitHp != null)
        {
            CurUnitHp.RefreshUI(CurHp, Hp);
        }
    }

    /// <summary>
    /// 闪避
    /// </summary>
    GameObject MissObje;
    protected void ShowMissEff()
    {
        if (MissObje != null) Destroy(MissObje);

        MissObje = new GameObject();
        SpriteRenderer newspriteMiss = MissObje.AddComponent<SpriteRenderer>();
        newspriteMiss.sprite = Resources.Load<Sprite>("Atlas/Fight/miss");
        MissObje.transform.parent = transform;
        MissObje.layer = gameObject.layer;
        MissObje.transform.localPosition = new Vector3(0f, 0.07f, 0f);

        MissObje.transform.localScale = new Vector3(0.001760563f, 0.001760563f, 1f);
        newspriteMiss.sortingOrder = 1000;

        Vector3 v3 = MissObje.transform.localPosition + new Vector3(0f, 0.1f, 0f);
        AnimationHelper.AnimationMoveTo(v3, MissObje, iTween.EaseType.easeOutQuad, gameObject, "MissObjectFade", 0.5f);
    }

    void MissObjectFade()
    {
        AnimationHelper.AnimationFadeTo(0, MissObje, iTween.EaseType.easeOutQuad, gameObject, "MissObjeDestroy", 0.5f);
    }

    void MissObjeDestroy()
    {
        //Debug.Log("YY " + MissObje);
        if (MissObje != null) Destroy(MissObje);
    }
    

    /// <summary>
    /// 受伤动画
    /// </summary>
    public virtual void HurtAnimation()
    {
            if (GetType() == typeof(PveBoss) || GetType() == typeof(PveMonster))
            {
                iTween.PunchPosition(RenderObject.gameObject, new Vector3(0.3f, 0, 0), 0.3f);
            }        
    }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="damage"> 伤害值</param>
   /// <param name="from">攻者</param>
   /// <param name="crit">暴击</param>
   /// <param name="miss">躲闪</param>
   /// <param name="prop">相克关系</param>
    public void HurtLabelShow(float damage, PveFightUnit from,bool crit=false, bool miss = false,float pr=1f)
    {
        if (miss) return;
        GameObject beHurtResource = Resources.Load("PreFabs/Fight/DamageLable") as GameObject;
        GameObject hurtObject = Instantiate(beHurtResource) as GameObject;
        hurtObject.transform.parent = transform.parent;
        //随机位置
        hurtObject.transform.localScale = new Vector3(1, 1, 1);
        int offsetX = UnityEngine.Random.Range(-30, 31);
        int offsetY = UnityEngine.Random.Range(-30, 31);
        hurtObject.transform.localPosition = new Vector3(transform.localPosition.x + offsetX, transform.localPosition.y + 30 + offsetY, transform.localPosition.z);
        FightDamage fd = hurtObject.GetComponent<FightDamage>();
        DungeonEnum.ElementAttributes defaultAttibute = DungeonEnum.ElementAttributes.Fire;
        if (from)
        {
            if (from.GetType() == typeof(PvePet))
            {
                PvePet p = (PvePet)from;
                defaultAttibute = p.CurUserPet.CurPetData.PetPro;
            }
            else if (from.GetType() == typeof(PveCharacter))
            {
                defaultAttibute = UserManager.CurUserInfo.CurWeapon.CurHardWareData.Element;
            }
        }
        fd.DamageShow(((int)damage).ToString(), defaultAttibute,crit,pr);
    }

    /// <summary>
    /// 回血数字
    /// </summary>
    /// <param name="heal"></param>
    public void HealLabelShow(float heal)
    {
        GameObject beHurtResource = Resources.Load("PreFabs/Fight/DamageLable") as GameObject;
        GameObject hurtObject = Instantiate(beHurtResource) as GameObject;
        hurtObject.transform.parent = transform.parent;
        //随机位置
        hurtObject.transform.localScale = new Vector3(1, 1, 1);
        int offsetX = UnityEngine.Random.Range(-30, 31);
        int offsetY = UnityEngine.Random.Range(-30, 31);
        hurtObject.transform.localPosition = new Vector3(transform.localPosition.x + offsetX, transform.localPosition.y + 30 + offsetY, transform.localPosition.z);
        FightDamage fd = hurtObject.GetComponent<FightDamage>();
        DungeonEnum.ElementAttributes defaultAttibute = DungeonEnum.ElementAttributes.Earth;
        fd.DamageShow(((int)heal).ToString(), defaultAttibute);
    }

    #endregion
    /// <summary>
    /// 被击退
    /// </summary>
    public void BeHitBackRender(float backCount, DungeonEnum.FaceDirection backDirection)
    {
        Debug.Log("击退" + backCount + backDirection);

        int xP = XPosition;
        int yP = YPosition;
        PveTile tb = null;
        if (backDirection == DungeonEnum.FaceDirection.Up)
        {
            int tempY = YPosition + (int)backCount;
            if (tempY > 8)
            {
                tempY = 8;
            }
            for (int i = YPosition + 1; i <= tempY; i++)
            {
				if (GameControl.FindEnemyOn(xP, i) == null)
				{
					yP++;
				}
				else
				{
					break;
				}
            }
			tb = GameControl.FindPveTile(xP, yP);
        }
        else if (backDirection == DungeonEnum.FaceDirection.Down)
        {
            int tempY = YPosition - (int)backCount;
            if (tempY < 0)
            {
                tempY = 0;
            }
            for (int i = YPosition - 1; i >= tempY; i--)
            {
				if (GameControl.FindEnemyOn(xP, i) == null)
				{
					yP--;
				}
				else
				{
					break;
				}
            }
			tb = GameControl.FindPveTile(xP, yP);
        }
        else if (backDirection == DungeonEnum.FaceDirection.Left)
        {
            int tempX = XPosition - (int)backCount;
            if (tempX < 0)
            {
                tempX = 0;
            }
            for (int i = XPosition - 1; i >= tempX; i--)
			{
				if (GameControl.FindEnemyOn(i, yP) == null)
				{
					xP--;
				}
				else
				{
					break;
				}
			}
			tb = GameControl.FindPveTile(xP, yP);
        }
        else if (backDirection == DungeonEnum.FaceDirection.Right)
        {
            int tempX = XPosition + (int)backCount;
            if (tempX > 6)
            {
                tempX = 6;
            }
            for (int i = XPosition + 1; i <= tempX; i++)
            {
				if (GameControl.FindEnemyOn(i, yP) == null)
				{
					xP++;
				}
				else
				{
					break;
				}
			}
			tb = GameControl.FindPveTile(xP, yP);
        }
        Debug.Log(tb + " ---- " + name);
        if (tb != null && name.IndexOf("Barrier") != 0 && name.IndexOf("Trigger") != 0)
        {
            //XPosition = tb.XPosition;
            //YPosition = tb.YPosition;
            SetPosition(tb.XPosition, tb.YPosition);
            SetName();
            Vector3 tempv0 = new Vector3(0,26,0);
            Vector3 tempv = tb.transform.localPosition - tempv0;
            AnimationHelper.AnimationMoveTo(tempv, gameObject, iTween.EaseType.linear, null, null, 0.25f);
        }

    }


    /// <summary>
    /// 设置透明度
    /// </summary>
    List<SpriteRenderer> UnitSprites = new List<SpriteRenderer>();
    Action FadeActionEnd;
    void GetUnitSprite(Transform trans)
    {
        foreach (Transform t in trans)
        {
            SpriteRenderer sprite = t.GetComponent<SpriteRenderer>();
            if (sprite != null) UnitSprites.Add(sprite);
            GetUnitSprite(t);
        }
    }

    void SetUnitAlpha(float alpha, Action callback)
    {
        FadeActionEnd = callback;
        UnitSprites.Clear();
        GetUnitSprite(transform);
        if(alpha > 0.5)
        {
            UpdateAlpha(1f);
        }
        else
        {
            AnimationHelper.AnimationValueTo(gameObject, 1f, alpha, 0.3f, "UpdateAlpha", gameObject, "FadeEnd", gameObject, null);
        }
    }

    void UpdateAlpha(float value)
    {
        foreach(SpriteRenderer sprite in UnitSprites)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, value);
        }
    }

    void FadeEnd()
    {
        FadeActionEnd();
    }

    /// <summary>
    /// 消失
    /// </summary>
    public void UnitDisappear(Action callback)
    {
        SetUnitAlpha(0f, () =>
        {
            callback();
        });
    }

    /// <summary>
    /// 出现
    /// </summary>
    public void UnitAppear()
    {
        SetUnitAlpha(1f, () => { });
    }

    /// <summary>
    /// 无敌消失
    /// </summary>
    public void SetDisappear(int round, Action callback)
    {
        Debug.Log(name);
        lastRoundCount = round;
        CurState = UnitState.guard;
        UnitDisappear(() => 
        {
            callback();
        });
    }

    /// <summary>
    /// 出现
    /// </summary>
    public void SetAppear()
    {
        CurState = UnitState.normal;
        UnitAppear();
    }

    /// <summary>
    /// 解析buff
    /// </summary>
    public void AnalyzeBuff()
    {
        Hp = BaseHp;
        Atk = BaseAtk;
        Def = BaseDef;
        Evade = BaseEvade;
        foreach (PveBuffData buff in CurBuffs)
        {
            Atk += BaseAtk * buff.AtkBuff;
            Def += (int)((float)BaseDef * buff.DefBuff);
            Hp += BaseHp * buff.HpBuff;
            Evade += buff.EvadeBuff;
        }
    }

    /// <summary>
    /// 添加buff
    /// </summary>
    public void AddBuff(PveBuffData buff)
    {
        CurBuffs.Add(buff);
        AnalyzeBuff();
    }

    /// <summary>
    /// 移除buff
    /// </summary>
    public void DeleteBuff(PveBuffData buff)
    {
        if (CurBuffs.Contains(buff)) CurBuffs.Remove(buff);
        AnalyzeBuff();
    }

    //清除到期buff
    public void ClearTimeOutBuffs()
    {
        int preCount = CurBuffs.Count;
        for(int i = CurBuffs.Count - 1; i >= 0; i--)
        {
            if(CurBuffs[i].LastTime <= 0)
            {
                CurBuffs.Remove(CurBuffs[i]);
            }
            else
            {
                CurBuffs[i].LastTime--;
            }
        }
        int laterCount = CurBuffs.Count;

        if(laterCount < preCount)
        {
            AnalyzeBuff();
        }
    }

    #region 暴击 闪躲 反击

    //反击几率，值随技能增减	
    public int Base_strike = 0;

    /// <summary>
    /// 暴击
    /// </summary>

    public float CritPre(PveFightUnit fu)
    {
        if (this.GetType() != typeof(PveCharacter)) return 0;

        // 暴击数据计算
        float critData = 1.0f;
        int crit = (int)ConfigManager.ParamConfig.GetParam().GlobalCrit;

        bool critBool = SummonRandom(crit);
        if (critBool) // 如果值不为 0 说明是暴击
        {
            CameraControl.ShakeCamera();
            critData = ConfigManager.ParamConfig.GetParam().GlobalCritDamageGrowthRate + this.pvpPlayerBuff.GetValueByBuffTypeToFloat(BuffTypeEnum.Recover_Crit_Hurt);
            Debug.Log("技能  暴击效果：---> " + critData);
        }
        return critData;
    }

    /// <summary>
    /// 闪躲
    /// </summary>

    public bool MissRandom()
    {
        int miss = (int)ConfigManager.ParamConfig.GetParam().GlobalMiss;
        return SummonRandom(miss);
    }

    /// <summary>
    /// 反击
    /// </summary>
    public bool StrickeRandom(int v)
    {
        return false;
    }
    /// <summary>
    /// 眩晕 回合失效
    /// </summary>    
    public bool HaloRandom()
    {
        int instr = (int)this.pvpPlayerBuff.GetValueByBuffTypeToFloat(BuffTypeEnum.Dizziness);
        return SummonRandom(instr);
    }

    ///随机位置 传送
    //public Vector3 PositionRandom(int round)
    //{
    //}

    /// <summary>
    /// 自己周围有效位置
    /// </summary>
    public Vector3 AreaRandom(Vector2 position)
    {
        Vector3 v3 = new Vector3(0, 0, 0);
        //自己周围8方砖  怪排除掉
        //List<PvpTile> roundTile = GameControl.FindEightGridList(this);
        return v3;
    }

    /// <summary>
    /// 技能随机

    public bool SkillRandom(int v)
    {
       return SummonRandom( v);
    }

    /// <summary>
    /// 普通攻击随机

    public bool AttackRandom(int v)
    {
        return SummonRandom( v);
    }

    /// <summary>
    /// 普通攻击随机 2

    public bool AttackRandom2(int v)
    {
       
        return SummonRandom(v);
    }

    /// <summary>
    /// 随机召唤
  
    public bool SummonRandom(int v)
    {
        return Seed(v);
    }


    public bool Seed(int data)
    {
        int val = Tools.GetRandom_n(10000);
        if (val <= data) return true;
        return false;
    }



    #endregion

    #region Mono
    void OnEnable()
    {
        lastRoundCount = -1;
    }
    #endregion
}

