using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 在地图最上方的墙面上会有若干图形,
/// 门的位置会有一个魔法球.玩家如果按照墙面上的提示,
/// 消除规定形状的方砖后就会激活该图形,当所有图形被激活后,
/// 魔法球就会对BOSS发动A点攻击.攻击完毕后初始化.
/// </summary>
public class BossSkillAI_18 : BossSkillAI
{
    public bool ConditionCheck()
    {
        return base.ConditionCheck();
    }

    public void UseSkill(Action callback)
    {
        callback();
    }

}
