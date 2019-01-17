using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;

public class ButtonLottery : MonoBehaviour
{
    //public GameObject FriendshipButton;
    //public GameObject DiamondButton;

    public GameObject BuyEnergyButton;
    public GameObject BuyEnergyResult;
    public UILabel BuyEnergyResultLabel;
    public UILabel BuyEnergyInfoLabel;

    public GameObject ExpandPetButton;
    public GameObject ExpandPetResult;
    public UILabel ExpandPetResultLabel;
    public UILabel ExpandPetInfoLabel;

    public GameObject ExpandWareButton;
    public GameObject ExpandWareResult;
    public UILabel ExpandWareResultLabel;
    public UILabel ExpandWareInfoLabel;

    public SetUser UserInfo;

    void Start()
    {
        BuyEnergyInfoLabel.text = "使用" + (5 * ConfigManager.ParamConfig.GetParam().EnergyPrice).ToString() + "颗钻石即可回满体力";
        ExpandPetInfoLabel.text = "使用" + (5 * ConfigManager.ParamConfig.GetParam().PethouseConsume).ToString() + "颗钻石即可扩张酒馆";
        ExpandWareInfoLabel.text = "使用" + (5 * ConfigManager.ParamConfig.GetParam().WarehouseConsume).ToString() + "颗钻石即可扩张物品栏";

        JsonObject args = new JsonObject();

        //购买体力
        UIEventListener.Get(BuyEnergyButton).onClick = (g) =>
        {
            SocketCenter.Request(GameRouteConfig.BuyEnergy, null, (r) =>
            {
                if (r.Code == SocketResult.ResultCode.Success)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        JsonArray elements = (JsonArray)r.Data["elements"];
                        UserManager.CurUserInfo.AddElements(elements);
                        UserInfo.SetInfo();
                        BuyEnergyResult.SetActive(true);
                        BuyEnergyResultLabel.text = "购买成功";
                    });
                }
                else
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        BuyEnergyResult.SetActive(true);
                        BuyEnergyResultLabel.text = "购买失败";
                    });
                    Debug.Log("体力购买失败");
                }
            }, null, true, true);
        };

        //扩展宠物栏
        UIEventListener.Get(ExpandPetButton).onClick = (g) =>
        {
            args.Clear();
            args.Add("count", 5);
            SocketCenter.Request(GameRouteConfig.ExpandPetHouse, args, (r) =>
            {
                if (r.Code == SocketResult.ResultCode.Success)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        JsonArray elements = (JsonArray)r.Data["consumes"];
                        UserManager.CurUserInfo.AddElements(elements);
                        UserInfo.SetInfo();
                        ExpandPetResult.SetActive(true);
                        ExpandPetResultLabel.text = "购买成功";
                        UserManager.CurUserInfo.SetExtend((JsonObject)r.Data["extend"]);
                    });
                }
                else
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        ExpandPetResult.SetActive(true);
                        ExpandPetResultLabel.text = "购买失败";
                    });
                }
            }, null, true, true);
        };

        //扩展装备栏
        UIEventListener.Get(ExpandWareButton).onClick = (g) =>
        {
            args.Clear();
            args.Add("count", 5);
            SocketCenter.Request(GameRouteConfig.ExpandWareHouse, args, (r) =>
            {
                if (r.Code == SocketResult.ResultCode.Success)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        JsonArray elements = (JsonArray)r.Data["consumes"];
                        UserManager.CurUserInfo.AddElements(elements);
                        UserInfo.SetInfo();
                        ExpandWareResult.SetActive(true);
                        ExpandWareResultLabel.text = "购买成功";
                        UserManager.CurUserInfo.SetExtend((JsonObject)r.Data["extend"]);
                    });
                }
                else
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        ExpandWareResult.SetActive(true);
                        ExpandWareResultLabel.text = "购买失败";
                    });
                }
            }, null, true, true);
        };
    }
}
