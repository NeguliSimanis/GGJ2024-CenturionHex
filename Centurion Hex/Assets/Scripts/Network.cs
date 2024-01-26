using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using UnityEngine.Events;
using UnityEngine.Networking;

public class Network : MonoBehaviour
{
    [System.Serializable]
    public class GameNetworkEvent : UnityEvent<object> { }

    public enum Messages
    {
        op_ping = 0,
        op_login = 1,
        op_logged_out = 2,
        op_game_data = 3
    }

    public enum NetworkStateEnum
    {
        nsDisconnected,
        nsConnecting,
        nsConnected,
        nsDisconnecting,
    };

    [Space(20)]
    public UnityEvent OnConnectedEvent;
    public UnityEvent OnDisconnectedEvent;

    [Header("Network Events")]
    [HideInInspector] public GameNetworkEvent onLogin;
    [HideInInspector] public GameNetworkEvent onLoggedOut;
    [HideInInspector] public GameNetworkEvent onPingEvent;
    [HideInInspector] public GameNetworkEvent onGameData;

    public const String version = "v 0.0.0";
    public string ip = "127.0.0.1";
    public int port = 7162;

    TcpClient client;
    NetworkStream stream;
    float ConnectTimeout;

    ByteArray outgoingData = new ByteArray();
    ByteArray incomingData = new ByteArray();
    UInt16 PacketSize = 0;

    float pingRecv = -1;
    float pingDelay = 0;
    float pingSent = 0;
    float pingRoundtrip = 0;

    NetworkStateEnum NetworkState = NetworkStateEnum.nsDisconnected;
    NetworkStateEnum PrevNetworkState = NetworkStateEnum.nsDisconnected;

    bool ShouldConnect = false;
    int loginRetries = 0;
    public uint CurrentTimestamp { get { return (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; } }
    public int LocalPlayerUID { get; private set; }

    // Update is called once per frame
    private void Update()
    {
        switch (NetworkState)
        {
            case NetworkStateEnum.nsConnecting:
                ConnectTimeout -= Time.deltaTime;
                if (ConnectTimeout < 0)
                {
                    try
                    {
                        if (client != null)
                        {
                            client.Close();
                        }

                        NetworkState = NetworkStateEnum.nsDisconnected;
                    }
                    catch (Exception)
                    {

                    }
                }
                break;
            case NetworkStateEnum.nsDisconnected:
                if (ShouldConnect)
                {
                    //uiController.SetConnectingOverlay(true);
                    //initiate connect
                    if (client == null)
                    {
                        client = new TcpClient();
                    }
                    NetworkState = NetworkStateEnum.nsConnecting;
                    ConnectTimeout = 10;
                    client.BeginConnect(ip, port, asyncResult =>
                    {
                        try
                        {
                            client.EndConnect(asyncResult);
                            //Debug.Log("socket connected");
                            stream = client.GetStream();
                            outgoingData.clear();
                            incomingData.clear();
                            PacketSize = 0;
                            pingRecv = -1;
                            NetworkState = NetworkStateEnum.nsConnected;
                            OnConnectedEvent.Invoke();
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                client.Close();
                                client = null;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("client.Close exception: " + ex.ToString());
                            }

                            throw new Exception("socket closed before connected; e: " + e.ToString());
                            NetworkState = NetworkStateEnum.nsDisconnected;
                        }
                    }, this);
                }
                break;
            case NetworkStateEnum.nsConnected:
                //send regular pings and receive data
                try
                {
                    while (client.Available > 0)
                    {
                        if (PacketSize == 0)
                        {
                            if (client.Available >= 2)
                            {
                                byte[] tmp = new byte[2];
                                if (stream.Read(tmp, 0, 2) != 2)
                                    throw new Exception();
                                PacketSize = BitConverter.ToUInt16(tmp, 0);
                                PacketSize -= 2;
                            }
                            else break;//not enough bytes
                        }
                        if (PacketSize > 0)
                        {
                            if (client.Available >= PacketSize)
                            {
                                incomingData.clear();
                                incomingData.ensure(PacketSize);
                                int bytesRead = stream.Read(incomingData.arr, 0, PacketSize);
                                if (bytesRead != PacketSize)
                                    throw new Exception();
                                incomingData.position = 0;
                                incomingData.length = PacketSize;
                                //process packet
                                while (incomingData.bytesAvailable() > 0)
                                {
                                    ProcessCommand();
                                }
                                incomingData.clear();
                                PacketSize = 0;
                            }
                            else break;//wait more
                        }
                    }
                    if (pingRecv == -1)
                    {
                        pingDelay = 0;
                        pingRecv = 0;
                        pingSent = Time.time;
                        outgoingData.writeByte((byte)Messages.op_ping);
                        Send("ping");
                    }
                    else
                    {
                        pingDelay += Time.deltaTime;
                        if (pingDelay > 5)
                        {
                            this.pingRecv = -1;//send another ping
                        }
                    }
                }
                catch (Exception e)
                {
                    //Debug.Log("socket disconnected on read " + e.ToString());
                    try
                    {
                        client.Close();
                        client = null;
                    }
                    catch (Exception)
                    {

                    }
                    NetworkState = NetworkStateEnum.nsDisconnected;
                }
                break;
        }

        if (PrevNetworkState != NetworkState)
        {
            switch (NetworkState)
            {
                case NetworkStateEnum.nsConnecting:
                    //uiController.SetConnectingOverlay(true);
                    break;
                case NetworkStateEnum.nsConnected:
                    //uiController.SetConnectingOverlay(false); // Wait login without disabling
                    outgoingData.writeByte((byte)Messages.op_login);
                    //outgoingData.writeUnsignedInt(userid);
                    //outgoingData.writeUnsignedInt(timestamp);
                    //outgoingData.writeUTF(hash);
                    //outgoingData.writeUTF(version);
                    Send("connected");
                    break;
                case NetworkStateEnum.nsDisconnected:
                    OnDisconnectedEvent.Invoke();

                    if (ShouldConnect)
                    {
                        //uiController.SetCurrentState(UI_Controller.UIStateType.Disconnected);
                    }

                    if (PrevNetworkState == NetworkStateEnum.nsConnecting)
                    {
                        //could not connect?
                        if (loginRetries++ < 3)
                        {
                            //GetLoginData(m_curEmail, m_curPass);
                        }
                        else
                        {
                            FailedToAuth();
                        }
                    }
                    else
                    {
                        //normal disconnect
                        if (!ShouldConnect)
                        {
                            //disconnected with force - show reconnect button
                            ShowReconnect();
                        }
                    }
                    break;
            }

            PrevNetworkState = NetworkState;
        }
    }

