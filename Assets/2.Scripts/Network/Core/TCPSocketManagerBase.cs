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
using static GamePacket;  //Protocol.cs (�ڵ�����)

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

    /// <summary> ������ ������ ������ </summary>
    private byte[] remainBuffer = Array.Empty<byte>();
    /// <summary> ���� ������ ������: 1ûũ. </summary>
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
    /// �ı��� (���� �ı� ���� �ʴ´ٸ� �� ���� ��) ���� ���� ����
    /// </summary>
    private void OnDestroy()
    {
        Disconnect();
    }

    /// <summary>
    /// ip, port �ʱ�ȭ �� ��Ŷ ó�� �޼ҵ� ���
    /// </summary>
    public void Init()
    {//TODO: StartScene�� ���� ��ư ���� �� ȣ��.
        InitPackets();
        Connect();
        isInitialized = true;
    }

    /// <summary>
    /// �� Packet�� Header�� �´� receiveDic �����.
    /// </summary>
    private void InitPackets()
    {
        if (isPacketInit) return;

        //Header Arr : ������ ��ȯ�� ��� ��Ŷ ����.
        string[] headers = Enum.GetNames(typeof(PayloadOneofCase));

        //receiveDic {Key:Header Value:Action<GamePacket>} ����.
        foreach (string header in headers)
        {
            //Key
            PayloadOneofCase keyHeader = (PayloadOneofCase)Enum.Parse(typeof(PayloadOneofCase), header);

            //Action�� ȣ���� �Լ� : SocketManager�� ����.
            MethodInfo method = GetType().GetMethod(header);
            if (method != null)
            {//Value : �׼� ���� �� Dictionary ���.
                Action<GamePacket> action = (Action<GamePacket>)Delegate.CreateDelegate(typeof(Action<GamePacket>), this, method);
                receiveDic.Add(keyHeader, action);
            }
        }
        isPacketInit = true;
    }

    /// <summary>
    /// ��ϵ� ip, port�� ���� ����
    /// </summary>
    private async void Connect(UnityAction callback = null)
    {
        IPEndPoint endPoint; //ip�ּ� + ��Ʈ��ȣ
        if (IPAddress.TryParse(ip, out IPAddress ipAddress))
        {
            //���� �ּҷ� IPEndPoint ����
            endPoint = new IPEndPoint(ipAddress, port);
        }
        else
        {
            //ip�� ��ȿ���� �ʴٸ� ���� ȣ��Ʈ �ּҷ� IPEndPoint ����
            endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        }
        Debug.Log("Tcp Ip : " + ipAddress.MapToIPv4().ToString() + ", Port : " + port);
        
        //Socket ����
        socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            //������ ���� �õ�
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
    /// ������ ��Ŷ�� ���� �� ȣ��.
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
    /// �����κ��� �����͸� �ޱ�.
    /// </summary>
    private async void OnReceive()
    {
        if (socket != null)
        {
            while (socket.Connected && isConnected)
            {
                try
                {
                    //���ο� �����͸� �ޱ� ������ ���.
                    int recvByteLength = await socket.ReceiveAsync(recvBuffer, SocketFlags.None); 
                    if (!isConnected)
                    {
                        Debug.LogWarning("Socket is disconnected");
                        break;
                    }
                    if (recvByteLength <= 0) continue; //������ ���ŵ��� ����.

                    //fullBuffer = remainBuffer + recvBuffer.
                    byte[] fullBuffer = new byte[remainBuffer.Length + recvByteLength];
                    Buffer.BlockCopy(remainBuffer, 0, fullBuffer, 0, remainBuffer.Length);
                    Buffer.BlockCopy(recvBuffer, 0, fullBuffer, remainBuffer.Length, recvByteLength);

                    int processedLength = 0;
                    while (processedLength < fullBuffer.Length)
                    {
                        //��Ŷ����(2) + ��������(1) + ��������ȣ(4) + ���̷ε� ����(4)
                        if (fullBuffer.Length - processedLength < 11) break;

                        /*������ �б�*/
                        using var stream = new MemoryStream(fullBuffer, processedLength, fullBuffer.Length - processedLength);
                        using var reader = new BinaryReader(stream);

                        /*��Ŷ ���� �б�*/ 
                        byte[] typeBytes = reader.ReadBytes(2);
                        Array.Reverse(typeBytes); //�� �����(��Ʈ��ũ) -> ��Ʋ �����(PC)
                        PayloadOneofCase type = (PayloadOneofCase)BitConverter.ToInt16(typeBytes);
                        //Debug.Log($"PacketType:{type}");

                        /*���� �б�*/
                        byte versionLength = reader.ReadByte();
                        if (fullBuffer.Length - processedLength < 11 + versionLength) break; //���� ���� ���� ��ȿ�� �˻�
                        byte[] versionBytes = reader.ReadBytes(versionLength);
                        string version = BitConverter.ToString(versionBytes);

                        /*������ ��ȣ: ��Ŷ ���� ������.*/
                        byte[] sequenceBytes = reader.ReadBytes(4);
                        Array.Reverse(sequenceBytes);
                        int sequence = BitConverter.ToInt32(sequenceBytes);

                        /*���̷ε�*/
                        byte[] payloadLengthBytes = reader.ReadBytes(4);
                        Array.Reverse(payloadLengthBytes);
                        int payloadLength = BitConverter.ToInt32(payloadLengthBytes);
                        if (fullBuffer.Length - processedLength < 11 + versionLength + payloadLength) break; //��ȿ
                        byte[] payloadBytes = reader.ReadBytes(payloadLength);
                        
                        /*��Ŷ ���� �� ť�� ����*/
                        int totalLength = 11 + versionLength + payloadLength;
                        Packet packet = new Packet(type, version, sequence, payloadBytes);
                        receiveQueue.Enqueue(packet);
                        //string result = string.Join(", ", payloadBytes);
                        //Debug.Log($"{result}");
                        //Debug.Log($"Enqueued Type: {type}|{receiveQueue.Count}");

                        processedLength += totalLength;
                    }

                    /*���� ������: ���� ���ſ� �̾ ó��.*/
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
                Debug.Log("���� ���ú� ���� �ٽ� ����");
                OnReceive();
            }
        }
    }

    /// <summary>
    /// sendQueue�� �����Ͱ� ���� �� ���Ͽ� ����
    /// </summary>
    private IEnumerator OnSendQueue()
    {
        while (true)
        {
            //Queue�� ��Ŷ�� ��������� ���
            yield return new WaitUntil(() => sendQueue.Count > 0); 
            
            Packet packet = sendQueue.Dequeue();
            int sent = socket.Send(packet.ToByteArray(), SocketFlags.None);
            Debug.Log($"Send Packet: {packet.type}\nSent bytes: {sent}");

            yield return new WaitForSeconds(0.01f);
        }
    }

    /// <summary>
    /// receiveQueue�� �����Ͱ� ���� �� ��Ŷ Ÿ�Կ� ���� �̺�Ʈ ȣ��
    /// </summary>
    private IEnumerator OnReceiveQueue()
    {
        while (true)
        {
            //Queue�� ��Ŷ�� ��������� ���
            yield return new WaitUntil(() => receiveQueue.Count > 0);

            try
            {
                var packet = receiveQueue.Dequeue();
                Debug.Log("Receive Packet : " + packet.type.ToString()); //header ���.
                receiveDic[packet.type].Invoke(packet.gamePacket); //�̺�Ʈ ȣ��
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
    /// �������� ���� ����
    /// </summary>
    /// <param name="isReconnect">�翬�� ����</param>
    public void Disconnect(bool isReconnect = false)
    {
        StopAllCoroutines();
        if (isConnected)
        {
            isConnected = false;

            // ������ ���� ���� �˸� ��Ŷ ����
            //GamePacket packet = new() { LoginRequest = new() };
            //OnSend(packet); // ������ ���� ���� ��û
            socket.Disconnect(isReconnect); //���� ���� ����

            if (isReconnect)
            {
                Connect();
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "StartScene")
                {
                    //TODO: StartScene �ε� �� ������ �߻��Ѵٸ� �񵿱�� �ٲ� ��.
                    SceneManager.LoadScene("StartScene");
                }
                else
                {
                    //TODO: �κ�, �� UI ����.
                }
            }
        }
    }
}