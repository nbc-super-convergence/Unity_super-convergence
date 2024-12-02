using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static GamePacket;  //Protocol.cs (자동생성)

public class TCPSocketManagerBase<T> : Singleton<TCPSocketManagerBase<T>>
{
    private Dictionary<PayloadOneofCase, Action<GamePacket>> receiveDic = new();
    private Queue<Packet> sendQueue = new();
    private Queue<Packet> receiveQueue = new();

    private Socket socket;
    private string version = "1.0.0";
    private int sequenceNumber = 1;

    [SerializeField] private string ip;
    [SerializeField] private int port;

    /// <summary> 이전에 수신한 데이터 </summary>
    private byte[] remainBuffer = Array.Empty<byte>();
    /// <summary> 새로 수신한 데이터: 1청크. </summary>
    private byte[] recvBuffer = new byte[1024];

    private bool isPacketInit = false;
    private bool _isConnected;
    public bool isConnected
    {
        get => _isConnected;
        set
        {
            if (_isConnected != value)
            {
                _isConnected = value;
                if (!_isConnected)
                    isLobby = false;
            }
        }
    }
    public bool isLobby = false;

    /// <summary>
    /// 파괴시 (따로 파괴 하지 않는다면 앱 종료 시) 소켓 연결 해제
    /// </summary>
    private void OnDestroy()
    {
        Disconnect();
    }

    /// <summary>
    /// ip, port 초기화 후 패킷 처리 메소드 등록
    /// </summary>
    public void Init()
    {//TODO: StartScene의 시작 버튼 누를 때 호출.
        InitPackets();
        Connect();
        isInitialized = true;
    }

    /// <summary>
    /// 각 Packet의 Header에 맞는 receiveDic 만들기.
    /// </summary>
    private void InitPackets()
    {
        if (isPacketInit) return;

        //Header Arr : 서버와 교환할 모든 패킷 종류.
        string[] headers = Enum.GetNames(typeof(PayloadOneofCase));

        //receiveDic {Key:Header Value:Action<GamePacket>} 생성.
        foreach (string header in headers)
        {
            //Key
            PayloadOneofCase keyHeader = (PayloadOneofCase)Enum.Parse(typeof(PayloadOneofCase), header);

            //Action이 호출할 함수 : SocketManager에 정의.
            MethodInfo method = GetType().GetMethod(header);
            if (method != null)
            {//Value : 액션 정의 및 Dictionary 등록.
                Action<GamePacket> action = (Action<GamePacket>)Delegate.CreateDelegate(typeof(Action<GamePacket>), this, method);
                receiveDic.Add(keyHeader, action);
            }
        }
        isPacketInit = true;
    }

