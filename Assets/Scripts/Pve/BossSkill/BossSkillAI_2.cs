using UnityEngine;
using System.Collections;
using System;
using SimpleJson;

#region BossSkillAI_2
/// <summary>
/// BOSS的普通攻击的伤害X%,恢复BOSS的生命力.
/// </summary>
public class BossSkillAI_2 : BossSkillAI
{
    public bool ConditionCheck() //检测
    {
        if (!base.ConditionCheck()) return false;
        if (CurBoss.CurHp >= CurBoss.Hp) return false;
        else return true;
    }

    public void UseSkill(float damage, Action callback) //具体
    {
        if (!string.IsNullOrEmpty(CurBossData.BossSkill.FXPrefab2))
        {
            SkillRender(BossSkillController.GetSkillPrefab(CurBossData.BossSkill.FXPrefab2), CurBossData.BossSkill.FXType2, () =>
            {
                if (!string.IsNullOrEmpty(CurBossData.BossSkill.FXPrefab3))
                {
                    SkillRender(BossSkillController.GetSkillPrefab(CurBossData.BossSkill.FXPrefab3), CurBossData.BossSkill.FXType3, () =>
                    {
                        float percentParam = ConfigManager.SkillConfig.GetParameterPercent(float.Parse(CurBossData.BossSkill.Xparameter));
                        float healValue = damage * percentParam;
                        if (CurBoss.CurHp + healValue > CurBoss.Hp)
                        {
                            healValue = CurBoss.Hp - CurBoss.CurHp;
                        }
                        CurBoss.CurHp += healValue;
                        CurBoss.HealLabelShow(healValue);
                        SkillUIRender(() =>
                        {
                            callback();
                        });
                    });
                }
                else
                {
                    float percentParam = ConfigManager.SkillConfig.GetParameterPercent(float.Parse(CurBossData.BossSkill.Xparameter));
                    float healValue = damage * percentParam;
                    if (CurBoss.CurHp + healValue > CurBoss.Hp)
                    {
                        healValue = CurBoss.Hp - CurBoss.CurHp;
                    }
                    CurBoss.CurHp += healValue;
                    CurBoss.HealLabelShow(damage * percentParam);
                    SkillUIRender(() =>
                    {
                        callback();
                    });
                }
            });
        }
        else
        {
            float percentParam = ConfigManager.SkillConfig.GetParameterPercent(float.Parse(CurBossData.BossSkill.Xparameter));
            float healValue = damage * percentParam;
            if (CurBoss.CurHp + healValue > CurBoss.Hp)
            {
                healValue = CurBoss.Hp - CurBoss.CurHp;
            }
            CurBoss.CurHp += healValue;
            CurBoss.HealLabelShow(damage * percentParam);
            SkillUIRender(() =>
            {
                callback();
            });
        }
    }

    #region Mono
    void OnEnable()
    {
        selfTrigger = BossSkillController.TriggerType.Attack;

        if (!string.IsNullOrEmpty(CurBossData.BossSkill.FXPrefab1) || CurBossData.BossSkill.FXType1 == SkillEffectTypeEnum.FIXED)
        {
            SkillRender(BossSkillController.GetSkillPrefab(CurBossData.BossSkill.FXPrefab1), CurBossData.BossSkill.FXType1, () => { });
        }

    }

    void Update()
    {
        DisableShadow(this.transform);
    }
    #endregion
}
#endregion