    public void Connect()
    {
        //uiController.SetCurrentState(UI_Controller.UIStateType.Connecting);
        //uiController.SetConnectingOverlay(true);
        Connect(ip, port);
    }

    public void Disconnect()
    {
        if(client != null)
        {
            client.Close();
            ShouldConnect = false;
            NetworkState = NetworkStateEnum.nsDisconnected;
            client = null;
        }
    }
    public void Connect(string _ip, int _port )
    {
        ip = _ip;
        port = _port;
        ShouldConnect = true;
    }

    private void ShowReconnect()
    {
//        uiController.SetConnectingOverlay(false);
  //      uiController.SetCurrentState(UI_Controller.UIStateType.Reconnect);
    }

    private void FailedToAuth()
    {
        ShouldConnect = false;
    //    uiController.SetConnectingOverlay(false);
      //  uiController.SetCurrentState(UI_Controller.UIStateType.Reconnect);
    }

    private void Send(string commandInfo, params string[] optionalParams)
    {
        if (outgoingData.length > 0)
        {
            try
            {
                stream.Write(BitConverter.GetBytes((ushort)outgoingData.length), 0, 2);
                stream.Write(outgoingData.arr, 0, outgoingData.length);
                stream.Flush();
                outgoingData.clear();
            }
            catch( Exception )
            {
                try
                {
                    client.Close();
                }
                catch (Exception)
                {

                }
            }
        }
    }

    //////////void onPing()
    //////////{
    //////////    //trace('onping');
    //////////    pingRecv = Time.time;
    //////////    pingRoundtrip = pingRecv - pingSent;
    //////////    pingDelay = 4;//wait one more second to send out next ping

    //////////    var stats = new Stats();
    //////////    stats.users = incomingData.readUnsignedShort();
    //////////    stats.online_friends = incomingData.readUnsignedShort();
    //////////    stats.online_buddies = incomingData.readUnsignedShort();
    //////////    stats.games = incomingData.readUnsignedShort();
    //////////    stats.time_till_reset = incomingData.readUnsignedInt();
    //////////    stats.uid = incomingData.readUnsignedInt();

