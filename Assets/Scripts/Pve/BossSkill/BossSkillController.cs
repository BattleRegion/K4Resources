using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// BOSS技能，回合前技能在PveEnemy中调用，回合后技能在PveGameControl中调用
/// </summary>
public class BossSkillController
{
    /// <summary>
    /// 类型
    /// </summary>
    public enum SkillType
    {
        None = 0,
        BossAI_1 = 1,
        BossAI_2 = 2,
        BossAI_3 = 3,
        BossAI_4 = 4,
        BossAI_5 = 5,
        BossAI_6 = 6,
        BossAI_7 = 7,
        BossAI_8 = 8,
        BossAI_9 = 9,
        BossAI_10 = 10,
        BossAI_11 = 11,
        BossAI_12 = 12,
        BossAI_13 = 13,
        BossAI_14 = 14,
        BossAI_15 = 15,
        BossAI_16 = 16,
        BossAI_17 = 17,
        BossAI_18 = 18
    }

    /// <summary>
    /// 触发时机
    /// </summary>
    public enum TriggerType
    {
        Default = 0,
        Round_Start = 1,
        Round_Over = 2,
        Attack = 3,
        Hurt = 4,
        Move = 5,
        Buff = 6,
        UnderAttack = 7,
        Player_Attack = 8
    }

