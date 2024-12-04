using UnityEngine;
using UnityEngine.UI;

public class SelectOrderUI : MonoBehaviour
{
    [SerializeField] private Slider aimSlider;
    [SerializeField] private Slider forceSlider;

    public void SetAimLimit(float min, float max)
    {
        aimSlider.minValue = min;
        aimSlider.maxValue = max;
    }

    public void GetAim(float val)
    {
        aimSlider.value = val;
    }

    public void SetForceLimit(float min, float max)
    {
        forceSlider.minValue = min;
        forceSlider.maxValue = max;
    }

    public void GetForce(float val)
    {
        forceSlider.value = val;
    }
}
