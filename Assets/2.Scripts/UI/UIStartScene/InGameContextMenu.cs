using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InGameContextMenu : MonoBehaviour, IPointerClickHandler
{

    //���콺 ����Ŭ�������� ui����ĳ��Ʈ�� � ui�� �ִ��� Ȯ���ϰ�, ���ǹ��� �����Ѵٸ� ���ؽ�Ʈ�޴��� ����.
    // �� ������Ʈ�� �־��� �ִ� ��ü��.

    // 
    public async void OnPointerClick(PointerEventData eventData)
    {
        try
        {
            if (eventData.button == PointerEventData.InputButton.Right )
            {
                var result = await UIManager.Show<UIKick>();
                result.SetPosition(eventData.position.x, eventData.position.y);
                var roomUser = GetComponent<RoomUser>();
                result.SetPlayerId(roomUser.playerId);
                Debug.Log($"Current PlayerId : {result.targetPlayerId}");
            }
        }
        catch 
        {

        }

    }
}
