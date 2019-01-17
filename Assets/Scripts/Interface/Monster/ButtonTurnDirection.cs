using UnityEngine;
using System.Collections;

public interface _TurnDirectionInter
{
    void _OnClickTurnButton();
}

public class ButtonTurnDirection : MonoBehaviour
{
    public _TurnDirectionInter inter;

    void OnClick()
    {
        if (inter != null)
        {
            inter._OnClickTurnButton();
        }
    }
}
