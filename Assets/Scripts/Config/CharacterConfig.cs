using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
public class CharacterConfig : GameConfig 
{
    public CharacterConfig()
    {
        this.ConfigName = "Characters";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            CharacterData cd = new CharacterData(data);
            Configs.Add(cd);
        }
    }

    public CharacterData CharacterById(string Id)
    {
        foreach (CharacterData c in Configs)
        {
            if (c.Id == Id)
            {
                return c;
            }
        }
        return null;
    }
}


/// <summary>
/// 单个角色数据结构
/// </summary>
public class CharacterData:ConfigData
{
    /// <summary>
    /// 标识
    /// </summary>
    public string Id;

    /// <summary>
    /// 攻击力
    /// </summary>
    public float Attack;

    /// <summary>
    /// 生命
    /// </summary>
    public float Hp;

    /// <summary>
    /// 皮肤ID
    /// </summary>
    public string SkinId;

    public CharacterData(JsonObject data)
    {
        Id = data["characterId"].ToString();
        Attack = float.Parse(data["Attack"].ToString());
        Hp = int.Parse(data["Hp"].ToString());
        SkinId = data["SkinId"].ToString();
        SkinId = "S1";
    }

}