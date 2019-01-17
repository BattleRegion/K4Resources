using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
public class PveOwnUnit : PveFightUnit
{
    public List<PveEliminate> CurActionPath = new List<PveEliminate>();

    public SkillData ChainSkill;
    public SkillData ChainSkill2;

    public SkillData MainSkill;
    public SkillData MainSkill2;

    

    #region 方法
    Action ActionEndCallback;
    int curMoveStep = -1;
    public virtual void OwnAction(Action actionEndCallback,List<PveEliminate> linkPath)
    {
        bool first = true;
        CurActionPath.Clear();
        foreach (PveEliminate pe in linkPath)
        {
            CurActionPath.Add(pe);
            if (first == false)
            {
                if (GameControl.TryTrap(pe.XPosition, pe.YPosition))
                {
                    break;
                }
            }
            first = false;
        }
        ActionEndCallback = actionEndCallback;
        curMoveStep = -1;
        OnceStepAction();
    }

    //走一格
    public void OnceStepAction()
    {
        curMoveStep++;
        if (curMoveStep < CurActionPath.Count)
        {
            if (GetType() == typeof(PveCharacter))
            {
				// 设置格子数
				if(this.GameControl != null && this.GameControl.PvePlayerInfo != null) this.GameControl.PvePlayerInfo.SetGridValue(1);
                GameControl.RecoverAllPetEnergy();
            }
            PveEliminate p = CurActionPath[curMoveStep];
            CurElim = p;
            DungeonEnum.FaceDirection direction = GetTargetDirection(p);
            MoveSpeed = 360;
            UnitMove(p.XPosition, p.YPosition, () =>
            {
                if (GetType() == typeof(PveCharacter))
                {
					//最后一个格子不消除
					if(curMoveStep < this.CurActionPath.Count - 1)
					{
                    	p.BlockEliminateRender();
					}
                }
                TryAttack(() =>
                {
                    //BOSS反击
                    PveGameControl.CurSkillTime = BossSkillController.TriggerType.UnderAttack;
                    if(GetType() == typeof(PveCharacter))
                    {
                        bool bossExist = false;
                        PveBoss CurBoss = null;
                        curEnemies = FindEnemies();
                        foreach(PveEnemyUnit e in curEnemies)
                        {
                            if (e.GetType() == typeof(PveBoss))
                            {
                                bossExist = true;
                                CurBoss = (PveBoss)e;
                                break;
                            }
                        }
                        if(bossExist)
                        {
                            if (CurBoss.ReadyForBossSkill())
                            {
                                CurBoss.UseSkill(null, () => 
                                {
                                    OnceStepAction();
                                });
                            }
                            else
                            {
                                OnceStepAction();
                            }
                        }
                        else
                        {
                            OnceStepAction();
                        }
                    }
                    //
                    else OnceStepAction();
                });
            }, direction);
        }
        else
        {
            PveEliminate.DesCount = 0;
            ActionEndCallback();
        }
    }

    Action AttackActionEnd;
    public List<PveEnemyUnit> curEnemies = new List<PveEnemyUnit>();
    public virtual void TryAttack(Action attackEnd)
    {
        curEnemies = FindEnemies();
        if (curEnemies.Count == 0)
        {
            attackEnd();
        }
        else
        {
            AttackActionEnd = attackEnd;
            DungeonEnum.FaceDirection direction = CurFaceDirection;
            if (curEnemies.Count == 1)
            {
                direction = GetTargetDirection(curEnemies[0]);
            }
            UnitAttack(direction);
        }
    }

    public override void AttackBump()
    {
        GameControl.AttackCL.SetAddNum(curEnemies.Count);
        foreach (PveFightUnit fu in curEnemies)
        {
            fu.BeHurt(this);
        }
    }

