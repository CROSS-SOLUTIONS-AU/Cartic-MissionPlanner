using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using MissionPlanner;
using MissionPlanner.Mavlink;
using MissionPlanner.Utilities;
using static MAVLink;
using MissionPlanner.Controls;
using MissionPlanner.GCSViews;
//loadassembly: MissionPlanner.WebAPIs

namespace cartic
{
    public class BoatControlsPlugin : MissionPlanner.Plugin.Plugin
    {
        private bool _recording = false;
        public bool Recording
        {
            get { return _recording; }
            private set
            {
                Console.WriteLine($"Recording set to {value}. Called from {new StackTrace().GetFrame(1).GetMethod().Name}");
                _recording = value;
            }
        }
        private Control recButton = new MissionPlanner.Controls.MyButton();
        private Control depthLabel = new MissionPlanner.Controls.MyLabel();
        public override string Name
        {
            get { return "Cartic Boat Controls"; }
        }

        public override string Version
        {
            get { return "0.10"; }
        }

        public override string Author
        {
            get { return "J Piper-Green"; }
        }

        public override bool Init()
        {
            MainV2.comPort.OnPacketReceived += ComPort_RecieveState;
            Host.FDMenuHud.BeginInvokeIfRequired(() =>
            {
                depthLabel.Text = "0.0";
                depthLabel.Padding = new Padding(10);
                depthLabel.Top = 5;
                depthLabel.Left = 10;

                recButton.Text = "Start Recording Soundings";
                recButton.Padding = new Padding (10);
                recButton.Left = 10;
                recButton.Top = 20;
                recButton.Size = new System.Drawing.Size(100, 50);
                recButton.Click += RecordingButtonClick;
                FlightData.instance.tabActionsSimple.Controls.Add(recButton);

            });
            return true;
        }

        public override bool Loaded()
        {
            loopratehz = 1.0f;
            

            return true;
        }


        public override bool Loop()
        {
            return true;
        }

        public override bool Exit()
        {
            return true;
        }

        private void RecordingButtonClick(Object sender, EventArgs e)
        {
            Console.WriteLine($"RecordingButtonClick fired. Sender: {sender.GetType().Name}, EventArgs: {e.GetType().Name}");

            if (e is MouseEventArgs mouseEvent)
            {
                Console.WriteLine($"Mouse button clicked: {mouseEvent.Button}");
            }

            Recording = !_recording; // Toggle the recording state

            if (Recording)
            {
                Parallel.ForEach(MainV2.Comports, mav =>
                {
                    mav.send_text((byte)MAV_SEVERITY.INFO,"RcrdDepth:1");
                });
                recButton.Text = "Trying to Start Recording...";
            }
            else
            {
                Parallel.ForEach(MainV2.Comports, mav =>
                {
                    mav.send_text((byte)MAV_SEVERITY.INFO, "RcrdDepth:0");
                });
                recButton.Text = "Start Recording Soundings";
            }
        }

        public void SendMessage(string message)
        {
            if (MainV2.comPort.BaseStream.IsOpen)
            {
                // Get current MAV state
                MAVState mavState = MainV2.comPort.MAV;
                int hostsysId = Host.comPort.sysidcurrent;
                int hostcompId = Host.comPort.compidcurrent;
                byte[] encodedText = Encoding.UTF8.GetBytes(message);

                // Create the message
                mavlink_statustext_t statusText = new mavlink_statustext_t
                { 
                    severity = (byte)MAV_SEVERITY.INFO,
                    text = encodedText
                };

                // Send the message
                //MainV2.comPort.sendPacket(statusText, hostsysId, hostcompId);
                MainV2.comPort.send_text((byte)MAV_SEVERITY.INFO, message);
            }
        }

        public void ComPort_RecieveState(object sender, MAVLink.MAVLinkMessage e)
        {
            if(e.msgid == (uint)MAVLink.MAVLINK_MSG_ID.RANGEFINDER)
            {
                Console.WriteLine("Rangefinder Recieved Compid " + e.compid);
                if (e.compid == 2)
                {
                    var depth = (MAVLink.mavlink_rangefinder_t)e.data;
                    Console.WriteLine("Depth: " + depth.ToString());
                    depthLabel.Text = depth.distance.ToString();
                    recButton.Text = (depth.voltage == 1) ? "End Recording" : "Start Recording Soundings";
                    Recording = (depth.voltage == 1) ? true : false;
                }
            }
        }
    }
}