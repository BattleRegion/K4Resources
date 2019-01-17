using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SimpleJson;

/// <summary>
/// BOSS每X回合后在Y元素属性方砖处召唤最多A个B怪物
/// </summary>
public class BossSkillAI_14 : BossSkillAI
{
    int roundCount = 0;

    public bool ConditionCheck()
    {
        if (!base.ConditionCheck()) return false;
        roundCount++;
        if (roundCount % int.Parse(CurBossData.BossSkill.Xparameter) == 0) return true;
        else return false;
    }

    public void UseSkill(Action callback)
    {
        BossEmptyAttack(() => 
        {
            DungeonEnum.ElementAttributes element = (DungeonEnum.ElementAttributes)int.Parse(CurBossData.BossSkill.Yparameter);
            MonsterData mData = ConfigManager.MonsterConfig.GetMonsterById(CurBossData.BossSkill.Bparameter);

            List<PveEliminate> tempEleminates = new List<PveEliminate>();
            List<PveEliminate> targetEleminates = new List<PveEliminate>();
            foreach (PveEliminate e in GameControl.AllEliminates)
            {
                if (e.CurEliminateAttribute == element && !GameControl.HasEnemyOnPosition(e.XPosition, e.YPosition, null))
                {
                    tempEleminates.Add(e);
                }
            }

            int count = int.Parse(CurBossData.BossSkill.Aparameter) < tempEleminates.Count ? int.Parse(CurBossData.BossSkill.Aparameter) : tempEleminates.Count;

            for (int i = 0; i < count; i++)
            {
                System.Random r = new System.Random();
                int rNum = r.Next(0, tempEleminates.Count - 1);
                PveEliminate e = tempEleminates[rNum];
                tempEleminates.Remove(e);
                targetEleminates.Add(e);
            }

            JsonArray drop = new JsonArray();

            if (!string.IsNullOrEmpty(CurBossData.BossSkill.FXPrefab1))
            {
                SkillRender(BossSkillController.GetSkillPrefab(CurBossData.BossSkill.FXPrefab1), CurBossData.BossSkill.FXType1, () => 
                {
                    foreach (PveEliminate e in targetEleminates)
                    {
                        GameControl.InitMonster(mData, e.XPosition, e.YPosition, 1, 1, drop);
                    }

                    callback();
                }, targetEleminates);
            }
        });
    }

    /// <summary>
    /// 暂时没用
    /// </summary>
    void SkillDetail()
    {
        DungeonEnum.ElementAttributes element = (DungeonEnum.ElementAttributes)int.Parse(CurBossData.BossSkill.Yparameter);
        MonsterData mData = ConfigManager.MonsterConfig.GetMonsterById(CurBossData.BossSkill.Bparameter);

        List<PveEliminate> tempEleminates = new List<PveEliminate>();
        List<PveEliminate> targetEleminates = new List<PveEliminate>();
        foreach(PveEliminate e in GameControl.AllEliminates)
        {
            if(e.CurEliminateAttribute == element && !GameControl.HasEnemyOnPosition(e.XPosition, e.YPosition, null))
            {
                tempEleminates.Add(e);
            }
        }

        int count = int.Parse(CurBossData.BossSkill.Aparameter) < tempEleminates.Count ? int.Parse(CurBossData.BossSkill.Aparameter) : tempEleminates.Count;

        for (int i = 0; i < count; i++)
        {
            System.Random r = new System.Random();
            int rNum = r.Next(0, tempEleminates.Count - 1);
            PveEliminate e = tempEleminates[rNum];
            tempEleminates.Remove(e);
            targetEleminates.Add(e);
        }

        JsonArray drop = new JsonArray();

        foreach(PveEliminate e in targetEleminates)
        {
            GameControl.InitMonster(mData, e.XPosition, e.YPosition, 1, 1, drop);
        }
    }

    #region mono
    void OnEnable()
    {
        selfTrigger = BossSkillController.TriggerType.Round_Over;
    }
    #endregion
}