    public override void AttackEnd()
    {
        base.AttackEnd();
        // 如果是角色
        if (this.GetType() == typeof(PveCharacter))
        {
            // 遍历处理普通攻击
            // 如果不是反击
            if (!this.StrickeStatus)
            {
                List<PveEnemyUnit> targetItemList = null;
                if (this.curEnemies != null && this.curEnemies.Count > 0) targetItemList = this.curEnemies;

                PveSkillManager.Trigger(SkillTriggerTypeEnum.Attack, GameControl.CurCharacter, targetItemList, (str) =>
                {
                    OnceStepAction();
                    //AttackActionEnd();
                });
            }
            else
            {
                PveSkillManager.Trigger(SkillTriggerTypeEnum.Attack, this, null, null);
                if (this.StrikeCallback != null) this.StrikeCallback(UserManager.pveUserInfo.UserId.ToString());
            }

            //boss技能模块
            PveGameControl.CurSkillTime = BossSkillController.TriggerType.Player_Attack;
            PveBoss CurBoss = null;
            foreach (PveEnemyUnit e in GameControl.AllEnemies)
            {
                if (e.GetType() == typeof(PveBoss))
                {
                    CurBoss = (PveBoss)e;
                    break;
                }
            }
            if(CurBoss != null)
            {
                if(CurBoss.ReadyForBossSkill())
                {
                    CurBoss.UseSkill(null, null);
                }
            }

        }
        else
        {
            OnceStepAction();
            //AttackActionEnd();
        }
    }


    public List<PveEnemyUnit> FindEnemies()
    {
        List<PveEnemyUnit> enemies = new List<PveEnemyUnit>();
        foreach (PveEnemyUnit peu in GameControl.AllEnemies)
        {
            if (peu.CurState == UnitState.guard) continue;
            if (peu.IsNeighbour(this))
            {
                if(peu.GetType() == typeof(PveBoss))
                {
                    enemies.Add(peu);
                }
                else if (peu.GetType() == typeof(PveMonster))
                {
                    PveMonster pm = (PveMonster)peu;
                    //if (pm.CurMonsterType != MonsterData.MonsterType.RunAway)
                    //{
                        enemies.Add(peu);
                    //}
                }
                else if(peu.GetType() == typeof(PveCannonTrigger))
                {
                    enemies.Add(peu);
                }
            }
           
        }
        return enemies;
    }
    #endregion

