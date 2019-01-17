using UnityEngine;
using System.Collections;

public class BackToParty : MonoBehaviour
{
    public ViewControl MainControl;

    public void OnClick()
    {
        MainControl.ToPartyView();
    }
}
