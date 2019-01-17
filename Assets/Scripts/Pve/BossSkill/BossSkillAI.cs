using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleJson;

/// <summary>
/// 技能基类
/// </summary>
public class BossSkillAI : MonoBehaviour
{
    #region 通用属性
    /// <summary>
    /// AI类型
    /// </summary>
    public BossSkillController.SkillType AI;

    /// <summary>
    /// 触发机制
    /// </summary>
    public BossSkillController.TriggerType selfTrigger;

    /// <summary>
    /// PVE控制器
    /// </summary>
    private PveGameControl gameControl;
    protected PveGameControl GameControl
    {
        get
        {
            if (gameControl == null)
            {
                gameControl = GameObject.Find("UI Root").GetComponent<PveGameControl>();
            }
            return gameControl;
        }
        set
        {
            gameControl = value;
        }
    }

    /// <summary>
    /// 当前BOSS
    /// </summary>
    private PveBoss curBoss;
    protected PveBoss CurBoss
    {
        get
        {
            if (curBoss == null)
            {
                curBoss = gameObject.GetComponent<PveBoss>();
            }
            return curBoss;
        }
        set { curBoss = value; }
    }

    /// <summary>
    /// 当前玩家
    /// </summary>
    private PveCharacter curCharacter;

    protected PveCharacter CurCharacter
    {
        get
        {
            if(curCharacter == null)
            {
                curCharacter = GameControl.CurCharacter;
            }
            return curCharacter;
        }
        set { curCharacter = value; }
    }


    /// <summary>
    /// 当前bossdata
    /// </summary>
    private BossData curBossData;

    public BossData CurBossData
    {
        get
        {
            curBossData = CurBoss.CurBoss;
            return curBossData;
        }
        set
        {
            curBossData = value;
        }
    }

    #endregion

    #region 通用方法
    public virtual bool ConditionCheck()
    {
        if (PveGameControl.CurSkillTime == selfTrigger) return true;
        else return false;
    }

    Action emptyAttackEnd;
    /// <summary>
    /// 施法动作,空攻击
    /// </summary>
    public  void BossEmptyAttack(Action callback)
    {
        CurBoss.UsingSkill = true;
        if(CurCharacter.YPosition > CurCharacter.YPosition)
        {
            CurBoss.UnitAttack(DungeonEnum.FaceDirection.Up);
        }
        else if(CurCharacter.YPosition < CurCharacter.YPosition)
        {
            CurBoss.UnitAttack(DungeonEnum.FaceDirection.Down);
        }
        else if(CurCharacter.XPosition < CurBoss.XPosition)
        {
            CurBoss.UnitAttack(DungeonEnum.FaceDirection.LeftDown);
        }
        else
        {
            CurBoss.UnitAttack(DungeonEnum.FaceDirection.RightDown);
        }
        emptyAttackEnd = callback; // 回调在帧事件函数AnimationCon里执行
    }

    public void DelayAttackEnd()  // 回调在帧事件函数AnimationCon里执行
    {
        emptyAttackEnd();
    }

    /// <summary>
    /// 范围技能单元表现 case 5
    /// </summary>
    IEnumerator DelaySingleSkillItemRender(PveTile pt, GameObject prefab, float duringTime)
    {
        yield return new WaitForSeconds(CurBoss.Distance(CurBoss, pt) * 0.15f);
        Instantiate(prefab, pt.transform.position, Quaternion.identity);
    }

    /// <summary>
    /// 关闭阴影
    /// </summary>
    protected void DisableShadow(Transform trans)
    {
        FindChildShadow(trans);
        if(shadow != null)
        {
            if(shadow.gameObject.activeSelf == true)
            {
                shadow.gameObject.SetActive(false);
            }
        }
    }

    Transform shadow = null;
    void FindChildShadow(Transform trans)
    {
        foreach(Transform t in trans)
        {
            if (t.name == "Shadow")
            {
                shadow = t;
                return;
            }
            FindChildShadow(t);
        }
    }

