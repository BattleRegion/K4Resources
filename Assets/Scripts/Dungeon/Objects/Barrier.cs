using UnityEngine;
using System.Collections;
using System;

public class Barrier : EnemyUnit
{

    #region 重写父类
    public override void SetObjectName()
    {
        name = "Barrier:" + XPosition + "," + YPosition;
    }

    /// <summary>
    /// 交互重写
    /// </summary>
    /// <param name="own"></param>
    public override void EnemyMutual(OwnUnit own, Action MutalEnd)
    {
        MutalEnd();
    }
    #endregion
}
