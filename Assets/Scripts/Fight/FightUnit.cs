using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
/// <summary>
/// 战斗单位，消除块上的玩家和怪物元素的基类
/// </summary>
public interface UnitInterFace
{
    void UnitDestory(FightUnit unit);
}
public class FightUnit : FightElement
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
        Dead = 70
    }

    public enum UnitType
    {
        None = 0,
        Character = 1,
    }
    #endregion

    #region 属性

    /// <summary>
    /// 单位类型
    /// </summary>
    public UnitType CurUnitType;

    /// <summary>
    /// 帧率
    /// </summary>
    public List<float> WaitingFrameRate = new List<float>();

    public List<float> RunningFrameRate = new List<float>();

    public List<float> AttackFrameRate = new List<float>();

    public List<float> HurtFrameRate = new List<float>();

    public List<float> DeadFrameRate = new List<float>();

    public int FireFrame;

    public RemoveBlock.LinkDirection InitDirection = RemoveBlock.LinkDirection.Up;

    public UnitInterFace UnitHandler;

    public int curHurtDamage = 0;

    public RemoveBlock lastStayBlock;

    public FightUnit lastUnit;

    #region 偏移属性
    public float WaitingOffsetX;
    public float WaitingOffsetY;

    public float AttackOffsetX;
    public float AttackOffsetY;

    public float RunOffsetX;
    public float RunOffsetY;

    public float DeadOffsetX;
    public float DeadOffsetY;

    public float HurtOffsetX;
    public float HurtOffsetY;
    #endregion

    #endregion

    #region 公用方法
    public override void RenderSprite()
    {
        UnitWaiting(InitDirection);
    }

    public override void ResertSpritePosition()
    {
        base.ResertSpritePosition();
        CorrectUnitPosition();
    }

    public void HurtLabelShow()
    {
        GameObject beHurtResource = Resources.Load("PreFabs/Fight/DamageLable") as GameObject;
        GameObject hurtObject = Instantiate(beHurtResource) as GameObject;
        hurtObject.transform.parent = transform.parent;
        //随机位置
        hurtObject.transform.localScale = new Vector3(1, 1, 1);
        int offsetX = UnityEngine.Random.Range(-30, 31);
        int offsetY = UnityEngine.Random.Range(-30, 31);
        hurtObject.transform.localPosition = new Vector3(transform.localPosition.x + offsetX, transform.localPosition.y + offsetY, transform.localPosition.z);
        FightDamage fd = hurtObject.GetComponent<FightDamage>();
       //fd.DamageShow(curHurtDamage.ToString());
    }

    #endregion

    #region 战斗单位基础行为
    public RemoveBlock.LinkDirection curWaitingDirection = RemoveBlock.LinkDirection.None;
    public ActionState curState = ActionState.None;
    /// <summary>
    /// 改变战斗单位状态
    /// </summary>
    /// <param name="state"></param>
    /// <param name="direction"></param>
    public void ChangeState(ActionState state, RemoveBlock.LinkDirection direction,Action StateChangeEnd)
    {
        curState = state;
        string stateAnimationPath = "Atlas/Character/" + SkinId.ToString() +"/" + SkinId.ToString() + ((int)state).ToString() + ((int)direction).ToString();
        bool forever = false;
        if (curState == ActionState.Waiting || curState == ActionState.Run)
        {
            forever = true;
        }
        PlayAnimation(stateAnimationPath, forever, StateChangeEnd);
        ResertSpritePosition();
    }

    public void CorrectUnitPosition()
    {
        float offsetX = 0;
        float offsetY = 0;
        if (curState == ActionState.Waiting)
        {
            offsetX = WaitingOffsetX;
            offsetY = WaitingOffsetY;
        }
        else if (curState == ActionState.Attack)
        {
            offsetX = AttackOffsetX;
            offsetY = AttackOffsetY;
        }
        else if (curState == ActionState.Dead)
        {
            offsetX = DeadOffsetX;
            offsetY = DeadOffsetY;
        }
        else if (curState == ActionState.Run)
        {
            offsetX = RunOffsetX;
            offsetY = RunOffsetY;
        }
        else if (curState == ActionState.Hurt)
        {
            offsetX = HurtOffsetX;
            offsetY = HurtOffsetY;
        }
        elementSpriteRender.transform.localPosition = new Vector3(elementSpriteRender.transform.localPosition.x + offsetX, elementSpriteRender.transform.localPosition.y + offsetY, elementSpriteRender.transform.localPosition.z);
    }

    /// <summary>
    /// 等待
    /// </summary>
    public void UnitWaiting(RemoveBlock.LinkDirection direction)
    {
        if (curWaitingDirection != direction)
        {
            curWaitingDirection = direction;
            ChangeState(ActionState.Waiting, direction,null);
        }
    }

    /// <summary>
    /// 强制等待
    /// </summary>
    /// <param name="direction"></param>
    public void ForceUnitWaiting(RemoveBlock.LinkDirection direction)
    {
        curWaitingDirection = direction;
        ChangeState(ActionState.Waiting, direction, null);
    }

    /// <summary>
    /// 单位攻击
    /// </summary>
    /// <param name="direction"></param>
    public virtual void UnitAttack(RemoveBlock.LinkDirection direction,Action attackEnd)
    {
        ChangeState(ActionState.Attack, direction, attackEnd);
    }
    #endregion

    #region 2d序列帧动画实现
    Sprite[] curAnimationSprites;
    int curFrame = -1;
    bool animationForever = false;
    Action CurAnimationEnd;
    public void PlayAnimation(string spritePath,bool forever,Action AnimationEnd)
    {
        StopAnimation();
        animationForever = forever;
        CurAnimationEnd = AnimationEnd;
        curAnimationSprites = Resources.LoadAll<Sprite>(spritePath);
        if (elementSpriteRender != null)
        {
            NextFrame();
        }
        else
        {
            Debug.LogError("no sprite render");
        }
    }

    void NextFrame()
    {
        if (curAnimationSprites == null || curAnimationSprites.Length == 0)
        {
            Debug.LogError("no sprite frames"); return;
        }
        curFrame++;
        elementSpriteRender.sprite = curAnimationSprites[curFrame];
        if(curFrame == curAnimationSprites.Length - 1)
        {
            if (animationForever)
            {
                curFrame = -1;
            }
            else
            {
                Invoke("AnimationEnd", GetCurFrameInterval()); return;
            }
        }
        Invoke("NextFrame", GetCurFrameInterval());
    }

    void AnimationEnd()
    {
        if (CurAnimationEnd != null)
        {
            curFrame = 0;
            CurAnimationEnd();
        }
    }

    public void StopAnimation()
    {
        CancelInvoke("NextFrame");
        curFrame = -1;
        animationForever = false;
        CurAnimationEnd = null;
    }

    float GetCurFrameInterval()
    {
        try
        {
            int index = curFrame;
            if (curFrame == -1)
            {
                index = curAnimationSprites.Length - 1;
            }
            if (curState == ActionState.Attack)
            {
                return AttackFrameRate[index];
            }
            else if (curState == ActionState.Run)
            {
                return RunningFrameRate[index];
            }
            else if (curState == ActionState.Waiting)
            {
                return WaitingFrameRate[index];
            }
            else if (curState == ActionState.Hurt)
            {
                return HurtFrameRate[index];
            }
            else if (curState == ActionState.Dead)
            {
                return DeadFrameRate[index];
            }
        }
        catch
        {
            return 0.2f;
        }
        return 0.2f;
    }
    #endregion
}