    /// <summary>
    /// 开场时在BOSS上添加AI脚本并给属性赋值
    /// </summary>
    public static void AddSkillAI(GameObject boss, SkillType AIType)
    {
        switch(AIType)
        {
            case SkillType.BossAI_1:
                {
                    BossSkillAI_1 AIComponent = boss.AddComponent<BossSkillAI_1>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_2:
                {
                    BossSkillAI_2 AIComponent = boss.AddComponent<BossSkillAI_2>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_3:
                {
                    BossSkillAI_3 AIComponent = boss.AddComponent<BossSkillAI_3>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_4:
                {
                    BossSkillAI_4 AIComponent = boss.AddComponent<BossSkillAI_4>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_5:
                {
                    BossSkillAI_5 AIComponent = boss.AddComponent<BossSkillAI_5>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_6:
                {
                    BossSkillAI_6 AIComponent = boss.AddComponent<BossSkillAI_6>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_7:
                {
                    BossSkillAI_7 AIComponent = boss.AddComponent<BossSkillAI_7>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_8:
                {
                    BossSkillAI_8 AIComponent = boss.AddComponent<BossSkillAI_8>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_9:
                {
                    BossSkillAI_9 AIComponent = boss.AddComponent<BossSkillAI_9>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_10:
                {
                    BossSkillAI_10 AIComponent = boss.AddComponent<BossSkillAI_10>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_11:
                {
                    BossSkillAI_11 AIComponent = boss.AddComponent<BossSkillAI_11>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_12:
                {
                    BossSkillAI_12 AIComponent = boss.AddComponent<BossSkillAI_12>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_13:
                {
                    BossSkillAI_13 AIComponent = boss.AddComponent<BossSkillAI_13>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_14:
                {
                    BossSkillAI_14 AIComponent = boss.AddComponent<BossSkillAI_14>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_15:
                {
                    BossSkillAI_15 AIComponent = boss.AddComponent<BossSkillAI_15>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_16:
                {
                    BossSkillAI_16 AIComponent = boss.AddComponent<BossSkillAI_16>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_17:
                {
                    BossSkillAI_17 AIComponent = boss.AddComponent<BossSkillAI_17>();
                    AIComponent.AI = AIType;
                    break;
                }
            case SkillType.BossAI_18:
                {
                    BossSkillAI_18 AIComponent = boss.AddComponent<BossSkillAI_18>();
                    AIComponent.AI = AIType;
                    break;
                }
        }

    }


    /// <summary>
    /// 检查就绪情况
    /// </summary>
    public static bool SkillConditionCheck(PveBoss boss)
    {
        switch (boss.CurBoss.AIType)
        {
            case SkillType.BossAI_1:
                {
                    return boss.GetComponent<BossSkillAI_1>().ConditionCheck();
                }
            case SkillType.BossAI_2:
                {
                    return boss.GetComponent<BossSkillAI_2>().ConditionCheck();
                }
            case SkillType.BossAI_3:
                {
                    return boss.GetComponent<BossSkillAI_3>().ConditionCheck();
                }
            case SkillType.BossAI_4:
                {
                    return boss.GetComponent<BossSkillAI_4>().ConditionCheck();
                }
            case SkillType.BossAI_5:
                {
                    return boss.GetComponent<BossSkillAI_5>().ConditionCheck();
                }
            case SkillType.BossAI_6:
                {
                    return boss.GetComponent<BossSkillAI_6>().ConditionCheck();
                }
            case SkillType.BossAI_7:
                {
                    return boss.GetComponent<BossSkillAI_7>().ConditionCheck();
                }
            case SkillType.BossAI_8:
                {
                    return boss.GetComponent<BossSkillAI_8>().ConditionCheck();
                }
            case SkillType.BossAI_9:
                {
                    return boss.GetComponent<BossSkillAI_9>().ConditionCheck();
                }
            case SkillType.BossAI_10:
                {
                    return boss.GetComponent<BossSkillAI_10>().ConditionCheck();
                }
            case SkillType.BossAI_11:
                {
                    return boss.GetComponent<BossSkillAI_11>().ConditionCheck();
                }
            case SkillType.BossAI_12:
                {
                    return boss.GetComponent<BossSkillAI_12>().ConditionCheck();
                }
            case SkillType.BossAI_13:
                {
                    return boss.GetComponent<BossSkillAI_13>().ConditionCheck();
                }
            case SkillType.BossAI_14:
                {
                    return boss.GetComponent<BossSkillAI_14>().ConditionCheck();
                }

            case SkillType.BossAI_15:
                {
                    return boss.GetComponent<BossSkillAI_15>().ConditionCheck();
                }
            case SkillType.BossAI_16:
                {
                    return boss.GetComponent<BossSkillAI_16>().ConditionCheck();
                }
            case SkillType.BossAI_17:
                {
                    return boss.GetComponent<BossSkillAI_17>().ConditionCheck();
                }
            case SkillType.BossAI_18:
                {
                    return boss.GetComponent<BossSkillAI_18>().ConditionCheck();
                }

        }
        return false;
    }

    /// <summary>
    /// 解析参数，调用技能
    /// </summary>
    public static void UseSkill(PveBoss boss, Hashtable args, Action callback)
    {
        switch (boss.CurBoss.AIType)
        {
            case SkillType.BossAI_1:
                {
                    boss.GetComponent<BossSkillAI_1>().UseSkill(() =>
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_2:
                {
                    float damage = (float)args["damage"];
                    boss.GetComponent<BossSkillAI_2>().UseSkill(damage, () =>
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_3:
                {
                    boss.GetComponent<BossSkillAI_3>().UseSkill(() => 
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_4:
                {
                    boss.GetComponent<BossSkillAI_4>().UseSkill(() =>
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_5:
                {
                    boss.GetComponent<BossSkillAI_5>().UseSkill(() => 
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_6:
                {
                    boss.GetComponent<BossSkillAI_6>().UseSkill(() =>
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_7:
                {
                    boss.GetComponent<BossSkillAI_7>().UseSkill(() =>
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_8:
                {
                    boss.GetComponent<BossSkillAI_8>().UseSkill(() =>
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_9:
                {
                    boss.GetComponent<BossSkillAI_9>().UseSkill(() =>
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_10:
                {
                    boss.GetComponent<BossSkillAI_10>().UseSkill(() =>
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_11:
                {
                    boss.GetComponent<BossSkillAI_11>().UseSkill(() =>
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_12:
                {
                    boss.GetComponent<BossSkillAI_12>().UseSkill(() =>
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_13:
                {
                    boss.GetComponent<BossSkillAI_13>().UseSkill(() => 
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_14:
                {
                    boss.GetComponent<BossSkillAI_14>().UseSkill(() => 
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_15:
                {
                    boss.GetComponent<BossSkillAI_15>().UseSkill(() => 
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_16:
                {
                    boss.GetComponent<BossSkillAI_16>().UseSkill(() => 
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_17:
                {
                    boss.GetComponent<BossSkillAI_17>().UseSkill(() =>
                    {
                        callback();
                    });
                    break;
                }
            case SkillType.BossAI_18:
                {
                    boss.GetComponent<BossSkillAI_18>().UseSkill(() =>
                    {
                        callback();
                    });
                    break;
                }
        }
    }

    public static readonly string ResourcePath = "PreFabs/FX/";

    /// <summary>
    /// 获得技能预设
    /// </summary>
    public static GameObject GetSkillPrefab(string skillPrefab)
    {
        return Resources.Load<GameObject>(ResourcePath + skillPrefab);
    }
}