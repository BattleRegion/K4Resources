using UnityEngine;
using System.Collections;

public interface RemovePetInterface
{
    void _OnClickRemove();
}

public class RemovePet : MonoBehaviour
{
    public RemovePetInterface rInter;

    void OnClick()
    {
        if (rInter != null)
        {
            rInter._OnClickRemove();
        }
    }

    void OnEnable()
    {
        gameObject.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
    }
}
