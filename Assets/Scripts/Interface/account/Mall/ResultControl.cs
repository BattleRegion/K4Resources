using UnityEngine;
using System.Collections;

public class ResultControl : MonoBehaviour
{
    void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
