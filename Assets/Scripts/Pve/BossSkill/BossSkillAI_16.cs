using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJson;

/// <summary>
/// 在地图的最上方或最两侧的边框上,有若干"大炮"(X参数描述位置),
/// 大炮可以对整个单条Y轴或X轴的所有怪物,BOSS和主角造成Y点攻击.
/// 在地图上有对应大炮数量和位置的触发器(A参数描述触发器位置).触发器开始处于关闭状态,
/// 只有玩家攻击后对其造成B次打击后,才会被激活.激活状态的触发器,上会有带有数字的箭头跳动,
/// 提示玩家手指按下该触发器就会触发对应箭头数字的"大炮"发射.
/// (大炮上也有编号,对应触发器被激活时,该大炮闪烁提示),发动完毕后该触发器,初始化.
/// </summary>
public class BossSkillAI_16 : BossSkillAI 
{
    int beHitCount = 0;
    bool used = false;
    string cannonResourcePath = "Prefabs/Pve/Cannon";
    string triggerResourcePath = "Prefabs/Pve/Trigger";

    public List<PveCannon> Cannons = new List<PveCannon>();

    public bool ConditionCheck()
    {
        if (!base.ConditionCheck()) return false;
        return !used;
    }

    public void UseSkill(Action callback)
    {
        used = true;

        string pattern = "\\((.*?)\\)";

        List<string> cannonPositions = new List<string>();
        MatchCollection mc = Regex.Matches(CurBossData.BossSkill.Xparameter, pattern);
        foreach (Match m in mc)
        {
            cannonPositions.Add(m.Groups[1].Value);
        }

        for (int i = 0; i < cannonPositions.Count; i++) //init cannon
        {
            string[] tempPosition = cannonPositions[i].Split(',');
            InitCannon(int.Parse(tempPosition[0]), int.Parse(tempPosition[1]), i + 1);
        }

        List<string> triggerPositions = new List<string>();
        MatchCollection t = Regex.Matches(CurBossData.BossSkill.Aparameter, pattern);
        foreach(Match m in t)
        {
            triggerPositions.Add(m.Groups[1].Value);
        }

        for (int i = 0; i < triggerPositions.Count; i++) //init trigger
        {
            string[] tempPosition = triggerPositions[i].Split(',');
            InitTrigger(int.Parse(tempPosition[0]), int.Parse(tempPosition[1]), i + 1);
        }

        callback();
    }

    void InitCannon(int xPosition, int yPosition, int  num)
    {
        GameObject CannonObject = NGUITools.AddChild(GameControl.FightUnitArea, Resources.Load<GameObject>(cannonResourcePath));
        PveCannon Cannon = CannonObject.GetComponent<PveCannon>();
        Cannon.GameControl = GameControl;
        Cannon.Number = num;
        Cannon.damage = float.Parse(CurBossData.BossSkill.Yparameter);
        Cannon.SetPosition(xPosition, yPosition);
        Cannon.RenderCannon();
        Cannons.Add(Cannon);
    }

    void InitTrigger(int xPosition, int yPosition, int num)
    {
        GameObject TriggerObject = NGUITools.AddChild(GameControl.FightUnitArea, Resources.Load<GameObject>(triggerResourcePath));
        PveCannonTrigger Trigger = TriggerObject.GetComponent<PveCannonTrigger>();
        Trigger.GameControl = GameControl;
        Trigger.Hp = float.Parse(CurBossData.BossSkill.Bparameter);
        Trigger.CurHp = Trigger.Hp;
        Trigger.XRange = 1;
        Trigger.YRange = 1;
        Trigger.Number = num;
        Trigger.Def = 99999;
        JsonArray drop = new JsonArray();
        Trigger.DropList = drop;
        Trigger.SetPosition(xPosition, yPosition);
        Trigger.RenderTrigger();
        GameControl.AllEnemies.Add(Trigger);
    }

    void OnEnable()
    {
        selfTrigger = BossSkillController.TriggerType.Buff;
    }
}
