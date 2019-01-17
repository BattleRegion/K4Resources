using UnityEngine;
using System.Collections;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;

public class BuyDiamonds : MonoBehaviour
{
    public GameObject BuyButton;

    public SetUser UserInfo;

    void Start()
    {
        UIEventListener.Get(BuyButton).onClick = (g) =>
        {
            SocketCenter.Request(GameRouteConfig.FreeDiamond, null, (r) =>
            {
                if (r.Code == SocketResult.ResultCode.Success)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        UserManager.CurUserInfo.AddElements((JsonArray)r.Data["elements"]);
                        UserInfo.SetInfo();
                    });
                }
                else
                {

                }
            }, null, true, true);
        };
    }
}
