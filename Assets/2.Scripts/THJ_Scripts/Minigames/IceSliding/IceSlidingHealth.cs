using UnityEngine;
using UnityEngine.UI;

public class IceSlidingHealth : MonoBehaviour
{
    [SerializeField] private Slider _hpBar; //ü�¹�

    private Transform _player;  //���� ���� �÷��̾��� ��ġ

    private const int _maxHP = 100;
    private int _playerHP = 100;

    /// <summary>
    /// ������ ����
    /// </summary>
    public int PlayerHP 
    {
        get
        {
            return _playerHP;
        }
        private set
        {   //�ش� ������ ����
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
    /// ������ HP ����
    /// </summary>
    /// <param name="dmg"></param>
    public void SetDamage(int dmg)
    {
        PlayerHP = dmg;
        Debug.Log(PlayerHP);
    }

    private void ShowHPBar()
    {
        //HP�� �÷��̾� UI���� ǥ��
        _hpBar.value = (float)_playerHP / 100;

        _hpBar.transform.position = _player.position + Vector3.up;
    }
}
