using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightPet : FightUnit {
    public string PetId;

    public int UserPetId;

    public bool HasAppear;

    //public FightControl Fc;

    public float Attack;

    //public List<FightEnemy> PathEnemy = new List<FightEnemy>();
    #region 重写父类
    public override void SetName()
    {
        name = "Pet:" +PetId+ "," + XPosition.ToString() + "," + YPosition.ToString();
    }
    #endregion

    #region 自己的方法
    public void Appear(int xPosition,int yPosition)
    {
        gameObject.SetActive(true);
        PetData data = ConfigManager.PetConfig.GetPetById(PetId);
        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(data.SkinId);
        SetRender(data.SkinId, xPosition, yPosition);
        SetName();
        HasAppear = true;
    }

    public void DisAppear()
    {
        gameObject.SetActive(false);
    }

    public void PetMove(RemoveBlock.LinkDirection direction, RemoveBlock moveToBlock)
    {
        //lastStayBlock = Fc.FindBlock(XPosition, YPosition);
        PetRun(direction);
        Hashtable args = new Hashtable();
        args.Add("position", moveToBlock.transform.localPosition);
        args.Add("time", 0.4f);
        args.Add("easetype", iTween.EaseType.linear);
        args.Add("islocal", true);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "moveToEnd");
        iTween.MoveTo(gameObject, args);
        XPosition = moveToBlock.XPosition;
        YPosition = moveToBlock.YPosition;
        SetName();
    }

    List<FightEnemy> curActionTargets;
    int curActionIndex = 0;
    void moveToEnd()
    {
        //curActionTargets = Fc.FindPetNeighbourEnemy(this);
        curActionIndex = 0;
        PetAction();
    }

    void PetAction()
    {
        if (curActionIndex == curActionTargets.Count)
        {
            Debug.Log("行为结束");
        }
        else
        {
            FightEnemy e = curActionTargets[curActionIndex];
            curActionIndex++;
        }
    }

    RemoveBlock.LinkDirection curRunDirection = RemoveBlock.LinkDirection.None;
    public void PetRun(RemoveBlock.LinkDirection direction)
    {
        if (curRunDirection != direction)
        {
            curRunDirection = direction;
            ChangeState(ActionState.Run, direction, null);
        }
    }

    /// <summary>
    /// 玩家攻击
    /// </summary>
    /// <param name="enemy"></param>
    public void PetAttack(FightEnemy enemy)
    {
        if (enemy.CurHp > 0)
        {
            //数据扣除
            enemy.CurHp = enemy.CurHp - Attack;
            enemy.curHurtDamage = (int)(Attack);
            RemoveBlock.LinkDirection attackDirection = GetTargetDirection(enemy);
            UnitAttack(attackDirection, () =>
            {
                ForceUnitWaiting(attackDirection);
            });
            StartCoroutine(AttackBump(enemy));
        }
        else
        {
            //PetAction();
        }
    }

    IEnumerator AttackBump(FightEnemy enemy)
    {
        float t = 0;
        for (int i = 0; i < FireFrame - 1; i++)
        {
            t = t + AttackFrameRate[i];
        }
        yield return new WaitForSeconds(t);
        enemy.Hurt(() =>
        {
            PetAction();
        }, true);
    }
    #endregion
}
