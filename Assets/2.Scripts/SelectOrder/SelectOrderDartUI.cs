using UnityEngine;
using UnityEngine.UI;

public class SelectOrderDartUI : MonoBehaviour
{
    [SerializeField] private Slider aimSlider;
    [SerializeField] private Slider forceSlider;

    #region 미사용 메서드
    public void SetAimLimit(float min, float max)
    {
        aimSlider.minValue = min;
        aimSlider.maxValue = max;
    }
    public void GetAim(float val)
    {
        aimSlider.value = val;
    }
    #endregion

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
