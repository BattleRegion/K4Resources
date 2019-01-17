using UnityEngine;
using System.Collections;

public class SkillEffectExchangeAttackHp : BaseSkillEffect 
{
	protected float hurtValue;
	
	public SkillEffectExchangeAttackHp(PvpBuffData buffData, PvpFightUnit buffItem, BaseSkillItem skillItem, float hurtValue):base(buffData, buffItem, skillItem)
	{
		this.hurtValue = hurtValue;
	}
	
	public override void Run()
	{
		if(buffItem != null) buffItem.ChangeHpShow(this.hurtValue);
	}
}
