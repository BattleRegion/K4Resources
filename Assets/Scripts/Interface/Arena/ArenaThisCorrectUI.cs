using UnityEngine;
using System.Collections;

public class ArenaThisCorrectUI : MonoBehaviour 
{
	public ArenaThisMenuItemList menuItemList;

	void OnEnable()
	{
		if(this.menuItemList != null)
		{
			this.menuItemList.ResetPosition();
		}
	}
}
