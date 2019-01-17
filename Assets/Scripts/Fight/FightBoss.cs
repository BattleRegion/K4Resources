using UnityEngine;
using System.Collections;
public interface BossInter
{
    void BossAppearEnd();
}
public class FightBoss : FightMonster
{
    #region 属性
    public BossInter BossHandler;
    /// <summary>
    /// BOSS出现粒子
    /// </summary>
    public GameObject BossAppearFX;
    #endregion

    #region 重写父类
    public override void SetName()
    {
        name = "Boss:" +MonsterId +","+ XPosition.ToString() + "," + YPosition.ToString();
        gameObject.SetActive(false);
    }
    #endregion

    #region 自己的方法
    /// <summary>
    /// BOSS出现
    /// </summary>
    Vector3 orgPosition;
    public void BossAppear()
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
        
    }

    void BossInEnd()
    {
        GameObject AppearObject = Instantiate(BossAppearFX) as GameObject;
        AppearObject.transform.parent = transform.parent;
        AppearObject.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - elementSpriteRender.sprite.rect.height/2, transform.localPosition.z);
        AppearObject.SetActive(true);
        CameraControl.BossInShake();
        BossHandler.BossAppearEnd();
    }

    /// <summary>
    /// Boss技能
    /// </summary>
    public void BossSkill()
    {

    }
    #endregion
}
