using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;

public class ErrorControl : MonoBehaviour
{
    public UILabel ErrorInfo;

    public GameObject BackButton;

    public GameObject BuyButton;

    public GameObject CancelButton;

    public GameObject InfoBoard;

	public bool pvpStatus;

    public enum ConsumptionGuideType
    {
        none = 0,
        guide_diamond = 2016,
        guide_energy = 2017,
        guide_itembagexpand = 5004,
        guide_petbagexpand = 6016,
        guide_relogin = 2000,
		guide_pvp_error = 1620
    }

    public ConsumptionGuideType guide_type = ConsumptionGuideType.none;

    void OnEnable()
    {
        
    }

    void Start()
    {
        ShowWindow();
        UIEventListener.Get(BackButton).onClick = (g) =>
        {
            Destroy(gameObject);

            if(guide_type == ConsumptionGuideType.guide_relogin)
            {
                Application.LoadLevel(0); //强制登出
            }

			// 如果在 PVP 中错误等级比较严重，则跳转到主场景
			if(this.pvpStatus)
			{
				// 如果是 1620，需要请求 PvpOver 接口
				if(guide_type == ConsumptionGuideType.guide_pvp_error)
				{
					JsonObject args = new JsonObject();
					args.Add("ticket_id", UserManager.CurUserInfo.ArenaPvpTicket);

					SocketCenter.Request(GameRouteConfig.PvpOver, args, (r) =>
					{
						if (r.Code == SocketResult.ResultCode.Success)
						{
							Loom.QueueOnMainThread(() =>
							{
								int starLevel = int.Parse(r.Data["star_level"].ToString());
								int starExp = int.Parse(r.Data["star_exp"].ToString());
								
								JsonArray fightData = (JsonArray)r.Data["elements"];
								
								// 任务信息
								JsonObject taskData = (JsonObject)r.Data["daily_task"];
								// 设置任务数据
								PvpOverUI.taskData = taskData;
								
								// 添加数据
								UserManager.CurUserInfo.AddElements(fightData);
								// 设置数据
								PvpOverUI.ChangeData(starLevel, starExp);

								// 设置场景跳转数据
								SceneDataManager.fromScene = SceneDataManager.PVP;

								Application.LoadLevel("MainScene");
							});
						}
					}, null, true, true);
				}else
				{
					Application.LoadLevel("MainScene");
				}
			}
        };

        UIEventListener.Get(BuyButton).onClick = (g) =>
        {
            SetUser userInformation = GameObject.Find("MainBoardControl").GetComponent<SetUser>();
            ViewControl viewController = userInformation.GetComponent<ViewControl>();
            switch(guide_type)
            {
                case ConsumptionGuideType.guide_diamond:
                    {
                        viewController.ToDiamondConsumeView();
                        //PartyAnime.moveEnd = viewController.Mall.GetComponent<MallViewControl>().ToDiamondConsumeView;
                        viewController.Mall.GetComponent<MallViewControl>().ToDiamondConsumeView();
                        Destroy(gameObject);
                        break;
                    }
                case ConsumptionGuideType.guide_energy:
                    {
                        SocketCenter.Request(GameRouteConfig.BuyEnergy, null, (r) =>
                        {
                            if (r.Code == SocketResult.ResultCode.Success)
                            {
                                Loom.QueueOnMainThread(() =>
                                {
                                    JsonArray elements = (JsonArray)r.Data["elements"];
                                    UserManager.CurUserInfo.AddElements(elements);
                                    userInformation.SetInfo();
                                    Destroy(gameObject);
                                });
                            }
                            else
                            {
                                Loom.QueueOnMainThread(() =>
                                {
                                    Destroy(gameObject);
                                });
                            }
                        }, null, true, true);
                        break;
                    }
                case ConsumptionGuideType.guide_itembagexpand:
                    {
                        JsonObject args = new JsonObject();
                        args.Add("count", 5);
                        SocketCenter.Request(GameRouteConfig.ExpandWareHouse, args, (r) =>
                        {
                            if (r.Code == SocketResult.ResultCode.Success)
                            {
                                Loom.QueueOnMainThread(() =>
                                {
                                    JsonArray elements = (JsonArray)r.Data["consumes"];
                                    UserManager.CurUserInfo.AddElements(elements);
                                    userInformation.SetInfo();
                                    UserManager.CurUserInfo.SetExtend((JsonObject)r.Data["extend"]);
                                    Destroy(gameObject);
                                });
                            }
                            else
                            {
                                Loom.QueueOnMainThread(() =>
                                {
                                    Destroy(gameObject);
                                });
                            }
                        }, null, true, true);
                        break;
                    }
                case ConsumptionGuideType.guide_petbagexpand:
                    {
                        JsonObject args = new JsonObject();
                        args.Add("count", 5);
                        SocketCenter.Request(GameRouteConfig.ExpandPetHouse, args, (r) =>
                        {
                            if (r.Code == SocketResult.ResultCode.Success)
                            {
                                Loom.QueueOnMainThread(() =>
                                {
                                    JsonArray elements = (JsonArray)r.Data["consumes"];
                                    UserManager.CurUserInfo.AddElements(elements);
                                    userInformation.SetInfo();
                                    UserManager.CurUserInfo.SetExtend((JsonObject)r.Data["extend"]);
                                    Destroy(gameObject);
                                });
                            }
                            else
                            {
                                Loom.QueueOnMainThread(() =>
                                {
                                    Destroy(gameObject);
                                });
                            }
                        }, null, true, true);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        };

        UIEventListener.Get(CancelButton).onClick = (g) =>
        {
            Destroy(gameObject);
        };
    }

    void ShowWindow()
    {
        InfoBoard.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        PetUIController.SetLayer(transform, LayerHelper.Cover);
        AnimationHelper.AnimationScaleTo(new Vector3(1, 1, 1), InfoBoard, iTween.EaseType.spring, null, null, 0.1f);
    }

    public void SetButtonState()
    {
        if(Application.loadedLevel == 1)
        {
            switch (guide_type)
            {
                case ConsumptionGuideType.guide_diamond:
                    {
                        BuyButton.SetActive(true);
                        CancelButton.SetActive(true);
                        BackButton.SetActive(false);
                        break;
                    }
                case ConsumptionGuideType.guide_energy:
                    {
                        BuyButton.SetActive(true);
                        CancelButton.SetActive(true);
                        BackButton.SetActive(false);
                        break;
                    }
                case ConsumptionGuideType.guide_itembagexpand:
                    {
                        BuyButton.SetActive(true);
                        CancelButton.SetActive(true);
                        BackButton.SetActive(false);
                        break;
                    }
                case ConsumptionGuideType.guide_petbagexpand:
                    {
                        BuyButton.SetActive(true);
                        CancelButton.SetActive(true);
                        BackButton.SetActive(false);
                        break;
                    }
                default:
                    {
                        BuyButton.SetActive(false);
                        CancelButton.SetActive(false);
                        BackButton.SetActive(true);
                        break;
                    }
            }
        }
        else
        {
            BuyButton.SetActive(false);
            CancelButton.SetActive(false);
            BackButton.SetActive(true);
        }
    }

    /// <summary>
    /// 设置报错信息
    /// </summary>
	public void SetInfo(int ErrorCode, bool pvpStatus = false)
    {
		this.pvpStatus = pvpStatus;

        guide_type = (ConsumptionGuideType)ErrorCode;
        SetButtonState();

        ErrorData data = ConfigManager.ErrorConfig.GetErrorDataByCode(ErrorCode);
        if (data != null)
        {
            if(Application.loadedLevel == 1)
            {
                switch (guide_type)
                {
                    case ConsumptionGuideType.guide_diamond:
                        {
                            ErrorInfo.text = "钻石不足，你可以在商城中购买钻石";
                            break;
                        }
                    case ConsumptionGuideType.guide_energy:
                        {
                            ErrorInfo.text = "体力不足，使用" + (5 * ConfigManager.ParamConfig.GetParam().EnergyPrice).ToString() + "颗钻石即可回满体力";
                            break;
                        }
                    case ConsumptionGuideType.guide_itembagexpand:
                        {
                            ErrorInfo.text = "物品栏已满，使用" + (5 * ConfigManager.ParamConfig.GetParam().PethouseConsume).ToString() + "颗钻石即可扩张物品栏";
                            break;
                        }
                    case ConsumptionGuideType.guide_petbagexpand:
                        {
                            ErrorInfo.text = "酒馆已满，使用" + (5 * ConfigManager.ParamConfig.GetParam().WarehouseConsume).ToString() + "颗钻石即可扩张酒馆";
                            break;
                        }
                    default:
                        {
                            ErrorInfo.text = data.Description;
                            break;
                        }

                }
            }
            else
            {
                ErrorInfo.text = data.Description;
            }
        }
        else
        {
            if (ErrorCode == 999)
            {
                ErrorInfo.text = "Error" + ErrorCode.ToString() + ":连接超时";
            }
            else
            {
                ErrorInfo.text = "Error" + ErrorCode.ToString() + ":未知错误";
            }
        }
    }

    public void SetInfo(string info)
    {
        SetButtonState();

        ErrorInfo.text = info;
    }
}
