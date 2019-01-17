using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using SimpleJson;

public class PvpFightUnit : PvpGameObject
{
	public static readonly string TOP = "top";
	public static readonly string BOTTOM = "bottom";
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
        Victor=130
		
		
		
	}
	#endregion

	#region 属性
	public DungeonEnum.FaceDirection CurFaceDirection = DungeonEnum.FaceDirection.None;
	
	public ActionState CurActionState = ActionState.None;

    public List<GameObject> AllAnimations = new List<GameObject>();

    public int MoveSpeed = 140;

    public List<PvpTile> RangeTiles = new List<PvpTile>();

    public float CurHp;

	/// <summary>
	/// 临时 HP
	/// </summary>
	public float VarHp;

    public float Hp;

	private float _atkVar;
    public float Atk
	{
		get
		{
			if(this.GetType() == typeof(PvpCharacter))
			{
				return this.PvpUserInfo.CurAtk;
			}else{
				return this._atkVar;
			}
		}
		set{ this._atkVar = value;}
	}

	private float _defVar;
	public float Def
	{
		get
		{
			if(this.GetType() == typeof(PvpCharacter))
			{
				return this.PvpUserInfo.CurDef;
			}else{
				return this._defVar;
			}
		}
		set{ this._defVar = value;}
	}

    public DungeonEnum.ElementAttributes Element;

    public UnitHp CurUnitHp;

    public SkinConfigData SkinData;

    public PvpEliminate CurElim;

    public bool IsChain = false;

    public bool Weakness = false;

    public Hashtable SHash = new Hashtable();

	public float DisFromeChar;
	
	public PvpUserInfo PvpUserInfo;

	public string UserType = "top";
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

	public BaseSkillItem SkillItem;

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
	/// 闪避状态
	/// </summary>
	public bool avoidStatus;
	public bool attackStatus;

    public int PreFight_x ;
	public int PreFight_y ;
	
	public string CacheKey;

	public int OrderIndex;

    #endregion

    #region 重写父类
    public override int GetLayer()
    {
        return LayerHelper.Unit;
    }

	public PvpUserInfo UserInfo
	{
		get
		{
			if(this.GetType() == typeof(PvpCharacter)) return this.PvpUserInfo;
			return GameControl.CurrentCharacter.UserInfo;
		}
	}

	public bool CritAndAvoidStatusCheck(PvpFightUnit fu, int step, int index = -1)
	{
		float critData = 0f;
		if(index == -1)
		{
			critData = CritRandom(GameControl.PvpRounds, step, fu);
		}else{
			critData = CritRandom(GameControl.PvpRounds, step + index, fu);
		}

		bool missBool = false;

		// 只有角色会闪避
		if(this.GetType() == typeof(PvpCharacter))
		{
			if(this.pvpPlayerBuff.CanOrNotMoveCheck())
			{
				if(index == -1)
				{
					missBool = MissRandom(GameControl.PvpRounds, step, this);  
				}else{
					missBool = MissRandom(GameControl.PvpRounds, step + index, this);  
				}
			}
		}

		if(critData > 0) return false;

		return missBool;
	}
	
	/// <summary>
	/// 这个函数可以与下面那个函数合并
	/// </summary>
	/// <returns>The hurt value.</returns>
	/// <param name="fu">Fu.</param>
	public float BeHurtValue(PvpFightUnit fu, int index, ref bool critBool, ref bool missBool)
	{
		float addtion = 1f;
		// 行动步骤
		int actionStep = 0;
		
		// 如果是角色
		if(fu.GetType() == typeof(PvpCharacter))
		{
			// 如果不是反击
			if(!fu.StrickeStatus)
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
		}else
		{
			addtion = fu.CurElim.Addition; // 方砖伤害加成
			actionStep = fu.CurElim.CurStep;
		}
		
		int sat = (int)fu.getAtk() ; // 攻击方攻击力
		
		int fp = ActP(fu); // 攻击方属性
		int sp = StP();    // 防御方属性
		
		// 暴击数据计算
		//float critData = 0.0f;
		float critData = 0f;
		
		// 如果是角色，计算暴击
		if(fu.GetType() == typeof(PvpCharacter))
		{
			if(index == -1)
			{
				critData = CritRandom(GameControl.PvpRounds, actionStep, fu);
			}else
			{
				critData = CritRandom(GameControl.PvpRounds, actionStep + index, fu);
			}
		}
		
		if(critData > 0)
		{
			critBool = true;
		}else
		{
			critBool = false;
		}
		
		float lastDamage = 0;
		// 闪躲数据计算
		missBool = false;
		
		if(this.GetType() == typeof(PvpCharacter))
		{
			// 如果没有眩晕技能，可以行走！
			if(this.pvpPlayerBuff.CanOrNotMoveCheck())
			{
				if(index == -1)
				{
					missBool = MissRandom(GameControl.PvpRounds, actionStep, this);  
				}else{
					missBool = MissRandom(GameControl.PvpRounds, actionStep + index, this);  
				}
			}
		}
		
		if (missBool && !critBool) // 如果闪避，并且没有触发暴击，先计算暴击，再计算闪避，暴击之后不会闪避
		{        
			lastDamage = 0;  
		}
		else
		{
			//普通攻击伤害计算公式 = (进攻方攻击力-防御方防御力) * (方砖伤害加成 +暴击伤害加成)
			lastDamage = (sat - this.Def * getDefUp ()) * (addtion + critData);  //最后伤害
			lastDamage = (int)lastDamage;
			if (lastDamage <= 0)
			{
				lastDamage = 1;
			}
			/*
			Debug.Log("----------------------------------------------------- 人物攻击开始计算伤害 ----------------------------------------");
			Debug.Log("敌方伤害值：" + sat);
			Debug.Log("已方防御值：" + this.Def);
			Debug.Log("步骤加成值：" + addtion);
			Debug.Log("暴击：" + critData);
			Debug.Log("最终伤害值：" + lastDamage);
			Debug.Log("----------------------------------------------------- 人物攻击开始结束伤害 ----------------------------------------");
			*/
		}
		/*
		#if UNITY_STANDALONE_WIN
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.Append ("---> 计算伤害数据开始\n");
			stringBuilder.Append ("攻击方攻击值：");
			stringBuilder.Append (sat);
			stringBuilder.Append ("，攻击方防御值：");
			stringBuilder.Append (fu.Def);
			stringBuilder.Append ("，防御方攻击值：");
			stringBuilder.Append (this.getAtk());
			stringBuilder.Append ("，防御方防御值：");
			stringBuilder.Append (this.Def);
			stringBuilder.Append ("，暴击状态：");
			stringBuilder.Append (critBool);
			stringBuilder.Append ("，暴击伤害：");
			stringBuilder.Append (critData);
			stringBuilder.Append ("，闪避状态：");
			stringBuilder.Append (missBool);
			stringBuilder.Append ("，步骤加成：");
			stringBuilder.Append (addtion);
			stringBuilder.Append ("，最终伤害：");
			stringBuilder.Append (lastDamage);
			stringBuilder.Append ("\n");
			PvpLogManager.Log (this.GameControl.PvpTableID, stringBuilder.ToString ());
		#endif
		*/
		return lastDamage;
	}

	/// <summary>
	/// 设置眩晕
	/// </summary>
	public void ChangeDizziness()
	{
		ChangeAnimation(CurFaceDirection, ActionState.Vertigo);
	}

	/// <summary>
	/// 设置等待
	/// </summary>
	public void ChangeWaiting()
	{
		ChangeAnimation(CurFaceDirection, ActionState.Waiting);
	}

	/// <summary>
	/// 设置隐藏状态
	/// </summary>
	/// <param name="status">If set to <c>true</c> status.</param>
	/// <param name="alpha">Alpha.</param>
	public void ChangeHiddenStatus(bool status, float alpha = 0.0f)
	{
		this.HiddenStatus = status;
		this.HiddenAlpha = alpha;

		if(alpha > 0f)
		{
			this.gameObject.SetActive(true);
			if(this.gameObject != null)
			{
				// 设置透明度
				SpriteRenderer[] spriteRenderList = this.gameObject.GetComponentsInChildren<SpriteRenderer> ();
				foreach(SpriteRenderer spriteRender in spriteRenderList)
				{
					spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, alpha);
				}
			}
		}else{
			this.gameObject.SetActive(false);
		}
	}


    public override void SetPosition(int xPosition, int yPosition)
    {
        base.SetPosition(xPosition, yPosition);
        CaculateRangeTiles();
        Vector3 basicPotisition = CaculateRealPosition(xPosition, yPosition);
		transform.localPosition = new Vector3(basicPotisition.x + XRange / 2 * 40, basicPotisition.y, basicPotisition.z);
		/*
		if(this.GetType() != typeof(PvpMonster))
		{
        	transform.localPosition = new Vector3(basicPotisition.x + XRange / 2 * 40, basicPotisition.y, basicPotisition.z);
		}else{
			transform.localPosition = new Vector3(basicPotisition.x + 1 / 2 * 40, basicPotisition.y, basicPotisition.z);
		}
		*/
        SetOrder();
    }
    public void SetPrePosition(int xPosition, int yPosition)
    {
        PreFight_x = xPosition;
        PreFight_y = yPosition;
        CaculateRangeTiles(true);
    }

    public override Vector3 CaculateRealPosition(int xPosition, int yPosition)
    {
        Vector3 v = base.CaculateRealPosition(xPosition, yPosition);
        return new Vector3(v.x, v.y - 26, 0);
    }
    #endregion

    #region 公用方法
    public void UnitChainAttack()
	{      
		ActionState curActionState = ActionState.FinishSkill; 
		// 如果是角色
		if(this.GetType() == typeof(PvpCharacter))
		{
			HardWareData.HardWareType ht = GameControl.CurrentCharacter.PvpUserInfo.CurWeapon.CurHardWareData.Style;
			if (ht == HardWareData.HardWareType.Far1 || ht == HardWareData.HardWareType.Far2)
			{
				if(ht == HardWareData.HardWareType.Far1)
				{
					curActionState = ActionState.Far1Attack;
				}else
				{
					curActionState = ActionState.Far2Attack;
				}
			}
		}
		ChangeAnimation(DungeonEnum.FaceDirection.LeftDown, curActionState);
    }
    /// <summary>
    /// 眩晕
    /// </summary> 
    public virtual void UnitVertigo(DungeonEnum.FaceDirection direction)
    {
    }

    public virtual void SetVertigo(int round)
    {
        //lastRoundCount = round;
        //CurState = UnitState.vertigo;
    }

    public void UnitAttack(DungeonEnum.FaceDirection direction)
    {
		if(this.StrickeStatus)
		{
			Debug.Log("调用反击攻击动作！！！！！！！");
		}
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
        Animator at = RenderObject.GetComponent<Animator>();
        at.SetInteger("ActionState", (int)ActionState.Waiting);
    }

    public Action<PvpFightUnit> CurDeadAction;
	public virtual void UnitDead(Action<PvpFightUnit> curDeadAction)
    {
		CurDeadAction = curDeadAction;

		if(this.GetType() != typeof(PvpEgg))
		{
			Debug.Log("调用死亡动作 ！ ChangeAnimation Dead");
			this.StartCoroutine(this.UnitDeadCheckAction());
			ChangeAnimation(DungeonEnum.FaceDirection.LeftDown, ActionState.Dead);
		}else
		{
			if(CurDeadAction != null) CurDeadAction(this);
		}
    }

	private IEnumerator UnitDeadCheckAction()
	{
		yield return new WaitForSeconds (1f);

		if(!PvpGameControl.DeadStatus)
		{
			// 重新调用死亡状态
			ChangeAnimation(DungeonEnum.FaceDirection.LeftDown, ActionState.Dead);
		}
	}

	/// <summary>
	/// 更新血量
	/// </summary>
	/// <param name="addHp">Add hp.</param>
	public void ChangeHp(float addHp, bool hurtStatus = true, bool showHurt = true)
	{
		if(hurtStatus)
		{
			this.StartCoroutine (this.HurtLabelShowEnumerator (addHp));
		}

		if(this.GetType() == typeof(PvpCharacter))
		{
			this.Hp = this.PvpUserInfo.CurHp;
		}
		this.CurHp += addHp;

		if(this.CurHp > this.Hp) this.CurHp = this.Hp;
		// 如果需要显示伤害
		if(showHurt && addHp < 0)
		{
			// 显示受伤动作
			if(this.CurActionState != ActionState.Vertigo) this.HurtAnimation();
		}

		//Debug.Log ("技能结束之后最终剩余血量：" + this.CurHp);

		this.RefreshHp ();
	}

	public void ChangeHpShow(float addHp)
	{
		this.StartCoroutine (this.HurtLabelShowEnumerator (addHp));
	}

	/// <summary>
	/// 换血
	/// </summary>
	/// <param name="hp">Hp.</param>
	public void TurnHp(float hp, bool hurtStatus = true, bool showHurt = true)
	{
		// 变化血量
		float changeHp = hp - this.CurHp;

		if(hurtStatus)
		{
			this.StartCoroutine (this.HurtLabelShowEnumerator (changeHp));
		}

		if(this.GetType() == typeof(PvpCharacter))
		{
			this.Hp = this.PvpUserInfo.CurHp;
		}
		this.CurHp = hp;

		if(this.CurHp > this.Hp) this.CurHp = this.Hp;

		// 如果需要显示伤害
		if(showHurt && changeHp < 0)
		{
			// 显示受伤动作
			if(this.CurActionState != ActionState.Vertigo) this.HurtAnimation();
		}

		//Debug.Log ("技能结束之后最终剩余血量：" + this.CurHp);

		this.RefreshHp ();
	}

	private IEnumerator HurtLabelShowEnumerator(float changeHp)
	{
		yield return null;

		if(this.GetType() == typeof(PvpEgg))
		{
			Debug.Log("HurtLabelShowEnumerator egg !!!");
		}

		// 显示伤害
		if(changeHp < 0)
		{
			HurtLabelShow(Math.Abs(changeHp), this);
		}else{
			HurtLabelShow(Math.Abs(changeHp), this, DungeonEnum.ElementAttributes.Earth);
		}
	}

	public virtual void RefreshHp(bool refresh = true)
	{
		//if(GameControl.fightResult != PvpGameControl.PvpFightResult.DEFAULT) return;

		CurUnitHp.RefreshUI(CurHp, Hp);
		if(this.GetType() == typeof(PvpCharacter))
		{
			if(refresh) GameControl.SetHpUIHpShow(this);
		}
	}

    public void DeadEnd()
    {
        CurDeadAction(this);
    }

	public void Magic()
	{
		Debug.Log ("施法！");
		this.ChangeAnimation (this.CurFaceDirection, ActionState.Skill);
	}

	public virtual void MagicEnd()
	{
		Debug.Log ("施法结束 ！！！！！");
		this.ChangeAnimation (this.CurFaceDirection, ActionState.Waiting);
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
		ChangeAnimation(direction, ActionState.Waiting);
    }

    Action MoveEndAction;
    public virtual void UnitMove(int xPosition, int yPosition, Action moveEndAction, DungeonEnum.FaceDirection direction)
	{
		MoveEndAction = moveEndAction;
		if(xPosition == this.XPosition && yPosition == this.YPosition)
		{
			this.MoveEnd();
			return;
		}
        Vector3 toVec = CaculateRealPosition(xPosition, yPosition);
		Vector3 realVec = new Vector3(toVec.x + XRange / 2 * 40, toVec.y, toVec.z);

        ChangeAnimation(direction, ActionState.Run);
        CleanZorder();
        XPosition = xPosition;
        YPosition = yPosition;
        SetOrder();
        SetName();
        AnimationHelper.AnimationMoveToSpeed(realVec, gameObject, iTween.EaseType.linear, gameObject, "MoveEnd", MoveSpeed);
    }

	public void UnitMoveDirection(DungeonEnum.FaceDirection direction)
	{
		ChangeAnimation(direction, ActionState.Run);
	}

    void MoveEnd()
    {
        if (MoveEndAction != null)
        {
            MoveEndAction();
        }
    }

	public bool NeighbourCheck(PvpFightUnit fightUnit, int xPos, int yPos)
	{
		List<PvpTile> tileRangeList = new List<PvpTile> ();		
		for (int i = this.XPosition; i < this.XPosition + XRange; i++)
		{
			for (int j = this.YPosition; j < this.YPosition + YRange; j++)
			{
				PvpTile pt = GameControl.FindPvpTile(i, j);
				if (pt)
				{
					tileRangeList.Add(pt);
				}
			}
		}
		//正四方向
		foreach (PvpTile pt in tileRangeList)
		{
			if ((pt.XPosition == xPos - 1 && pt.YPosition == yPos) ||
			    (pt.XPosition == xPos + 1 && pt.YPosition == yPos) ||
			    (pt.XPosition == xPos && pt.YPosition == yPos - 1) ||
			    (pt.XPosition == xPos && pt.YPosition == yPos + 1))
			{
				return true;
			} 
		}
		// 如果是角色
		if(fightUnit.GetType() == typeof(PvpCharacter))
		{
			HardWareData.HardWareType ht = fightUnit.PvpUserInfo.CurWeapon.CurHardWareData.Style;
			//主角 轻武器 补四个斜角
			if (ht == HardWareData.HardWareType.Light)
			{            
				foreach (PvpTile pt in tileRangeList)
				{
					if ((pt.XPosition == xPos - 1 && pt.YPosition == yPos + 1) ||
					    (pt.XPosition == xPos + 1 && pt.YPosition == yPos + 1) ||
					    (pt.XPosition == xPos + 1 && pt.YPosition == yPos - 1) ||
					    (pt.XPosition == xPos - 1 && pt.YPosition == yPos - 1))
					{
						return true;
					}
				}
			}
		}
		
		return false;
	}

    public bool IsNeighbour(PvpFightUnit fu,bool Pre=false)
    {
        int fx;
        int fy;
        if (Pre)
        {
            fx = fu.PreFight_x;
            fy = fu.PreFight_y;
        }
        else
        {
            fx = fu.XPosition;
            fy = fu.YPosition;
        }

        //正四方向
        foreach (PvpTile pt in RangeTiles)
        {
            if ((pt.XPosition == fx - 1 && pt.YPosition == fy) ||
           (pt.XPosition == fx + 1 && pt.YPosition == fy) ||
           (pt.XPosition == fx && pt.YPosition == fy - 1) ||
            (pt.XPosition == fx && pt.YPosition == fy + 1))
            {
                return true;
            } 
		}

        //Debug.Log("sty= "+ht);
		// 如果是角色
		if(fu.GetType() == typeof(PvpCharacter))
		{
			HardWareData.HardWareType ht = fu.PvpUserInfo.CurWeapon.CurHardWareData.Style;
	        //主角 轻武器 补四个斜角
	        if (fu.name.IndexOf("Character") == 0 && ht == HardWareData.HardWareType.Light)
	        {            
	            foreach (PvpTile pt in RangeTiles)
	            {
                    if ((pt.XPosition == fx - 1 && pt.YPosition == fy + 1) ||
                    (pt.XPosition == fx + 1 && pt.YPosition == fy + 1) ||
                    (pt.XPosition == fx + 1 && pt.YPosition == fy - 1) ||
                    (pt.XPosition == fx - 1 && pt.YPosition == fy - 1))
	                {
	                    return true;
	                }
	            }
	        }
		}

        return false;
    }
    #endregion

	private bool shadowStatus = true;
	public void HiddenShadow(bool shadowStatus)
	{
		this.shadowStatus = shadowStatus;
		if(!this.shadowStatus) this.ChangeAnimation (this.CurFaceDirection, this.CurActionState);
	}

	public void InitAnimation(Action callback, bool callbackStatus = true)
	{
		if(!PvpGameObjectManager.CACHE_STATUS)
		{
			if(callback != null) callback();
			return;
		}
		DungeonEnum.FaceDirection[] directionList = new DungeonEnum.FaceDirection[]
		{
			DungeonEnum.FaceDirection.Up,
			DungeonEnum.FaceDirection.UpRight,
			DungeonEnum.FaceDirection.Right,
			DungeonEnum.FaceDirection.RightDown,
			DungeonEnum.FaceDirection.Down,
			DungeonEnum.FaceDirection.LeftDown,
			DungeonEnum.FaceDirection.Left,
			DungeonEnum.FaceDirection.LeftUp
		};

		this.InitAnimationItem (callback, directionList, 0, callbackStatus);
	}

	private void InitAnimationItem(Action callback, DungeonEnum.FaceDirection[] directionList, int index = 0, bool callbackStatus = true)
	{
		if(this.animationCacheDict.Count < directionList.Length)
		{
			DungeonEnum.FaceDirection faceDirection = directionList[index];
			if(callbackStatus)
			{
				PvpGameObjectManager.Create(GetAnimationObject(faceDirection), (GameObject animationObject)=>
				{
					animationObject.transform.parent = transform;
					animationObject.transform.localPosition = new Vector3(0,0,0);
					animationObject.SetActive(false);
					// 如果不包含数据
					if(!this.animationCacheDict.ContainsKey((int)faceDirection))
					{
						this.animationCacheDict.Add((int)faceDirection, animationObject);
					}
					this.InitAnimationItem(callback, directionList, index + 1);
				});
			}else
			{
				GameObject animationObject = GameObject.Instantiate(GetAnimationObject(faceDirection)) as GameObject;;
				animationObject.transform.parent = transform;
				animationObject.transform.localPosition = new Vector3(0,0,0);
				animationObject.SetActive(false);
				// 如果不包含数据
				if(!this.animationCacheDict.ContainsKey((int)faceDirection))
				{
					this.animationCacheDict.Add((int)faceDirection, animationObject);
				}
				this.InitAnimationItem(callback, directionList, index + 1);
			}
		}else
		{
			if(callbackStatus && callback != null) callback();
		}
	}

	private Dictionary<int, GameObject> animationCacheDict = new Dictionary<int, GameObject> ();
    #region 动画方法
    public void ChangeAnimation(DungeonEnum.FaceDirection faceDirection, ActionState actionState)
	{
		CurFaceDirection = faceDirection;
		CurActionState = actionState;

		if(animationCacheDict.ContainsKey((int)faceDirection))
		{
			this.ChangeAnimationAction(animationCacheDict[(int)faceDirection], faceDirection, actionState);
		}else
		{
			PvpGameObjectManager.Create(this.GetAnimationObject(), (GameObject animationObject)=>
			{
				// 如果需要缓存
				if(PvpGameObjectManager.CACHE_STATUS)
				{
					this.animationCacheDict.Add((int)faceDirection, animationObject);
				}
				this.ChangeAnimationAction(animationObject, faceDirection, actionState);
			});
		}
	}

	private void ChangeAnimationAction(GameObject animationObject, DungeonEnum.FaceDirection faceDirection, ActionState actionState)
	{
		GameObject tempObject = RenderObject;
		RenderObject = animationObject;
		RenderObject.SetActive (true);
		
		RenderObject.transform.parent = transform;
		RenderObject.transform.localPosition = new Vector3(0,0,0);
		
		if(GetType() == typeof(PvpCharacter) || GetType() == typeof(PvpPet) || GetType() == typeof(PvpMonster))
		{
			AnimationCon animationCon = this.RenderObject.GetComponent<AnimationCon> ();
			if(animationCon != null)
			{
				GameObject.Destroy (animationCon);
			}
			PvpAnimationCon pvpAnimationCon = this.RenderObject.GetComponent<PvpAnimationCon>();
			if(pvpAnimationCon == null)
			{
				this.RenderObject.AddComponent <PvpAnimationCon>();
			}
		}

		if (tempObject)
		{
			RenderObject.transform.localPosition = tempObject.transform.localPosition;
			// 如果不需要缓存，直接销毁
			if(!PvpGameObjectManager.CACHE_STATUS)
			{
				Destroy(tempObject);
			}else
			{
				if(tempObject != RenderObject)
				{
					PvpAnimationCon pvpAnimationCon = tempObject.GetComponent<PvpAnimationCon>();
					if(pvpAnimationCon != null)
					{
						GameObject.Destroy(pvpAnimationCon);
					}
					tempObject.SetActive(false);
				}
			}
		}
		this.ChangeAnimationActionState (actionState);
	}

	private void ChangeAnimationActionState(ActionState actionState)
	{
		if(this.RenderObject == null) return;

		this.SetOrder();

		if (this.GetType() == typeof(PvpCharacter))
		{
			PlayerAvata pveAvata = RenderObject.GetComponent<PlayerAvata>();
			// 如果没有初始化过
			if(!pveAvata.InitStatus)
			{
				// 设置初始化状态
				pveAvata.InitStatus = true;
				// 如果需要隐藏阴影
				if(!this.shadowStatus)
				{
					GameObject playerShadow = pveAvata.transform.FindChild ("Shadow").gameObject;
					// 获取阴影对象
					if(playerShadow != null) playerShadow.SetActive (false);	
				}
				if(this.UserInfo != null)
				{
					if (this.UserInfo.CurWeapon != null)
					{
						pveAvata.AddAvataWare(this.UserInfo.CurWeapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
						pveAvata.WeaponEffectShow();
					}
					
					if (this.UserInfo.CurHelmet != null)
					{
						pveAvata.AddAvataWare(this.UserInfo.CurHelmet.CurHardWareData.SkinId, CurFaceDirection);
					}
					
					if (this.UserInfo.CurArmor != null)
					{
						pveAvata.AddAvataWare(this.UserInfo.CurArmor.CurHardWareData.SkinId, CurFaceDirection);
					}
				}
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
				if(actionState == ActionState.Hurt)
				{
					am.speed = 0.5f;
				}else
				{
					am.speed = 1f;
				}
			}
		}
		
		//Debug.Log(name+"  "+faceDirection+"  actionState="+actionState);
		Animator at = RenderObject.GetComponent<Animator>();
		at.SetInteger("ActionState", (int)CurActionState);

		if(this.StrickeStatus)
		{
			this.strickeDelayStatus = true;
			this.StartCoroutine(this.StrickeDelayCoroutine());
		}
		
		this.ResertAllLayer();
	}

	protected bool strickeDelayStatus;
	private IEnumerator StrickeDelayCoroutine()
	{
		yield return new WaitForSeconds (1f);

		if(this.strickeDelayStatus)
		{
			// 无限切换攻击动作
			this.UnitAttack(this.CurFaceDirection);
		}
	}

    #endregion

    #region 工具方法
    public void CaculateRangeTiles(bool pre=false)
    {
        RangeTiles.Clear();
        int x1;
        int y1;
        if (pre)
        {
            x1 = PreFight_x;
            y1 = PreFight_y;
        }
        else
        {
            x1 = XPosition;
            y1 = YPosition;
        }

		for (int i = x1; i < x1 + XRange; i++)
        {
			for (int j = y1; j < y1 + YRange; j++)
            {
				PvpTile pt = GameControl.FindPvpTile(i, j);

                if (pt)
                {
                    RangeTiles.Add(pt);
                }
            }
        }
    }
	/*
    public List<PvpTile> GetRangeTiles(int xPosition, int yPosition)
    {
        List<PvpTile> tiles = new List<PvpTile>();
        for (int i = xPosition; i < xPosition + XRange; i++)
        {
            for (int j = yPosition; j < yPosition + YRange; j++)
            {
				PvpTile pt =  GameControl.FindPvpTile(i, j);
                if (pt)
                {
                    tiles.Add(pt);
                }
            }
        }
        return tiles;
    }
	*/

	public void ResertAllLayer()
	{
		if(this.RenderObject == null) return;

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
	/*
    public void ResertAllLayer()
	{
		if(RenderObject == null) return;

        List<GameObject> childs = GetChild(RenderObject.transform);
        foreach (GameObject go in childs)
        {
            go.layer = GetLayer();

        }
    }

    public List<GameObject> GetChild(Transform trans)
    {
		List<GameObject> childs = new List<GameObject>();
		Transform[] transformList = trans.GetComponentsInChildren<Transform> ();
		if(transformList != null && transformList.Length > 0)
		{
			foreach(Transform transformItem in transformList)
			{
				childs.Add(transformItem.gameObject);
			}
		}
        return childs;
    }*/

    public GameObject GetAnimationObject()
    {
		return this.GetAnimationObject (this.CurFaceDirection);
		/*
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
        return null;*/
    }

	public GameObject GetAnimationObject(DungeonEnum.FaceDirection faceDirection)
	{
		if (faceDirection == DungeonEnum.FaceDirection.Up)
		{
			return AllAnimations[0];
		}
		else if (faceDirection == DungeonEnum.FaceDirection.UpRight)
		{
			return AllAnimations[1];
		}
		else if (faceDirection == DungeonEnum.FaceDirection.Right)
		{
			return AllAnimations[2];
		}
		else if (faceDirection == DungeonEnum.FaceDirection.RightDown)
		{
			return AllAnimations[3];
		}
		else if (faceDirection == DungeonEnum.FaceDirection.Down)
		{
			return AllAnimations[4];
		}
		else if (faceDirection == DungeonEnum.FaceDirection.LeftDown)
		{
			return AllAnimations[5];
		}
		else if (faceDirection == DungeonEnum.FaceDirection.Left)
		{
			return AllAnimations[6];
		}
		else if (faceDirection == DungeonEnum.FaceDirection.LeftUp)
		{
			return AllAnimations[7];
		}
		return null;
	}

    int highestOrder = 0;
    public override void CleanZorder()
    {
        if (RenderObject)
        {
            List<GameObject> childs = GetChild(RenderObject.transform);
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
					if (this.GetType() == typeof(PvpPet)) sr.sortingOrder = sr.sortingOrder + 1;
					
					if (highestOrder < sr.sortingOrder)
					{
						highestOrder = sr.sortingOrder;
					}
				}
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

    int GetBasicZorder()
    {
        return 200 * (XPosition - YPosition);
    }

    /// <summary>
	/// 技能造成的伤害 
	/// </summary>
    /// <param name="skill">Skill.</param>
    public void SkillHurtRender(SkillData skill,PvpOwnUnit fu,int index=-1)
    {
        float attack = skill.Aparameter;
        if (skill.SkillFX == "SkFX0")
        {                    
			attack = fu.UserInfo.CurAtk;
            //走普通伤害逻辑
            BeHurt(fu,index);
            return;
        }
        
		Debug.Log ("终结技能为：" + skill.Name + ":" + attack + ":" + Def);

        float at = attack - Def;
		Debug.Log ("终结技能造成的伤害为：：：" + at);
        if (at < 1) at = 1;

        CurHp = CurHp - at;
        HurtLabelShow(at, null);
        //装备技能击退
        int id;
        int.TryParse(skill.Id.Substring(2).ToString(), out id);
        Debug.Log("id =" + id);
        if (skill.SkillFX == "SkFX1" && skill.Cparameter > 0.01f)
        {
            //击退
			DungeonEnum.FaceDirection direction = GameControl.CurrentCharacter.GetTargetDirection(this);
            BeHitBackRender(skill.Cparameter, direction);
            //PvpEliminate eb = GameControl.FineEliminate(XPosition, YPosition);
            //eb.SetToPlayerRender();
        }

        HurtAnimation();

		this.RefreshHp ();
		//GameControl.SetHpUIHpShow (this);
        //CurUnitHp.RefreshUI(CurHp, Hp);
    }


    //属性相克  f 攻击 ， s 防守
    //风3>>水4>>火2>>地1>>风3
    public float GetElementParam(int f, int s)
    {
        float p = 1;
		return p;

        if (s > 10)
        {
            int tss = s % 10;
            int tss1 = s / 10;
            if (tss == tss1) s = tss;
        }
        //Debug.Log("s="+s);

        if (s == 0)
        {
        }
        else if (s == 1)
        {
            if (f == 2) p = 2;
            if (f == 3) p = 0.5f;
        }
        else if (s == 2)
        {
            if (f == 4) p = 2;
            if (f == 1) p = 0.5f;
        }
        else if (s == 3)
        {
            if (f == 1) p = 2;
            if (f == 4) p = 0.5f;
        }
        else if (s == 4)
        {
            if (f == 3) p = 2;
            if (f == 2) p = 0.5f;
        }
        else if (s >= 10)
        {
            //防守方为主角 且 双 防装 p=1                        
            p = 1;
        }
        return p;
    }

    //攻击方属性
    public int ActP(PvpFightUnit fu)
    {
        int fp = 0;

        if (fu.GetType() == typeof(PvpPet))
        {
            PvpPet p = (PvpPet)fu;
            fp = (int)p.CurUserPet.CurPetData.PetPro;
        }
        if (fu.GetType() == typeof(PvpCharacter))
        {
			fp = (int)fu.UserInfo.CurWeapon.CurHardWareData.Element;
        }
		
        return fp;

    }
    //防守方属性
    public int StP()
    {
        int sp = 0;
        if (name.IndexOf("Pet") == 0)
        {
            PvpPet p = (PvpPet)this;
            sp = (int)p.CurUserPet.CurPetData.PetPro;
        }
        if (name.IndexOf("Character") == 0)
        {
            //头盔 和护盾
            sp = 0;
			if (this.UserInfo.CurHelmet != null && this.UserInfo.CurArmor != null)
            {
				sp = (int)this.UserInfo.CurHelmet.CurHardWareData.Element * 10 + (int)this.UserInfo.CurArmor.CurHardWareData.Element;

            }
            else
            {
				if (this.UserInfo.CurHelmet != null)
                {
					sp = (int)this.UserInfo.CurHelmet.CurHardWareData.Element;
                }
				if (this.UserInfo.CurArmor != null)
                {
					sp = (int)this.UserInfo.CurArmor.CurHardWareData.Element;
                }
            }
        }
		
        return sp;
    }

    public float GetRandomHurtPer()
    {
        float ra = 1;
        int jg = Tools.GetRandom_n(10) - 5;
        ra = 1 + jg * 0.01f;
        return ra;
    }
    
    
   

    public float getDefUp()
    {
        return 1;
    }

    public virtual float getAtk()
    {
		// 如果是宠物
		if(this.GetType() == typeof(PvpPet))
		{
			// 获取宠物攻击加成
			PvpBuffValueData pvpBuffValueData = GameControl.CurrentCharacter.pvpPlayerBuff.GetValueByBuffTypeToClass(true);
			// 加上宠物攻击数据
			int atk = (int)(Atk * (1 + pvpBuffValueData.attack_odds / PvpUserInfo.ODDS_VALUE));

			//Debug.Log("原来的宠物攻击力：" + Atk + "，新的数据：" + atk);

			return atk;
		}else
		{
        	return Atk;
		}
    }
  
    public virtual void BeHurt(PvpFightUnit fu,int index = -1)
    {
		bool critBool = false;
		bool missBool = false;
		
		float lastDamage = this.BeHurtValue (fu, index, ref critBool, ref missBool);
		//Debug.Log ("PvpFightUnit 开始计算伤害 --------------------------------------->");
		/*int sat = (int)fu.getAtk(); // 攻击方攻击力
		
		int fp = ActP(fu); // 攻击方属性
		int sp = StP();    // 防御方属性

        float def = Def;     
		float addtion = fu.CurElim.Addition; // 方砖伤害加成

		float lastDamage = ((sat - def * getDefUp ()) * addtion);  // 最后伤害
		lastDamage = (int)lastDamage;
		if (lastDamage <= 0)
		{
			lastDamage = 1;
		}*/
		
		CurHp = CurHp - lastDamage;
		HurtLabelShow(lastDamage, fu);
		
		string sId = fu.SkinData.Id;
		if (fu.GetType() == typeof(PvpCharacter))
		{
			if (this.UserInfo.CurWeapon != null)
			{
				sId = this.UserInfo.CurWeapon.CurHardWareData.SkinId;
			}
		}
		SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(sId);
		PvpGameObjectManager.Create("PreFabs/FX/" + skinData.FireFXName, (GameObject beHitObject)=>
		                            {
			beHitObject.transform.parent = transform;
			beHitObject.transform.localPosition = new Vector3(0, 0.05f, 0);
			/*GameObject g = Resources.Load("PreFabs/FX/" + skinData.FireFXName) as GameObject;
			if (g)
			{
				GameObject beHitObject = Instantiate(g) as GameObject;
				beHitObject.transform.parent = transform;
				beHitObject.transform.localPosition = new Vector3(0, 0.05f, 0);
			}*/
		});
		HurtAnimation();

		this.RefreshHp ();
		//CurUnitHp.RefreshUI(CurHp, Hp);
    }

    /// <summary>
    /// 受伤动画
    /// </summary>
    public virtual void HurtAnimation()
    {
        iTween.PunchPosition(RenderObject.gameObject, new Vector3(0.1f, 0, 0), 0.2f);
    }

    /// <summary>
    /// 受伤跳血数字
    /// </summary>
	public void HurtLabelShow(float damage, PvpFightUnit from, DungeonEnum.ElementAttributes elementAttribute = DungeonEnum.ElementAttributes.None, bool crit = false, bool miss = false)
    {
		if (!crit && miss) return;

		PvpGameObjectManager.Create("PreFabs/Fight/DamageLable", (GameObject hurtObject)=>
		{
			//GameObject beHurtResource = Resources.Load("PreFabs/Fight/DamageLable") as GameObject;
			//GameObject hurtObject = Instantiate(beHurtResource) as GameObject;
			hurtObject.transform.parent = transform.parent;
			//随机位置
			hurtObject.transform.localScale = new Vector3(1, 1, 1);
			int offsetX = UnityEngine.Random.Range(-30, 31);
			int offsetY = UnityEngine.Random.Range(-30, 31);
			hurtObject.transform.localPosition = new Vector3(transform.localPosition.x + offsetX, transform.localPosition.y + 30 + offsetY, transform.localPosition.z);
			FightDamage fd = hurtObject.GetComponent<FightDamage>();
			DungeonEnum.ElementAttributes defaultAttibute = DungeonEnum.ElementAttributes.Fire;
			//pvp 内数字颜色统一为白色
			defaultAttibute = DungeonEnum.ElementAttributes.None;
			// 添加调用的颜色
			if(elementAttribute != DungeonEnum.ElementAttributes.None) defaultAttibute = elementAttribute; 
			
            //if (from)
            //{
            //    if (from.GetType() == typeof(PvpPet))
            //    {
            //        PvpPet p = (PvpPet)from;
            //        defaultAttibute = p.CurUserPet.CurPetData.PetPro;
            //    }
            //}
			fd.DamageShow(((int)damage).ToString(), defaultAttibute, crit);
		});
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
        PvpTile tb = null;
        if (backDirection == DungeonEnum.FaceDirection.Up)
        {
            int tempY = YPosition + (int)backCount;
            if (tempY > 8)
            {
                tempY = 8;
            }
			
			tb = GameControl.FindPvpTile(xP, yP);
        }
        else if (backDirection == DungeonEnum.FaceDirection.Down)
        {
            int tempY = YPosition - (int)backCount;
            if (tempY < 0)
            {
                tempY = 0;
            }
			
			tb = GameControl.FindPvpTile(xP, yP);
        }
        else if (backDirection == DungeonEnum.FaceDirection.Left)
        {
            int tempX = YPosition - (int)backCount;
            if (tempX < 0)
            {
                tempX = 0;
			}
			
			tb = GameControl.FindPvpTile(xP, yP);
        }
        else if (backDirection == DungeonEnum.FaceDirection.Right)
        {
            int tempX = XPosition + (int)backCount;
            if (tempX > 6)
            {
                tempX = 6;
            }
			
			tb = GameControl.FindPvpTile(xP, yP);
        }
        Debug.Log(tb + " ---- " + name);
        if (tb != null && name.IndexOf("Barrier") != 0)
        {
            XPosition = tb.XPosition;
            YPosition = tb.YPosition;
            SetName();
            AnimationHelper.AnimationMoveTo(tb.transform.localPosition, gameObject, iTween.EaseType.linear, null, null, 0.25f);
        }

    }
	/*
     public List<PvpTile> FindRangeTile(int xPosition,int yPosition)
    {
        List<PvpTile> ranges = new List<PvpTile>();
        for (int i = xPosition; i < xPosition + XRange; i++)
        {
            for (int j = yPosition; j < yPosition + YRange; j++)
            {
                PvpTile pt = GameControl.FindPvpTile(i, j);
                if (pt)
                {
                    ranges.Add(pt);
                }
            }
        }
        return ranges;
    }
*/

    #region 暴击 闪躲 反击

    //反击几率，值随技能增减	
    public int Base_strike = 0;

    /// <summary>
    /// 暴击
    /// </summary>
    public float CritRandom(int round, int step, PvpFightUnit pvpFightUnit)
    {
		if(pvpFightUnit.GetType() != typeof(PvpCharacter)) return 0f;

		long index = pvpFightUnit.PvpUserInfo.SeedUserID * (round + GameControl.PvpTableID) + step;
		float critOdds = ConfigManager.ParamConfig.GetParam ().GlobalCrit + pvpFightUnit.PvpUserInfo.CurCrit;

		int crit = (int)(critOdds / 100f);
        if (Seed(index, crit ))
        {
			//Debug.Log("有没有暴击？");
			float critHurdOdds = ConfigManager.ParamConfig.GetParam().GlobalCritDamageGrowthRate + pvpFightUnit.PvpUserInfo.CurCritHurt;
			//Debug.Log("-> " + critHurdOdds + ":" + (critHurdOdds / 10000f));
			return critHurdOdds / 10000f;
        }
        return 0f;
    }

    /// <summary>
    /// 闪躲
    /// </summary>

    public bool MissRandom(int round, int step, PvpFightUnit pvpFightUnit)
    {
		if(pvpFightUnit.GetType() != typeof(PvpCharacter)) return false;

		long index = pvpFightUnit.PvpUserInfo.SeedUserID / (round + GameControl.PvpTableID) + step;
		float avoidOdds = ConfigManager.ParamConfig.GetParam ().GlobalMiss + pvpFightUnit.PvpUserInfo.CurAvoid;

		//Debug.Log ("闪避几率：" + avoidOdds + ":" + index);

		return Seed(index, (int)(avoidOdds / 100f));
    }

    /// <summary>
    /// 反击
    /// </summary>
    public bool StrickeRandom(int round, int step, int value)
    {
		long index = PvpUserInfo.SeedUserID * (step + 10) + round + GameControl.PvpTableID;
		return Seed(index, (Base_strike +value)/ 100);
    }
    /// <summary>
    /// 眩晕 回合失效
    /// </summary>    
    public bool HaloRandom(int round, int step)
    {
		long index = PvpUserInfo.SeedUserID * (round + 1) * (round + 2) * (GameControl.PvpTableID + 3);
        int instr = (int)this.pvpPlayerBuff.GetValueByBuffTypeToFloat(BuffTypeEnum.Dizziness);
        return Seed(index, (0 +instr)/ 100);
    }  

    ///随机位置（传送） ?
    public Vector3 PositionRandom(int round)
    {
        JsonArray ja = ConfigManager.RandomDataConfig.GetData();

		List<PvpTile> positionList = new List<PvpTile> ();
		for(int i = 0; i < this.GameControl.GameWidth; i ++)
		{
			for(int j = 0; j < this.GameControl.GameHeight; j ++)
			{
				if(!this.GameControl.FindEightGridCheckSelf(new Vector2(i, j), this as PvpCharacter))
				{
					positionList.Add(this.GameControl.FindPvpTile(i, j));
				}
			}
		}

		long dataValue = this.PvpUserInfo.SeedUserID * (round + GameControl.PvpTableID) * (this.XPosition + 10) * (this.YPosition + 5);
		int index = (int)(dataValue % ja.Count);

		PvpTile pvpTile = positionList[int.Parse(ja[index].ToString()) % positionList.Count];
		return new Vector3 (pvpTile.XPosition, pvpTile.YPosition, 0f);
    }
    


    /// <summary>
    /// 自己周围有效位置
    /// </summary>
    public Vector3 AreaRandom(int round, Vector2 position)
    {
        JsonArray ja = ConfigManager.RandomDataConfig.GetData();
        Vector3 v3 = new Vector3(0, 0, 0);
        //自己周围8方砖  怪排除掉
		List<PvpTile> roundTile = GameControl.FindEightGridList(position);        
        
		if (roundTile.Count <= 0) 
		{
			int start_x = this.XPosition + 1;
			int start_y = this.YPosition + 1;

			for (int m = start_x; m < (start_x + this.GameControl.GameWidth); m++) 
			{
				int x1 = m % this.GameControl.GameWidth;
				for (int n = start_y; n < (start_y + this.GameControl.GameHeight); n++) 
				{
					int y1 = n % this.GameControl.GameHeight;
					if(!this.GameControl.FindEightGridCheck(new Vector2(x1, y1)))
					{
						roundTile.Add(this.GameControl.FindPvpTile(x1, y1));
						break;
					}
				}
				
				if (roundTile.Count > 0) 
				{
					break;
				}
			}
		}
		long valueData = long.Parse (ja [(PvpUserInfo.SeedUserID * (round + GameControl.PvpTableID)) % ja.Count].ToString ());
        //随机定位置
		int index = (int)(valueData % roundTile.Count);
        PvpTile p = roundTile[index];
        return new Vector3(p.XPosition, p.YPosition, 0);
    }
   	
	/// <summary>
	/// 技能随机
	/// </summary>
	/// <returns><c>true</c>, if random was skilled, <c>false</c> otherwise.</returns>
	/// <param name="round">Round.</param>
	/// <param name="value">Value.</param>
	public bool SkillRandom(int round, int value) 
	{
		long index = this.PvpUserInfo.SeedUserID % ((round + 1 + GameControl.PvpTableID) * (round + 3));
		return Seed(index, value);
	}

	/// <summary>
	/// 普通攻击随机
	/// </summary>
	/// <returns><c>true</c>, if random was attacked, <c>false</c> otherwise.</returns>
	/// <param name="round">Round.</param>
	/// <param name="step">Step.</param>
	/// <param name="value">Value.</param>
	public bool AttackRandom(int round, int step, int value) 
	{
		long index = (this.PvpUserInfo.SeedUserID / ((round + GameControl.PvpTableID) * (step + 1)));
		return Seed(index, value);
	}

	/// <summary>
	/// 普通攻击随机 2
	/// </summary>
	/// <returns><c>true</c>, if random2 was attacked, <c>false</c> otherwise.</returns>
	/// <param name="round">Round.</param>
	/// <param name="step">Step.</param>
	/// <param name="value">Value.</param>
	public bool AttackRandom2(int round, int step, int value)
	{
		long index = this.PvpUserInfo.SeedUserID * this.PvpUserInfo.SeedUserID / ((round + 6) * (step + GameControl.PvpTableID) + 1);
		return Seed(index, value);
	}

	/// <summary>
	/// 眩晕几率
	/// </summary>
	/// <returns><c>true</c>, if random was dizzinessed, <c>false</c> otherwise.</returns>
	/// <param name="round">Round.</param>
	/// <param name="value">Value.</param>
	public bool DizzinessRandom(int round, int value)
	{
		long index = this.PvpUserInfo.SeedUserID * (round + 1) * (round + 2) * (round + 3);
		return Seed(index, value / 100);
	}

	/// <summary>
	/// 随机召唤
	/// </summary>
	/// <returns><c>true</c>, if random was summoned, <c>false</c> otherwise.</returns>
	/// <param name="round">Round.</param>
	/// <param name="step">Step.</param>
	/// <param name="value">Value.</param>
	public bool SummonRandom(int round, int step, int value, int houseID) 
	{
		long index = this.PvpUserInfo.SeedUserID * (round + GameControl.PvpTableID) * (houseID + 3) * (step + 5);
		return Seed(index, value);
	}

   
    public bool Seed(long index, int data)
    {
        index = index % ConfigManager.RandomDataConfig.GetData().Count;
        int val = int.Parse(ConfigManager.RandomDataConfig.GetData()[(int)index].ToString());
        //Debug.Log("val= " + val);
        if (val <= data) return true;
        return false;
    }



    #endregion
}
