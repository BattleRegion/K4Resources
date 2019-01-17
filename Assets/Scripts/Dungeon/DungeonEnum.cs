using UnityEngine;
using System.Collections;

public class DungeonEnum  {

    /// <summary>
    /// 属性枚举
    /// </summary>
    public enum ElementAttributes
    {
        None = 0,
        Earth = 1,
        Fire = 2,
        Wind = 3,
        Water = 4,
        Player = 5
    }
    //风3>>水4>>火2>>地1>>风3

    /// <summary>
    /// 攻击类型枚举
    /// </summary>
    public enum AttackType
    {
        Close = 1,
        Far = 2
    }

    public enum FaceDirection
    {
        None = 0,
        Up = 10,
        UpRight = 20,
        Right = 30,
        RightDown =40,
        Down = 50,
        LeftDown = 60,
        Left = 70,
        LeftUp = 80
    }
}
