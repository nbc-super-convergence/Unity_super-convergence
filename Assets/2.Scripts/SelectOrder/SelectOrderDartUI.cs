using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectOrderDartUI : UIBase
{
    [SerializeField] private Image background;  //배경
    [SerializeField] private Image HowToBG; //설명 창
    [SerializeField] private TextMeshProUGUI HowToText; //설명 텍스트
    
    //힘 조절 슬라이드
    [SerializeField] private Slider forceSlider;

    public override void Opened(object[] param)
    {
        base.Opened(param);
    }

    /// <summary>
    /// 힘 조절 한계 설정
    /// </summary>
    public void SetForceLimit(float min, float max)
    {
        forceSlider.minValue = min;
        forceSlider.maxValue = max;
    }

    /// <summary>
    /// 데이터를 UI에 적용
    /// </summary>
    /// <param name="val"></param>
    public void GetForce(float val)
    {
        forceSlider.value = val;
    }
}
