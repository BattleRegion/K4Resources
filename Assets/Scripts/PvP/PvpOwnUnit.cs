using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
public class PvpOwnUnit : PvpFightUnit
{
    public List<PvpEliminate> CurActionPath = new List<PvpEliminate>();

    public SkillData ChainSkill;
    public SkillData ChainSkill2;

    public SkillData MainSkill;
    public SkillData MainSkill2;

    #region 方法
    Action ActionEndCallback;
	// 当前步骤
    public int curMoveStep = -1;
	protected List<bool> filterPath;

    public virtual void OwnAction(Action actionEndCallback, List<PvpEliminate> linkPath)
    {
		if(powerEliminateList != null) this.powerEliminateList.Clear();

		if(this.GetType() == typeof(PvpCharacter) || this.GetType() == typeof(PvpMonster))
		{
			if(linkPath != null && linkPath.Count > 0)
			{
				// 设置起点可以走
				this.GameControl.aStarUtils.GetNode(linkPath[0].XPosition, linkPath[0].YPosition).walkable = true;
			}
		}
		if(this.GetType() == typeof(PvpCharacter))
		{
			Debug.Log("Pvp Character ->" + this.XPosition + ":" + this.YPosition + ":" + linkPath.Count);
		}
        CurActionPath.Clear();
		// 筛选路径
		this.filterPath = this.PathFilter (linkPath);
        foreach (PvpEliminate pe in linkPath)
        {
            CurActionPath.Add(pe);
        }
        ActionEndCallback = actionEndCallback;
        curMoveStep = -1;
        OnceStepAction();
    }

	private List<PvpEliminate> GetMovePathList()
	{
		if(this.filterPath == null || this.filterPath.Count == 0) return null;

		List<PvpEliminate> resultList = new List<PvpEliminate> ();
		for(int index = this.curMoveStep + 1; index < this.filterPath.Count; index ++)
		{
			resultList.Add(this.CurActionPath[index]);
			if(this.filterPath[index]) break;
		}

		return resultList;
	}

	private List<PvpEliminate> powerEliminateList = new List<PvpEliminate> ();

    //走一格
	private bool moveEndStatus = false;
    public virtual void OnceStepAction()
    {
		// 重置数据
		this.StrickeStatus = false;
		this.StrickeHurtStatus = false;
		this.StrickeFightUnit = null;
		this.StrikeCallback = null;

		//curMoveStep++;

        if (curMoveStep < CurActionPath.Count - 1)
        {
			List<PvpEliminate> movePathList = this.GetMovePathList ();
			if(movePathList != null && movePathList.Count > 0)
			{
				// 如果只走一步
				if(this.moveEndStatus && this.curMoveStep != -1)
				{
					movePathList.Insert(0, this.CurActionPath[this.curMoveStep]);
					this.curMoveStep --;
				}
				this.moveEndStatus = false;

				PvpMoveManager.Move(movePathList, this, (PvpEliminate pvpEliminate)=>
				{
					this.curMoveStep ++;
					// 设置当前元素为当前消除格
					this.CurElim = pvpEliminate;

					// 移动结束，设置位置
					this.SetPosition(pvpEliminate.XPosition, pvpEliminate.YPosition);

					if (this.GetType() == typeof(PvpCharacter))
					{
						if(this.powerEliminateList.IndexOf(pvpEliminate) == -1)
						{
							this.powerEliminateList.Add(pvpEliminate);
							GameControl.RecoverAllPetEnergy(this);
						}
						// 最后一个格子不消除
						if(curMoveStep < this.CurActionPath.Count - 1)
						{
							pvpEliminate.BlockEliminateRender();
						}
					}
					
				}, ()=>
				{
					this.moveEndStatus = true;
					// 开始攻击 ！
					TryAttack(() =>
					{
						OnceStepAction();
					});
				});
			}
        }
        else
        {
            PvpEliminate.DesCount = 0;
			if(ActionEndCallback != null) ActionEndCallback();
        }
    }

