using UnityEngine;
using UnityEngine.UI;

public class IceSlidingHealth : MonoBehaviour
{
    [SerializeField] private Slider _hpBar; //체력바

    private Transform _player;  //띄우기 위한 플레이어의 위치

    private const int _maxHP = 10;
    private int _playerHP = _maxHP + 1; //떨어질때 1깎이는 버그 땜에...

    /// <summary>
    /// 데미지 감소
    /// </summary>
    public int PlayerHP 
    {
        get
        {
            return _playerHP;
        }
        private set
        {   //해당 데미지 감소
            _playerHP = Mathf.Max(0, _playerHP - value);
        }
    }

    private void Awake()
    {
        _player = GetComponent<IceSlidingBase>().transform;
    }

    private void Update()
    {
        ShowHPBar();
    }

    /// <summary>
    /// 변동된 HP 갱신
    /// </summary>
    /// <param name="dmg"></param>
    public void SetDamage(int dmg)
    {
        PlayerHP = dmg;
        //Debug.Log(PlayerHP);
    }

    /// <summary>
    /// 반영된 체력을 UI에 표시
    /// </summary>
    private void ShowHPBar()
    {
        //HP를 플레이어 UI위에 표시
        _hpBar.value = (float)_playerHP / _maxHP;

        _hpBar.transform.position = _player.position + Vector3.up;
    }
}
