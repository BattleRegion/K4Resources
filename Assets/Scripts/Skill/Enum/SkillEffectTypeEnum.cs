using System;

public class SkillEffectTypeEnum
{
	/// <summary>
	/// 错误数据
	/// </summary>
	public const int NONE = 0;

	/// <summary>
	/// 光环
	/// </summary>
	public const int FIXED = 1;

	/// <summary>
	/// 敌方单体
	/// </summary>
	public const int ENEMY_SINGLE = 2;

	/// <summary>
	/// 我方单体
	/// </summary>
	public const int SELF_SINGLE = 3;

	/// <summary>
	/// 飞向敌方
	/// </summary>
	public const int FLY_TO_ENEMY = 4;

	/// <summary>
	/// 坐标逐个
	/// </summary>
	public const int ITEM = 5;

	/// <summary>
	/// 我方 Buff
	/// </summary>
	public const int SELF_BUFF = 6;

	/// <summary>
	/// 敌方 Buff
	/// </summary>
	public const int ENEMY_BUFF = 7;

	/// <summary>
	/// 召唤
	/// </summary>
	public const int SUMMON = 8;
}

