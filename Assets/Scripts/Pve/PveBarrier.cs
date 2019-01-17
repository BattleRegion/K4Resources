using UnityEngine;
using System.Collections;
using System;

public class PveBarrier : PveEnemyUnit
{
    public override  void SetName()
    {
        name = "Barrier:" + XPosition + "," + YPosition;
    }

    public void RenderBarrier()
    {
        //RenderObject.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Atlas/Character/S1000/S10001050")[0];        
        gameObject.layer = LayerHelper.Unit;
        SetOrder();
    }

    public override void SetPosition(int xPosition, int yPosition)
    {
        base.SetPosition(xPosition, yPosition);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y -12, transform.localPosition.z);
    }

    public override void UnitDead(Action<PveFightUnit> curDeadAction)
    {
        curDeadAction(this);
    }
}