using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonListAnime : MonoBehaviour {

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
        UIGrid grid = transform.FindChild("Grid").GetComponent<UIGrid>();
        int j = 0;
        for(int i = 0; i < cells.Count; i++)
        {
            cells[i].SetActive(true);
            cells[i].transform.localPosition = new Vector3(800, -grid.cellHeight * i, cells[i].transform.localPosition.z);
            //Debug.Log(List.gameObject.name + ": " + List.gameObject.activeSelf + " Time: " + Time.time);
            if (List.gameObject.activeSelf)
            {
                StartCoroutine(DelayAnimation(j, cells[i]));
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
        yield return new WaitForSeconds(0.05f * index);
        AnimationHelper.AnimationMoveTo(new Vector3(0, go.transform.localPosition.y, go.transform.localPosition.z), go, iTween.EaseType.linear, null, null, 0.1f);
    }

    public float Show()
    {
        //List.SetDragAmount(0, 0, false); //ListView回到最上方
        //List.UpdateScrollbars();
        float f = 0f;
        foreach (GameObject go in cells)
        {
            go.SetActive(false);
            f += 0.2f;
        }
        Invoke("showCells", 0.2f);
        return f;
    }
}