	public List<PvpFightUnit> curFightUnits = new List<PvpFightUnit>();
    public virtual void TryAttack(Action attackEnd)
	{
		//Debug.Log ("PvpOwnUnit 开始攻击 --------------------------------------->");
		curFightUnits = FindEnemies();

		if (curFightUnits.Count == 0)
        {
			//Debug.Log ("PvpOwnUnit 开始攻击 ---------------------------------------> attackEnd");
            attackEnd();
        }
        else
        {
			//Debug.Log ("PvpOwnUnit 开始攻击 ---------------------------------------> attackEnd length " + curFightUnits.Count);
            DungeonEnum.FaceDirection direction = CurFaceDirection;
			if (curFightUnits.Count == 1)
            {
				direction = GetTargetDirection(curFightUnits[0]);
            }
            UnitAttack(direction);
        }
    }

    public override void AttackBump()
    {
		//Debug.Log ("PvpOwnUnit 攻击伤害计算 --------------------------------------->");
		this.attackStatus = true;
		//GameControl.AttackCL.SetAddNum(curFightUnits.Count);
		foreach (PvpFightUnit fu in curFightUnits)
        {
			fu.BeHurt(this);
			// 如果对方闪避，只有 fu 为 PvpCharacter 时， avoidStatus 才会根据情况改变
			if(fu.avoidStatus) this.attackStatus = false;
        }
    }

    public override void AttackEnd()
    {
        base.AttackEnd();

		this.strickeDelayStatus = false;

		//Debug.Log ("PvpOwnUnit 攻击结束 --------------------------------------->");
		// 如果是角色
		if(this.GetType() == typeof(PvpCharacter))
		{
			Debug.Log("PvpCharacter -------------------->" + this.PvpUserInfo.UserId + ":" + this.StrickeStatus + ":" + this.attackStatus);
			// 如果可以攻击，调用效果
			if(this.attackStatus)
			{
				// 遍历处理普通攻击
				// 如果不是反击
				if(!this.StrickeStatus)
				{
					List<PvpFightUnit> targetItemList = null;
					if(this.curFightUnits != null) targetItemList = this.curFightUnits;

					SkillManager.Trigger(SkillTriggerTypeEnum.Attack, curMoveStep, GameControl.CurrentCharacter, GameControl.GetCurrentTargetItem(), targetItemList, (str)=>
					{
						Debug.Log("PvpCharacter --------------------> true ");
						OnceStepAction();
					});
				}else
				{
					Debug.Log("PvpCharacter --------------------> false " + this.StrikeCallback);
					SkillManager.Trigger(SkillTriggerTypeEnum.Attack, curMoveStep, this, this.StrickeFightUnit);
					if(this.StrikeCallback != null) this.StrikeCallback(this.PvpUserInfo.UserId.ToString());
				} 
			}else
			{
				// 如果没有反击
				if(!this.StrickeStatus)
				{
					Debug.Log("PvpCharacter --------------------> strike false ");
					OnceStepAction();
				}else
				{
					Debug.Log("PvpCharacter --------------------> strike true ");
					// 如果是反击状态，调用反击回调函数
					if(this.StrikeCallback != null) this.StrikeCallback(this.PvpUserInfo.UserId.ToString());
				}
			}
		}
		else
		{
			this.OnceStepAction();
		}
    }

    public List<PvpFightUnit> FindEnemies()
    {
		List<PvpFightUnit> fightUnits = new List<PvpFightUnit>();
		foreach (PvpFightUnit peu in GameControl.AllBarrier)
        {
			// 如果是敌方
			if(peu.UserType != GameControl.CurrentCharacter.UserType || peu.GetType() == typeof(PvpEgg))
			{
				if (peu.IsNeighbour(this))
	            {
					// 如果非怪物
					if(this.GetType() != typeof(PvpMonster))
					{
		                if(peu.GetType() == typeof(PvpCharacter) || peu.GetType() == typeof(PvpMonster) || peu.GetType() == typeof(PvpEgg))
		                {
							fightUnits.Add(peu);
		                }
					}else
					{ // 如果是怪物，只攻击主角
						if(peu.GetType() == typeof(PvpCharacter))
						{
							fightUnits.Add(peu);
						}
					}
	            }
			}
        }
		return fightUnits;
    }

	private List<bool> PathFilter(List<PvpEliminate> pathList)
	{
		List<bool> resultList = new List<bool> ();
		for(int index = 0; index < pathList.Count; index ++)
		{
			resultList.Add(this.EnemyCheck(pathList[index].XPosition, pathList[index].YPosition));
		}
		return resultList;
	}

