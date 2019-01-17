using UnityEngine;
using System.Collections;

public class TreasureBox : FightItem
{
    #region 重写父类
    public override void SetName()
    {
        name = "Treasure:"+Id+"," + XPosition.ToString() + "," + YPosition.ToString();
    }

    public override void ElementEventDeal(FightPlayer player)
    {
        Debug.Log("打开宝箱");
        player.PlayerNextAction();
    }
    #endregion
}
