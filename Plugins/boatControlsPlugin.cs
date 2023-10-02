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
        private bool recording = false;
        private Control recButton = new MissionPlanner.Controls.MyButton();
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
                recButton.Text = "Start Recording Soundings";
                recButton.Padding = new Padding (10);
                recButton.Left = 10;
                recButton.Top = 20;
                recButton.Size = new System.Drawing.Size(100, 50);
                recButton.MouseClick += RecordingButtonClick;
                FlightData.instance.tabActionsSimple.Controls.Add(recButton);

            });
            return true;
        }

        public override bool Loaded()
        {
            loopratehz = 1;
            
            return true;
        }


        public override bool Loop()
        {
            string recordingState = recording ? "RcrdDpth:1" : "RcrdDpth:0";
            SendMessage(recordingState);
            return true;
        }

        public override bool Exit()
        {
            return true;
        }

        private void RecordingButtonClick(Object sender, EventArgs e)
        {
            recording = !recording;
            if(recording) {
                recButton.Text = "End Recording";
            } else
            {
                recButton.Text = "Start Recording Soundings";
            }
        }

        public void SendMessage(string message)
        {
            if (MainV2.comPort.BaseStream.IsOpen)
            {
                // Get current MAV state
                MAVState mavState = MainV2.comPort.MAV;

                byte[] encodedText = Encoding.ASCII.GetBytes(message);

                // Create the message
                mavlink_statustext_t statusText = new mavlink_statustext_t
                {
                    severity = (byte)MAV_SEVERITY.MAV_SEVERITY_INFO,
                    text = encodedText
                };

                // Send the message
                MainV2.comPort.sendPacket(statusText, mavState.sysid, mavState.compid);
            }
        }

        public void ComPort_RecieveState(object sender, MAVLink.MAVLinkMessage e)
        {
            if(e.msgid == (uint)MAVLink.MAVLINK_MSG_ID.RANGEFINDER)
            {
                var depth = (MAVLink.mavlink_rangefinder_t)e.data;
                recording = true;
                Console.WriteLine("Depth: " + depth.ToString());
            }
        }
    }
}