using UnityEngine;

public class IceSlidingCharacterRotate : MonoBehaviour
{
    [SerializeField] private float _rotationY = 0f; //������ �� �ʱ� �ޱۺ��� ���

    //���⼭ ȸ�� ����
    private void FixedUpdate()
    {
        //���⼭ Atan�� �̿��Ͽ� (x,z �� ����Ұ�����) ĳ���� ȸ��
        ApplyRotate();
    }

    /// <summary>
    /// �Է��� ������ ������ ����
    /// </summary>
    /// <param name="dir">IceSlidingController�� InputControll���� ���� ����</param>
    public void SetInput(Vector3 dir)
    {
        if(dir != Vector3.zero)
            _rotationY = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// ������ _rotationY���� ������ ����
    /// </summary>
    private void ApplyRotate()
    {
        transform.rotation = Quaternion.Euler(0, _rotationY, 0);
    }
}
