using UnityEngine;
using System.Collections.Generic;

public class PvpMessageManager
{
	private static List<BasePvpMessage> messageList = new List<BasePvpMessage>();

	public static PvpGameControl gameControl;

	/// <summary>
	/// 添加消息
	/// </summary>
	/// <param name="pvpMessage">Pvp message.</param>
	/// <param name="gameControl">Game control.</param>
	public static void InsertMessage(BasePvpMessage pvpMessage)
	{
		messageList.Add(pvpMessage);

		Debug.Log("添加消息 ！！！！！！ " + pvpMessage.messageID + ":" + messageList.Count + ":" + PvpMessageManager.gameControl.fightStep + ":" + PvpMessageManager.gameControl.CurrentCharacter.UserType);

		// 如果消息为 1，并且当前行动的角色不是自己，并且不是在战斗状态
		if(messageList.Count == 1)
		{
			// 如果是自己，并且还在战斗状态
			if(PvpMessageManager.gameControl.fightStep == PvpGameControl.PvpFightStep.ACTION && PvpMessageManager.gameControl.CurrentCharacter.UserType == PvpMessageManager.gameControl.PvpCharacterSelf.UserType) return;
			// 如果还在施法阶段
			if(PvpMessageManager.gameControl.fightStep == PvpGameControl.PvpFightStep.MAGIC) return;

			Run();
		}
	}

	/// <summary>
	/// 消息数量
	/// </summary>
	/// <value>The run count.</value>
	public static int MessageCount { get { return messageList.Count; } }

	/// <summary>
	/// 消息状态
	/// </summary>
	public static bool MessageStatus = false;

	/// <summary>
	/// 执行消息列表
	/// </summary>
	public static void Run()
	{
		if(messageList.Count > 0)
		{
			BasePvpMessage pvpMessage = messageList[0];
			Debug.Log("调用消息 ！！！！！！ " + pvpMessage.messageID);
			pvpMessage.Run(()=>
			{
				PvpMessageManager.gameControl.messageCallback_102 = null;
				PvpMessageManager.gameControl.messageCallback_103 = null;
				messageList.RemoveAt(0);
				// 递归调用
				Run();
			});
		}
	}
}
