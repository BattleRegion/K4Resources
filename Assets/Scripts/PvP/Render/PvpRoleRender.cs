using UnityEngine;
using System.Collections;

public class PvpRoleRender : MonoBehaviour 
{
	public GameObject roleItem;
	public Camera roleCamera;
	public RenderTexture renderTexture;
	public UITexture uiTexture;

	void Start()
	{
		Texture2D myTexture2D = new Texture2D(this.renderTexture.width,this.renderTexture.height);
		RenderTexture.active = this.renderTexture;
		myTexture2D.ReadPixels(new Rect(0, 0, this.renderTexture.width, this.renderTexture.height), 0, 0);
		myTexture2D.Apply();
		RenderTexture.active = null;

		this.uiTexture.mainTexture = myTexture2D;

		GameObject.Destroy (roleItem);
		this.roleCamera.gameObject.SetActive (false);
	}
}