	private bool EnemyCheck(int xPos, int yPos)
	{
		List<PvpFightUnit> fightUnits = new List<PvpFightUnit>();
		foreach (PvpFightUnit peu in GameControl.AllBarrier)
		{
			// 如果是敌方
			if (peu.UserType != this.UserType || peu.GetType() == typeof(PvpEgg))
			{
				if (peu.NeighbourCheck(this, xPos, yPos))
				{
					if (peu.GetType() == typeof(PvpCharacter) || peu.GetType() == typeof(PvpMonster) || peu.GetType() == typeof(PvpEgg))
					{
						fightUnits.Add(peu);
					}
				}
			}
		}
		if(fightUnits.Count == 0) return false;
		return true;
	}

    public List<bool> FindPreFightEnemies(int step, int xPos, int yPos)
    {
		List<bool> dataList = new List<bool> ();

        List<PvpFightUnit> fightUnits = new List<PvpFightUnit>();
        foreach (PvpFightUnit peu in GameControl.AllBarrier)
        {
            // 如果是敌方
            if (peu.UserType != GameControl.CurrentCharacter.UserType || peu.GetType() == typeof(PvpEgg))
            {
				if (peu.NeighbourCheck(this, xPos, yPos))
                {
                    // 主角
                    if (this.GetType() == typeof(PvpCharacter))
                    {
                        if (peu.GetType() == typeof(PvpCharacter) || peu.GetType() == typeof(PvpMonster) || peu.GetType() == typeof(PvpEgg))
                        {
							fightUnits.Add(peu);
                        }
                    }
                   
                }
            }
        }

		if(fightUnits.Count == 0) return dataList;

		foreach(PvpFightUnit peu in fightUnits)
		{
			// 如果闪避了
			dataList.Add(!peu.CritAndAvoidStatusCheck(this, step, -1));
		}

		return dataList;
    }

	public List<PvpEgg> FindPreFightEggs(int step, int xPos, int yPos, bool chainStatus = false)
	{
		List<PvpEgg> dataList = new List<PvpEgg> ();
		
		List<PvpFightUnit> fightUnits = new List<PvpFightUnit>();
		foreach (PvpFightUnit peu in GameControl.AllBarrier)
		{
			// 如果是蛋
			if (peu.GetType() == typeof(PvpEgg))
			{
				if (peu.NeighbourCheck(this, xPos, yPos))
				{
					dataList.Add(peu as PvpEgg);
				}
			}
		}

		if(chainStatus && this.ChainSkill != null)
		{
			List<PvpTile> pvpTileList = GameControl.CaculateSkillRangeTileList(xPos, yPos, this.ChainSkill);
			// 如果达到了终结技能条件
			if (this.ChainSkill.Bparameter < step + 1 && this.HasEggs(pvpTileList))
			{
				foreach (PvpTile t in pvpTileList)
				{
					if (t.XPosition != xPos || t.YPosition != yPos)
					{
						PvpFightUnit e = this.GameControl.FindEnemyOn(t.XPosition, t.YPosition);
						if (e != null && e.GetType() == typeof(PvpEgg)) dataList.Add(e as PvpEgg);
					}
				}
			}
		}
		
		return dataList;
	}
	#endregion
	
	#region 技能
	public void TryPopSkillDetail(UserPet userPet)
	{
		if (MainSkill == null) return;//无主动技能
		
		// 如果技能在 CD 中
		/*if(!this.GameControl.PvpCharacterSelf.pvpPlayerSkill.SkillCdCheck(this.MainSkill.Id, userPet.UserPetId))
		{
			return;
		}*/

		bool powerStatus = CurPower >= MainSkill.SkillPower;
		// 主动状态
		bool initiativeStatus = (this.MainSkill.Type != SkillMagicTypeEnum.Passiveness);
		// CD 状态
		bool cdStatus = this.GameControl.PvpCharacterSelf.pvpPlayerSkill.SkillCdCheck (this.MainSkill.Id, userPet.UserPetId);

        if (GameControl.IsSkilling == false && GameControl.UserInputLock == false)
        {
			// 不能划线！
			GameControl.UserInputLock = true;
            GameControl.CurReadySkillUnit = this;
			GameControl.SkillConfirm.ChangeData(powerStatus, cdStatus, initiativeStatus);
            GameControl.SkillConfirm.gameObject.SetActive(true);
            GameControl.SkillConfirm.SetDes(MainSkill.Description);

			GameControl.CaculateSkillRangeTile(GameControl.FindPvpTile(GameControl.PvpCharacterSelf.XPosition, GameControl.PvpCharacterSelf.YPosition), MainSkill);
            foreach (PvpTile t in GameControl.AllPvpTiles)
            {
                PvpEliminate eb = GameControl.FineEliminate(t.XPosition, t.YPosition);
                if (eb)
                {
                    if (GameControl.AllSkillRangesTile.Contains(t))
                    {
                        eb.SetSkillEnabel(true);
                    }
                    else
                    {
                        eb.SetSkillEnabel(false);
                    }
                }
            }

        }
    }

