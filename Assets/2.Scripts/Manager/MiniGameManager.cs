using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : Singleton<MiniGameManager>
{
    #region �������
    MiniGameData gameData;
    #endregion

    #region Unity�޼ҵ�
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
    #endregion

    #region ���� �޼���
    public virtual void GameStart()
    {

    }

    public virtual void GameStop() 
    { 
    
    }
   
    public virtual void GameEnd()
    {

    }
    #endregion

    #region ���� �޼���
    //�ʱ�ȭ �۾� - kh
    public virtual void Init()
    {
        isDontDestroyOnLoad = false;
    }

    //���� ���� ���� - kh
    public void ChangeScore(int _score)
    {

    }

    //���� �ð� ���� - kh
    public void ChangeTime(float _time)
    {

    }

    //�̴ϰ��� �� ���� - kh
    public void OnChangeMap()
    {

    }

    //����� ���� - kh
    public void OnChangedBGM()
    {

    }
    #endregion
}
