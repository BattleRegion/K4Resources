using UnityEngine;
using System.Collections;
using System;

public class PveBoss : PveMonster
{
    public GameObject BossAppearFX;
    public int FightOff;
    public BossData CurBoss;

    /// <summary>
    /// 设置名字
    /// </summary>
    public override void SetName()
    {
        name = "Boss:" + XPosition + "," + YPosition;
    }

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
        AppearObject.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        AppearObject.SetActive(true);
        CameraControl.BossInShake();
    }
    #endregion

    #region Skill
    /// <summary>
    /// BOSS技能判断
    /// </summary> 
    public bool ReadyForBossSkill() 
    {
        bool condition = BossSkillController.SkillConditionCheck(this);
       // Debug.Log("SkillTime: " + PveGameControl.CurSkillTime.ToString() + "||" + "BossAIType: " + CurBoss.AIType + "||" + " Ready: " + condition);
        return condition;
    }

    /// <summary>
    /// BOSS使用技能
    /// </summary>
    public void UseSkill(Hashtable args, Action callback)
    {
        Debug.Log(name + " use " + CurBoss.AIType);
        BossSkillController.UseSkill(this, args, () =>
        {
            if (callback != null)
            {
                callback();
            }
        });
    }

    void Start()
    {
        BossSkillController.AddSkillAI(gameObject, CurBoss.AIType);
        PveGameControl.CurSkillTime = BossSkillController.TriggerType.Buff;
        if(ReadyForBossSkill())
        {
            UseSkill(null, null);
        }
    }
    #endregion
}