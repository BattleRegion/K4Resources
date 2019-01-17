using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

#region BossSkillAI_7
/// <summary>
/// BOSS回合开始时,时主角周围X参数内方砖变为Y属性的方砖.
/// </summary>
public class BossSkillAI_7 : BossSkillAI
{
    public bool ConditionCheck() //条件
    {
        if (!base.ConditionCheck()) return false;
        return true;
    }

    Action Callback;
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
                                    Callback = callback;
                                    ChangeRender();
                                    Invoke("DelayCallback", 2f);
                                });
                            }
                            else
                            {
                                Callback = callback;
                                ChangeRender();
                                Invoke("DelayCallback", 2f);
                            }
                        });
                    }
                    else
                    {
                        Callback = callback;
                        ChangeRender();
                        Invoke("DelayCallback", 2f);
                    }
                });
            }
            else
            {
                Callback = callback;
                ChangeRender();
                Invoke("DelayCallback", 2f);
            }
        });
    }

    void DelayCallback()
    {
        Callback();
    }

    void ChangeRender() //转换方砖
    {
        SkillData tempSkill = new SkillData(CurBossData.BossSkill);
        List<PveTile> SkillRangeTiles = GameControl.CaculateSkillRangeTile(GameControl.FindPveTile(CurCharacter.XPosition, CurCharacter.YPosition), tempSkill);
        foreach (PveTile tb in SkillRangeTiles)
        {
            StartCoroutine(ChangeBlock(GameControl.FineEliminate(tb.XPosition, tb.YPosition)));
        }
    }

    IEnumerator ChangeBlock(PveEliminate b)
    {
        yield return new WaitForSeconds(0.2f);
        if (b)
        {
            b.CurEliminateAttribute = (DungeonEnum.ElementAttributes)(int.Parse(CurBossData.BossSkill.Yparameter));
            b.ReturnToCommon();
        }
    }


    #region MONO
    void OnEnable()
    {
        selfTrigger = BossSkillController.TriggerType.Round_Start;
    }
    #endregion
}
#endregion

