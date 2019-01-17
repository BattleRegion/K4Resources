using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


#region BossSkillAI_1
/// <summary>
/// 在BOSS行动回合,如果玩家主角的当前生命力大于BOSS生命力,则BOSS在行走前使用灵魂隔断的效果,使BOSS的当前生命力=主角当前生命力,可超过BOSS的最大生命力上限.
/// </summary>
public class BossSkillAI_1 : BossSkillAI
{

    public override bool ConditionCheck() //触发条件检测
    {
        if (!base.ConditionCheck()) return false;
        else
        {
            if (CurCharacter.CurHp > CurBoss.CurHp && CurBoss.CurHp < CurBoss.Hp)
            {
                Debug.Log(CurBoss.CurHp + "/" + CurBoss.Hp);
                return true;
            }
            else return false;
        }
    }

    public void UseSkill(Action callback) //技能详细
    {
        BossEmptyAttack(() => 
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
                                    float healValue = CurCharacter.CurHp > CurBoss.Hp ? (CurBoss.Hp - CurBoss.CurHp) : (CurCharacter.CurHp - CurBoss.CurHp);
                                    CurBoss.CurHp = CurCharacter.CurHp > CurBoss.Hp ? (CurBoss.Hp) : (CurCharacter.CurHp);
                                    CurBoss.HealLabelShow((float)(int)healValue);
                                    SkillUIRender(() =>
                                    {
                                        callback();
                                    });
                                });
                            }
                            else
                            {
                                float healValue = CurCharacter.CurHp > CurBoss.Hp ? (CurBoss.Hp - CurBoss.CurHp) : (CurCharacter.CurHp - CurBoss.CurHp);
                                CurBoss.CurHp = CurCharacter.CurHp > CurBoss.Hp ? (CurBoss.Hp) : (CurCharacter.CurHp);
                                CurBoss.HealLabelShow((float)(int)healValue);
                                SkillUIRender(() =>
                                {
                                    callback();
                                });
                            }
                        });
                    }
                    else
                    {
                        float healValue = CurCharacter.CurHp > CurBoss.Hp ? (CurBoss.Hp - CurBoss.CurHp) : (CurCharacter.CurHp - CurBoss.CurHp);
                        CurBoss.CurHp = CurCharacter.CurHp > CurBoss.Hp ? (CurBoss.Hp) : (CurCharacter.CurHp);
                        CurBoss.HealLabelShow((float)(int)healValue);
                        SkillUIRender(() =>
                        {
                            callback();
                        });
                    }
                });
            }
            else
            {
                float healValue = CurCharacter.CurHp > CurBoss.Hp ? (CurBoss.Hp - CurBoss.CurHp) : (CurCharacter.CurHp - CurBoss.CurHp);
                CurBoss.CurHp = CurCharacter.CurHp > CurBoss.Hp ? (CurBoss.Hp) : (CurCharacter.CurHp);
                CurBoss.HealLabelShow((float)(int)healValue);
                SkillUIRender(() =>
                {
                    callback();
                });
            }
        });

    }


    #region MONO函数
    void OnEnable()
    {
        selfTrigger = BossSkillController.TriggerType.Round_Start;
    }
    #endregion
}
#endregion

