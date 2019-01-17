using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;

public class PvpTest : MonoBehaviour 
{
	public UITexture uiTexture;
	public GameObject objectItem;
	public Camera objectCamera;

	void Start()
	{
		this.StartCoroutine (this.ScreenShot());
	}

	IEnumerator ScreenShot()
	{
		yield return new WaitForEndOfFrame();
		
		RenderTexture renderTexture = new RenderTexture (Screen.width, Screen.height, 0);
		this.objectCamera.targetTexture = renderTexture;
		this.objectCamera.Render ();
		
		RenderTexture.active = renderTexture;

		int width = 100; //Screen.width;
		int height = 200; //Screen.height;

		int dstX = (int)(Screen.width * 0.5f + objectItem.transform.localPosition.x - 50);
		int dstY = (int)(Screen.height * 0.5f + objectItem.transform.localPosition.y - 200);

		Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
		tex.ReadPixels(new Rect(dstX, dstY, width, height), 0, 0);  
		tex.Apply();

		if(uiTexture != null) uiTexture.mainTexture = tex;

		this.objectCamera.targetTexture = null;
		RenderTexture.active = null;
		
	}
}
