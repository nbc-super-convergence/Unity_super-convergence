using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowBubble : ObjectPoolBase
{
    [SerializeField] private Image background;
    [SerializeField] private RectTransform arrow;

    // Rotation값 90이면 왼쪽, 270이면 오른쪽    
        
    public void SetArrowBubble(Color backColor, float dir)
    {
        this.background.color = backColor;
        this.arrow.rotation = Quaternion.Euler(0, 0, dir);
    }

    public void ColorChange(Color backColor)
    {
        background.color = backColor;
    }

    public override void Init(params object[] param)
    {
        throw new System.NotImplementedException();
    }
}
