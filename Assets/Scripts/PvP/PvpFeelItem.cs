using UnityEngine;
using System.Collections;

public class PvpFeelItem : MonoBehaviour 
{
	public UISprite icon;

	public string faceID;

	void Awake()
	{
		UIEventListener.Get(this.gameObject).onClick = (g) => 
		{
			if(!string.IsNullOrEmpty(this.faceID))
			{
				PvpGameControl gc = GameObject.Find("UI Root").GetComponent<PvpGameControl>();
				gc.PvpFaceSubmit(this.faceID);
			}
		};
	}
}