    #region 技能
    public void TryPopSkillDetail(UserPet userPet)
    {
        if (MainSkill == null) return;

		bool powerStatus = CurPower >= MainSkill.SkillPower;
		bool cdStatus = this.GameControl.CurCharacter.pvpPlayerSkill.SkillCdCheck (this.MainSkill.Id, userPet.UserPetId);
        // 主动状态
        bool initiativeStatus = (this.MainSkill.Type != SkillMagicTypeEnum.Passiveness);

        if (GameControl.IsSkilling == false  && GameControl.UserInputLock==false)
        {
            // 不能划线！
            GameControl.UserInputLock = true;
            GameControl.CurReadySkillUnit = this;
			GameControl.SkillConfirm.ChangeData(powerStatus, cdStatus, initiativeStatus, MainSkill.OkPve == 1);
            GameControl.CurCharacter.ShowArrow(false);
            GameControl.SkillConfirm.gameObject.SetActive(true);
            GameControl.SkillConfirm.SetDes(MainSkill.Description);

            GameControl.CaculateSkillRangeTile(GameControl.FindPveTile(XPosition, YPosition), MainSkill);
            if (MainSkill.SkillFX == "SkFX2")
            {
                //转化
                GameControl.AllSkillRangesTile.Clear();
                DungeonEnum.ElementAttributes att = (DungeonEnum.ElementAttributes)(int.Parse(MainSkill.Yparameter));
                foreach (PveEliminate e in GameControl.AllEliminates)
                {
                    if (e.CurEliminateAttribute == att)
                    {
                        GameControl.AllSkillRangesTile.Add(GameControl.FindPveTile(e.XPosition, e.YPosition));
                    }
                }
            }

            foreach (PveTile t in GameControl.AllPveTiles)
            {
                PveEliminate eb = GameControl.FineEliminate(t.XPosition, t.YPosition);
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
        if (GetType() == typeof(PvePet))
        {
            if (MainSkill == null)
            {
                return ;
            }

            PvePet p = (PvePet)this;            
            CurPower = GameControl.TotalPower;
            if (CurPower >= MainSkill.SkillPower && MainSkill.Type==1)
            {
                GameControl.PvePlayerInfo.AvataEffectRender(true, p.CurUserPet);
            }
            else
            {
                GameControl.PvePlayerInfo.AvataEffectRender(false, p.CurUserPet);
            }
          
        }
    }
    public void ResetPetPowerAvatar()
    {
        PvePet p = (PvePet)this;
        if (p != null) GameControl.PvePlayerInfo.AvataEffectRender(false, p.CurUserPet);
    }

    //宠物使用技能
    public int CurPower;
   
    public override void SkillAttackBump()
    {
        SkillRenderShow();
    }

    public override void SkillAttackEnd()
    {
        //RemoveBack();
        //UnitWaiting(CurFaceDirection);
        GameControl.EndUseSkillWithPetFly();
    }
    public  void SkillAttackEnd_sys()
    {
        RemoveBack();
        UnitWaiting(CurFaceDirection);
    }
    //技能动画
    void SkillRenderShow()
    {       
        //GameObject resource = Resources.Load(DungeonSpritePathManager.SkillBumpFX(MainSkill.FXPrefab1)) as GameObject;
        //GameObject skill = null;
        //if (resource)
        //{
        //    skill = Instantiate(resource) as GameObject;
        //}
        GameControl.UseSkill(GameControl.CurReadySkillUnit.MainSkill);
    }

    IEnumerator ChangeBlock(PveEliminate b)
    {
        yield return new WaitForSeconds(0.2f);
        if (b)
        {
            Debug.Log(MainSkill.Aparameter + "  "+MainSkill.Id);
            b.CurEliminateAttribute = (DungeonEnum.ElementAttributes)((int)(MainSkill.Aparameter));
            b.ReturnToCommon();
        }
    }

    //技能后怪物状态处理
    void RemoveBack()
    {
        GameControl.CurCharacter.ShowArrow(true);
        GameControl.ShowSkillBack(false);

        List<PveEnemyUnit> allDead = new List<PveEnemyUnit>();
        foreach (PveEnemyUnit pe in GameControl.AllEnemies)
        {
            if (pe.CurHp <= 0)
            {
                allDead.Add(pe);
            }
        }
        if (allDead.Count == 0)
        {
            GameControl.UserInputLock = false;
            GameControl.IsSkilling = false;
        }
        else
        {
            int curHasDeadCount = 0;
            foreach (PveEnemyUnit pe in allDead)
            {
                if (pe.CurHp <= 0)
                {
                    
                    pe.UnitDead((p) =>
                    {
                        GameObject disObject = Instantiate(Resources.Load("PreFabs/FX/Monster_D")) as GameObject;
                        disObject.transform.position = p.transform.position;
                      
                        GameControl.EnemyUnitEliminateEnable(true);//2015-04-20
                        GameControl.AllEnemies.Remove((PveEnemyUnit)p);
                        Destroy(p.gameObject);

                        

                        curHasDeadCount++;
						int count = 0;
						foreach (PveEnemyUnit pu in GameControl.AllEnemies)
						{
							if(pu.GetType()!=typeof(PveBarrier))
							{
								count++;
							}
						}
						if (count!=0)
                        {
                            if (curHasDeadCount == allDead.Count)
                            {
                                GameControl.UserInputLock = false;
                                GameControl.IsSkilling = false;
                            }
                        }
                        else
                        {
                            if (GameControl.CurFloor < ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId).FloorCount)
                            {
                                GameControl.Door.OpenDoor();
                                GameObject doorOpen = Instantiate(Resources.Load("PreFabs/FX/KEY_use")) as GameObject;
                                doorOpen.transform.position = new Vector3(0, 0.75f, 0);
                                doorOpen.SetActive(true);
                                PveTile endTile = GameControl.FindPveTile(3, 8);
                                GameControl.CurCharacter.UnitMove(endTile.XPosition, endTile.YPosition, () =>
                                {
                                    GameControl.AttackCL.Out();
                                    GameControl.CurCharacter.ShowArrow(false);
                                    GameControl.IsSkilling = false;
                                    GameControl.BeginFloorLoad();
                                }, DungeonEnum.FaceDirection.Up);
                            }
                            else
                            {                                
                                GameControl.RenderSuccess();
                            }
                        }
                    });
                }
            }
        }
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
        PveTrap hasTrapcurEli =GameControl.TryTrap(XPosition, YPosition);
        

        if (ChainSkill != null && ChainSkill.Bparameter < CurActionPath.Count && HasEnemies() && hasTrapcurEli==null)
        {
            Debug.Log(ChainSkill.SkillFX);
            //暂时只有主角
            skillCareObject = Instantiate(Resources.Load("PreFabs/FX/Skill_0")) as GameObject;
            skillCareObject.transform.position = transform.position;
            Invoke("PowerGetEnd", 1.2f);
            IsChain = true;
        }
        else
        {
            ChainSkillUseAction();
        }
    }
    



    private string GetChainSkillType(ref int skillType)
    {
        skillType = SkillEffectTypeEnum.NONE;

        if (this.ChainSkill == null) return "";

        if (this.ChainSkill.FXType1 == SkillEffectTypeEnum.ITEM || this.ChainSkill.FXType1 == SkillEffectTypeEnum.SELF_SINGLE)
        {
            skillType = this.ChainSkill.FXType1;
            return this.ChainSkill.FXPrefab1;
        }
        if (this.ChainSkill.FXType2 == SkillEffectTypeEnum.ITEM || this.ChainSkill.FXType2 == SkillEffectTypeEnum.SELF_SINGLE)
        {
            skillType = this.ChainSkill.FXType2;
            return this.ChainSkill.FXPrefab2;
        }
        if (this.ChainSkill.FXType3 == SkillEffectTypeEnum.ITEM || this.ChainSkill.FXType3 == SkillEffectTypeEnum.SELF_SINGLE)
        {
            skillType = this.ChainSkill.FXType3;
            return this.ChainSkill.FXPrefab3;
        }

        return "";
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
            foreach (PveTile tb in GameControl.AllSkillRangesTile)
            {
                if (GameControl.HasNoRunEnemyOnPosition(tb.XPosition, tb.YPosition, null) == true)
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

    public override void ChainAttackBump()
    {
        HardWareData.HardWareType ht=UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style;
        if (ht == HardWareData.HardWareType.Heavy || ht == HardWareData.HardWareType.Light)
        {
            int skillType = SkillEffectTypeEnum.NONE;
            string skillPrefab = this.GetChainSkillType(ref skillType);
            // 调用终结技表现效果
            GameControl.PowerSkillRenderBumpAttack(skillType, skillPrefab, this as PveCharacter, true);
            //this.PowerSkillRenderBumpAttack();          
        }
        else if (ht == HardWareData.HardWareType.Far1 || ht == HardWareData.HardWareType.Far2)
        {
           //远程终结技
            List<PveEnemyUnit>  enlist= GetFarChainSkillAim();
            int num=1;

			bool status = false;

            foreach (PveEnemyUnit t in enlist)
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

                    PveTile pt = GameControl.FindPveTile(t.XPosition, t.YPosition);
                    StartCoroutine(PowerSkillRenderBump(pt, false,num));
                    num++;
                    Invoke("BumpEnd", 1);
                }
            }

        }
        else
        {
            foreach (PveTile t in GameControl.AllSkillRangesTile)
            {
                if (GameControl.FindEnemyOn(t.XPosition, t.YPosition))
                {
                    StartCoroutine(PowerSkillRenderBump(t, false));
                    Invoke("BumpEnd", 0.5f);
                }
            }
        }
    }

    void BumpEnd()
    {
        ChainSkillUseAction();
    }
    List<PveEnemyUnit> GetFarChainSkillAim()
    {
        List<PveEnemyUnit> enlist = new List<PveEnemyUnit>();
        foreach(PveEnemyUnit peu in GameControl.AllEnemies)
        {
            Debug.Log(peu.name + "    " + peu.CurState +"   " );
            if (peu.CurState != UnitState.guard && peu.name.IndexOf("Barrier") == -1)
            {
                enlist.Add(peu);
            }
        }

        foreach (PveEnemyUnit en in enlist)
        {
           en.DisFromeChar= Distance(this, en);          
        }

		enlist=enlist.OrderBy(item=>Math.Abs(item.DisFromeChar)).ToList();

        int skillX = int.Parse(ChainSkill.Xparameter);
        //Debug.Log(enlist.Count + "  farChainskill " + skillX);
        if (enlist.Count > skillX)
        {
            int c = enlist.Count;
            for (int k = skillX; k < c; k++)
            {
                //Debug.Log("--- "+k);
                enlist.RemoveAt(enlist.Count - 1);
            }
        }
        else
        {
            //目标数量 < 攻击次数   补足 
            int c = enlist.Count;
            for (int k = c; k < skillX; k++)
            {
                int index=k % c;
                enlist.Add(enlist[index]);
            }
        }

        Debug.Log(enlist.Count + "  farChainskill " + skillX);

        return enlist;            
    }
    /// <summary>
    /// 逐个延迟渲染
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    IEnumerator PowerSkillRenderBump(PveTile t,bool needdelay,int num=1)
    {
        if (needdelay)
        {
            yield return new WaitForSeconds(Distance(this, t) * 0.25f);
        }
        else
        {
            yield return new WaitForSeconds(0.25f*num);
        }

        GameObject skillResource = Resources.Load(DungeonSpritePathManager.SkillBumpFX(ChainSkill.FXPrefab1)) as GameObject;
        if (ChainSkill.FXType1 == 5)
        {
            //逐格
            if (skillResource != null)
            {
                GameObject skillBump = Instantiate(skillResource) as GameObject;
                skillBump.transform.position = t.transform.position;
                skillBump.SetActive(true);
            }        
        }       
        PveEnemyUnit e = GameControl.FindEnemyOn(t.XPosition, t.YPosition);
        //Debug.Log(e.name);
        if (e)
        {
            string skillPrefab = "";
            if (ChainSkill.FXType1 != SkillEffectTypeEnum.FIXED)
            {
                skillPrefab = ChainSkill.FXPrefab1;
            }
            else if (ChainSkill.FXType2 != SkillEffectTypeEnum.FIXED)
            {
                skillPrefab = ChainSkill.FXPrefab2;
            }
            else if (ChainSkill.FXType3 != SkillEffectTypeEnum.FIXED)
            {
                skillPrefab = ChainSkill.FXPrefab3;
            }
            if (!string.IsNullOrEmpty(skillPrefab))
            {
                GameObject skillresource = Resources.Load(DungeonSpritePathManager.SkillBumpFX(skillPrefab)) as GameObject;
                GameObject skillBump = Instantiate(skillresource) as GameObject;
                skillBump.transform.position = t.transform.position;
                skillBump.SetActive(true);
                CameraControl.ShakeCamera();
                e.SkillHurtRender(ChainSkill);                
            }
            //CameraControl.ShakeCamera();
        }
    }

    public override void ChainAttackEnd()
    {
        IsChain = false;
        UnitWaiting(CurFaceDirection);
    }
    #endregion
}