    Action Callback;
    GameObject SkillPrefab;
    /// <summary>
    /// 技能表现
    /// </summary>
    protected void SkillRender(GameObject prefab, int SkillEffectType, Action callback, List<PveEliminate> tarPositions = null, List<PveTile> tarPositions_2 = null)
    {
        if(prefab)
        {
            Callback = callback;

            Component AnimationJscript = prefab.GetComponent("DeadTime");  //C#访问JS
            float duringTime = 0f;
            if(AnimationJscript)
            {
                FieldInfo parameter = AnimationJscript.GetType().GetField("deadTime");
                duringTime = float.Parse(parameter.GetValue(AnimationJscript).ToString());
            }

            switch(SkillEffectType)
            {
                case 0: break;
                case 1: // 光环
                    {
                        GameObject SkillFX = NGUITools.AddChild(CurBoss.gameObject, prefab);
                        //SkillFX.transform.localPosition += new Vector3(0f, 0.04f, 0f);
                        SkillFX.transform.localPosition += new Vector3(0f, 0f, 0f);
                        callback();
                        break;
                    }
                case 2: // 玩家单体
                    {

                        GameObject SkillFX = Instantiate(prefab, CurCharacter.transform.position, Quaternion.identity) as GameObject;
                        Tools.SetLayer(SkillFX.transform, LayerHelper.UnitFX);
                        Invoke("SkillRenderEnd", duringTime);
                        break;
                    }
                case 3: // BOSS单体
                    {
                        GameObject SkillFX = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
                        Tools.SetLayer(SkillFX.transform, LayerHelper.UnitFX);
                        Invoke("SkillRenderEnd", duringTime);
                        break;
                    }
                case 4: //飞向玩家
                    {
                        GameObject SkillFX = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
                        Tools.SetLayer(SkillFX.transform, LayerHelper.UnitFX);
                        SkillPrefab = SkillFX;
                        AnimationHelper.AnimationMoveTo(CurCharacter.transform.position, SkillFX, iTween.EaseType.easeInOutCirc, gameObject, "SkillRenderEnd", 1f);
                        break;
                    }
                case 5: // 坐标逐个
                    {
                        PveTile centerTile = GameControl.FindPveTile(CurBoss.XPosition, CurBoss.YPosition);
                        SkillData tempSkill = new SkillData(CurBossData.BossSkill);
                        List<PveTile> range = GameControl.CaculateSkillRangeTile(centerTile, tempSkill);
                        float totalTime = CurBoss.Distance(CurBoss, CurCharacter) * 0.25f + duringTime;
                        foreach(PveTile pt in range)
                        {
                            StartCoroutine(DelaySingleSkillItemRender(pt, prefab, duringTime));
                        }
                        Invoke("SkillRenderEnd", totalTime);
                        break;
                    }
                case 6: // BOSS Buff
                    {
                        GameObject SkillFX = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
                        callback();
                        break;
                    }
                case 7: // 玩家 Buff
                    {
                        GameObject SkillFX = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
                        callback();
                        break;
                    }
                case 8: // 召唤
                    {
                        if(tarPositions != null)
                        {
                            foreach(PveEliminate pe in tarPositions)
                            {
                                GameObject SkillFX = Instantiate(prefab, pe.transform.position, Quaternion.identity) as GameObject;
                            }
                        }
                        else if(tarPositions_2 != null)
                        {
                            foreach(PveTile pt in tarPositions_2)
                            {
                                GameObject SkillFX = Instantiate(prefab, pt.transform.position, Quaternion.identity) as GameObject;
                            }
                        }
                        callback();
                        break;
                    }
                default: break;
            }
        }
        else
        {
            callback();
        }
    }

    /// <summary>
    /// 移除技能渲染
    /// </summary>
    protected void DestroySkillRender()
    {
        if (SkillPrefab != null) Destroy(SkillPrefab);
    }

    void SkillRenderEnd()
    {
        DestroySkillRender();
        Callback();
    }

    protected void SkillUIRender(Action callback)
    {
        CurBoss.CurUnitHp.RefreshUI(CurBoss.CurHp, CurBoss.Hp);
        CurBoss.CurUnitHp.RealRefreshUI(() =>
        {
            callback();
        });
    }
    #endregion
}
