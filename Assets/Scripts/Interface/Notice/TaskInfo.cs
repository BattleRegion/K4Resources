using UnityEngine;
using System.Collections;
using SimpleJson;

public class TaskInfo : MonoBehaviour
{
    public UILabel Description;
    public UILabel Reward;
    public UILabel Progress;
    public UILabel Type;

    public UISprite NewTag;

    UserTask curTask;

    public void SetTaskInfo(UserTask ut)
    {
        curTask = ut;
        MissionData m = ut.CurMission;

        NewTag.gameObject.SetActive(ut.newTask);
        curTask.newTask = false;

        if (GameObject.Find("Button_Notice"))
            GameObject.Find("Button_Notice").transform.FindChild("new").gameObject.SetActive(false);

        Description.text = curTask.CurMission.Description;
        if(m.RewardId == "Currency1")
        {
            Reward.text = "奖励：" + "钻石" + "*" + m.RewardRate;
        }
        else if (ConfigManager.PetConfig.GetPetById(m.RewardId) != null)
        {
            Reward.text = "奖励：" + ConfigManager.PetConfig.GetPetById(m.RewardId).Name + "*" + m.RewardRate;
        }
        else if (ConfigManager.ItemConfig.GetItemById(m.RewardId) != null)
        {
            Reward.text = "奖励：" + ConfigManager.ItemConfig.GetItemById(m.RewardId).Description + "*" + m.RewardRate;
        }
        else if (ConfigManager.HardWareConfig.GetHardWareById(m.RewardId) != null)
        {
            Reward.text = "奖励：" + ConfigManager.HardWareConfig.GetHardWareById(m.RewardId).Name + "*" + m.RewardRate;
        }

        //switch(ut.CurMission.Goal)
        //{
        //    case 1:
        //        {
        //            Type.text = "完成普通关卡";
        //            break;
        //        }
        //    case 2:
        //        {
        //            Type.text = "完成活动关卡";
        //            break;
        //        }
        //    case 3:
        //        {
        //            Type.text = "完成竞技场";
        //            break;
        //        }
        //    case 4:
        //        {
        //            Type.text = "竞技场胜利";
        //            break;
        //        }
        //    default: break;
        //}

        Progress.text = curTask.progress.ToString() + "/" + curTask.CurMission.Turn.ToString();
    }
}
