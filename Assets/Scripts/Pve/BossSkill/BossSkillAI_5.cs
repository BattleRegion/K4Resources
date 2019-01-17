using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

#region BossSkillAI_5
/// <summary>
/// BOSS回合开始后释放缠绕使主角无法移动
/// </summary>
public class BossSkillAI_5 : BossSkillAI
{
    bool Used = false;
    public bool ConditionCheck() //条件
    {
        if (!base.ConditionCheck()) return false;
        return !Used;
    }

    public void UseSkill(Action callback) //实现
    {
        BossEmptyAttack(() =>
        {
            Used = true;
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
                                    CurCharacter.DistanceLimit = 0;
                                    callback();
                                });
                            }
                            else
                            {
                                CurCharacter.DistanceLimit = 0;
                                callback();
                            }
                        });
                    }
                    else
                    {
                        CurCharacter.DistanceLimit = 0;
                        callback();
                    }
                });
            }
            else
            {
                CurCharacter.DistanceLimit = 0;
                callback();
            }

        });
    }

    #region MONO
    void OnEnable()
    {
        selfTrigger = BossSkillController.TriggerType.Round_Start;
    }
    #endregion
}
#endregion