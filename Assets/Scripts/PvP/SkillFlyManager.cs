using UnityEngine;
using System.Collections.Generic;

public class SkillFlyManager
{
	private static List<string> nameList = new List<string> ();

	public static void Run(string text, SkillFlyItem skillFlyItem)
	{
		if(nameList == null) nameList = new List<string>();
		nameList.Add (text);

		// 如果数量为 1，直接运行
		if(nameList.Count == 1)
		{
			// 调用子数据
			RunItem (skillFlyItem);
		}
	}

	private static void RunItem(SkillFlyItem skillFlyItem)
	{
		if(nameList.Count > 0)
		{
			string name = nameList [0];
			skillFlyItem.Run(name, ()=> 
			{
				// 移除第一个位置数据
				nameList.RemoveAt(0);
				// 递归调用
				RunItem(skillFlyItem);
			});
		}
	}
}
