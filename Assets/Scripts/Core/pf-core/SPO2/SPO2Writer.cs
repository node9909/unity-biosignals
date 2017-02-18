﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace pfcore {
    class SPO2Writer {
        private string filenameBase;
        private bool done = false;
        private readonly string port;

        struct BPMEvent {
            public int bpm;
            public long timestamp;
        }

        public SPO2Writer(string port) {
            filenameBase = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            this.port = port;
        }

        public void Start() {
            Console.WriteLine("Writing SpO2 session to files.");

            StringBuilder peaksString = new StringBuilder();
            StringBuilder hrString = new StringBuilder();

            Console.WriteLine("Press ENTER to end session.");

            Thread writeThread = new Thread(new ThreadStart(WriteLoop));
            writeThread.Start();

            Console.Read();

            done = true;
            writeThread.Join();

            Console.WriteLine("All done.");
            Console.ReadKey();
        }

        private void WriteLoop() {
            SPO2Reader reader = new SPO2Reader(port, 100000);
            SPO2Processor processor = new SPO2Processor(reader);
            processor.Start();

            List<SPO2Packet> allPackets = new List<SPO2Packet>();
            List<BPMEvent> bpmEvents = new List<BPMEvent>();

            while (!done) {
                processor.Update();

                List<SPO2Packet> packets = processor.ProcessedPackets;
                allPackets.AddRange(packets);

                BPMEvent bpmEvent;
                bpmEvent.bpm = processor.GetBPM();
                bpmEvent.timestamp = DateTime.Now.Ticks;
                bpmEvents.Add(bpmEvent);
            }

            processor.StopAndJoin();

            Console.WriteLine("Done. Packets read: " + allPackets.Count);
            Console.WriteLine("Writing files...");

            StringBuilder rawBpms = new StringBuilder();
            StringBuilder procBpms = new StringBuilder();
            StringBuilder peaks = new StringBuilder();

            foreach (SPO2Packet packet in allPackets) {
                int rawHr = packet.HeartRate;
                string line = rawHr.ToString() + "," + packet.timeStamp;
                rawBpms.AppendLine(line);

                line = (packet.Peak ? "1" : "0") + "," + packet.timeStamp;
                peaks.AppendLine(line); 
            }

            foreach (BPMEvent ev in bpmEvents) {
                string line = ev.bpm.ToString() + "," + ev.timestamp;
                procBpms.AppendLine(line);
            }
        }
    }
}
