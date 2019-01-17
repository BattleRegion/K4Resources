using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Monster : EnemyUnit
{
    #region 属性
    public int Range;

    public int RunPower;

    public bool HasAttackInOwnRound = false;
    #endregion

    #region 资源指针
    public string FarAttackFlyFX;

    public string FarAttackBumpFX;
    #endregion

    #region 重写父类
    public override void SetSkin(SkinConfigData skinData)
    {
        base.SetSkin(skinData);
        FarAttackFlyFX = skinData.FlyFXName;
        FarAttackBumpFX = skinData.FireFXName;
    }

    public override void SetObjectName()
    {
        name = "Monster:" + XPosition + "," + YPosition; 
    }

    /// <summary>
    /// 交互重写
    /// </summary>
    /// <param name="own"></param>
    public override void EnemyMutual(OwnUnit own, Action MutalEnd)
    {
        //攻击
        own.OwnUnitAttack(this, () =>
        {
            MutalEnd();
        });
    }

    /// <summary>
    /// 回合行为
    /// </summary>
    public override void RoundAction()
    {
        if (PlayerInRange())
        {
            MonsterAttack(DungeonScene.CurPlayer, () =>
            {
                base.RoundAction();
            });
        }
        else
        {
            if (RunPower > 0)
            {
                CaculateActionPath();
                if (path.Count > 0)
                {
                    UnitMove(GetTargetDirection(path[moveStep]), path[moveStep], "RoundMoveEnd", gameObject);
                    moveStep++;
                }
                else
                {
                    base.RoundAction();
                }
            }
            else
            {
                base.RoundAction();
            }
        }
    }

    #region SLG路径计算
    public List<TileBlock> path = new List<TileBlock>();
    int curCaculateCount = 0;
    void CaculateActionPath()
    {
        path.Clear();
        curCaculateCount = 0;
        moveStep = 0;
        //获取单位周围的BLOCK
        List<TileBlock> neighbourBlocks= new List<TileBlock>();
        foreach (TileBlock b in PositionTiles)
        {
            List<TileBlock> ns = DungeonScene.FindNeighbourTileIn(b, DungeonScene.FloorTiles);
            foreach (TileBlock nb in ns)
            {
                if (!neighbourBlocks.Contains(nb)&&!PositionTiles.Contains(nb))
                {
                    neighbourBlocks.Add(nb);
                }
            }
        }
        GetOnceStep(neighbourBlocks);
    }

    void GetOnceStep(List<TileBlock> blocks)
    {
        //按距离排序冒泡
        for (int i = 0; i < blocks.Count; i++)
        {
            for (int j = 0; j < blocks.Count - i - 1; j++)
            {
                TileBlock tb = blocks[j];
                TileBlock tb1 = blocks[j + 1];
                if(tb.Distance(tb,DungeonScene.CurPlayer)>tb1.Distance(tb1,DungeonScene.CurPlayer))
                {
                    TileBlock temp = blocks[j];
                    blocks[j] = blocks[j+1];
                    blocks[j + 1] = temp;
                }
            }
        }

        TileBlock curTb = null;
        for (int i = 0; i < blocks.Count; i++)
        {
            TileBlock tb = blocks[i];
            if (!InExistPath(tb) && canIn(tb) && tb != DungeonScene.FindTile(DungeonScene.CurPlayer.XPosition, DungeonScene.CurPlayer.YPosition))
            {
                curTb = tb; break;
            }
        }
        if(curTb!=null)
        {
            path.Add(curTb);
            curCaculateCount++;
            if (curCaculateCount < RunPower)
            {
                List<TileBlock> neighbours = DungeonScene.FindNeighbourTileIn(curTb, DungeonScene.FloorTiles);
                GetOnceStep(neighbours);
            }
        }
    }
    #endregion

    int moveStep = 0;
    void CaculatePath()
    {
        //(1)计算当前可行步中离目标最近的点
        curCaculateCount = RunPower;
        moveStep = 0;
        TileBlock t = DungeonScene.FindTile(XPosition, YPosition);
        GetCloseMoveToTile(t);
    }

    bool InExistPath(TileBlock tb)
    {
        foreach (EnemyUnit eu in DungeonScene.AllEnemyUnits)
        {
            if (eu.GetType() == typeof(Monster) || eu.GetType() == typeof(Boss))
            {
                Monster m = (Monster)eu;
                if (m.path.Contains(tb))
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool canIn(TileBlock tb)
    {
        List<TileBlock> rangBlocks = new List<TileBlock>();
        for (int i = tb.XPosition; i < tb.XPosition + XRange; i++)
        {
            for (int j = tb.YPosition; j < tb.YPosition + YRange; j++)
            {
                TileBlock t = DungeonScene.FindTile(i, j);
                if (t)
                {
                    rangBlocks.Add(t);
                }
            }
        }
        foreach (EnemyUnit eu in DungeonScene.AllEnemyUnits)
        {
            if (eu != this)
            {
                foreach (TileBlock t in rangBlocks)
                {
                    if (eu.XPosition == t.XPosition && eu.YPosition == t.YPosition)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    void GetCloseMoveToTile(TileBlock f)
    {
        curCaculateCount--;
        TileBlock closeTb = null;
        float closeDis = 99999;
        List<TileBlock> neighbour = DungeonScene.FindNeighbourTileIn(f, DungeonScene.FloorTiles);
        foreach (TileBlock tb in neighbour)
        {
            if (DungeonScene.FindEnemyUnit(tb.XPosition, tb.YPosition) == null && !InExistPath(tb)&&canIn(tb))
            {
                float curDis = Distance(tb, DungeonScene.CurPlayer);
                if (curDis < closeDis)
                {
                    closeDis = curDis;
                    closeTb = tb;
                }
            }
        }
        if (closeTb != null)
        {
            path.Add(closeTb);
        }
        if (closeTb.isNeighbour(DungeonScene.CurPlayer) || curCaculateCount == 0)
        {
            if (path.Count > 0)
            {
                UnitMove(GetTargetDirection(path[moveStep]), path[moveStep], "RoundMoveEnd", gameObject);
                moveStep++;
            }
            else
            {
                base.RoundAction();
            }
        }
        else
        {
            GetCloseMoveToTile(closeTb);
        }
    }

    void RoundMoveEnd()
    {
        SetZorder();
        ResertPosition();
        if (moveStep == path.Count)
        {
            path.Clear();
            if (isNeighbour(DungeonScene.CurPlayer))
            {
                MonsterAttack(DungeonScene.CurPlayer, () =>
                {
                    base.RoundAction();
                });
            }
            else
            {
                UnitWaiting(curRunDirection);
                base.RoundAction();
            }
        }
        else
        {
            UnitMove(GetTargetDirection(path[moveStep]), path[moveStep], "RoundMoveEnd", gameObject);
            moveStep++;
        }
    }

    bool PlayerInRange()
    {
        foreach (TileBlock tb in PositionTiles)
        {
            int minX = tb.XPosition - Range;
            int maxX = tb.XPosition + Range;
            int minY = tb.YPosition - Range;
            int maxY = tb.YPosition + Range;
            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    if (DungeonScene.CurPlayer.XPosition == i && DungeonScene.CurPlayer.YPosition == j)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 消失
    /// </summary>
    public override void ObjectDisAppear()
    {
        Invoke("MonsterDeadFxShow", 0.3f);
        ChangeState(ActionState.Dead, DungeonEnum.FaceDirection.RightDown, () =>
        {
            Destroy(gameObject);
        }, false);
    }

    void MonsterDeadFxShow()
    {
        GameObject disObject = Instantiate(DisAppearFX) as GameObject;
        disObject.transform.position = transform.position;
        bool hasDeadAllMonster = true;
        foreach (EnemyUnit e in DungeonScene.AllEnemyUnits)
        {
            if (e.GetType() == typeof(Monster) || e.GetType() == typeof(Boss))
            {
                hasDeadAllMonster = false;
            }
        }
        if (hasDeadAllMonster == true)
        {
            if (DungeonScene.CurFloor == ConfigManager.DungeonConfig.GetDungeonById(DungeonScene.curDid).FloorCount)
            {
                DungeonScene.WinRender();
            }
            else
            {
                DungeonScene.CurDoor.OpenDoor();
            }
        }
    }
    #endregion 

    #region 自己的方法
    /// <summary>
    /// 怪物攻击的方法
    /// </summary>
    /// <param name="target"></param>
    public Action curAttackEndCallback;
    public void MonsterAttack(OwnUnit target,Action callback)
    {
        curAttackEndCallback = callback;
        if (Range!=1)
        {
            StartCoroutine(FarAttackParFly(target));
        }
        else
        {
            target.BeHurt(this,true);
        }
        DungeonEnum.FaceDirection direction = GetTargetDirection(target);
        ChangeState(ActionState.Attack, direction, () =>
        {
            UnitWaiting(direction);
            if (Range == 1)
            {
                curAttackEndCallback();
                curAttackEndCallback = null;
            }

        }, false);
    }

    /// <summary>
    /// 远程攻击粒子飞行
    /// </summary>
    DungeonUnit curTarget; 
    GameObject farAttackBall;
    IEnumerator FarAttackParFly(OwnUnit target)
    {
        curTarget = target;
        float t = 0;
        for (int i = 0; i < FireFrame - 1; i++)
        {
            t = t + AttackFrameRate;
        }
        yield return new WaitForSeconds(t);
        farAttackBall = Instantiate(Resources.Load("PreFabs/FX/"+FarAttackFlyFX) as GameObject) as GameObject;
        farAttackBall.transform.position = transform.position;
        farAttackBall.SetActive(true);
        AnimationHelper.AnimationMoveToSpeed(target.transform.position, farAttackBall, iTween.EaseType.easeInExpo, gameObject, "FarParFlyEnd", 1.3f);
    }

    void FarParFlyEnd()
    {
        curTarget.BeHurt(this,false);
        Vector3 v = farAttackBall.transform.position;
        Destroy(farAttackBall);
        GameObject bump = Instantiate(Resources.Load("PreFabs/FX/" + FarAttackBumpFX)) as GameObject;
        bump.transform.position = v;
        bump.SetActive(true);
        curAttackEndCallback();
        curAttackEndCallback = null;
    }
    #endregion

}