    /// <summary>
    /// 回复能量
    /// </summary>
    public void RecoverPower()
    {
		if(MainSkill == null) return;

        if (GetType() == typeof(PvpPet))
        {
            PvpPet p = (PvpPet)this;            
            CurPower = GameControl.TotalPower;
			// 如果能量大于零，并且不是被动技能
            if (CurPower >= MainSkill.SkillPower && MainSkill.Type != SkillMagicTypeEnum.Passiveness)
            {                
                GameControl.AvataEffectRender(true, p.CurUserPet);
            }else{
				GameControl.AvataEffectRender(false, p.CurUserPet);
			}

            float pro = (float)CurPower / (float)MainSkill.SkillPower * 1.1f;
            if (pro > 1.1f) pro = 1.1f;
            GameControl.AvataProgress(pro, p.CurUserPet);
        }
    }

	public void ResetPetPowerAvatar()
	{
		PvpPet p = (PvpPet)this;   
		if(p != null) GameControl.ResetAvataEffectRender(false, p.CurUserPet);
	}

    public int CurPower;

   

    //众多技能对应一个动作
    public override void SkillAttackBump()
    {
        //众技能渲染 （不分先后，同时）
        SkillRenderShow();
    }
    //技能结束，Waiting
    public override void SkillAttackEnd()
    {
        //RemoveBack();
		//UnitWaiting(DungeonEnum.FaceDirection.Down);
		this.GameControl.EndUseSkillWithPetFly();
    }
    //技能渲染
    void SkillRenderShow()
    {
		// 创建技能效果
		/*PvpGameObjectManager.Create(DungeonSpritePathManager.SkillBumpFX(MainSkill.FXPrefab1), (GameObject skill)=>
		{
			//GameObject resource = Resources.Load(DungeonSpritePathManager.SkillBumpFX(MainSkill.FXPrefab1)) as GameObject;
			//GameObject skill = null;
			//if (resource)
			//{
			//	skill = Instantiate(resource) as GameObject;
			//} 
		});   */  
		this.GameControl.UseSkill(this.GameControl.CurReadySkillUnit.MainSkill);
    }

    void RemoveBack()
    {
		// 设置技能为 false
		GameControl.IsSkilling = false;
        GameControl.ShowSkillBack(false);	
	
    }

    #endregion
    //终结技
    #region chianSkill
    Action ChainSkillUseAction;
    GameObject skillCareObject;
    public void ChainSkillUse(Action chainSkillUse)
    {
        IsChain = false;
        ChainSkillUseAction = chainSkillUse;
        if (ChainSkill != null && ChainSkill.Bparameter < CurActionPath.Count && HasEnemies())
        {
            //Debug.Log(ChainSkill.SkillFX);
            //暂时只有主角
			PvpGameObjectManager.Create("PreFabs/FX/Skill_0", (GameObject objectItem)=>
			{
				//skillCareObject = Instantiate(Resources.Load("PreFabs/FX/Skill_0")) as GameObject;
				skillCareObject = objectItem;
				skillCareObject.transform.position = transform.position;
				Invoke("PowerGetEnd", 1.2f);
				IsChain = true;
			});
        }
        else
        {
            ChainSkillUseAction();
        }
    }

    bool HasEnemies()
    {
        if (ChainSkill.SkillFX == "SkFX0")
        {
            //远程终结技,目标为最近数个敌人     
            return true;             
        }
        else if (ChainSkill.SkillFX == "SkFX1")
        {
            //普通终结技
            foreach (PvpTile tb in GameControl.AllSkillRangesTile)
            {
                if (GameControl.HasNoRunEnemyOnPosition(this, tb.XPosition, tb.YPosition) == true)
                {
                    return true;
                }
            }
        }        
        return false;
    }

