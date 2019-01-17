using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AccountRegisterControl : MonoBehaviour
{
    public UIInput Account;
    public UIInput Password;

    string account
    {
        get { return Account.value; }
    }
    string password
    {
        get { return Password.value; }
    }

    void OnClick()
    {
        LoginControl.Register(account, password, AppMember.AccountType.Normal, SystemInfo.deviceUniqueIdentifier, (r) =>
        {
            if (r == LoginControl.RegisterCode.RegisterSuccess)
            {
                ApplicationControl.CurApp.ShowInfoWindow("注册成功,请登录");
            }
            else
            {

            }
        });
    }
}
