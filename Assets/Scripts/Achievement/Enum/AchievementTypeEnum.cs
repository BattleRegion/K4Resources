using UnityEngine;
using System.Collections;

public class AchievementTypeEnum
{
	/// <summary>
	/// 剩余回合
	/// </summary>
	public static readonly int Residue_Round = 1;

	/// <summary>
	/// 剩余 HP
	/// </summary>
	public static readonly int Residue_Hp = 2;

	/// <summary>
	/// 消除 A 格
	/// </summary>
	public static readonly int Residue_Eliminate = 3;

	/// <summary>
	/// 使用 A 武器
	/// </summary>
	public static readonly int Use_Weapon = 4;

	/// <summary>
	/// 不使用主动技能
	/// </summary>
	public static readonly int No_Skill = 5;

	/// <summary>
	/// 战力值小等于 A 值
	/// </summary>
	public static readonly int Less_Fight = 6;

	/// <summary>
	/// 不携带 A 元素属性的英雄
	/// </summary>
	public static readonly int No_Element_Hero = 7;

	/// <summary>
	/// 不穿戴 A 元素属性的装备
	/// </summary>
	public static readonly int No_Element_Equip = 8;

	/// <summary>
	/// 终结技杀死 BOSS
	/// </summary>
	public static readonly int Chain_Kill_Boss = 9;

	/// <summary>
	/// 没有怪逃跑
	/// </summary>
	public static readonly int No_Monster_Escape = 10;
}
