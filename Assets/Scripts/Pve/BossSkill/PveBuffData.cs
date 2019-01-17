using UnityEngine;
using System.Collections;

/// <summary>
/// buff数据
/// "atk" = AtkBuff  (float)
/// "def" = DefBuff  (float)
/// "hp" = HpBuff  (float)
/// "evade" = EvadeBuff  (float)
/// "vertigo" = VertigoBuff  (bool)
/// "movelimit" = MoveLimitBuff  (int, default unlimited = -1)
/// "guard" = GuardBuff (bool)
/// </summary>
public class PveBuffData
{
    #region 数值buff
    /// <summary>
    /// (1 + buff) * 属性值
    /// </summary>
    public float AtkBuff = 0f;  
    public float DefBuff = 0f;
    public float HpBuff = 0f;

    /// <summary>
    /// buff + 属性值
    /// </summary>
    public float EvadeBuff = 0f;
    #endregion

    #region 限制buff
    public bool VertigoBuff = false;
    public int MoveLimitBuff = -1;
    public bool GuardBuff = false;
    #endregion

    #region Buff作用时间
    public int LastTime = -1;
    #endregion

    /// <summary>
    /// 初始化buff
    /// </summary>
    /// "atk" = AtkBuff  (float)
    /// "def" = DefBuff  (float)
    /// "hp" = HpBuff  (float)
    /// "evade" = EvadeBuff  (float)
    /// "vertigo" = VertigoBuff  (bool)
    /// "movelimit" = MoveLimitBuff  (int, default unlimited = -1)
    /// "guard" = GuardBuff (bool)
    public PveBuffData(Hashtable args, int time = -1) //time回合数
    {
        LastTime = time;

        if (args.ContainsKey("atk")) AtkBuff = float.Parse(args["atk"].ToString());
        if (args.ContainsKey("def")) DefBuff = float.Parse(args["def"].ToString());
        if (args.ContainsKey("hp")) HpBuff = float.Parse(args["hp"].ToString());
        if (args.ContainsKey("evade")) EvadeBuff = float.Parse(args["evade"].ToString());
        if (args.ContainsKey("vertigo")) VertigoBuff = bool.Parse(args["vertigo"].ToString());
        if (args.ContainsKey("guard")) GuardBuff = bool.Parse(args["guard"].ToString());
        if (args.ContainsKey("movelimit")) MoveLimitBuff = int.Parse(args["movelimit"].ToString());
    }
}
