using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectOrderUI : MonoBehaviour
{
    [SerializeField] private Slider aimSlider;
    [SerializeField] private Slider forceSlider;

    public void GetAim(float val)
    {
        aimSlider.value = val;
    }

    public void GetForce(float val)
    {
        forceSlider.value = val;
    }
}
