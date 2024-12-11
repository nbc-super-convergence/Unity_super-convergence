
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
public class InsuDebugger : Singleton<InsuDebugger>
{
    public bool isSingle;

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        { OpenDance(); }
        if (Input.GetKeyDown(KeyCode.Alpha9) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        {
            StartCoroutine(CourtshipDanceSkipper());             
        }
        

        if (Input.GetKeyDown(KeyCode.Alpha4) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        {
            UIManager.Get<UICourtshipDance>().Next("Session1");
            UIManager.Get<UICourtshipDance>().Next("Session2");
            UIManager.Get<UICourtshipDance>().Next("Session3");
            UIManager.Get<UICourtshipDance>().Next("Session4");
        }
    }

    private void OpenDance()
    {
        // 보드에서 미니게임 입장.
        SceneManager.LoadScene(3, LoadSceneMode.Additive);
    }

    private IEnumerator CourtshipDanceSkipper()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "BoardScene");

        while (true)
        {
            if (Input.GetKey(KeyCode.LeftControl) &&
            Input.GetKeyDown(KeyCode.Alpha8))
            {
                GamePacket packet = new GamePacket()
                {
                    DanceMiniGameReadyNotification = new()
                    {
                        Players =
                        {
                            new PlayerInfo
                            {
                                SessionId = "Session1",
                                TeamNumber = 1
                            },
                            new PlayerInfo
                            {
                                SessionId = "Session2",
                                TeamNumber = 2
                            },
                            new PlayerInfo
                            {
                                SessionId = "Session3",
                                TeamNumber = 3
                            },
                            new PlayerInfo
                            {
                                SessionId = "Session4",
                                TeamNumber = 4
                            },
                        }
                    }
                };
                SocketManager.Instance.DanceMiniGameReadyNotification(packet);
                break;
            }
            yield return null;

        }

        while(true)
        {
            if (Input.GetKey(KeyCode.LeftControl) &&
            Input.GetKeyDown(KeyCode.Alpha7))
            {
                GamePacket packet = new GamePacket()
                {
                    DanceStartNotification = new()
                    {
                        StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 4000
                    }
                };
                SocketManager.Instance.DanceStartNotification(packet);
                break;
            }
            yield return null;
        }

        while(true)
        {
            if (Input.GetKey(KeyCode.LeftControl) &&
            Input.GetKeyDown(KeyCode.Alpha6))
            {
                GamePacket packet = new GamePacket()
                {
                    DanceGameOverNotification = new()
                    {
                        TeamRank = {1,2,3,4 },
                        Result = 
                        {
                            new TeamResult
                            {
                                SessionId = { "Session1" },
                                Score = 20,
                                EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                            },
                            new TeamResult
                            {
                                SessionId = { "Session2" },
                                Score = 15,
                                EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                            },
                            new TeamResult
                            {
                                SessionId = { "Session3" },
                                Score = 10,
                                EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                            },
                            new TeamResult
                            {
                                SessionId = { "Session4" },
                                Score = 5,
                                EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                            }
                        },
                        Reason = GameEndReason.TimeOver,
                        EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 4000
                    }
                };
                SocketManager.Instance.DanceGameOverNotification(packet);
                break;
            }
            yield return null;
        }
    }

}
#endif