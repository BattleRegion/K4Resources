using UnityEngine;
using System.Collections;

public class BuffTypeEnum
{
	public static readonly string[] BuffTypeNameList = new string[]{
		"眩晕",
		"攻击",
		"换 HP",
		"普通攻击转 HP",
		"召唤",
		"恢复 HP",
		"恢复 生命 上限",
		"恢复 攻击",
		"恢复 防御",
		"恢复 闪避",
		"恢复 暴击",
		"恢复 暴击伤害",
		"恢复 能量",
		"恢复 能量 上限",
		"反击",
		"触发技能",
		"传送",
		"阻止眩晕",
		"阻止传送"
	};

	/// <summary>
	/// 眩晕
	/// </summary>
	public const int Dizziness = 1;

	/// <summary>
	/// 攻击
	/// </summary>
	public const int Attack = 2;

	/// <summary>
	/// 换 HP
	/// </summary>
	public const int Exchange_Hp = 3;
	
	/// <summary>
	/// 普通攻击转 HP
	/// </summary>
	public const int Exchange_Attack_Hp = 4;

	/// <summary>
	/// 召唤
	/// </summary>
	public const int Summon = 5;

	/// <summary>
	/// 恢复 HP
	/// </summary>
	public const int Recover_Hp = 6;
	
	/// <summary>
	/// 恢复 生命 上限
	/// </summary>
	public const int Recover_Hp_Limit = 7;
	
	/// <summary>
	/// 恢复 攻击
	/// </summary>
	public const int Recover_Attack = 8;
	
	/// <summary>
	/// 恢复 防御
	/// </summary>
	public const int Recover_Denfense = 9;
	
	/// <summary>
	/// 恢复 闪避
	/// </summary>
	public const int Recover_Avoid = 10;
	
	/// <summary>
	/// 恢复 暴击
	/// </summary>
	public const int Recover_Crit = 11;
	
	/// <summary>
	/// 恢复 暴击伤害
	/// </summary>
	public const int Recover_Crit_Hurt = 12;
	
	/// <summary>
	/// 恢复 能量
	/// </summary>
	public const int Recover_Energy = 13;
	
	/// <summary>
	/// 恢复 能量 上限
	/// </summary>
	public const int Recover_Energy_Limit = 14;

	/// <summary>
	/// 反击
	/// </summary>
	public const int Strike = 15;

	/// <summary>
	/// 触发技能
	/// </summary>
	public const int Trigger_Skill = 16;

	/// <summary>
	/// 传送
	/// </summary>
	public const int Transmit = 17;

	/// <summary>
	/// 阻止眩晕
	/// </summary>
	public const int Stop_Dizziness = 18;

	/// <summary>
	/// 阻止传送
	/// </summary>
	public const int Stop_Transmit = 19;
}