    /// <summary>
    /// 등록된 ip, port로 소켓 연결
    /// </summary>
    private async void Connect(UnityAction callback = null)
    {
        IPEndPoint endPoint; //ip주소 + 포트번호
        if (IPAddress.TryParse(ip, out IPAddress ipAddress))
        {
            //서버 주소로 IPEndPoint 생성
            endPoint = new IPEndPoint(ipAddress, port);
        }
        else
        {
            //ip가 유효하지 않다면 로컬 호스트 주소로 IPEndPoint 생성
            endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        }
        Debug.Log("Tcp Ip : " + ipAddress.MapToIPv4().ToString() + ", Port : " + port);
        
        //Socket 생성
        socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            //서버와 연결 시도
            await socket.ConnectAsync(endPoint);
            isConnected = socket.Connected;

            OnReceive();
            StartCoroutine(OnSendQueue());
            StartCoroutine(OnReceiveQueue());
            StartCoroutine(Ping());

            callback?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    /// <summary>
    /// 서버로 패킷을 보낼 때 호출.
    /// </summary>
    /// <param name="gamePacket"></param>
    public void OnSend(GamePacket gamePacket)
    {
        if (socket == null) return;
        var byteArray = gamePacket.ToByteArray();
        var packet = new Packet(gamePacket.PayloadCase, version, sequenceNumber++, byteArray);
        sendQueue.Enqueue(packet);
    }

    /// <summary>
    /// 서버로부터 데이터를 받기.
    /// </summary>
    private async void OnReceive()
    {
        if (socket != null)
        {
            while (socket.Connected && isConnected)
            {
                try
                {
                    //새로운 데이터를 받기 전까지 대기.
                    int recvByteLength = await socket.ReceiveAsync(recvBuffer, SocketFlags.None); 
                    if (!isConnected)
                    {
                        Debug.LogWarning("Socket is disconnected");
                        break;
                    }
                    if (recvByteLength <= 0) continue; //데이터 수신되지 않음.

                    //fullBuffer = remainBuffer + recvBuffer.
                    byte[] fullBuffer = new byte[remainBuffer.Length + recvByteLength];
                    Buffer.BlockCopy(remainBuffer, 0, fullBuffer, 0, remainBuffer.Length);
                    Buffer.BlockCopy(recvBuffer, 0, fullBuffer, remainBuffer.Length, recvByteLength);

                    int processedLength = 0;
                    while (processedLength < fullBuffer.Length)
                    {
                        //패킷유형(2) + 버전길이(1) + 시퀀스번호(4) + 페이로드 길이(4)
                        if (fullBuffer.Length - processedLength < 11) break;

                        /*데이터 읽기*/
                        using var stream = new MemoryStream(fullBuffer, processedLength, fullBuffer.Length - processedLength);
                        using var reader = new BinaryReader(stream);

                        /*패킷 유형 읽기*/ 
                        byte[] typeBytes = reader.ReadBytes(2);
                        Array.Reverse(typeBytes); //빅 엔디언(네트워크) -> 리틀 엔디언(PC)
                        PayloadOneofCase type = (PayloadOneofCase)BitConverter.ToInt16(typeBytes);
                        //Debug.Log($"PacketType:{type}");

                        /*버전 읽기*/
                        byte versionLength = reader.ReadByte();
                        if (fullBuffer.Length - processedLength < 11 + versionLength) break; //버전 정보 길이 유효성 검사
                        byte[] versionBytes = reader.ReadBytes(versionLength);
                        string version = BitConverter.ToString(versionBytes);

                        /*시퀀스 번호: 패킷 순서 추적용.*/
                        byte[] sequenceBytes = reader.ReadBytes(4);
                        Array.Reverse(sequenceBytes);
                        int sequence = BitConverter.ToInt32(sequenceBytes);

                        /*페이로드*/
                        byte[] payloadLengthBytes = reader.ReadBytes(4);
                        Array.Reverse(payloadLengthBytes);
                        int payloadLength = BitConverter.ToInt32(payloadLengthBytes);
                        if (fullBuffer.Length - processedLength < 11 + versionLength + payloadLength) break; //무효
                        byte[] payloadBytes = reader.ReadBytes(payloadLength);
                        
                        /*패킷 생성 및 큐에 저장*/
                        int totalLength = 11 + versionLength + payloadLength;
                        Packet packet = new Packet(type, version, sequence, payloadBytes);
                        receiveQueue.Enqueue(packet);
                        //string result = string.Join(", ", payloadBytes);
                        //Debug.Log($"{result}");
                        //Debug.Log($"Enqueued Type: {type}|{receiveQueue.Count}");

                        processedLength += totalLength;
                    }

                    /*남은 데이터: 다음 수신에 이어서 처리.*/
                    int remainLength = fullBuffer.Length - processedLength;
                    if (remainLength > 0)
                    {
                        remainBuffer = new byte[remainLength];
                        Array.Copy(fullBuffer, processedLength, remainBuffer, 0, remainLength);
                        break;
                    }
                    remainBuffer = Array.Empty<byte>();
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.StackTrace}");
                }
            }
            if (socket != null && socket.Connected)
            {
                Debug.Log("소켓 리시브 멈춤 다시 시작");
                OnReceive();
            }
        }
    }

    /// <summary>
    /// sendQueue에 데이터가 있을 시 소켓에 전송
    /// </summary>
    private IEnumerator OnSendQueue()
    {
        while (true)
        {
            //Queue에 패킷이 들어오기까지 대기
            yield return new WaitUntil(() => sendQueue.Count > 0); 
            
            Packet packet = sendQueue.Dequeue();
            int sent = socket.Send(packet.ToByteArray(), SocketFlags.None);
            Debug.Log($"Send Packet: {packet.type}\nSent bytes: {sent}");

            yield return new WaitForSeconds(0.01f);
        }
    }

    /// <summary>
    /// receiveQueue에 데이터가 있을 시 패킷 타입에 따라 이벤트 호출
    /// </summary>
    private IEnumerator OnReceiveQueue()
    {
        while (true)
        {
            //Queue에 패킷이 들어오기까지 대기
            yield return new WaitUntil(() => receiveQueue.Count > 0);

            try
            {
                var packet = receiveQueue.Dequeue();
                Debug.Log("Receive Packet : " + packet.type.ToString()); //header 출력.
                receiveDic[packet.type].Invoke(packet.gamePacket); //이벤트 호출
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator Ping()
    {
        while (SocketManager.Instance.isConnected)
        {
            yield return new WaitForSeconds(5);
            //GamePacket packet = new() { LoginResponse = new() };
            //SocketManager.Instance.OnSend(packet);
        }
    }

    /// <summary>
    /// 서버와의 연결 해제
    /// </summary>
    /// <param name="isReconnect">재연결 여부</param>
    public void Disconnect(bool isReconnect = false)
    {
        StopAllCoroutines();
        if (isConnected)
        {
            isConnected = false;

            // 서버에 연결 해제 알림 패킷 생성
            //GamePacket packet = new() { LoginRequest = new() };
            //OnSend(packet); // 서버에 연결 해제 요청
            socket.Disconnect(isReconnect); //소켓 연결 해제

            if (isReconnect)
            {
                Connect();
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "StartScene")
                {
                    //TODO: StartScene 로딩 시 지연이 발생한다면 비동기로 바꿀 것.
                    SceneManager.LoadScene("StartScene");
                }
                else
                {
                    //TODO: 로비, 룸 UI 끄기.
                }
            }
        }
    }
}