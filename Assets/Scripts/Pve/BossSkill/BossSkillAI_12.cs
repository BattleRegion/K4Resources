using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BossSkillAI_12 : BossSkillAI 
{
    PveBuffData CurBuff;

    public override bool ConditionCheck()
    {
        if(!base.ConditionCheck()) return false;
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
                                if (CurBoss.PlayerInRange())
                                {
                                    CurBoss.AddBuff(CurBuff);
                                    CurBoss.Attack(() =>
                                    {
                                        CurBoss.DeleteBuff(CurBuff);
                                        callback();
                                    });
                                }
                                else callback();
                            });
                        }
                        else
                        {
                            if (CurBoss.PlayerInRange())
                            {
                                CurBoss.AddBuff(CurBuff);
                                CurBoss.Attack(() =>
                                {
                                    CurBoss.DeleteBuff(CurBuff);
                                    callback();
                                });
                            }
                            else callback();
                        }
                    });
                }
                else
                {
                    if (CurBoss.PlayerInRange())
                    {
                        CurBoss.AddBuff(CurBuff);
                        CurBoss.Attack(() =>
                        {
                            CurBoss.DeleteBuff(CurBuff);
                            callback();
                        });
                    }
                    else callback();
                }
            });
        }
        else
        {
            if (CurBoss.PlayerInRange())
            {
                CurBoss.AddBuff(CurBuff);
                CurBoss.Attack(() =>
                {
                    CurBoss.DeleteBuff(CurBuff);
                    callback();
                });
            }
            else callback();
        }
    }

    #region mono
    void OnEnable()
    {
        Hashtable args = new Hashtable();
        args.Add("atk", ConfigManager.SkillConfig.GetParameterPercent(float.Parse(CurBossData.BossSkill.Xparameter)));
        selfTrigger = BossSkillController.TriggerType.UnderAttack;
    }
    #endregion
}