using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeScreen : UIBase
{
    [SerializeField] private Image black;

    public override void Opened(object[] param)
    {
        if(param[0].ToString() == "FadeOut")
        {
            FadeScreen.Instance.FadeOut(() => UIManager.Hide<UIFadeScreen>());
        }
        else if (param[0].ToString() == "FadeIn")
        {
            FadeScreen.Instance.FadeIn(() => UIManager.Hide<UIFadeScreen>());
        }
    }

    private void ShowInvisibleWall()
    {
        black.raycastTarget = true;
    }

    
}
