using UnityEngine;
using System.Collections;

public class DailyTaskView : MonoBehaviour
{
    public ListViewAnime TaskListAnimation;
    public UIGrid ListGrid;
    public GameObject TaskCell;

    public void AddTaskCell(UserTask ut)
    {
        GameObject CellItem;
        CellItem = NGUITools.AddChild(ListGrid.gameObject, TaskCell);
        TaskListAnimation.cells.Add(CellItem);
        CellItem.GetComponent<TaskInfo>().SetTaskInfo(ut);
    }

    void OnEnable()
    {
        foreach(UserTask ut in UserManager.CurUserInfo.UserDailyTasks)
        {
            AddTaskCell(ut);
        }
        ListGrid.Reposition();
        TaskListAnimation.List.GetComponent<UIScrollView>().enabled = false;
    }

    void OnDisable()
    {
        foreach(GameObject g in TaskListAnimation.cells)
        {
            Destroy(g);
        }
        TaskListAnimation.cells.Clear();
    }
}
