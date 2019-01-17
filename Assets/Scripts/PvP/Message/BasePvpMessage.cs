using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class BasePvpMessage
{
	protected PvpGameControl gameControl;
	protected JsonObject pvpData;

	public int messageID;

	public BasePvpMessage(PvpGameControl gameControl, JsonObject pvpData)
	{
		this.gameControl = gameControl;
		this.pvpData = pvpData;
	}

	public virtual void Run(Action callback)
	{
		
	}
}
