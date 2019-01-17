using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PvePet : PveOwnUnit
{
    #region 属性
    public UserPet CurUserPet;
    #endregion

    #region 资源指针
    public GameObject[] PetFlyPar;

    public GameObject[] PetFlyBumpPar;
    #endregion

    #region 重写父类
    public override void SetName()
    {
        name = "Pet:" + XPosition + "," + YPosition;
    }

    public override void OwnAction(Action actionEndCallback, System.Collections.Generic.List<PveEliminate> linkPath)
    {
        PetFlyToScene(() =>
        {
            base.OwnAction(actionEndCallback, linkPath);
        });
    }

    int curEnemiesCount;
    public override void TryAttack(Action attackEnd)
    {
        curEnemies = FindEnemies();
        if (curEnemies.Count == 0)
        {
            attackEnd();
        }
        else
        {
            //一个个攻击过来
            curEnemiesCount = 0;
            Attack();
        }
    }

    PveEnemyUnit curTargetPe;
    void Attack()
    {
        curTargetPe = curEnemies[curEnemiesCount];
		DungeonEnum.FaceDirection direction = GetNoXieDirection(curTargetPe);
        UnitAttack(direction);
        curEnemiesCount++;
    }


	DungeonEnum.FaceDirection GetNoXieDirection(PveFightUnit u)
	{
		if (u.XPosition < XPosition)
		{
			return DungeonEnum.FaceDirection.Left;
		}
		else if (u.XPosition > XPosition)
		{
			return DungeonEnum.FaceDirection.Right;
		}
		else if (u.YPosition < YPosition)
		{
			return DungeonEnum.FaceDirection.Down;
		}
		else if (u.YPosition > YPosition)
		{
			return DungeonEnum.FaceDirection.Up;
		}
		return DungeonEnum.FaceDirection.None;
	}


    public override void AttackBump()
    {
        GameControl.AttackCL.SetAddNum(1);
        if (curTargetPe) curTargetPe.BeHurt(this);
    }

    public override void AttackEnd()
    {
        UnitWaiting(CurFaceDirection);
        if (curEnemiesCount < curEnemies.Count)
        {
            Attack();
        }
        else
        {
            OnceStepAction();
        }
    }
    #endregion

    #region 飞行
    /// <summary>
    /// 宠物飞入场景
    /// </summary>
    PetAvata pa;
    Action ToSceneAction;
    Action ToAvataAction;
    public void PetFlyToScene(Action flyToSceneEnd, int skill = 0)
    {
        ToSceneAction = flyToSceneEnd;
        pa = GameControl.PvePlayerInfo.FindPetAvata(CurUserPet);
        pa.hasFocus = false;
        if (skill == 0)
        {
            PetParFly(transform.position, pa.transform.position);
        }
        else if (skill == 1)
        {
            ToSceneAction();
        }
		
    }

    /// <summary>
    /// 粒子飞行
    /// </summary>
    /// <param name="to"></param>
    GameObject curFlyPetPar;
    void PetParFly(Vector3 to, Vector3 from)
    {
        curFlyPetPar = Instantiate(GetPetFlyFX()) as GameObject;
        curFlyPetPar.transform.position = from;

        float rotateAngle = 0;
        float x = Mathf.Abs(to.x - from.x);
        float y = Mathf.Abs(to.y - from.y);
        rotateAngle = 90 - Mathf.Atan(y / x) * 180 / Mathf.PI;
        if (to.x > from.x)
        {
            rotateAngle = rotateAngle * -1;
        }
        curFlyPetPar.transform.localEulerAngles = new Vector3(0, 0, rotateAngle);
        AnimationHelper.AnimationScaleTo(new Vector3(1.2f, 1.2f, 1.2f), curFlyPetPar, iTween.EaseType.easeOutExpo, gameObject, "ParScaleBigEnd", 0.05f);
    }

    /// <summary>
    /// 粒子放大回调
    /// </summary>
    void ParScaleBigEnd()
    {
        AnimationHelper.AnimationScaleTo(new Vector3(0.4f, 1.8f, 1), curFlyPetPar, iTween.EaseType.easeInExpo, null, null, 0.1f);
        Invoke("ParFly", 0.05f);
    }

    /// <summary>
    /// 粒子飞行动画
    /// </summary>
    void ParFly()
    {
        PveTile pe;     
        pe = GameControl.FindPveTile(XPosition, YPosition);
                
        AnimationHelper.AnimationMoveTo(new Vector3(pe.transform.position.x, pe.transform.position.y , 0), curFlyPetPar, iTween.EaseType.linear, null, null, 0.2f);
        pa.AvataRender.gameObject.SetActive(false);
        FrameAnimation();
        Invoke("ParFlyEnd", 0.16f);
    }

    /// <summary>
    /// 飞行结束
    /// </summary>
    void ParFlyEnd()
    {
        AnimationHelper.AnimationScaleTo(new Vector3(0.5f, 0.5f, 0.5f), curFlyPetPar, iTween.EaseType.easeOutExpo, gameObject, "ParFlyEndScale", 0.05f);
    }

    void ParFlyEndScale()
    {
        Destroy(curFlyPetPar);
        //Debug.Log("---- 4  Destroy curflypetpar");
        GameObject parBump = Instantiate(GetBumpPar()) as GameObject;
        parBump.transform.position = transform.position;
        AnimationHelper.AnimationFadeTo(1, gameObject, iTween.EaseType.linear, gameObject, "FlyInEnd", 0.2f);

    }


    void FlyInEnd()
    {
        ToSceneAction();
    }

    /// <summary>
    /// 获取宠物飞行粒子
    /// </summary>
    /// <returns></returns>
    GameObject GetPetFlyFX()
    {
        if (CurUserPet.CurPetData.TempPetPro == DungeonEnum.ElementAttributes.Earth)
        {
            return PetFlyPar[1];
        }
        else if (CurUserPet.CurPetData.TempPetPro == DungeonEnum.ElementAttributes.Fire)
        {
            return PetFlyPar[2];
        }
        else if (CurUserPet.CurPetData.TempPetPro == DungeonEnum.ElementAttributes.Wind)
        {
            return PetFlyPar[3];
        }
        else if (CurUserPet.CurPetData.TempPetPro == DungeonEnum.ElementAttributes.Water)
        {
            return PetFlyPar[0];
        }
        return null;
    }

    GameObject GetBumpPar()
    {
        if (CurUserPet.CurPetData.TempPetPro == DungeonEnum.ElementAttributes.Earth)
        {
            return PetFlyBumpPar[1];
        }
        else if (CurUserPet.CurPetData.TempPetPro == DungeonEnum.ElementAttributes.Fire)
        {
            return PetFlyBumpPar[2];
        }
        else if (CurUserPet.CurPetData.TempPetPro == DungeonEnum.ElementAttributes.Wind)
        {
            return PetFlyBumpPar[3];
        }
        else if (CurUserPet.CurPetData.TempPetPro == DungeonEnum.ElementAttributes.Water)
        {
            return PetFlyBumpPar[0];
        }
        return null;
    }

    /// <summary>
    /// 飞回头像
    /// </summary>
    public void PetBackToAvata(Action backToAvata)
    {

       
        ToAvataAction = backToAvata;
        UnitWaiting(DungeonEnum.FaceDirection.Up);
        AnimationHelper.AnimationFadeTo(0, gameObject, iTween.EaseType.linear, null, null, 0.2f);
        PetFlyBack(pa.transform.position, transform.position);
    }

    void PetFlyBack(Vector3 to, Vector3 from)
    {
        curFlyPetPar = Instantiate(GetPetFlyFX()) as GameObject;
        curFlyPetPar.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        curFlyPetPar.transform.position = from;
        float rotateAngle = 0;
        float x = Mathf.Abs(to.x - from.x);
        float y = Mathf.Abs(to.y - from.y);
        rotateAngle = 90 - Mathf.Atan(y / x) * 180 / Mathf.PI;
        if (to.x < from.x)
        {
            rotateAngle = rotateAngle * -1;
        }
        curFlyPetPar.transform.localEulerAngles = new Vector3(0, 0, rotateAngle);
        ParBackScaleBigEnd();
    }

    void ParBackScaleBigEnd()
    {
        AnimationHelper.AnimationScaleTo(new Vector3(0.4f, 1.8f, 1), curFlyPetPar, iTween.EaseType.easeInExpo, null, null, 0.1f); 
        
        Invoke("ParBackFly", 0.05f);
    }

    void ParBackFly()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(pa.transform.position.x, pa.transform.position.y, 0), curFlyPetPar, iTween.EaseType.linear, null, null, 0.2f);
        Invoke("ParFlyBackEnd", 0.1f);
    }

    void ParFlyBackEnd()
    {
        //Debug.Log("---- 7");
        //AnimationHelper.AnimationScaleTo(new Vector3(1.2f, 1.2f, 1.2f), curFlyPetPar, iTween.EaseType.easeOutExpo, gameObject, "ParBackFlyEndScale", 0.05f);
        pa.AvataRender.gameObject.SetActive(true);
        FrameAnimation();

        ParBackFlyEndScale();
    }

    void ParBackFlyEndScale()
    {
        //Debug.Log("---- 8  Destroy curflypetpar");
        Destroy(curFlyPetPar);
        GameObject parBump = Instantiate(GetBumpPar()) as GameObject;
        parBump.transform.position = pa.transform.position;
        ToAvataAction();
        Invoke("FrameNormalEnd", 1f);
    }

    void FrameAnimation()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(pa.transform.localPosition.x, -30, pa.transform.localPosition.z), pa.gameObject, iTween.EaseType.linear, null, "", 0.1f);
        AnimationHelper.AnimationScaleTo(new Vector3(1.2f, 1.2f, 1.2f), pa.gameObject, iTween.EaseType.easeOutExpo, gameObject, "FrameScaleEnd", 0.1f);
    }
    //petavata frame scale
    void FrameScaleEnd()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(pa.transform.localPosition.x, -26, pa.transform.localPosition.z), pa.gameObject, iTween.EaseType.linear, null, "", 0.05f);
        AnimationHelper.AnimationScaleTo(new Vector3(1, 1, 1), pa.gameObject, iTween.EaseType.easeInExpo, null, "", 0.05f);
        
        
    }
    void FrameNormalEnd()
    {
        gameObject.SetActive(false);
    }
    #endregion


}
