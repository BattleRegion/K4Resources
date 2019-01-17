using UnityEngine;
using System.Collections;

public class BuffStageTypeEnum
{
	/// <summary>
	/// 回合开始
	/// </summary>
	public const int Round_Begin = 1;
	
	/// <summary>
	/// 攻击（普攻|技能）
	/// </summary>
	public const int Attack = 2;

	/// <summary>
	/// 行走结束
	/// </summary>
	public const int Walk = 3;

	/// <summary>
	/// 固定加成，只在战斗前计算
	/// </summary>
	public const int Fixed = 4;

	/// <summary>
	/// 触发技能
	/// </summary>
	public const int Trigger_Skill = 5;
	
	/// <summary>
	/// 固定加成，只在战斗前计算，并且对宠物有效
	/// </summary>
	public const int Fixed_Pet = 6;
	
	/// <summary>
	/// 回合结束
	/// </summary>
	//public const int Round_End = 7;
	
	/// <summary>
	/// 行走结束
	/// </summary>
	//public const int Walk_Begin = 8;
}
