using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public class MinigameClientTest : Singleton<MinigameClientTest>
{
    private void Start()
    {
        StartCoroutine(IceBoardSkipper());
    }

    private IEnumerator IceBoardSkipper()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.LeftControl) &&
            Input.GetKey(KeyCode.LeftShift) &&
            Input.GetKeyDown(KeyCode.I))
            {
                GameManager.isGameStart = true;

                GamePacket packet = new()
                {
                    IceMiniGameReadyNotification = new()
                    {
                        
                    }
                };

                SocketManager.Instance.IceMiniGameReadyNotification(packet);
                break;
            }
            yield return null; 
        }
    }
}
#endif