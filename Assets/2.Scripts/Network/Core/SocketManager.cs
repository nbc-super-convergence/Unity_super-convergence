using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Threading.Tasks;
using static GamePacket;

public class SocketManager : TCPSocketManagerBase<SocketManager>
{
    //Sample : http://wocjf84.synology.me:8418/ExternalSharing/Sparta_Node6th_Chapter5/src/branch/main/Assets/_Project/Scripts/Manager/SocketManager.cs

    //TODO: 새로운 함수 만들 때 private void로 만들 것.
    //TODO: 함수의 이름은 반드시 PayloadOneOfCase Enum과 맞출 것.
    //TODO: 인자는 GamePacket gamePacket.

    private void LoginResponse(GamePacket gamePacket)
    {
        //var response = gamePacket.LoginResponse;
        //if (response.Success)
        //{
        //    if (response.MyInfo != null)
        //    {
        //        UserInfo.myInfo = new UserInfo(response.MyInfo);
        //    }
        //    UIManager.Get<PopupLogin>().OnLoginEnd(response.Success);
        //}
    }
}