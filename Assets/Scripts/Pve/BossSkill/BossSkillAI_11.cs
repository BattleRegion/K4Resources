using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BossSkillAI_11 : BossSkillAI
{
    public override bool ConditionCheck()
    {
        if (!base.ConditionCheck()) return false;
        return true;
    }

    public void UseSkill(Action callback)
    {

    }
}
