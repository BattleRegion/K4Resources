using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

/// <summary>
/// 助战选择条
/// </summary>
public class HelpCell : MonoBehaviour, itemInterface
{
    public UILabel NickName;

    public UILabel Level;

    public UISprite HelpType;

    public UISprite HelpPointType;

    public ItemInterface Leader;

    /// <summary>
    /// 好友Id
    /// </summary>
    public int Uid;

    public UserPet HelpPet;

    public void SetHelpInfo(HelpData helpData)
    {
        Uid = helpData.Uid;

        NickName.text = helpData.NickName;

        Level.text = "Lv. " + helpData.Level.ToString();

        switch (helpData.helpType)
        {
            case HelpData.HelperType.friend:
                {
                    HelpType.spriteName = "friend";
                    HelpPointType.spriteName = "friend_point_10";
                    break;
                }
            case HelpData.HelperType.adventurer:
                {
                    HelpType.spriteName = "adventurer";
                    HelpPointType.spriteName = "friend_point_5";
                    break;
                }
            default: break;
        }

        HelpPet = helpData.HelpPet;
        Leader.SetItem(HelpPet);
        Leader.itemInter = this;
    }

    public void Click()
    {
        //JsonObject args = new JsonObject();
        //args.Add("dungeon_id", PveGameControl.CurDungeonId);
        //args.Add("queue_id", UserManager.CurUserInfo.CurPartyIndex);
        //args.Add("friend_id", Uid);
        ////UserManager.CurUserInfo.UserPets.Add(HelpPet);
        //SocketCenter.Request(GameRouteConfig.EnterScene, args, (r) =>
        //{
        //    if (r.Code == SocketResult.ResultCode.Success)
        //    {
        //        Loom.QueueOnMainThread(() =>
        //        {
        //            JsonObject sceneContext = (JsonObject)r.Data["scene_context"];
        //            PveGameControl.CurPveData = (JsonObject)sceneContext["drop_list"];

        //            if (GuiderLocal.TriggerPve())
        //            {
        //                Debug.Log("triggerpve guid   true");
        //                PveGameControl.CurFriend = FriendInfo.GuiderFriend;
        //            }
        //            else
        //            {
        //                PveGameControl.CurFriend = new FriendInfo((JsonObject)sceneContext["friend"]);
        //            }

        //            UserManager.CurUserInfo.AddElements((JsonArray)r.Data["consume"]);
        //            UserPet fp = PveGameControl.CurFriend.FriendLeader;

        //            Application.LoadLevel("Pve");
        //        });
        //    }
        //}, null, true, false);



        GameObject.Find("Dungeon").GetComponent<DungeonSelectUI>().ExShowSelectBoard(Uid);
    }

    public void _OnClickItem(int Uid)
    {

    }

    public void _OnLongPressItem(int Uid)
    {
        GameObject g = GameObject.Find("Detail");
        GameObject PetDetailView = g.transform.FindChild("Detail_pet").gameObject;
        if (PetDetailView != null)
        {
            PetDetailView.SetActive(true);
            PetDetailView.GetComponent<SetMonsterDetail>().SetDetail(HelpPet);
        }
    }
}