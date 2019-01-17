using UnityEngine;
using System.Collections;
using System;
public interface FightEnemyInter
{
    void BeAttacking();
}

public class FightEnemy : FightUnit
{

    #region 属性
    /// <summary>
    /// 击打数LABEL
    /// </summary>
    public FightLabel EnemyBeHitLabel;

    /// <summary>
    /// 当前呗打击次数
    /// </summary>
    public int CurBeHitNum = 0;
	// Use this for initialization

    /// <summary>
    /// 敌人生命条
    /// </summary>
    public FightHp EnemyHp;

    /// <summary>
    /// 当前血量
    /// </summary>
    public float CurHp;

    /// <summary>
    /// 血量上限
    /// </summary>
    public float Hp;

    /// <summary>
    /// 默认呗攻击特效
    /// </summary>
    public GameObject DefaultBeAttackResources;

    /// <summary>
    /// 接口指针
    /// </summary>
    public FightEnemyInter BaseHandler;


    /// <summary>
    /// 本次造成的伤害
    /// </summary>
    public string BeHurtNum;

    /// <summary>
    /// 在本回合中已经行动过
    /// </summary>
    public bool hasAction = false;
    #endregion

    #region HitLabel
    /// <summary>
    /// 设置呗击打次数的LABEL
    /// </summary>
    public void BeHitNumShow()
    {
        CurBeHitNum++;
        //EnemyBeHitLabel.transform.localPosition = new Vector3(0, -1*elementSpriteRender.sprite.rect.height/2 + 28, 0);
        EnemyBeHitLabel.HitSetNum(CurBeHitNum);
    }

    /// <summary>
    /// 回退HIT
    /// </summary>
    public void BackHitNumShow()
    {
        if (CurBeHitNum > 0)
        {
            CurBeHitNum--;
            EnemyBeHitLabel.HitSetNum(CurBeHitNum);
        }
    }

    public void ClearHitNumShow()
    {
        CurBeHitNum = 0;
        EnemyBeHitLabel.HitSetNum(CurBeHitNum);
    }
    #endregion

    #region 重写父类
    public override void SetZorder()
    {
        base.SetZorder();
        ResetHpShow();
    }

    /// <summary>
    /// 呗技能击中
    /// </summary>
    /// <param name="player"></param>
    public override void ElementBeSkilled(SkillData skill)
    {
        //占时写死没有技能
        CurHp = CurHp - skill.Aparameter;
        curHurtDamage = (int)skill.Aparameter;
        Hurt(() =>
            {
            },false);
    }

    /// <summary>
    /// 元素事件处理玩家
    /// </summary>
    public override void ElementEventDeal(FightPlayer player)
    {
        player.PlayerAttack(this);
    }

    #endregion

    #region 血条和跳血数字Label
    protected void ResetHpShow()
    {
        //EnemyHp.transform.localPosition = new Vector3(0, -1 * elementSpriteRender.sprite.rect.height / 2 + EnemyHp.frameSprite.sprite.rect.height - 7, 0);
        EnemyHp.SetZorder(elementSpriteRender.sortingOrder);
    }
    #endregion

    #region 方法
    /// <summary>
    /// 受伤
    /// </summary>
    public virtual void Hurt(Action hurtEnd,bool needBeHitPara)
    {
        //伤害数字
        HurtLabelShow();
        if (needBeHitPara)
        {
            //受伤粒子
            GameObject beHitObject = Instantiate(DefaultBeAttackResources) as GameObject;
            beHitObject.transform.parent = transform;
            beHitObject.transform.localPosition = new Vector3(0, 0, 0);
        }
        //受伤动画
        HurtAnimation(hurtEnd);
        //左右晃动
        iTween.PunchPosition(gameObject, new Vector3(0.1f, 0, 0), 0.5f);
        EnemyHp.RefreshHp(CurHp, Hp);
        //攻击时候的COMBO数字
        BaseHandler.BeAttacking();
    }

    /// <summary>
    /// 受伤动画
    /// </summary>
    public virtual void HurtAnimation(Action hurtEnd)
    {
       ChangeState(ActionState.Hurt, curWaitingDirection, () =>
       {
           ForceUnitWaiting(curWaitingDirection);
           DealWithLife(hurtEnd);
       });
    }

    /// <summary>
    /// 处理受伤后血量导致的行为
    /// </summary>
    public void DealWithLife(Action hurtEnd)
    {
        if (CurHp <= 0)
        {
            EnemyDead(hurtEnd);
        }
        else
        {
            EnemyAction(hurtEnd);
        }
    }

    /// <summary>
    /// 反击
    /// </summary>
    public virtual void EnemyAction(Action hurtEnd)
    {
        hurtEnd();
    }

    /// <summary>
    /// 敌人死亡
    /// </summary>
    public virtual void EnemyDead(Action hurtEnd)
    {
        hurtEnd(); 
    }
    #endregion
}
