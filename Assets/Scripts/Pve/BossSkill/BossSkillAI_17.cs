using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJson;

/// <summary>
/// 在地图配置位置有若干水晶(X参数描述位置),
/// 玩家回合开始,随机激活一个水晶.
/// 处于激活状态的水晶能够使停留在周围8格内的BOSS防御力=0(水晶对BOSS释放攻击动作,BOSS状态变为灰色,BOSS头上播放减防效果),
/// 水晶使BOSS减防或激活后经过Y次玩家回合后,该水晶变回非激活状态.玩家回合,玩家回合当地图上没有任何水晶处于激活状态时,
/// 随机激活任意一个水晶.水晶占据一个位置,玩家和BOSS都不能穿越,且无法破坏,也不会受到伤害.长按水晶可知其攻击范围.
/// </summary>
public class BossSkillAI_17 : BossSkillAI
{
    bool used = false;
    public List<PveCrystal> crystals = new List<PveCrystal>();
    string crystalResourcePath = "Prefabs/Pve/PveCrystal";

    public override bool ConditionCheck()
    {
        return base.ConditionCheck();
    }

    public void UseSkill(Action callback)
    {
        if(!used)
        {
            used = true;
            string pattern = "\\((.*?)\\)";

            List<string> cannonPositions = new List<string>();
            MatchCollection mc = Regex.Matches(CurBossData.BossSkill.Xparameter, pattern);
            foreach (Match m in mc)
            {
                cannonPositions.Add(m.Groups[1].Value);
            }

            for (int i = 0; i < cannonPositions.Count; i++) //init crystal
            {
                string[] tempPosition = cannonPositions[i].Split(',');
                InitCrystal(int.Parse(tempPosition[0]), int.Parse(tempPosition[1]));
            }
            selfTrigger = BossSkillController.TriggerType.Round_Start;
            callback();
        }
        else
        {
            int activeCrystalCount = 0;
            foreach(PveCrystal pc in crystals)
            {
                if(pc.activeState)
                {
                    activeCrystalCount++;

                    pc.BeginAction(() =>
                    {
                        callback();
                    });
                }
            }

            if(activeCrystalCount == 0)
            {
                System.Random r = new System.Random();
                int j = r.Next();
                Debug.Log("水晶随机值" + j);
                int i = j % crystals.Count;
                crystals[i].SetCrystalState(true);
                crystals[i].BeginAction(() => 
                {
                    callback();
                });
            }
        }
    }


    void InitCrystal(int xPosition, int yPosition)
    {
        GameObject crystalObject = NGUITools.AddChild(GameControl.FightUnitArea, Resources.Load<GameObject>(crystalResourcePath));
        PveCrystal Crystal = crystalObject.GetComponent<PveCrystal>();
        Crystal.CurState = PveFightUnit.UnitState.guard;
        Crystal.GameControl = GameControl;
        Crystal.Hp = 1;
        Crystal.CurHp = Crystal.Hp;
        Crystal.XRange = 1;
        Crystal.YRange = 1;
        Crystal.Def = 99999;
        JsonArray drop = new JsonArray();
        Crystal.DropList = drop;
        Crystal.shutDownCount = int.Parse(CurBossData.BossSkill.Yparameter);
        Crystal.SetPosition(xPosition, yPosition);
        GameControl.AllEnemies.Add(Crystal);

        Crystal.SetCrystalState(false);
        crystals.Add(Crystal);
    }

    void OnEnable()
    {
        selfTrigger = BossSkillController.TriggerType.Buff;
    }
}