	bool HasEggs(List<PvpTile> pvpTileList)
	{
		if (this.ChainSkill.SkillFX == "SkFX0")
		{
			//远程终结技,目标为最近数个敌人     
			return true;             
		}
		else if (this.ChainSkill.SkillFX == "SkFX1")
		{
			//普通终结技
			foreach (PvpTile tb in pvpTileList)
			{
				if (GameControl.HasNoRunEnemyOnPosition(this, tb.XPosition, tb.YPosition) == true)
				{
					return true;
				}
			}
		}        
		return false;
	}

    void PowerGetEnd()
    {
        Destroy(skillCareObject);
        UnitChainAttack();
    }

	public List<PvpFightUnit> FarChainEnmeyList;

    public override void ChainAttackBump()
    {
        HardWareData.HardWareType ht = GameControl.CurrentCharacter.PvpUserInfo.CurWeapon.CurHardWareData.Style;
        if (ht == HardWareData.HardWareType.Heavy || ht == HardWareData.HardWareType.Light)
        {
			int skillType = SkillEffectTypeEnum.NONE;
			string skillPrefab = this.GetChainSkillType (ref skillType);
			// 调用终结技表现效果
			this.GameControl.PowerSkillRenderBumpAttack(skillType, skillPrefab, this as PvpCharacter, true);
        }
        else if (ht == HardWareData.HardWareType.Far1 || ht == HardWareData.HardWareType.Far2)
        {
           //远程终结技
			FarChainEnmeyList = GetFarChainSkillAim();
            int index = 0;

			bool status = false;

			foreach (PvpFightUnit t in FarChainEnmeyList)
            {
                if (t.XPosition == XPosition && t.YPosition == YPosition)
                {
                }
                else
                {
					// 这儿要添加数量，否则会有问题
					if(status)
					{
						this.GameControl.actionOwnUnitCount ++;
					}else{
						status = true;
					}

					PvpTile pt = GameControl.FindPvpTile(t.XPosition, t.YPosition);
					this.StartCoroutine(PowerSkillRenderBumpOther(pt, t, true, index, (PvpFightUnit unitItem)=>
					{
						// 如果没有闪避
						if(!unitItem.CritAndAvoidStatusCheck(this, this.curMoveStep, index))
						{
							// 调用普通攻击表现
							SkillManager.Trigger(SkillTriggerTypeEnum.Attack, this.curMoveStep, this, unitItem);
						}
						this.BumpEnd();
					}));
                    index++;
                }
            }
        }
        else
        {
            foreach (PvpTile t in GameControl.AllSkillRangesTile)
            {
                if (GameControl.FindEnemyOn(t.XPosition, t.YPosition))
                {
					StartCoroutine(PowerSkillRenderBumpOther(t, null, false));
                    Invoke("BumpEnd", 0.5f);
                }
            }
        }
    }

    void BumpEnd()
    {
        ChainSkillUseAction();
    }

	public List<PvpEgg> GetFarChainSkillAimEgg(int xPos, int yPos)
	{
		List<PvpFightUnit> fightUnitList = this.GetFarChainSkillAim (xPos, yPos);

		List<PvpEgg> dataList = new List<PvpEgg> ();
		foreach(PvpFightUnit fightUnit in fightUnitList)
		{
			if(fightUnit.GetType() == typeof(PvpEgg))
			{
				dataList.Add(fightUnit as PvpEgg);
			}
		}

		return dataList;
	}

