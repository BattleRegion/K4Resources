using UnityEngine;
using System.Collections;

/// <summary>
/// 数量显示的自动隐藏
/// </summary>
public class AutoFade : MonoBehaviour
{
    public UIWidget FadeTarget;

    bool fading = false;

    void Fade()
    {
        //FadeTarget.alpha = 0f;
        //fading = false;
    }

    void Show()
    {
        //fading = true;
        //FadeTarget.alpha = 1f;
        //Invoke("Fade", 5f);
    }

    void OnEnable()
    {
        //Show();
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0) && !fading)
        //{
        //    Show();
        //}
    }
}
