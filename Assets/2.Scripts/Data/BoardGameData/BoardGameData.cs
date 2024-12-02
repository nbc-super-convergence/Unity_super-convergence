using System;
using System.Collections.Generic;

[Serializable]
public class BoardGameData
{
    public int diceCount = 0; // ���ݱ��� ������ ��
    public int purchaseArea = 0; //���ݱ��� �Ϲ� ���� ������ �� 
    public int sellArea = 0; //�μ� ���� ��
    public int payment = 0; //������ ������ ��
    public int Tax = 0; //������ ������ ��
    public int arriveMineArea = 0; //�� ���� ������ ��
    public int loseMiniGame = 0; //�̴ϰ��� �й��
    public int WinMiniGame = 0; //�̴ϰ��� �¸���
    public int highSaveCoin = 0; //���� �� �ִ� ���� ������
}


public interface IGameResult
{
    public List<int> Result();
}