using UnityEngine;
using System.Collections;
using System;

#region BossSkillAI_6
/// <summary>
/// BOSS回合开始时,时2主角防御力*X%,闪躲率+Y%,持续N回合.
/// </summary>
public class BossSkillAI_6 : BossSkillAI
{
    public PveBuffData CurSkillBuff;

    bool Used = false;

    public bool ConditionCheck() //条件
    {
        if (!base.ConditionCheck()) return false;
        if (Used) return false;
        return !CurCharacter.CurBuffs.Contains(CurSkillBuff);
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
                                    CurCharacter.AddBuff(CurSkillBuff);
                                    callback();
                                });
                            }
                            else
                            {
                                CurCharacter.AddBuff(CurSkillBuff);
                                callback();
                            }
                        });
                    }
                    else
                    {
                        CurCharacter.AddBuff(CurSkillBuff);
                        callback();
                    }
                });
            }
            else
            {
                CurCharacter.AddBuff(CurSkillBuff);
                callback();
            }
        });
    }

    #region MONO
    void OnEnable()
    {
        Hashtable args = new Hashtable();
        args.Add("def", ConfigManager.SkillConfig.GetParameterPercent(float.Parse(CurBossData.BossSkill.Xparameter)));
        args.Add("evade", ConfigManager.SkillConfig.GetParameterPercent(float.Parse(CurBossData.BossSkill.Yparameter)));
        CurSkillBuff = new PveBuffData(args);
        selfTrigger = BossSkillController.TriggerType.Round_Start;
    }
    #endregion
}
#endregion