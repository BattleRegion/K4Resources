using UnityEngine;
using System.Collections;

public class ArenaIconItem : MonoBehaviour 
{
	public UITexture icon;
	
	public void ChangeData(int level)
	{
		this.icon.mainTexture = Tools.GetPvPIcon (level);
	}
}
