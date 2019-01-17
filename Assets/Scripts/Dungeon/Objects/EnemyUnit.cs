using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public interface EnemyInterface
{
    void EnemyUnitRoundActionEnd();
}

public class EnemyUnit :DungeonUnit
{
    #region 属性
    /// <summary>
    /// 敌方单位接口
    /// </summary>
    public EnemyInterface EnemyHandler;

    /// <summary>
    /// 打击次数LABEL
    /// </summary>
    public EnemyBeHitLabel HitLabel;

    /// <summary>
    /// 当前打击次数
    /// </summary>
    public int CurHitNum;

    public int XRange;
    public int YRange;

    public List<TileBlock> PositionTiles = new List<TileBlock>();

    #endregion

    #region 资源指针
    #endregion

    #region 工具方法
    /// <summary>
    /// 用于判断敌方单位是否可以攻击
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static bool CanAttack(EnemyUnit e,OwnUnit o)
    {
        if (e.GetType() == typeof(Monster) || e.GetType() == typeof(Boss))
        {
            return true;
        }
        if (o.GetType() == typeof(Player) && e.GetType() == typeof(Barrier))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 可以在回合内攻击
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static bool CanRoundAction(EnemyUnit e)
    {
        if (e.GetType() == typeof(Monster) || e.GetType()==typeof(Boss))
        {
            if (e.CurHp > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    #endregion

    #region 虚方法
    /// <summary>
    /// 回合行为
    /// </summary>
    public virtual void RoundAction()
    {
        EnemyHandler.EnemyUnitRoundActionEnd();
    }

    /// <summary>
    /// 与敌方交互
    /// </summary>
    public virtual void EnemyMutual(OwnUnit own,Action MutalEnd)
    {
        MutalEnd();
    }

    /// <summary>
    /// 呗打击次数渲染
    /// </summary>
    public virtual void WillBeHitRender(int hitNum)
    {
        if (HitLabel&&hitNum!=CurHitNum)
        {
            CurHitNum = hitNum;
            HitLabel.SetNum(hitNum.ToString() + "h");
        }
    }

    #endregion

    #region 重写
    public override void SetPosition(int xPosition, int yPosition)
    {
 	    base.SetPosition(xPosition, yPosition);
        ResertPosition();
    }

    public void CaculatePositionTile()
    {
        PositionTiles.Clear();
        for (int i = XPosition; i < XPosition + XRange; i++)
        {
            for (int j = YPosition; j < YPosition + YRange; j++)
            {
                TileBlock tb = DungeonScene.FindTile(i, j);
                if (tb)
                {
                    PositionTiles.Add(tb);
                }
            }
        }
    }

    public void ResertPosition()
    {
        CaculatePositionTile();
        //重设位置
        transform.localPosition = new Vector3(transform.localPosition.x + (XRange - 1) * 42, transform.localPosition.y, transform.localPosition.z);
    }


    /// <summary>
    /// 被击退
    /// </summary>
    public override void BeHitBackRender(float backCount, DungeonEnum.FaceDirection backDirection)
    {
        Debug.Log("击退" + backCount + backDirection);
        int xP = XPosition;
        int yP = YPosition;
        TileBlock tb = null;
        if (backDirection == DungeonEnum.FaceDirection.Up)
        {
            int tempY = YPosition + (int)backCount;
            if (tempY > 8)
            {
                tempY = 8;
            }
            for (int i = YPosition + 1; i <= tempY; i++)
            {
                if (DungeonScene.FindEnemyUnit(xP, i) == null)
                {
                    yP++;
                }
                else
                {
                    break;
                }
            }
            tb = DungeonScene.FindTile(xP, yP);
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
                if (DungeonScene.FindEnemyUnit(xP, i) == null)
                {
                    yP--;
                }
                else
                {
                    break;
                }
            }
            tb = DungeonScene.FindTile(xP, yP);
        }
        else if (backDirection == DungeonEnum.FaceDirection.Left)
        {
            int tempX = YPosition - (int)backCount;
            if (tempX < 0)
            {
                tempX = 0;
            }
            for (int i = XPosition - 1; i >= tempX; i--)
            {
                if (DungeonScene.FindEnemyUnit(i, yP) == null)
                {
                    xP--;
                }
                else
                {
                    break;
                }
            }
            tb = DungeonScene.FindTile(xP, yP);
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
                if (DungeonScene.FindEnemyUnit(i, yP) == null)
                {
                    xP++;
                }
                else
                {
                    break;
                }
            }
            tb = DungeonScene.FindTile(xP, yP);
        }
        XPosition = tb.XPosition;
        YPosition = tb.YPosition;
        CaculatePositionTile();
        SetObjectName();
        AnimationHelper.AnimationMoveTo(tb.transform.localPosition, gameObject, iTween.EaseType.linear, null, null, 0.25f);
    }
    #endregion
}
