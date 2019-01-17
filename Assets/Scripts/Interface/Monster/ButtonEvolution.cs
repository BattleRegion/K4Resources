using UnityEngine;
using System.Collections;

public interface evolutionBtn
{
    void _OnClickEvolution(int UserPetID);
}

public class ButtonEvolution : MonoBehaviour
{
    public evolutionBtn evolutionBtnInter;

    public string id;

    public int userPetID;

    void OnClick()
    {
        if (evolutionBtnInter != null)
        {
            evolutionBtnInter._OnClickEvolution(userPetID);
        }
    }
}
