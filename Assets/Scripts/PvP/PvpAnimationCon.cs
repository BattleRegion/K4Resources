using UnityEngine;
using System.Collections;

public class PvpAnimationCon : MonoBehaviour {


    void AttackEnd()
    {
        PvpFightUnit fu = transform.parent.gameObject.GetComponent<PvpFightUnit>();
        if (fu)
		{
			if(fu.StrickeStatus)
			{
				Debug.Log("调用反击攻击动作结束！！！！！！！" + fu.GameControl.IsSkilling + ":" + fu.IsChain);
			}

            if (fu.GameControl.IsSkilling == false)
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
            else
            {
                fu.SkillAttackEnd();
            }
        }
    }
    void BeHurtEnd()
    {
        PvpFightUnit fu = transform.parent.gameObject.GetComponent<PvpFightUnit>();
        if (fu)
        {
            fu.BeHurtEnd();
        }
    }
    
    void ChainAttackBump()
    {
        PvpOwnUnit fu = transform.parent.gameObject.GetComponent<PvpOwnUnit>();
       
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
        PvpFightUnit fu = transform.parent.gameObject.GetComponent<PvpFightUnit>();
      
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
    void AttackBump()
    {
        PvpFightUnit fu = transform.parent.gameObject.GetComponent<PvpFightUnit>();
        if (fu)
        {
            if (fu.GameControl.IsSkilling == false)
            {
                if (fu.IsChain)
                {
                    fu.ChainAttackBump();
                }
                else
                {
					fu.AttackBump();
                }
            }
            else
            {
                fu.SkillAttackBump();
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
        PvpFightUnit fu = transform.parent.gameObject.GetComponent<PvpFightUnit>();
        if (fu && fu.name.IndexOf("Barrier")!=0)
        {
			fu.DeadEnd();
        }
    }
}
