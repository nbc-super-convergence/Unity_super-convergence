using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UIMakeRoom : UIBase
{
    [SerializeField] private TMP_InputField RoomNameInput;

    private TaskCompletionSource<bool> sourceTcs;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnApplyBtn();
        }
    }

    public void OnBackBtn()
    {
        UIManager.Hide<UIMakeRoom>();
    }

    public async void OnApplyBtn()
    {
        if (RoomNameInput.text.Length > 12 || RoomNameInput.text.Length < 2)
        {
            await UIManager.Show<UIError>("방 이름은 2자 이상 12자 이하이어야 합니다.");
            return;
        }

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
            UIManager.Hide<UILobby>();
            UIManager.Hide<UIMakeRoom>();
        }
    }
    public void TrySetTask(bool isSuccess)
    {
        bool b = sourceTcs.TrySetResult(isSuccess);
    }
}
