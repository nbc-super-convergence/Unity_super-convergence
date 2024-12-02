using UnityEngine;
using UnityEngine.UI;

public class SelectOrderDartManage : MonoBehaviour
{
    //½Ì±ÛÅæ
    private static SelectOrderDartManage instance;
    public static SelectOrderDartManage Instance
    {
        get => instance;
        set
        {
            if (instance == null)
                instance = value;
        }
    }

    //UI
    [SerializeField] private Slider aimSlider;
    [SerializeField] private Slider forceSlider;
    

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        NextDart();
    }

    public void NextDart()
    {
        if(transform.childCount > 0)
            transform.GetChild(0).gameObject.SetActive(true);
    }
}
