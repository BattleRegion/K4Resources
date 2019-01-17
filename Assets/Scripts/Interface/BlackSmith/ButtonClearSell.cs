using UnityEngine;
using System.Collections;

public interface _ButtonClearSell
{
    void _OnClickClear();
}

public class ButtonClearSell : MonoBehaviour
{
    public _ButtonClearSell inter;

    void OnClick()
    {
        if (inter != null)
        {
            inter._OnClickClear();
        }
    }
}
