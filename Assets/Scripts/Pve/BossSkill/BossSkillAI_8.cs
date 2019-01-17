using UnityEngine;
using System.Collections;
using System;

#region BossSkillAI_8
/// <summary>
/// BOSS自身闪躲率+%.
/// </summary>
public class BossSkillAI_8 : BossSkillAI
{
    PveBuffData CurBuff;
    public bool ConditionCheck() //条件
    {
        if (!base.ConditionCheck()) return false;
        return true;
    }

    public void UseSkill(Action callback) //实现
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
                                CurBoss.AddBuff(CurBuff);
                                callback();
                            });
                        }
                        else
                        {
                            CurBoss.AddBuff(CurBuff);
                            callback();
                        }
                    });
                }
                else
                {
                    CurBoss.AddBuff(CurBuff);
                    callback();
                }
            });
        }
        else
        {
            CurBoss.AddBuff(CurBuff);
            callback();
        }

    }

    #region MONO
    void OnEnable()
    {
        Hashtable args = new Hashtable();
        args.Add("evade", ConfigManager.SkillConfig.GetParameterPercent(float.Parse(CurBossData.BossSkill.Xparameter)));
        CurBuff = new PveBuffData(args);
        selfTrigger = BossSkillController.TriggerType.Buff;
    }
    #endregion
}
#endregion