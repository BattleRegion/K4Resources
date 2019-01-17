using UnityEngine;
using System.Collections;

public class FightKey : FightItem
{

    #region 重写父类
    public override void SetName()
    {
        name = "Key:" + Id + "," + XPosition.ToString() + "," + YPosition.ToString();
        transform.localPosition = GetBasicPosition(XPosition, YPosition);
    }

    public override void ElementEventDeal(FightPlayer player)
    {
        
        //旋转
        Hashtable args = new Hashtable();
        args.Add("amount",new Vector3(0,0,1));
        args.Add("time", 0.6f);
        args.Add("easetype", iTween.EaseType.linear);
        args.Add("looptype", iTween.LoopType.loop);
        iTween.RotateBy(gameObject, args);

        Hashtable args1 = new Hashtable();
        args1.Add("position", new Vector3(transform.localPosition.x, transform.localPosition.y + 168, transform.localPosition.z));
        args1.Add("islocal", true);
        args1.Add("time", 0.6f);
        args1.Add("easetype", iTween.EaseType.linear);
        args1.Add("oncomplete", "KeyMoveEnd");
        args1.Add("oncompletetarget", gameObject);
        args1.Add("oncompleteparams", player);
        iTween.MoveTo(gameObject, args1);
    }


    void KeyMoveEnd(object param)
    {
        FightPlayer player = (FightPlayer)param;
        player.HasKey = true;
        FightDoor door = GameObject.Find("NextDoor").GetComponent<FightDoor>();
        door.OpenDoor();
        UnitHandler.UnitDestory(this);
        Destroy(gameObject);
        player.PlayerNextAction();
    }
    #endregion
}
