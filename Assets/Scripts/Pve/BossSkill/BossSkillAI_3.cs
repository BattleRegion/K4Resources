using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

#region BossSkillAI_3
/// <summary>
/// BOSS行动回合开始时,如果玩家主角在X参数方位内,则发动Y属性A点攻击,并眩晕N回合(眩晕效果不会连续作用)
/// </summary>
public class BossSkillAI_3 : BossSkillAI
{
    public override bool ConditionCheck() //条件
    {
        if (!base.ConditionCheck()) return false;
        if(CurCharacter.CurState != PveFightUnit.UnitState.vertigo)
        {
            PveTile centerTile = GameControl.FindPveTile(CurBoss.XPosition, CurBoss.YPosition);
            SkillData tempSkill = new SkillData(CurBossData.BossSkill);

            List<PveTile> range = GameControl.CaculateSkillRangeTile(centerTile, tempSkill);
            return GameControl.CharacterInRange(range);
        }
        else return false;
    }
    Action actionEnd;
    public void UseSkill(Action callback)
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
                                    CurCharacter.SetVertigo(int.Parse(CurBossData.BossSkill.Nparameter));
                                    if (int.Parse(CurBossData.BossSkill.Aparameter) > 0)
                                    {
                                        CurCharacter.BeHurt(float.Parse(CurBossData.BossSkill.Aparameter), CurBoss);
                                    }
                                    actionEnd = callback;
                                    Invoke("DelayCallback", 2f);
                                });
                            }
                            else
                            {
                                CurCharacter.SetVertigo(int.Parse(CurBossData.BossSkill.Nparameter));
                                if (int.Parse(CurBossData.BossSkill.Aparameter) > 0)
                                {
                                    CurCharacter.BeHurt(float.Parse(CurBossData.BossSkill.Aparameter), CurBoss);
                                }
                                actionEnd = callback;
                                Invoke("DelayCallback", 2f);
                            }
                        });
                    }
                    else
                    {
                        CurCharacter.SetVertigo(int.Parse(CurBossData.BossSkill.Nparameter));
                        if (int.Parse(CurBossData.BossSkill.Aparameter) > 0)
                        {
                            CurCharacter.BeHurt(float.Parse(CurBossData.BossSkill.Aparameter), CurBoss);
                        }
                        actionEnd = callback;
                        Invoke("DelayCallback", 2f);
                    }
                });
            }
            else
            {
                CurCharacter.SetVertigo(int.Parse(CurBossData.BossSkill.Nparameter));
                if (int.Parse(CurBossData.BossSkill.Aparameter) > 0)
                {
                    CurCharacter.BeHurt(float.Parse(CurBossData.BossSkill.Aparameter), CurBoss);
                }
                actionEnd = callback;
                Invoke("DelayCallback", 2f);
            }
        });
    }

    void DelayCallback()
    {
        actionEnd();
    }

    #region Mono
    void OnEnable()
    {
        selfTrigger = BossSkillController.TriggerType.Round_Start;
    }
    #endregion
}
#endregion
