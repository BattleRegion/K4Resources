using UnityEngine;
using System.Collections;

public class SkillEffectExchangeHp : BaseSkillEffect
{
	protected float hurtValue;
	
	public SkillEffectExchangeHp(PvpBuffData buffData, PvpFightUnit buffItem, BaseSkillItem skillItem, float hurtValue):base(buffData, buffItem, skillItem)
	{
		this.hurtValue = hurtValue;
	}
	
	public override void Run()
	{
		if(buffItem != null) buffItem.ChangeHpShow(this.hurtValue);
	}
}
