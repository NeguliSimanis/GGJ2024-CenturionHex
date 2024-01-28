using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.TextCore.Text;

public class Network : MonoBehaviour
{
    [System.Serializable]
    public class GameNetworkEvent : UnityEvent<object> { }

    public enum Messages
    {
        op_ping,
        op_login,
        op_logged_out,
        op_game_data,
        op_game_round_update,
        op_wealth_from_building,
        op_move_character,
        op_cover_tile,
        op_wealth_from_cover,
        op_point_from_cover,
        op_point_from_building,
        op_character_hurt,
        op_end_move,
        op_hurt_tile,
        op_building_hurt,
        op_buy_building_card,
        op_buy_character_card,
        op_place_building,
        op_place_character,
        op_update_gold,
        op_update_points
    }

    public enum NetworkStateEnum
    {
        nsDisconnected,
        nsConnecting,
        nsConnected,
        nsDisconnecting,
    };
    public static Network instance;
    public CenturionGame Game;

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
    public bool loggedIn = false;

    public uint CurrentTimestamp { get { return (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; } }
    public int LocalPlayerUID { get; private set; }

    

    private void Start()
    {
        instance = this;
        Connect();
    }

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
                            //OnConnectedEvent.Invoke();
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
                                //Debug.Log("got packet size " + PacketSize.ToString());
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
                                    if(incomingData.bytesAvailable() > 0)
                                    {
                                        //Debug.LogError("Some leftover data here " + incomingData.bytesAvailable().ToString());
                                    }
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
                    Debug.Log("socket disconnected on read " + e.ToString());
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
                    outgoingData.writeUTF(SystemInfo.deviceUniqueIdentifier);
                    //outgoingData.writeUnsignedInt(userid);
                    //outgoingData.writeUnsignedInt(timestamp);
                    //outgoingData.writeUTF(hash);
                    //outgoingData.writeUTF(version);
                    Send("connected");
                    break;
                case NetworkStateEnum.nsDisconnected:
                    //OnDisconnectedEvent.Invoke();

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
        {
            //Debug.Log("Command received: " + command.ToString() );
        }

        switch (command)
        {
            case Messages.op_ping:
                //onPing();
                pingRecv = DateTime.Now.Ticks;
                pingRoundtrip = (float)(pingRecv - pingSent) / 10000000.0f;
                pingDelay = 4;//wait one more second to send out next ping

                //InvokeEventWithData(onPingEvent, command, null);
                break;
            case Messages.op_login:
                loggedIn = incomingData.readBoolean();
                break;
            case Messages.op_game_data:
                {
                    Game.LoadFromNetwork(incomingData);
                    //InvokeEventWithData(onGameData, command, incomingData);
                }
                break;
            case Messages.op_game_round_update:
                Game.OnRoundUpdate(incomingData.readBoolean(), incomingData.readBoolean(), (CenturionGame.RoundState)incomingData.readByte());
                break;
            case Messages.op_wealth_from_building:
                Game.OnWealthFromBuilding((Team.TeamType)incomingData.readByte(), incomingData.readByte(), incomingData.readUnsignedInt());
                break;
            case Messages.op_move_character:
                Game.OnCharacterMoved(incomingData.readUnsignedInt(), incomingData.readByte(), incomingData.readByte(), incomingData.readByte());
                break;
            case Messages.op_cover_tile:
                Game.OnTileCovered(incomingData.readByte(), incomingData.readByte(), (TileCover.CoverType)incomingData.readByte(), (TileCover.BonusType)incomingData.readByte());
                break;
            case Messages.op_wealth_from_cover:
                Game.OnWealthFromCover((Team.TeamType)incomingData.readByte(), incomingData.readByte(), incomingData.readByte(), incomingData.readByte());
                break;
            case Messages.op_point_from_cover:
                Game.OnPointFromCover((Team.TeamType)incomingData.readByte(), incomingData.readByte(), incomingData.readByte(), incomingData.readByte());
                break;
            case Messages.op_point_from_building:
                Game.OnPointFromBuilding((Team.TeamType)incomingData.readByte(), incomingData.readByte(), incomingData.readUnsignedInt());
                break;
            case Messages.op_character_hurt:
                Game.OnCharacterHurt(incomingData.readUnsignedInt(), incomingData.readByte());
                break;
            case Messages.op_building_hurt:
                Game.OnBuildingHurt(incomingData.readUnsignedInt(), incomingData.readByte());
                break;
            case Messages.op_buy_building_card:
                Game.OnBuildingBought(incomingData.readUnsignedInt(), (Team.TeamType)incomingData.readByte());
                break;
            case Messages.op_buy_character_card:
                Game.OnCharacterBought(incomingData.readUnsignedInt(), (Team.TeamType)incomingData.readByte());
                break;
            case Messages.op_update_gold:
                Game.OnUpdateGold((Team.TeamType)incomingData.readByte(), incomingData.readByte());
                break;
            case Messages.op_update_points:
                Game.OnUpdatePoints((Team.TeamType)incomingData.readByte(), incomingData.readByte());
                break;
            case Messages.op_place_character:
                Game.OnPlaceCharacter(incomingData.readUnsignedInt(), (Team.TeamType)incomingData.readByte(), incomingData.readByte(), incomingData.readByte() );
                break;
            case Messages.op_place_building:
                Game.OnPlaceBuilding(incomingData.readUnsignedInt(), (Team.TeamType)incomingData.readByte(), incomingData.readByte(), incomingData.readByte());
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

    public void MoveCharacter(uint characterId, int x, int y)
    {
        outgoingData.writeByte((byte)Messages.op_move_character);
        outgoingData.writeUnsignedInt(characterId);
        outgoingData.writeByte((byte)x);
        outgoingData.writeByte((byte)y);
        Send("move_character");
    }

    public void HurtTile(uint characterId, int x, int y)
    {
        outgoingData.writeByte((byte)Messages.op_hurt_tile);
        outgoingData.writeUnsignedInt(characterId);
        outgoingData.writeByte((byte)x);
        outgoingData.writeByte((byte)y);
        Send("hurt_tile");
    }

    public void EndMove()
    {
        outgoingData.writeByte((byte)Messages.op_end_move);
        Send("end_move");
    }

    public void BuyBuilding()
    {
        outgoingData.writeByte((byte)Messages.op_buy_building_card);
        Send("buy_building");
    }
    public void BuyCharacter()
    {
        outgoingData.writeByte((byte)Messages.op_buy_character_card);
        Send("buy_character");
    }
    public void PlaceCharacter(uint cid, int x, int y )
    {
        outgoingData.writeByte((byte)Messages.op_place_character);
        outgoingData.writeUnsignedInt(cid);
        outgoingData.writeByte((byte)x);
        outgoingData.writeByte((byte)y);
        Send("place_character");
    }
    public void PlaceBuilding(uint cid, int x, int y)
    {
        outgoingData.writeByte((byte)Messages.op_place_building);
        outgoingData.writeUnsignedInt(cid);
        outgoingData.writeByte((byte)x);
        outgoingData.writeByte((byte)y);
        Send("place_building");
    }
}
