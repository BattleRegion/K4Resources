using UnityEngine;
using System.Collections;

public class RequestRoute
{
    #region 登录
    static readonly public string GetAllServer = "login.loginHandler.getAllServer";
    static readonly public string UserLoginRoute = "login.loginHandler.userLogin";
    static readonly public string UserRegisterRoute = "login.loginHandler.userRegister";
    static readonly public string UserBindAccount = "login.loginHandler.bindAccount";
    static readonly public string PlayerLoginRoute = "login.loginHandler.queryPlayer";
    #endregion

    #region account
    static readonly public string RegisterRoute = "/membership/user/register";
    static readonly public string LoginRoute = "/membership/user/login";
    static readonly public string NoticeRoute = "/notice/list";
    static readonly public string BindAccount = "player.playerHandler.bindAccount";
    #endregion

    #region 网关
    static readonly public string GetConnectorRoute = "gate.gateHandler.getServer";

	static readonly public string GetTogetherRoute = "way.gateHandler.getServer";

    #endregion
}
