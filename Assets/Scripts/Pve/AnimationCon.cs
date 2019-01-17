using UnityEngine;
using System.Collections;

public class AnimationCon : MonoBehaviour {

    void AttackBump()
    {
        PveFightUnit fu = transform.parent.gameObject.GetComponent<PveFightUnit>();
        if (fu)
        {

            if (fu.UsingSkill)
            {

            }
            else if (fu.GameControl.IsSkilling == true)
            {
                fu.SkillAttackBump();
            }
            else
            {
                if (fu.IsChain)
                {
                    fu.ChainAttackBump();
                    //Debug.Log("fu.ChainAttackBump");
                }
                else
                {
                    fu.AttackBump();
                    //Debug.Log("fu.AttackBump");
                }
            }
        }
    }
    void AttackEnd()
    {
        PveFightUnit fu = transform.parent.gameObject.GetComponent<PveFightUnit>();
        if (fu)
        {
            if (fu.UsingSkill)
            {
                fu.ChangeAnimation(fu.CurFaceDirection, PveFightUnit.ActionState.Waiting);
                fu.UsingSkill = false;
                fu.GetComponent<BossSkillAI>().DelayAttackEnd();
            }
            else if (fu.GameControl.IsSkilling == true)
            {
                fu.SkillAttackEnd();
            }
            else
            {
                if (fu.IsChain)
                {
                    fu.ChainAttackEnd();
                }
                else
                {
                    fu.AttackEnd();
                }
            }
        }
    }
    void BeHurtEnd()
    {
        PveFightUnit fu = transform.parent.gameObject.GetComponent<PveFightUnit>();
        if (fu)
        {
            fu.BeHurtEnd();
        }
    }
    
    void ChainAttackBump()
    {
        PveOwnUnit fu = transform.parent.gameObject.GetComponent<PveOwnUnit>();
       
        if (fu)
        {
            if (fu.GameControl.IsSkilling == false)
            {
                if (fu.IsChain)
                {
                    fu.ChainAttackBump();
                }
               
            }
            else
            {
                fu.SkillAttackBump();
            }
        }        
    }

    void ChainAttackEnd()
    {
        PveFightUnit fu = transform.parent.gameObject.GetComponent<PveFightUnit>();
      
        if (fu)
        {
            if (fu.GameControl.IsSkilling == false)
            {
                if (fu.IsChain)
                {
                    fu.BeHurtEnd();
                }              
            }
            else
            {
                fu.SkillAttackEnd();
            }
        }

    }
    

	void MagicEnd()
	{
		PvpFightUnit fu = transform.parent.gameObject.GetComponent<PvpFightUnit>();
		if (fu)
		{
			fu.MagicEnd();
		}
	}

    void DeadEnd()
    {
        PveFightUnit fu = transform.parent.gameObject.GetComponent<PveFightUnit>();
        if (fu && fu.name.IndexOf("Barrier")!=0)
        {
            //Debug.Log(fu.name+" JC ."); 
            fu.DeadEnd();
        }
    }
}
