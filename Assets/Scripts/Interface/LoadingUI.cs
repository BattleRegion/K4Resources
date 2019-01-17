using UnityEngine;
using System.Collections;

public class LoadingUI : MonoBehaviour 
{
	public GameObject loadingTxt;
	public GameObject loadingBack;
    //public GameObject loadingSmoke;
	public LoadingControl loadingControl;

	void Awake()
	{
		this.ChangeStatus (false);
	}

	IEnumerator Start()
	{
		yield return new WaitForSeconds (1f);
		this.ChangeStatus (true);
	}

	private void ChangeStatus(bool status)
	{
		if(this.loadingTxt != null)
		{
			this.loadingTxt.SetActive(status);		
		}
		if(this.loadingBack != null)
		{
			this.loadingBack.SetActive(status);		
		}
        //if(this.loadingSmoke != null)
        //{
        //    this.loadingSmoke.SetActive(status);		
        //}
		if(this.loadingControl != null)
		{
			this.loadingControl.enabled = status;
		}
	}
}
