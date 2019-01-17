using UnityEngine;
using System.Collections;

public class Boss : Monster
{
    #region 属性
    public int FightOff;
    #endregion

    #region 资源指针
    public GameObject BossAppearFX;
    #endregion

    #region 重写MONO
    void Awake()
    {
    }
    #endregion

    #region 重写父类
    /// <summary>
    /// 设置名字
    /// </summary>
    public override void SetObjectName()
    {
        name = "Boss:" + XPosition + "," + YPosition;
    }

    /// <summary>
    /// 消失
    /// </summary>
    public override void ObjectDisAppear()
    {
        base.ObjectDisAppear();
    }

    public override void BeHurt(DungeonUnit hurtFrom, bool needDelay)
    {
        base.BeHurt(hurtFrom, needDelay);
        if (CurHp <= 0)
        {
            DungeonScene.CurPlayer.GodMode = true;
        }
    }
    #endregion

    #region BOSS出现
    public void BossAppearRender()
    {
        Hashtable args = new Hashtable();
        args.Add("position", new Vector3(transform.localPosition.x, 650, transform.localPosition.z));
        args.Add("islocal", true);
        args.Add("time", 0.8f);
        args.Add("easetype", iTween.EaseType.easeInQuart);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "BossInEnd");
        gameObject.SetActive(true);
        iTween.MoveFrom(gameObject, args);
        gameObject.SetActive(true);
    }

    void BossInEnd()
    {
        GameObject AppearObject = Instantiate(BossAppearFX) as GameObject;
        AppearObject.transform.parent = transform.parent;
        AppearObject.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - ObjectSprite.sprite.rect.height / 2, transform.localPosition.z);
        AppearObject.SetActive(true);
        CameraControl.BossInShake();
    }
    #endregion
}
