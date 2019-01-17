using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#region BossSkillAI_4
/// <summary>
/// BOSS如果攻击到玩家主角,则在回合结束时,将BOSS随机传送并消失无敌N回合.(无敌状态无法收到伤害,且经过无法发动攻击)
/// </summary>
public class BossSkillAI_4 : BossSkillAI
{
    public override bool ConditionCheck() //条件
    {
        if (!base.ConditionCheck()) return false;
        return CurBoss.AttackTag;
    }

    public void UseSkill(Action callback) //实现
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
                                    CurBoss.SetDisappear(int.Parse(CurBossData.BossSkill.Nparameter), () =>
                                    {
                                        List<PveEliminate> bs = new List<PveEliminate>();
                                        foreach (PveEliminate b in GameControl.AllEliminates)
                                        {
                                            if (GameControl.HasEnemyOnPosition(b.XPosition, b.YPosition, null) == false && (GameControl.CurCharacter.XPosition != b.XPosition || GameControl.CurCharacter.YPosition != b.YPosition))
                                            {
                                                bs.Add(b);
                                            }
                                        }
                                        int randomIndex = UnityEngine.Random.Range(0, bs.Count);
                                        PveEliminate tb = bs[randomIndex];

                                        //GameObject skill = Instantiate(Resources.Load(DungeonSpritePathManager.SkillBumpFX("skill_05"))) as GameObject;
                                        //skill.transform.position = tb.transform.position;

                                        CurBoss.SetPosition(tb.XPosition, tb.YPosition);
                                        callback();
                                    });
                                });
                            }
                            else
                            {
                                CurBoss.SetDisappear(int.Parse(CurBossData.BossSkill.Nparameter), () =>
                                {
                                    List<PveEliminate> bs = new List<PveEliminate>();
                                    foreach (PveEliminate b in GameControl.AllEliminates)
                                    {
                                        if (GameControl.HasEnemyOnPosition(b.XPosition, b.YPosition, null) == false && (GameControl.CurCharacter.XPosition != b.XPosition || GameControl.CurCharacter.YPosition != b.YPosition))
                                        {
                                            bs.Add(b);
                                        }
                                    }
                                    int randomIndex = UnityEngine.Random.Range(0, bs.Count);
                                    PveEliminate tb = bs[randomIndex];

                                    //GameObject skill = Instantiate(Resources.Load(DungeonSpritePathManager.SkillBumpFX("skill_05"))) as GameObject;
                                    //skill.transform.position = tb.transform.position;

                                    CurBoss.SetPosition(tb.XPosition, tb.YPosition);
                                    callback();
                                });
                            }
                        });
                    }
                    else
                    {
                        CurBoss.SetDisappear(int.Parse(CurBossData.BossSkill.Nparameter), () =>
                        {
                            List<PveEliminate> bs = new List<PveEliminate>();
                            foreach (PveEliminate b in GameControl.AllEliminates)
                            {
                                if (GameControl.HasEnemyOnPosition(b.XPosition, b.YPosition, null) == false && (GameControl.CurCharacter.XPosition != b.XPosition || GameControl.CurCharacter.YPosition != b.YPosition))
                                {
                                    bs.Add(b);
                                }
                            }
                            int randomIndex = UnityEngine.Random.Range(0, bs.Count);
                            PveEliminate tb = bs[randomIndex];

                            //GameObject skill = Instantiate(Resources.Load(DungeonSpritePathManager.SkillBumpFX("skill_05"))) as GameObject;
                            //skill.transform.position = tb.transform.position;

                            CurBoss.SetPosition(tb.XPosition, tb.YPosition);
                            callback();
                        });
                    }
                });
            }
            else
            {
                CurBoss.SetDisappear(int.Parse(CurBossData.BossSkill.Nparameter), () =>
                {
                    List<PveEliminate> bs = new List<PveEliminate>();
                    foreach (PveEliminate b in GameControl.AllEliminates)
                    {
                        if (GameControl.HasEnemyOnPosition(b.XPosition, b.YPosition, null) == false && (GameControl.CurCharacter.XPosition != b.XPosition || GameControl.CurCharacter.YPosition != b.YPosition))
                        {
                            bs.Add(b);
                        }
                    }
                    int randomIndex = UnityEngine.Random.Range(0, bs.Count);
                    PveEliminate tb = bs[randomIndex];

                    //GameObject skill = Instantiate(Resources.Load(DungeonSpritePathManager.SkillBumpFX("skill_05"))) as GameObject;
                    //skill.transform.position = tb.transform.position;

                    CurBoss.SetPosition(tb.XPosition, tb.YPosition);
                    callback();
                });
            }
        });

    }

    #region MOMO
    void OnEnable()
    {
        selfTrigger = BossSkillController.TriggerType.Round_Over;
    }
    #endregion
}
#endregion