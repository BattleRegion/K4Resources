using UnityEngine;
using System.Collections;
using System;

public class PveTrap :PveEnemyUnit {

    public int DamagePersent;

    public GameObject TrapChild;

    public override void SetName()
    {
        name = "Trap:" + XPosition + "," + YPosition;
    }

    public override void SetOrder()
    {
        SpriteRenderer sr = TrapChild.GetComponent<SpriteRenderer>();
        gameObject.layer = LayerHelper.Basic;
        sr.sortingOrder = 2;
    }

    public override void SetPosition(int xPosition, int yPosition)
    {
        base.SetPosition(xPosition, yPosition);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y +28 , transform.localPosition.z);
    }

    //public override void UnitDead(Action<PveFightUnit> curDeadAction)
    //{
    //    curDeadAction(this);
    //}

    public void TrapAnimation()
    {
        gameObject.layer = LayerHelper.Top;
        TrapChild.layer = LayerHelper.Top;
        Invoke("delay", 0.2f);
    }

    void delay()
    {
        Animator a = GetComponent<Animator>();
        a.enabled = true;
        Invoke("delay1",0.3f);
    }

    void delay1()
    {
        SpriteRenderer sr = TrapChild.GetComponent<SpriteRenderer>();
        Animator a = GetComponent<Animator>();
        a.enabled = false;
        sr.sprite = Resources.LoadAll<Sprite>("Sprites/_Props/trap")[0];
        transform.localScale = new Vector3(1, 1, 1);
        Invoke("delay2", 0.2f);

		GameObject explosionItem = GameObject.Instantiate(Resources.Load("PreFabs/FX/FX52")) as GameObject;
		explosionItem.layer = LayerHelper.Top;
		explosionItem.transform.position = this.transform.position;
    }

    void delay2()
    {
        gameObject.layer = LayerHelper.Basic;
        TrapChild.layer = LayerHelper.Basic;
    }
}
