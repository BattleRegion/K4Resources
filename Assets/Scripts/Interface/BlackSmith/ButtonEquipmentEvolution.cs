using UnityEngine;
using System.Collections;

public interface EquipmentEvolutionInter
{
    void _OnClickEquipmentEvoButton(int Id);
}

public class ButtonEquipmentEvolution : MonoBehaviour
{
    public EquipmentEvolutionInter inter;

    public string id;

    public int userWareID;

    void OnClick()
    {
        if (inter != null)
        {
            inter._OnClickEquipmentEvoButton(userWareID);
        }
    }
}
