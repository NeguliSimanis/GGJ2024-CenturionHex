using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Alteruna
{
	public class TextChatSynchronizable : Synchronizable
	{
		[Tooltip("Amount of chat messages to keep in buffer")] [SerializeField]
		private int chatBuffer = 10;

		public UnityEvent<string> TextChatUpdate;

		[Space] public bool UseRichText = true;

		// if alpha is zero, it will be set to a unique color for each user
		[HideInInspector] public Color NameColor = new Color(0, 0, 0, 0);
		[HideInInspector] public bool BoldNames = true;

		private ChatEvent[] _chat = Array.Empty<ChatEvent>();
		private readonly List<ChatEvent> _outgoingMessages = new List<ChatEvent>();

		/// <summary>
		/// Change max number of buffered chat lines
		/// </summary>
		public int ChatBuffer
		{
			get => chatBuffer;
			set => UpdateBufferSize(chatBuffer = value);
		}

		private void Start()
		{
			_chat = new ChatEvent[chatBuffer];
			Multiplayer.OnRoomJoined.AddListener(OnJoinedRoom);
			if (Multiplayer.InRoom && NameColor.a == 0)
			{
				NameColor = UniqueAvatarColor.HueToId(Color.red, Multiplayer.Me);
			}
		}

		private void OnJoinedRoom(Multiplayer arg0, Room arg1, User arg2)
		{
			if (NameColor.a == 0)
			{
				NameColor = UniqueAvatarColor.HueToId(Color.red, arg0.Me);
			}
		}

		private void UpdateBufferSize(int size)
		{
			var temp = _chat;
			_chat = new ChatEvent[size];
			Array.Copy(temp, _chat, Math.Min(temp.Length, _chat.Length));
		}

		private void AddChatEventToBuffer(ChatEvent chatEvent)
		{
			//shift array
			for (int i = chatBuffer - 2; i >= 0; i--)
			{
				_chat[i + 1] = _chat[i];
			}

			_chat[0] = chatEvent;

			//sort chat by time stamp
			for (int i = 1; i < chatBuffer && _chat[i].TimeStamp > _chat[i - 1].TimeStamp; i++)
			{
				chatEvent = _chat[i];
				_chat[i] = _chat[i - 1];
				_chat[i - 1] = chatEvent;
			}

			Debug.Log(chatEvent.Msg);

			TextChatUpdate.Invoke(ToString());
		}

		public void SendChatMessage(string msg) => SendChatMessageUnformulated(msg);

		public void SendChatMessageUnformulated(string msg)
		{
			if (!Multiplayer.InRoom || msg.Length == 0)
			{
				return;
			}

			// Add sender name to message
			{
				string newMsg = Multiplayer.Me.Name;
				if (UseRichText)
				{
					newMsg = "<color=#" + ColorUtility.ToHtmlStringRGB(NameColor) + ">" + newMsg + "</color>";
					if (BoldNames)
					{
						newMsg = "<b>" + newMsg + "</b>";
					}
				}

				msg = newMsg + ' ' + msg;
			}

			_outgoingMessages.Add(new ChatEvent(msg));
			Multiplayer.Sync(this);
		}

		public override void AssembleData(Writer writer, byte LOD = 100)
		{
			int messageCount = _outgoingMessages.Count;
			writer.Write((byte)messageCount);
			for (int i = 0; i < messageCount; i++)
			{
				AddChatEventToBuffer(_outgoingMessages[i]);
				_outgoingMessages[i].Write(writer);
			}

			_outgoingMessages.Clear();
		}

		public override void DisassembleData(Reader reader, byte LOD = 100)
		{
			byte messageCount = reader.ReadByte();
			for (byte i = 0; i < messageCount; i++)
			{
				AddChatEventToBuffer(new ChatEvent(reader));
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			for (int i = chatBuffer - 1; i >= 0; i--)
			{
				sb.AppendLine(_chat[i].Msg);
			}

			return sb.ToString();
		}

		private readonly struct ChatEvent
		{
			public readonly int TimeStamp;
			public readonly string Msg;

			public ChatEvent(string s = "", int time = -1)
			{
				Msg = s;
				if (time < 0)
				{
					var localNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
					var timeOffset = new DateTimeOffset(localNow, TimeZoneInfo.Utc.GetUtcOffset(localNow));
					TimeStamp = timeOffset.Millisecond + timeOffset.Second * 1000 + timeOffset.Minute * 60000 + timeOffset.Hour * 3600000;
				}
				else
				{
					TimeStamp = time;
				}
			}

			public ChatEvent(Reader reader)
			{
				TimeStamp = reader.ReadInt();
				Msg = reader.ReadString();
			}

			public void Write(Writer writer)
			{
				writer.Write(TimeStamp);
				writer.Write(Msg);
			}
		}
	}
#if UNITY_EDITOR
	[CustomEditor(typeof(TextChatSynchronizable))]
	public class TextChatSynchronizableEditor : UniqueIDEditor
	{
		public override void OnInspectorGUI()
		{
			DrawUID();

			var textChat = (TextChatSynchronizable)target;

			DrawDefaultInspector();

			// ReSharper disable once AssignmentInConditionalExpression
			if (textChat.UseRichText)
			{
				textChat.NameColor = EditorGUILayout.ColorField("Name Color", textChat.NameColor);
				textChat.BoldNames = EditorGUILayout.Toggle("Bold Names", textChat.BoldNames);
			}
		}
	}
#endif
}