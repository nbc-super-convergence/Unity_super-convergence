
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static S2C_IceMiniGameReadyNotification.Types;

#if UNITY_EDITOR
public class InsuDebugger : Singleton<InsuDebugger>
{
    public bool isSingle;
    [SerializeField] private AudioClip sfxWatch;
    [SerializeField] private AudioClip sfxWhistle;


    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        {
            Debug.Log("디버그 시작");
            StartCoroutine(IceSkipper());
        }
        if (Input.GetKeyDown(KeyCode.Keypad1) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        {
            Debug.Log("대기방 시작");
            UIManager.Get<UIRoom>().GameStart();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        {
            Debug.Log("디버그 시작");
            StartCoroutine(CourtshipDanceSkipper());
        }
    }

    private void OpenDance()
    {
        // 보드에서 미니게임 입장.
        SceneManager.LoadScene(3, LoadSceneMode.Additive);
    }

    private IEnumerator CourtshipDanceSkipper()
    {
        //yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "BoardScene");

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
                        StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 1000
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
                                EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 1000
                            },
                            new TeamResult
                            {
                                SessionId = { "Session3" },
                                Score = 10,
                                EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 1200
                            },
                            new TeamResult
                            {
                                SessionId = { "Session4" },
                                Score = 5,
                                EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 1500
                            }
                        },
                        Reason = GameEndReason.TimeOver,
                        EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()// + 9000
                    }
                };
                SocketManager.Instance.DanceGameOverNotification(packet);
                break;
            }
            yield return null;
        }
    }

    private IEnumerator IceSkipper()
    {
        while (true)
        {

            if (Input.GetKey(KeyCode.CapsLock) && Input.GetKeyDown(KeyCode.C))
            {
                string[] Ids = new string[2];
                int num = 0;
                foreach (var item in GameManager.Instance.SessionDic)
                {
                    Ids[num] = item.Key;
                    num++;
                }

                GamePacket packet = new()
                {
                    IceMiniGameReadyNotification = new()
                    {
                        Players =
                        {                            
                            new startPlayers
                            {
                                SessionId = Ids[0],
                                Position = new Vector { X = -4, Y = 0.2f, Z = 4 },
                                Rotation = 135
                            },
                            new startPlayers
                            {
                                SessionId = Ids[1],
                                Position = new Vector { X = 4, Y = 0.2f, Z = 4 },
                                Rotation = -135
                            },                            
                        }
                    }
                };

                SocketManager.Instance.IceMiniGameReadyNotification(packet);
                break;
            }
            yield return null;
        }

        while (true)
        {
            if (Input.GetKey(KeyCode.CapsLock) && Input.GetKeyDown(KeyCode.V))
            {
                GamePacket packet = new()
                {
                    IceMiniGameStartNotification = new()
                };
                SocketManager.Instance.IceMiniGameStartNotification(packet);
                break;
            }
            yield return null;
        }
    }
}

#endif