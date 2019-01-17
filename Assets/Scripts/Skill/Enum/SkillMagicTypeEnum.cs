using UnityEngine;
using System.Collections;

public class SkillMagicTypeEnum
{
	/// <summary>
	/// 主动终结
	/// </summary>
	public const int InitiativeChain = 0;

	/// <summary>
	/// 主动即时
	/// </summary>
	public const int InitiativeNow = 1;

	/// <summary>
	/// 主动延时
	/// </summary>
	public const int InitiativeDelay = 2;

	/// <summary>
	/// 被动
	/// </summary>
	public const int Passiveness = 3;
}
