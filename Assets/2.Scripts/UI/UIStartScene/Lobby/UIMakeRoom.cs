using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UIMakeRoom : UIBase
{
    [SerializeField] private TMP_InputField RoomNameInput;

    private TaskCompletionSource<bool> sourceTcs;

    public void OnBackBtn()
    {
        UIManager.Hide<UIMakeRoom>();
    }

    public async void OnApplyBtn()
    {
        //Send Make Room

        if (SocketManager.Instance.isConnected)
        {
            GamePacket packet = new();

            packet.CreateRoomRequest = new()
            {
                SessionId = GameManager.Instance.myInfo.SessionId,
                RoomName = RoomNameInput.text
            };

            sourceTcs = new();
            SocketManager.Instance.OnSend(packet);
        }

        bool isSuccess = await sourceTcs.Task;
        if (isSuccess)
        {
            //await UIManager.Show<UIRoom>();
            UIManager.Hide<UILobby>();

            UIManager.Hide<UIMakeRoom>();
        }
    }
    public void TrySetTask(bool isSuccess)
    {
        bool boolll = sourceTcs.TrySetResult(isSuccess);
        Debug.Log(boolll ? "规 积己 己傍" : "规 积己 角菩");
    }
}
