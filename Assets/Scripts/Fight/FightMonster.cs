using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public interface MonsterInter
{
    void FarAttackEnd();
}
public class FightMonster : FightEnemy
{
    #region 属性
    /// <summary>
    /// 攻击类型
    /// </summary>
    public MonsterData.AttackType CurAttackType;

    /// <summary>
    /// 攻击力
    /// </summary>
    public float Attack;

    /// <summary>
    /// 怪物Id
    /// </summary>
    public string MonsterId;

    /// <summary>
    /// 死亡资源
    /// </summary>
    public GameObject DeadResource;

    /// <summary>
    /// 远程怪物攻击粒子
    /// </summary>
    public GameObject FarAttackResource;

    public GameObject FarAttackBumpResource;

    /// <summary>
    /// 接口指针
    /// </summary>
    public MonsterInter MonsterHandler;
    #endregion

    #region 重写父类
    public override void SetName()
    {
        name = "Monster:" + MonsterId+"," + XPosition.ToString() + "," + YPosition.ToString() ;
    }

    /// <summary>
    /// 怪物死亡重写
    /// </summary>
    public override void EnemyDead(Action hurtEnd)
    {
        float deadTime = 0;
        foreach (float t in DeadFrameRate)
        {
            deadTime = deadTime + t;
        }
        Invoke("DeadFX", deadTime);
        ChangeState(ActionState.Dead, RemoveBlock.LinkDirection.LeftDown, () =>
        {
            UnitHandler.UnitDestory(this);
            Destroy(gameObject);
            base.EnemyDead(hurtEnd);
        });
    }

    void DeadFX()
    {
        GameObject dead = Instantiate(DeadResource) as GameObject;
        dead.transform.position = transform.position;
    }

    IEnumerator FarBallFly(FightPlayer target)
    {
        float t = 0;
        for (int i = 0; i < FireFrame - 1; i++)
        {
            t = t + AttackFrameRate[i];
        }
        yield return new WaitForSeconds(t);
        GameObject farAttackBall = Instantiate(FarAttackResource) as GameObject;
        farAttackBall.transform.position = transform.position;
        farAttackBall.SetActive(true);
        Hashtable args = new Hashtable();
        args.Add("position", target.transform.position);
        args.Add("speed", 1.2f);
        args.Add("easetype", iTween.EaseType.easeInExpo);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "FarAttackEnd");
        List<object> p = new List<object>();
        p.Add(farAttackBall);
        p.Add(target);
        args.Add("oncompleteparams", p);
        iTween.MoveTo(farAttackBall, args);
    }

    /// <summary>
    /// 怪物攻击
    /// </summary>
    public void MonsterAttack(FightPlayer target, Action hurtEnd)
    {
        target.CurHp = target.CurHp - Attack;
        target.curHurtDamage = (int)Attack;
        if (CurAttackType == MonsterData.AttackType.Far)
        {
            StartCoroutine(FarBallFly(target));
        }
        RemoveBlock.LinkDirection attackDirection = GetTargetDirection(target);
        UnitAttack(attackDirection, () =>
        {
            ForceUnitWaiting(attackDirection);
            if (CurAttackType == MonsterData.AttackType.Close)
            {
                if (!target.Hurt())
                {
                    hurtEnd();
                }
            }
        });
    }

    void FarAttackEnd(List<object> p)
    {
        //爆炸
        GameObject ball = (GameObject)p[0];
        GameObject bump = Instantiate(FarAttackBumpResource) as GameObject;
        bump.transform.position = ball.transform.position;
        bump.SetActive(true);
        Destroy(ball);
        FightPlayer target = (FightPlayer)p[1];
        if (!target.Hurt())
        {
            MonsterHandler.FarAttackEnd();
        }
    }

    /// <summary>
    /// 怪物行动
    /// </summary>
    public override void EnemyAction(Action hurtEnd)
    {
        if (CurAttackType == MonsterData.AttackType.Close)
        {
            if (hasAction == false)
            {
                hasAction = true;
                GameObject fight = GameObject.Find("Fight");
                //FightControl fc = fight.GetComponent<FightControl>();
               // MonsterAttack(fc.CurPlayer, hurtEnd);
            }
            else
            {
                hurtEnd();
            }
        }
        else
        {
            hurtEnd();
        }
    }

    #endregion
}
