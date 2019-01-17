using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DungeonDetail : MonoBehaviour {

    public GameObject BackBar;

    public GameObject Tabel;

    int BackBarInitX = -1000;

    int TableInitX = 1000;

    public GameObject CellResource;

    public GameObject DungeonCellResource;

    public UIGrid Grid;

    public enum type
    {
        Chapter = 1,
        Dungeon = 2
    }

    public type CurDetailType = type.Chapter;
	// Use this for initialization
	void Start () 
    {
        
	}

    void OnEnable()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(0, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(0, Tabel.transform.localPosition.y, Tabel.transform.localPosition.z), Tabel, iTween.EaseType.linear, null, null, 0.2f);
        if (CurDetailType == type.Chapter)
        {
            Invoke("ShowChapter", 0.2f);
        }
        else
        {
            Invoke("ShowDungeon", 0.2f);
        }
    }

    public void OnClick()
    {
        CurDetailType = type.Chapter;
        Close();
    }

    void Close()
    {
        foreach (GameObject go in cells)
        {
            DestroyImmediate(go);
        }
        cells.Clear();
        AnimationHelper.AnimationMoveTo(new Vector3(BackBarInitX, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(TableInitX, Tabel.transform.localPosition.y, Tabel.transform.localPosition.z), Tabel, iTween.EaseType.linear, null, null, 0.2f);
        Invoke("EnabelDetail", 0.2f);
    }

    void EnabelDetail()
    {
        gameObject.SetActive(false);
    }

    List<GameObject> cells = new List<GameObject>();
    void ShowChapter()
    {
        //作假一个章节
        GameObject cell = Instantiate(CellResource) as GameObject;
        cell.transform.parent = Grid.transform;
        cell.transform.localScale = new Vector3(1, 1, 1);
        Cell c = cell.GetComponent<Cell>();
        c.Title.text = "巴哈姆特的监牢";
        c.CL.SetNum("1");
        UIEventListener.Get(cell.gameObject).onClick = (go) =>
        {
            ClickChapter();
        };
        cells.Add(cell);
        Grid.Reposition();
        foreach (GameObject go in cells)
        {
            go.transform.localPosition = new Vector3(600, go.transform.localPosition.y, go.transform.localPosition.z);
            AnimationHelper.AnimationMoveTo(new Vector3(0, go.transform.localPosition.y, go.transform.localPosition.z),go,iTween.EaseType.linear,null,null,0.2f);
        }
    }

    void ClickChapter()
    {
        CurDetailType = type.Dungeon;
        Close();
        Invoke("ReEnabel", 0.3f);
    }

    void ReEnabel()
    {
        gameObject.SetActive(true);
    }

    void ShowDungeon()
    {
        int i = 1;
        foreach (DungeonData d in ConfigManager.DungeonConfig.GetAllData())
        {
            //作假一个章节
            GameObject cell = Instantiate(DungeonCellResource) as GameObject;
            cell.transform.parent = Grid.transform;
            cell.transform.localScale = new Vector3(1, 1, 1);
            Cell c = cell.GetComponent<Cell>();
            c.Id = d.Id;
            c.Title.text = "巴哈姆特的监牢-"+i.ToString();
            c.CL.SetNum("10");
            i++;
            UIEventListener.Get(cell.gameObject).onClick = (go) =>
            {
                Cell goc = go.GetComponent<Cell>();
                PveGameControl.CurDungeonId = goc.Id;
                Application.LoadLevel("Pve");
            };
            cells.Add(cell);
            Grid.Reposition();
        }

        int j = 0;
        foreach (GameObject go in cells)
        {
            go.transform.localPosition = new Vector3(600, go.transform.localPosition.y, go.transform.localPosition.z);
            StartCoroutine(DelayAnimation(j, go));
            j++;
        }
    }

    IEnumerator DelayAnimation(int index,GameObject go)
    {
        yield return new WaitForSeconds(0.2f * index);
        AnimationHelper.AnimationMoveTo(new Vector3(0, go.transform.localPosition.y, go.transform.localPosition.z), go, iTween.EaseType.linear, null, null, 0.2f);
    }
}
