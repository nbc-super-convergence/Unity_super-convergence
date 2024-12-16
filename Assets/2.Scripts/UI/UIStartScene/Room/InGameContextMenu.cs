using UnityEngine;
using UnityEngine.EventSystems;

public class InGameContextMenu : MonoBehaviour, IPointerClickHandler
{
    public async void OnPointerClick(PointerEventData eventData)
    {
        try
        {
            if (!UIManager.Get<UIRoom>().IsHost) return;

            if (eventData.button == PointerEventData.InputButton.Right )
            {
                var result = await UIManager.Show<UIKick>();
                result.SetPosition(eventData.position.x, eventData.position.y);
                var roomUser = GetComponent<RoomUserSlot>();
                //result.SetPlayerId(roomUser.sessionId);
                Debug.Log($"Current PlayerId : {result.targetPlayerId}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in InGameContextMenu.OnPointerClick: {e.Message}");
            Debug.LogException(e);
        }
    }
}
