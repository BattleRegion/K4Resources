using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// 障碍物，组织填充
/// </summary>
/// 
public interface BarrierInter
{
    void BarrierDestoryed(FightBarrier barrier);
}

public class FightBarrier : FightEnemy
{
    #region 属性

    //障碍物呗清除的粒子
    public GameObject BarrierDeadResource;

    public BarrierInter Handler;

    public string BarrierId;
    #endregion


    #region 重写父类
    public override void SetName()
    {
        name = "Barrier:" + BarrierId+"," + XPosition.ToString() + "," + YPosition.ToString();
    }

    /// <summary>
    /// 死亡重写
    /// </summary>
    public override void EnemyDead(Action hurtEnd)
    {
         GameObject barrierDead = Instantiate(BarrierDeadResource) as GameObject;
         barrierDead.transform.position = transform.position;
         Hashtable args = new Hashtable();
         args.Add("time", 0.5f);
         args.Add("alpha", 0);
         args.Add("oncomplete", "fadeEnd");
         args.Add("oncompletetarget", gameObject);
         iTween.FadeTo(gameObject, args);
         UnitHandler.UnitDestory(this);
         Handler.BarrierDestoryed(this);
         base.EnemyDead(hurtEnd);
    }

    void fadeEnd()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 受伤动画重写
    /// </summary>
    /// <param name="hurtEnd"></param>
    public override void HurtAnimation(Action hurtEnd)
    {
        //延迟一下
        StartCoroutine(DelayDeal(hurtEnd));
        
    }
    IEnumerator DelayDeal(Action hurtEnd)
    {
        yield return new WaitForSeconds(0.2f);
        DealWithLife(hurtEnd);
    }
    #endregion
}
