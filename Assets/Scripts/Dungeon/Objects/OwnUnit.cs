using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public interface OwnUnitInterFace
{
    void OwnUnitActionEnd(OwnUnit unit);
}
public class OwnUnit : DungeonUnit
{
    #region 属性
    /// <summary>
    /// 当前回合行动路劲
    /// </summary>
    public List<TileBlock> CurActionPath;

    /// <summary>
    /// 接口指针
    /// </summary>
    public OwnUnitInterFace OwnUnitHandler;

    /// <summary>
    /// 当前行走的步数
    /// </summary>
    public int curMoveStep = 0;

    /// <summary>
    /// 总步数
    /// </summary>
    public int totalStep = 0;

    /// <summary>
    /// 当前步周围的敌人
    /// </summary>
    public List<EnemyUnit> curStepNeighbourEnemies;

    /// <summary>
    /// 当前处理的敌人数量
    /// </summary>
    int curDealEnemyCount = 0;

    /// <summary>
    /// 能量
    /// </summary>
    public int SkillPower;

    /// <summary>
    /// 技能
    /// </summary>
    public SkillData Skill;

    public int CurPower;
    #endregion

    #region 虚方法
    /// <summary>
    /// 单位开始行动
    /// </summary>
    public virtual void OwnBeginAction()
    {
        curMoveStep = 0;
        totalStep = CurActionPath.Count;
        if (this.GetType() == typeof(Player))
        {
            totalStep = CurActionPath.Count - 1;
            CurActionPath.RemoveAt(0);
        }

        if (totalStep > 0)
        {
            OwnUnitOnceAction();
        }
        else
        {
            AllActionEnd();
        }
    }

    /// <summary>
    /// 单位一次行动
    /// </summary>
    public virtual void OwnUnitOnceAction()
    {
        if (GetType() == typeof(Player))
        {
            DungeonScene.RecoverAllPetEnergy();
        }
        TileBlock moveToBlock = CurActionPath[curMoveStep];
        curMoveStep++;
        DungeonEnum.FaceDirection direction = GetTargetDirection(moveToBlock);
        if (XPosition == moveToBlock.XPosition && YPosition == moveToBlock.YPosition)
        {
            direction = GetTargetDirection(DungeonScene.CurPlayer);
        }
        UnitMove(direction, moveToBlock, "UnitMoveEnd",gameObject);
    }

    /// <summary>
    /// 一次移动结束后监测周围行为
    /// </summary>
    public void OnceMoveEndCheckNeighbourEnemy()
    {
        curStepNeighbourEnemies = DungeonScene.FindNeighbourEnemy(this);
        if (curStepNeighbourEnemies.Count > 0)
        {
            UnitBeginDealWithEnemy();
        }
        else
        {
            if (curMoveStep < totalStep)
            {
                OwnUnitOnceAction();
            }
            else
            {
                AllActionEnd();
            }
        }
    }

    /// <summary>
    /// 所有行为结束
    /// </summary>
    public virtual void AllActionEnd()
    {
        DungeonEnum.FaceDirection direction = curRunDirection;
        if (curRunDirection == DungeonEnum.FaceDirection.None)
        {
            direction = curAttackDirection;
        }
        UnitWaiting(direction);
        OwnUnitHandler.OwnUnitActionEnd(this);
    }

    /// <summary>
    /// 单位和敌人交互
    /// </summary>
    public virtual void UnitBeginDealWithEnemy()
    {
        UnitOnceDealWithEnemy();
    }

    /// <summary>
    /// 处理一次敌人
    /// </summary>
    public virtual void UnitOnceDealWithEnemy()
    {
        if (curDealEnemyCount < curStepNeighbourEnemies.Count)
        {
            EnemyUnit eu = curStepNeighbourEnemies[curDealEnemyCount];
            eu.EnemyMutual(this, () =>
            {
                curDealEnemyCount++;
                UnitOnceDealWithEnemy();
            });
        }
        else
        {
            curDealEnemyCount = 0;
            if (curMoveStep < totalStep)
            {
                OwnUnitOnceAction();
            }
            else
            {
                AllActionEnd();
            }
        }
    }

    /// <summary>
    /// 攻击
    /// </summary>
    public virtual void OwnUnitAttack(EnemyUnit target, Action callback)
    {
        curRunDirection = DungeonEnum.FaceDirection.None;
        curAttackDirection = GetTargetDirection(target);
        //延迟受伤
        target.BeHurt(this,true);
        ChangeState(ActionState.Attack, curAttackDirection, () =>
        {
            DungeonScene.AttackCL.SetAttackNum();
            if (target.GetType() == typeof(Monster) || target.GetType() == typeof(Boss))
            {
                callback();
            }
            else
            {
                callback();
            }
        }, false);
    }
    #endregion

    #region 单位行为

    /// <summary>
    /// 移动结束的回调
    /// </summary>
     void UnitMoveEnd()
    {
        SetZorder();
        if (this.GetType() == typeof(Player)&&curMoveStep == totalStep)
        {
            DungeonScene.FindEliminateByPosition(XPosition, YPosition).BlockEliminateRender();
        }
        OnceMoveEndCheckNeighbourEnemy();
    }

    public void SetCurActionPath(List<TileBlock> curPath)
    {
        CurActionPath.Clear();
        foreach (TileBlock b in curPath)
        {
            CurActionPath.Add(b);
        }
    }
    #endregion

    #region 释放技能
    public virtual void UseSkill()
    {
        GameObject skill = Instantiate(Resources.Load(DungeonSpritePathManager.SkillFX(Skill.Id))) as GameObject;
    }

    public void SkillComfirmShow()
    {
        if (!DungeonScene.IsSkilling && DungeonScene.userInputLock == false && CurPower >= Skill.SkillPower)
        {
            DungeonScene.userInputLock = true;
            DungeonScene.CurReadySkillUnit = this;
            DungeonScene.SkillConfirm.gameObject.SetActive(true);
            DungeonScene.SkillConfirm.SetDes(Skill.Description);
            DungeonScene.CaculateSkillRangeTile(DungeonScene.FindTile(XPosition, YPosition), Skill);
            if (Skill.Id == "PSk439")
            {
                DungeonScene.AllRangesTile.Clear();
                DungeonEnum.ElementAttributes att = (DungeonEnum.ElementAttributes)(int.Parse(Skill.Yparameter));
                foreach (EliminateBlock e in DungeonScene.CurAllEliminateBlock)
                {
                    if (e.CurEliminateAttribute == att)
                    {
                        DungeonScene.AllRangesTile.Add(DungeonScene.FindTile(e.XPosition, e.YPosition));
                    }
                }
            }
            foreach (TileBlock t in DungeonScene.FloorTiles)
            {
                EliminateBlock eb = DungeonScene.FindEliminateByPosition(t.XPosition, t.YPosition);
                if (eb)
                {
                    if (DungeonScene.AllRangesTile.Contains(t))
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
    #endregion
}
