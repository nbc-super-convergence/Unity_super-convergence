using UnityEngine;

public class RoomPlayerManager : MonoBehaviour
{
    [SerializeField]private RoomUser[] users = new RoomUser[4];
    private int currentUserCount = 0;

    private void RefrashRoomUser(RoomUser user, string nickName)
    {
        user.SetNickname(nickName);
    }


    #region 유저 입장

    // 



    #endregion


    #region 유저 퇴장

    #endregion

}