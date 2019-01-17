using UnityEngine;
using System.Collections;
using SimpleJson;
using System.Collections.Generic;
using System;

public class UserTask
{
    public MissionData CurMission;

    public int progress;

    public bool status;

    public int UserTaskId;

    public bool newTask // 0 = 未读 ，1 = 已读
    {
        get
        {
            if (PlayerPrefs.GetInt("task_" + UserTaskId.ToString()) == 1) return false;
            else return true;
        }
        set
        {
            if(value == false)
            {
                PlayerPrefs.SetInt("task_" + UserTaskId.ToString(), 1);
            }
            else
            {
                PlayerPrefs.SetInt("task_" + UserTaskId.ToString(), 0);
            }
        }
    }

    public UserTask(JsonObject data)
    {
        try
        {
            UserTaskId = int.Parse(data["daily_id"].ToString());
            CurMission = ConfigManager.MissionConfig.GetMissionDataById(data["task_id"].ToString());
            progress = int.Parse(data["progress"].ToString());
            status = (progress >= CurMission.Turn);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
}
