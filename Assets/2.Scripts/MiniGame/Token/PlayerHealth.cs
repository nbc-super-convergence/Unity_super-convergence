using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private MiniPlayer player;
    [SerializeField] private TextMeshProUGUI hpText; //체력텍스트

    private const int maxHP = 10;
    private int playerHP = maxHP; //떨어질때 1깎이는 버그 땜에...

    /// <summary>
    /// 데미지 감소
    /// </summary>
    public int PlayerHP 
    {
        get
        {
            return playerHP;
        }
        private set
        {   //해당 데미지 감소
            playerHP = Mathf.Max(0, playerHP - value);
        }
    }

    private void Awake()
    {
        player = GetComponent<MiniPlayer>();
    }

    private void Update()
    {
        ShowHPBar();
    }

    /// <summary>
    /// 변동된 HP 갱신
    /// </summary>
    /// <param name="dmg"></param>
    public void DecreaseHP(int dmg)
    {
        PlayerHP = dmg;
        //Debug.Log(PlayerHP);
    }

    /// <summary>
    /// 반영된 체력을 UI에 표시
    /// </summary>
    private void ShowHPBar()
    {
        hpText.text = playerHP.ToString();
    }
}