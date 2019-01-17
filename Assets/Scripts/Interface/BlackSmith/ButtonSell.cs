using UnityEngine;
using System.Collections;

public interface _ButtonSell
{
    void _OnClickSell();
}

public class ButtonSell : MonoBehaviour
{
    public _ButtonSell inter;

    void OnClick()
    {
        if (inter != null)
        {
            inter._OnClickSell();
        }
    }
}
