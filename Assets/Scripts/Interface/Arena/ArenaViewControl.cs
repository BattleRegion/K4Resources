using UnityEngine;
using System.Collections.Generic;

public class ArenaViewControl : MonoBehaviour 
{
	public List<GameObject> viewList = new List<GameObject>();

	public void SetBSViews()
	{
		foreach (GameObject g in this.viewList)
		{
			if (g.name == "Sprite_Main" || g.name == "Background")
			{
				if (g.activeSelf == false)
				{
					g.SetActive(true);
				}
			}
			else if (g.activeSelf == true)
			{
				g.SetActive(false);
			}
		}
	}
	
	void OnEnable()
	{
		SetBSViews();
	}
}
