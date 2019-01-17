using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// BOSS如果攻击不到玩家时,将在玩家周围X参数上施加Y的封印效果(一种地图元素),
/// 主角普通攻击无法攻击封印,但是可以使用终结技或范围技能对其进行攻击(障碍物?),
/// BOSS可以经过封印.封印持续N回合后自动消失.(封印有血量标示)
/// </summary>
public class BossSkillAI_13 : BossSkillAI
{
    int roundCount;

    /// <summary>
    /// 技能创建的barrier和存在的回合数
    /// </summary>
    List<PveBarrier> barriers = new List<PveBarrier>();
    List<int> barrierCounts = new List<int>();

    public bool ConditionCheck()
    {
        if(!base.ConditionCheck()) return false;

        roundCount++;
        for(int i = barriers.Count - 1; i >= 0; i--)
        {
            PveBarrier key = barriers[i];
            int Value = barrierCounts[i];

            if(key == null)
            {
                barriers.Remove(key);
                barrierCounts.Remove(Value);
            }
            else if (roundCount >= Value)
            {
                //GameObject disObject = null;
                //disObject = Instantiate(Resources.Load("PreFabs/FX/Stone_D")) as GameObject;
                //disObject.transform.position = key.transform.position;
                GameControl.AllEnemies.Remove((PveEnemyUnit)key);
                barriers.Remove(key);
                barrierCounts.Remove(Value);
                Destroy(key.gameObject);
            }
        }

        List<PveEliminate> moveRange = GameControl.FindRunPowerEliminates(CurBoss);

        Debug.Log(GameControl.FindAttackEliminates(CurBoss, moveRange).Contains(CurCharacter.CurElim));
        if (GameControl.FindAttackEliminates(CurBoss, moveRange).Contains(CurCharacter.CurElim)) return false;
        else return true;
    }

    public void UseSkill(Action callback)
    {
        BossEmptyAttack(() =>
        {
            SkillData tempSkill = new SkillData(CurBossData.BossSkill);
            PveTile position = GameControl.FindPveTile(CurCharacter.XPosition, CurCharacter.YPosition);
            List<PveTile> targetTiles = GameControl.CaculateSkillRangeTile(position, tempSkill);
            PveTile characterPosition = GameControl.FindPveTile(CurCharacter.XPosition, CurCharacter.YPosition);
            if (targetTiles.Contains(characterPosition)) targetTiles.Remove(characterPosition);
            List<PveBarrier> skillBarriers = new List<PveBarrier>();
            int lastCount = int.Parse(CurBossData.BossSkill.Nparameter);

            if (!string.IsNullOrEmpty(CurBossData.BossSkill.FXPrefab1))
            {
                SkillRender(BossSkillController.GetSkillPrefab(CurBossData.BossSkill.FXPrefab1), CurBossData.BossSkill.FXType1, () =>
                {
                    foreach (PveTile pt in targetTiles)
                    {
                        if (!GameControl.HasEnemyOnPosition(pt.XPosition, pt.YPosition, null))
                        {
                            BarrierData b = ConfigManager.BarrierConfig.GetBarrierById(CurBossData.BossSkill.Yparameter);
                            PveBarrier pb = GameControl.InitBarrier(b, pt.XPosition, pt.YPosition, 1, 1, null);
                            skillBarriers.Add(pb);
                            barriers.Add(pb);
                            barrierCounts.Add(roundCount + lastCount);
                        }
                    }

                    callback();
                }, null, targetTiles);
            }
        });
    }

    /// <summary>
    /// 暂时不用
    /// </summary>
    void SkillDetail()
    {
        SkillData tempSkill = new SkillData(CurBossData.BossSkill);
        PveTile position = GameControl.FindPveTile(CurCharacter.XPosition, CurCharacter.YPosition);
        List<PveTile> targetTiles = GameControl.CaculateSkillRangeTile(position, tempSkill);
        PveTile characterPosition = GameControl.FindPveTile(CurCharacter.XPosition, CurCharacter.YPosition);
        if (targetTiles.Contains(characterPosition)) targetTiles.Remove(characterPosition);
        List<PveBarrier> skillBarriers = new List<PveBarrier>();
        int lastCount = int.Parse(CurBossData.BossSkill.Nparameter);
        foreach(PveTile pt in targetTiles)
        {
            if(!GameControl.HasEnemyOnPosition(pt.XPosition, pt.YPosition, null))
            {
                BarrierData b = ConfigManager.BarrierConfig.GetBarrierById(CurBossData.BossSkill.Yparameter);
                PveBarrier pb = GameControl.InitBarrier(b, pt.XPosition, pt.YPosition, 1, 1, null);
                skillBarriers.Add(pb);
                barriers.Add(pb);
                barrierCounts.Add(roundCount + lastCount);
            }
        }
    }

    #region mono
    void OnEnable()
    {
        roundCount = 0;
        selfTrigger = BossSkillController.TriggerType.Round_Start;
    }
    #endregion
}