    //////////    Debug.Log("Ping: " + pingRoundtrip );

    //////////    onPingEvent.Invoke(stats);

    //////////    //this.in_func = 'onStats';
    //////////    //this.callbackObject.onStats(stats);
    //////////    //this.in_func = '';
    //////////}

    private void ProcessCommand()
    {
        Messages command = (Messages)incomingData.readByte();

        //if (command != Messages.op_ping)
        //{
        //    Debug.Log("Command received: " + command.ToString() + AppendTime);
        //}

        switch (command)
        {
            case Messages.op_ping:
                //onPing();
                pingRecv = DateTime.Now.Ticks;
                pingRoundtrip = (float)(pingRecv - pingSent) / 10000000.0f;
                pingDelay = 4;//wait one more second to send out next ping

                InvokeEventWithData(onPingEvent, command, null);
                break;
            case Messages.op_login:
                break;
            case Messages.op_game_data:
                {
                    InvokeEventWithData(onGameData, command, incomingData);
                }
                break;
            default:
                UnityEngine.Debug.LogError("Message not handled: " + command);
                throw new NotImplementedException("Message not handled: " + command);
        }        
    }

    //////////private void onLogin()
    //////////{
    //////////    Params p = new Params();
    //////////    p.CHOOSE_TIMEOUT = incomingData.readByte();
    //////////    p.THINK_TIMEOUT = incomingData.readByte();
    //////////    p.PRESENT_TIMEOUT = incomingData.readByte();
    //////////    p.CONTINUE_TIMEOUT = incomingData.readByte();

    //////////    //this.callbackObject.onParams(params);

    //////////    var user = this.readUser();
    //////////    this.uid = user.id;

    //////////    if (user.id != 0)
    //////////    {
    //////////        loginRetries = 0;
             
    //////////        user.money = incomingData.readUnsignedInt();
    //////////        user.score = incomingData.readUnsignedInt();
    //////////        //user.chips = this.mReadBuffer.readUnsignedInt();
    //////////        //user.userdata = mReadBuffer.readUTF();
    //////////        user.lvl_start = incomingData.readUnsignedInt();
    //////////        user.lvl_end = incomingData.readUnsignedInt();
    //////////        user.true_skill = incomingData.readUnsignedInt();
    //////////    }

    //////////    //this.callbackObject.onLogin(user);

    //////////    if (user.id == 0)
    //////////    {
    //////////        hash = "";

    //////////        Debug.Log("Failed to log in");
    //////////        //this.callbackObject.onLoggedOut(false);
    //////////        //maybe hash expired - retrieve new one
    //////////        if (loginRetries++ < 3)
    //////////        {
    //////////            ShouldConnect = false;
    //////////            GetLoginData();
    //////////        }
    //////////        else
    //////////        {
    //////////            //can't login - maybe server down or smth? - fallback to fb screen
    //////////            FailedToAuth();
    //////////        }
    //////////    }
    //////////    else
    //////////    {
    //////////        Debug.Log("logged in - " + user.id);
    //////////        MainMenuPanel.SetActive(true);
    //////////        TopInfoPanel.SetActive(true);//fill also with values ( money n shit )
    //////////    }
    //////////}

    #region GameNetworkEvents
    private void InvokeEventWithData(GameNetworkEvent gameNetworkEvent, Messages command, object data)
    {
        InvokeEventWithData(gameNetworkEvent, command.ToString(), data);
    }

    private void InvokeEventWithData(GameNetworkEvent gameNetworkEvent, string command, object data)
    {
        gameNetworkEvent.Invoke(data);
    }

    /// <summary>
    /// WARNING Doesnt output object memeber lists
    /// </summary>
    private string ConvertToReadable(object data)
    {
        if (data.GetType().IsPrimitive)
        {
            return data.ToString();
        }
        else if (data is IList list)
        {
            string output = "";

            if (list.Count > 0)
            {
                foreach (object element in list)
                {
                    output += ConvertToReadable(element) + ", ";
                }
                return output.Substring(0, output.Length - 2); ;
            }
            else
            {
                return "#empty list#";
            }
        }
        else
        {
            return data.GetType().Name + JsonUtility.ToJson(data);
        }
    }

    #endregion  // ~GameNetworkEvents
}