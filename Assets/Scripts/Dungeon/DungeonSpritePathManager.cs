using UnityEngine;
using System.Collections;

public class DungeonSpritePathManager {

    /// <summary>
    /// 地砖路径
    /// </summary>
    static public string TilePath = "Atlas/Fight/BasicTile";

    /// <summary>
    /// 消除块路径
    /// </summary>
    static public string EliminatePath = "Atlas/Fight/EliminateBlock";

    /// <summary>
    /// 技能消除块
    /// </summary>
    static public string SkillPath = "Atlas/Fight/SkillBlock";

    /// <summary>
    /// 单位动画路径
    /// </summary>
    /// <param name="skinId"></param>
    /// <param name="direction"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    static public string UnitAnmationPath(string skinId, DungeonEnum.FaceDirection direction,DungeonUnit.ActionState state)
    {
        return "Atlas/Character/" + skinId.ToString() + "/" + skinId.ToString() + ((int)state).ToString() + ((int)direction).ToString();
    }

    /// <summary>
    /// 宠物头像路径
    /// </summary>
    /// <param name="avataName"></param>
    /// <returns></returns>
    static public string PetAvataPath(string avataName)
    {
        return "Atlas/PetAvatars/" + avataName +"s"; 
    }

    /// <summary>
    /// 技能
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    static public string SkillBumpFX(string SkillPrefab)
    {      
        return "PreFabs/FX/" + SkillPrefab;
    }

    static public string SkillFX(string skillId)
    {
        string name = "";
        if (skillId == "PSk438")
        {
            name = "Skill_02";
        }
        else if (skillId == "PSk492")
        {
            name = "Skill_01";
        }
        else if (skillId == "PSk472")
        {
            name = "Skill_04";
        }
        else if (skillId == "PSk490")
        {
            name = "Skill_03";
        }
        else if (skillId == "PSk575")
        {
            name = "Skill_06";
        }
        return "PreFabs/FX/" + name;
    }
}
