using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// 战斗玩家
/// </summary>
public interface PlayerInterFace
{
    void OnceMoveEnd(RemoveBlock moveToBlock);
    void OnceActionEnd(RemoveBlock moveToBlock);
    void PowerSkillEnd();
    void PowerSkillRender();
}

public class FightPlayer : FightUnit
{
    #region 属性
    /// <summary>
    /// 攻击力
    /// </summary>
    public float Attack;

    /// <summary>
    /// 当前生命值
    /// </summary>
    public float CurHp;

    /// <summary>
    /// 生命上限
    /// </summary>
    public float Hp;

    /// <summary>
    /// 接口指针
    /// </summary>
    public PlayerInterFace Handler;

    /// <summary>
    /// 箭头
    /// </summary>
    public PlayerArrow Arrow;

    /// <summary>
    /// 角色Id
    /// </summary>
    public string CharacterId;
    /// <summary>
    /// 攻击特效
    /// </summary>
    public FightHp PlayerHp;

    /// <summary>
    /// 蓄力粒子资源
    /// </summary>
    public GameObject PlayerPowerResources;

    /// <summary>
    /// 当前连击系数
    /// </summary>
    public float CurComboAddition = 1.0f;

    /// <summary>
    /// 是否有钥匙
    /// </summary>
    public bool HasKey = false;

    /// <summary>
    /// 临时记录玩家技能
    /// </summary>
    public SkillData CurPlayerSkill;
    //public FightControl Fc;
    #endregion

    #region 玩家行为
    RemoveBlock curMoveToBlock;
    GameObject powerObject;
    RemoveBlock.LinkDirection curRunDirection = RemoveBlock.LinkDirection.None;
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="moveToBlock"></param>
    public void PlayerMove(RemoveBlock.LinkDirection direction,RemoveBlock moveToBlock)
    {
        //lastStayBlock = Fc.FindBlock(XPosition, YPosition);
        //Fc.TryPetAction();
        //bool hasAppear = Fc.TryAppearPet(() =>
        //{
        //    moveToEnd();
        //});
        CurComboAddition = CurComboAddition + 0.1f;
        XPosition = moveToBlock.XPosition;
        YPosition = moveToBlock.YPosition;
        SetName();
        PlayerRun(direction);
        curMoveToBlock = moveToBlock;
        Hashtable args = new Hashtable();
        args.Add("position", moveToBlock.transform.localPosition);
        args.Add("time",0.4f);
        args.Add("easetype", iTween.EaseType.linear);
        args.Add("islocal", true);
        //if (!hasAppear)
        //{
        //    args.Add("oncompletetarget", gameObject);
        //    args.Add("oncomplete", "moveToEnd");
        //}
        iTween.MoveTo(gameObject, args);
    }

     /// <summary>
     /// 移动结束的回调
     /// </summary>
    void moveToEnd()
    {
        SetZorder();
        curRunDirection = RemoveBlock.LinkDirection.None;
        curWaitingDirection = RemoveBlock.LinkDirection.None;
        Handler.OnceMoveEnd(curMoveToBlock);
    }

    /// <summary>
    /// 跑
    /// </summary>
    /// <param name="direction"></param>
    public void PlayerRun(RemoveBlock.LinkDirection direction)
    {
        if (curRunDirection != direction)
        {
            curRunDirection = direction;
            ChangeState(ActionState.Run, direction,null);
        }
    }

    /// <summary>
    /// 玩家攻击
    /// </summary>
    /// <param name="enemy"></param>
    public void PlayerAttack(FightEnemy enemy)
    {
        if (enemy.CurHp > 0)
        {
            //数据扣除
            enemy.CurHp = enemy.CurHp - Attack * CurComboAddition;
            enemy.curHurtDamage = (int)(Attack * CurComboAddition);
            RemoveBlock.LinkDirection attackDirection = GetTargetDirection(enemy);
            UnitAttack(attackDirection, () =>
            {
                ForceUnitWaiting(attackDirection);
            });
            StartCoroutine(AttackBump(enemy));
        }
        else
        {
            PlayerNextAction();
        }
    }

