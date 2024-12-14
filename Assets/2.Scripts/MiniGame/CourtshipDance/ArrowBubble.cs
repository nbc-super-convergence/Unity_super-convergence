using UnityEngine;
using UnityEngine.UI;

public class ArrowBubble : ObjectPoolBase
{
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject effect;

    [SerializeField] private Image background;
    [SerializeField] private RectTransform arrow;

    // Rotation값 90이면 왼쪽, 270이면 오른쪽    
        
    public void SetArrowBubble(Color backColor, float dir)
    {
        this.background.color = backColor;
        this.arrow.rotation = Quaternion.Euler(0, 0, dir);
    }
    public void SetArrowBubble(BubbleInfo info)
    {
        switch (info.Color)
        {
            case 0:
                this.background.color = Color.red;
                break;
            case 1:
                this.background.color = Color.yellow;
                break;
            case 2:
                this.background.color = Color.green;
                break;
            case 3:
                this.background.color = Color.blue;
                break;
            default:
                this.background.color = Color.gray;
                break;
        }
        this.arrow.rotation = Quaternion.Euler(0, 0, info.Rotation);
    }

    public void ColorChange(int backColor)
    {
        switch (backColor)
        {
            case 0:
                this.background.color = Color.red;
                break;
            case 1:
                this.background.color = Color.yellow;
                break;
            case 2:
                this.background.color = Color.green;
                break;
            case 3:
                this.background.color = Color.blue;
                break;
            default:
                this.background.color = Color.gray;
                break;
        }
    }

    public void PlayEffect()
    {
        effect.SetActive(true);
        main.SetActive(false);
    }

    public override void Init(params object[] param)
    {
        if(!main.activeSelf)
        {
            main.SetActive(true);
            effect.SetActive(false);
        }
    }
}
