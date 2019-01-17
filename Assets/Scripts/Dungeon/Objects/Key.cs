using UnityEngine;
using System.Collections;
using System;

public class Key : EnemyUnit {

    #region 重写父类
    public override void SetObjectName()
    {
        name = "Key:" + XPosition + "," + YPosition;
    }

    /// <summary>
    /// 交互重写
    /// </summary>
    /// <param name="own"></param>
    public override void EnemyMutual(OwnUnit own, Action MutalEnd)
    {
        if (own.GetType() == typeof(Player))
        {
            AnimationHelper.AnimationRotateBy(new Vector3(0, 0, 1), iTween.LoopType.loop, gameObject, iTween.EaseType.linear, null, null, 0.4f);
            AnimationHelper.AnimationMoveTo(new Vector3(transform.localPosition.x, transform.localPosition.y + 168, transform.localPosition.z), gameObject, iTween.EaseType.linear, gameObject, "KeyMoveEnd", 0.4f);
        }
        MutalEnd();
    }

    public void KeyMoveEnd()
    {
        FightDoor door = GameObject.Find("NextDoor").GetComponent<FightDoor>();
        door.OpenDoor();
        DungeonScene.CurPlayer.HasFloorKey = true;
        ObjectHandler.ObjectDestoryFromDungeon(this);
    }
    #endregion
}
