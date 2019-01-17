using UnityEngine;
using System.Collections;

public class EvolutionSort : MonoBehaviour
{
    public GameObject SortButton;
    public EquipmentBagControl WeaponBag;
    public EquipmentBagControl ArmorBag;

    void OnEnable()
    {
        UIEventListener.Get(SortButton).onClick = (g) =>
        {
            if(WeaponBag.gameObject.activeSelf)
            {
                WeaponBag.SwitchSorting();
                WeaponBag.SetNoEvoCover();
            }
            else if(ArmorBag.gameObject.activeSelf)
            {
                ArmorBag.SwitchSorting();
                ArmorBag.SetNoEvoCover();
            }
        };
    }
}
