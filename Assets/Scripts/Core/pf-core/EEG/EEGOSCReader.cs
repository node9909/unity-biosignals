﻿using System;
using OSC;

namespace pfcore
{
	public class EEGOSCReader : EEGReader
	{
		private OSCServer server;
		private int port;

		public EEGOSCReader(int port)
		{
			PacketQueue = new ConcurrentQueue<OSCPacket>();
			this.port = port;
		}

		public override void Start()
		{
			server = new OSCServer(port);
			server.PacketReceivedEvent += OnPacketReceived;
		}

		private void OnPacketReceived(OSCServer s, OSCPacket packet)
		{
			((OSCMessage)packet.Data[0]).Extra = (byte)EyesStatus.NONE;
			PacketQueue.Enqueue(packet);
		}

		public override void Stop()
		{
			if (server != null) {
				server.Close();
			}
		}
	}
}