    public List<PvpFightUnit> GetFarChainSkillAim(int xPos = -1, int yPos = -1)
    {
		List<PvpFightUnit>  enlist;
		enlist = new List<PvpFightUnit>(GameControl.AllBarrier.ToArray());

        List<PvpFightUnit> enlist_no = new List<PvpFightUnit>();
        foreach (PvpFightUnit en in enlist)
        {
			// 如果是敌方，并且没有在隐形状态
			if(en.UserType != this.UserType || en.GetType() == typeof(PvpEgg))
			{
                
				if(xPos != -1 && yPos != -1)
				{
					en.DisFromeChar = Distance(xPos, yPos, en);
				}else
				{
           			en.DisFromeChar = Distance(this, en);
				}
                enlist_no.Add(en);
			}
        }

        enlist_no = enlist_no.OrderBy(item => Math.Abs(item.DisFromeChar)).ToList();

        int skillX = int.Parse(ChainSkill.Xparameter);
        // Debug.Log(enlist.Count + "  farChainskill " + skillX);
        if (enlist_no.Count > skillX)
        {
            int c = enlist_no.Count;
            for (int k = skillX; k < c; k++)
            {
                Debug.Log("--- " + k);
                enlist_no.RemoveAt(enlist_no.Count - 1);
            }
        }
        else
        {
            //目标数量 < 攻击次数   补足 
            int c = enlist_no.Count;
            for (int k = c; k < skillX; k++)
            {
                int index = k % c;
                enlist_no.Add(enlist_no[index]);
            }
        }

        //Debug.Log(enlist_no.Count + "  farChainskill " + skillX);

        return enlist_no;            
    }

    /// <summary>
    /// 逐个延迟渲染
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    IEnumerator PowerSkillRenderBumpOther(PvpTile t, PvpFightUnit fightUnit, bool farChain, int index = -1, Action<PvpFightUnit> callback = null)
    {
		if (farChain)
        {
			//yield return new WaitForSeconds(Distance(this, t) * 0.15f * index); //Distance(this, t) * 0.25f
			yield return new WaitForSeconds(0.5f * index); //Distance(this, t) * 0.25f
        }
        else
        {
			yield return new WaitForSeconds(0.25f);
        }
		PvpFightUnit e = fightUnit;
		if(e == null) e = GameControl.FindEnemyOn(t.XPosition, t.YPosition);
        if (e)
        {
			string skillPrefab = "";
			if(ChainSkill.FXType1 != SkillEffectTypeEnum.FIXED)
			{
				skillPrefab = ChainSkill.FXPrefab1;
			}
			else if(ChainSkill.FXType2 != SkillEffectTypeEnum.FIXED)
			{
				skillPrefab = ChainSkill.FXPrefab2;
			}
			else if(ChainSkill.FXType3 != SkillEffectTypeEnum.FIXED)
			{
				skillPrefab = ChainSkill.FXPrefab3;
			}

			if(!string.IsNullOrEmpty(skillPrefab))
			{
				// 创建预设
				PvpGameObjectManager.Create(DungeonSpritePathManager.SkillBumpFX(skillPrefab), (GameObject skillBump)=>
				{
					//GameObject skillResource = Resources.Load(DungeonSpritePathManager.SkillBumpFX(skillPrefab)) as GameObject;
					//GameObject skillBump = Instantiate(skillResource) as GameObject;
					skillBump.transform.position = t.transform.position;
					skillBump.SetActive(true);
					e.SkillHurtRender(ChainSkill,this,index);
					
					if(!farChain)
					{
						CameraControl.ShakeCamera();
					}
					// 调用回调
					if(callback != null) callback(fightUnit);
				});
			}
			else
			{
				// 调用回调
				if(callback != null) callback(fightUnit);
			}
        }else
		{
			// 调用回调
			if(callback != null) callback(fightUnit);
		}
    }

	private string GetChainSkillType(ref int skillType)
	{
		skillType = SkillEffectTypeEnum.NONE;

		if(this.ChainSkill == null) return "";

		if(this.ChainSkill.FXType1 == SkillEffectTypeEnum.ITEM || this.ChainSkill.FXType1 == SkillEffectTypeEnum.SELF_SINGLE)
		{
			skillType = this.ChainSkill.FXType1;
			return this.ChainSkill.FXPrefab1;
		}
		if(this.ChainSkill.FXType2 == SkillEffectTypeEnum.ITEM || this.ChainSkill.FXType2 == SkillEffectTypeEnum.SELF_SINGLE)
		{
			skillType = this.ChainSkill.FXType2;
			return this.ChainSkill.FXPrefab2;
		}
		if(this.ChainSkill.FXType3 == SkillEffectTypeEnum.ITEM || this.ChainSkill.FXType3 == SkillEffectTypeEnum.SELF_SINGLE)
		{
			skillType = this.ChainSkill.FXType3;
			return this.ChainSkill.FXPrefab3;
		}

		return "";
	}

    public override void ChainAttackEnd()
    {
        IsChain = false;
        UnitWaiting(CurFaceDirection);
    }
    #endregion
}
