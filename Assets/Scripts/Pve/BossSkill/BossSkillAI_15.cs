using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 该地图中的所有BOSS和怪物共享一个生命值.
/// </summary>
public class BossSkillAI_15 : BossSkillAI
{
    public bool ConditionCheck()
    {
        if (!base.ConditionCheck()) return false;
        return true;
    }

    public void UseSkill(Action callback)
    {
        if (!string.IsNullOrEmpty(CurBossData.BossSkill.FXPrefab1))
        {
            SkillRender(BossSkillController.GetSkillPrefab(CurBossData.BossSkill.FXPrefab1), CurBossData.BossSkill.FXType1, () =>
            {
                if (!string.IsNullOrEmpty(CurBossData.BossSkill.FXPrefab2))
                {
                    SkillRender(BossSkillController.GetSkillPrefab(CurBossData.BossSkill.FXPrefab2), CurBossData.BossSkill.FXType2, () =>
                    {
                        if (!string.IsNullOrEmpty(CurBossData.BossSkill.FXPrefab3))
                        {
                            SkillRender(BossSkillController.GetSkillPrefab(CurBossData.BossSkill.FXPrefab3), CurBossData.BossSkill.FXType3, () =>
                            {
                                SkillDetail();
                                callback();
                            });
                        }
                        else
                        {
                            SkillDetail();
                            callback();
                        }
                    });
                }
                else
                {
                    SkillDetail();
                    callback();
                }
            });
        }
        else
        {
            SkillDetail();
            callback();
        }
    }

    void SkillDetail()
    {
        float totalMaxLife = 0;
        float totalCurrentLife = 0;
        foreach(PveFightUnit pfu in GameControl.AllEnemies)
        {
            if(pfu.GetType() == typeof(PveMonster) || pfu.GetType() == typeof(PveBoss))
            {
                totalCurrentLife += pfu.CurHp;
                totalMaxLife += pfu.Hp;
            }
        }

        float rate = totalCurrentLife / totalMaxLife;

        foreach (PveFightUnit pfu in GameControl.AllEnemies)
        {
            if (pfu.GetType() == typeof(PveMonster) || pfu.GetType() == typeof(PveBoss))
            {
                pfu.CurHp = pfu.Hp * rate;
                pfu.RefreshHp();
            }
        }
    }

    void OnEnable()
    {
        selfTrigger = BossSkillController.TriggerType.Player_Attack;
    }
}
