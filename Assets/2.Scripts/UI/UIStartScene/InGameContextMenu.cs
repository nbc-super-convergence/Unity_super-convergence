using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InGameContextMenu : MonoBehaviour, IPointerClickHandler
{

    //마우스 오른클릭했을때 ui레이캐스트에 어떤 ui가 있는지 확인하고, 조건문에 부합한다면 컨텍스트메뉴를 연다.
    // 이 컴포넌트가 넣어져 있는 객체만.

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
