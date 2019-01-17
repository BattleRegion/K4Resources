using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ListViewAnime : MonoBehaviour {

    /// <summary>
    /// 用于控制scrollview重置
    /// </summary>
    public UIScrollView List;

    /// <summary>
    /// Grid中的各item
    /// </summary>
    public List<GameObject> cells = new List<GameObject>();

    /// <summary>
    /// 依次显示grid中的item
    /// </summary>
    public void showCells()
    {
		if(!this.gameObject.activeSelf) return;

        //Debug.Log("!!!" + gameObject.name + "ShowCells");
        int j = 0;
        foreach (GameObject go in cells)
        {
            go.SetActive(true);
            go.transform.localPosition = new Vector3(800, go.transform.localPosition.y, go.transform.localPosition.z);
            //Debug.Log(List.gameObject.name + ": " + List.gameObject.activeSelf + " Time: " + Time.time);
            if(List.gameObject.activeSelf)
            {
                StartCoroutine(DelayAnimation(j, go));
            }
            j++;
        }
    }


    /// <summary>
    /// 取自AnimationHelper
    /// </summary>
    /// <param name="index"></param>
    /// <param name="go"></param>
    /// <returns></returns>
    IEnumerator DelayAnimation(int index, GameObject go)
    {
		if(!this.gameObject.activeSelf) yield break;

        yield return new WaitForSeconds(0.05f * index);

		if(!this.gameObject.activeSelf) yield break;

        //Debug.Log(index);
        //Debug.Log(Time.time); //该函数执行会有延时不准确情况
        AnimationHelper.AnimationMoveTo(new Vector3(0, go.transform.localPosition.y, go.transform.localPosition.z), go, iTween.EaseType.linear, null, null, 0.1f);
    }

    void OnEnable()
    {
        //List.SetDragAmount(0, 0, false); //ListView回到最上方
        //List.UpdateScrollbars();
        foreach (GameObject go in cells)
        {
            go.SetActive(false);
        }
        Invoke("showCells", 0.2f);
        //List.ResetPosition();
    }

	void OnDisable()
	{
		this.CancelInvoke ("showCells");	
	}
}