    IEnumerator AttackBump(FightEnemy enemy)
    {
        float t = 0;
        for (int i = 0; i < FireFrame-1; i++)
        {
            t = t + AttackFrameRate[i];
        }
        yield return new WaitForSeconds(t);
        enemy.Hurt(() =>
        {
            PlayerNextAction();
        },true);
    }

    /// <summary>
    /// 玩家行走结束后行动
    /// </summary>
    int curActionTargetIndex = -1;
    List<FightElement> curTargets;
    public void PlayerAction(List<FightElement> actionTargets)
    {
        curTargets = actionTargets;
        PlayerNextAction();
    }

    public void PlayerNextAction()
    {
        curActionTargetIndex++;
        if (curActionTargetIndex == curTargets.Count)
        {
            curActionTargetIndex = -1;
            Handler.OnceActionEnd(curMoveToBlock);
        }
        else
        {
            FightElement target = curTargets[curActionTargetIndex];
            target.ElementEventDeal(this);
        }
    }

    /// <summary>
    /// 箭头显示控制
    /// </summary>
    /// <param name="enabel"></param>
    public void ArrowEnabel(bool enabel)
    {
        if (enabel)
        {
            iTween.FadeTo(Arrow.gameObject, 1, 0.3f);
        }
        else
        {
            iTween.FadeTo(Arrow.gameObject, 0, 0.3f);
        }
    }

    public override void SetName()
    {
        name = "Character:"+CharacterId+"," + XPosition.ToString() + "," + YPosition.ToString();
    }

    public override void SetZorder()
    {
        base.SetZorder();
        //PlayerHp.transform.localPosition = new Vector3(0, -1 * basicHeight / 2 + PlayerHp.frameSprite.sprite.rect.height / 2, 0);
        PlayerHp.SetZorder(elementSpriteRender.sortingOrder);
    }

    /// <summary>
    /// 角色蓄力一击
    /// </summary>
    public void PowerSkillAttack()
    {
        powerObject = Instantiate(PlayerPowerResources) as GameObject;
        powerObject.transform.parent = transform;
        powerObject.transform.localPosition = new Vector3(0, 0, 0);
        StopAnimation();
        string stateAnimationPath = "Atlas/Character/" + SkinId.ToString() + "/" + SkinId.ToString() + ((int)ActionState.Attack).ToString() + ((int)RemoveBlock.LinkDirection.Down).ToString();
        Sprite[] attackSprites = Resources.LoadAll<Sprite>(stateAnimationPath);
        elementSpriteRender.sprite = attackSprites[2];
        Invoke("PowerGetEnd",1.5f);
    }

    void PowerGetEnd()
    {
        PlayerUseSkill();
    }

    /// <summary>
    /// 使用技能
    /// </summary>
    void PlayerUseSkill()
    {
        Destroy(powerObject);
        float t = 0;
        for (int i = 0; i < FireFrame - 1; i++)
        {
            t = t + AttackFrameRate[i];
        }
        Invoke("SkillShow",t);
        ChangeState(ActionState.Attack, RemoveBlock.LinkDirection.Down, () =>
        {
            Handler.PowerSkillEnd();
        });
    }

    void SkillShow()
    {
        Handler.PowerSkillRender();
        CameraControl.ShakeCamera();
    }
    #endregion

    #region 被攻击
    public bool Hurt()
    {
        HurtLabelShow();
        PlayerHp.RefreshHp(CurHp, Hp);
        if (CurHp <= 0)
        {
            PlayerLose();
            return true;
        }
        return false;
    }

    #endregion

    #region 胜利失败
    public void PlayerLose()
    {
        Debug.Log("游戏结束");
    }

    public void PlayerWin()
    {
        Debug.Log("玩家胜利");
    }
    #endregion
}