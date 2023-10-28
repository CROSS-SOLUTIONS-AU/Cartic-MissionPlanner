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
using static MAVLink;
using MissionPlanner.Controls;
using MissionPlanner.GCSViews;
using System.Speech.Synthesis;
using System.Security.Cryptography.X509Certificates;
using Accord.Math;
//loadassembly: MissionPlanner.WebAPIs

namespace cartic
{
    public class BathyLogger : IDisposable
    {
        private readonly string _filePath;
        private StreamWriter _streamWriter;  // Class-level StreamWriter



        public BathyLogger()
        {
            // Determine the file name based on the current date and time
            string fileName = $"bathylog-{DateTime.Now:yyyyMMddHHmmss}.csv";
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Bathy_Logs", fileName);

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath));

            _streamWriter = new StreamWriter(_filePath, true, Encoding.UTF8);
            _streamWriter.WriteLine("epoch,lat,long,depth,fixState,altitude");
        }

        public void WriteLine(double epoch, double lat, double lon, double depth, float fixState, double altitude)
        {
            var line = $"{epoch},{lat},{lon},{depth},{fixState},{altitude}";
            _streamWriter.WriteLine(line);
        }

        public void CloseAndFlush()
        {
            if (_streamWriter != null)
            {
                _streamWriter.Flush();
                _streamWriter.Close();
                _streamWriter = null;
            }
        }

        // Implementing IDisposable
        public void Dispose()
        {
            CloseAndFlush();
        }
    }

    public class BoatControlsPlugin : MissionPlanner.Plugin.Plugin
    {
        private int _depthRx = 0;
        private bool _recording = false;
        private string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bathy_Logs");
        private BathyLogger fileLogger = null;

        private int? depthOffset;  // DOFFSET
        private string range;        // RANGE
        private string ping;         // PING
        private List<int> pingsPerSecond; // PINGSPS
        private List<int> pulsesPerSecond; // PULSESPP
        private double? depthFilter;  // DFILTER
        private double? sampleFilter; // SFILTER
        private double? depthBlank;   // DBLANK

        private Control recButton = new MissionPlanner.Controls.MyButton();
        private Control showLogsButton = new MissionPlanner.Controls.MyButton();
        private Control depthLabel = new MissionPlanner.Controls.MyLabel();
        private Control recStateLabel = new MissionPlanner.Controls.MyLabel();
        private Control depthRxLabel = new MissionPlanner.Controls.MyLabel();

        private Control depthOffsetLabel = new MissionPlanner.Controls.MyLabel();
        private Control rangeLabel = new MissionPlanner.Controls.MyLabel();
        private Control pingLabel = new MissionPlanner.Controls.MyLabel();
        private Control pingsPerSecondLabel = new MissionPlanner.Controls.MyLabel();
        private Control pulsesPerSecondLabel = new MissionPlanner.Controls.MyLabel();
        private Control depthFilterLabel = new MissionPlanner.Controls.MyLabel();
        private Control sampleFilterLabel = new MissionPlanner.Controls.MyLabel();
        private Control depthBlankLabel = new MissionPlanner.Controls.MyLabel();

        public event EventHandler DepthRecieved;
        public event EventHandler RecordingChanged;

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

        protected virtual void OnDepthRecieved(EventArgs e)
        {
            DepthRecieved?.Invoke(this, e);
        }
        protected virtual void OnRecordingChanged(EventArgs e, bool newValue)
        {
            RecordingChanged?.Invoke(this, e);
        }

        public int DepthRx
        {
            get { return _depthRx;  }
            private set 
            { 
                _depthRx = value;
            }
        }

        public bool Recording
        {
            get { return _recording; }
            private set
            {
                if (value != _recording)
                {
                    _recording = value;
                    OnRecordingChanged(new EventArgs(), value);
                }

            }
        }

        public double? DepthOffset
        {
            get { return depthOffset; }
            set { depthOffset = value; }
        }

        public string Range
        {
            get { return range; }
            set { range = value; }
        }

        public string Ping
        {
            get { return ping; }
            set { ping = value; }
        }

        public List<int> PingsPerSecond
        {
            get { return pingsPerSecond; }
            set { pingsPerSecond = value; }
        }

        public List<int> PulsesPerSecond
        {
            get { return pulsesPerSecond; }
            set { pulsesPerSecond = value; }
        }

        public double? DepthFilter
        {
            get { return depthFilter; }
            set { depthFilter = value; }
        }
        public double? SampleFilter
        {
            get { return sampleFilter; }
            set { sampleFilter = value; }
        }

        public double? DepthBlank
        {
            get { return depthBlank; }
            set { depthBlank = value; }
        }

        public override bool Init()
        {
            Host.FDMenuHud.BeginInvokeIfRequired(() =>
            {
                depthLabel.Text = "--- m";
                depthLabel.Padding = new Padding(10);
                depthLabel.Top = 5;
                depthLabel.Left = 10;
                depthLabel.Size = new System.Drawing.Size(80, 30);
                depthLabel.Font = new System.Drawing.Font("Arial", 16);

                depthRxLabel.Text = "Rx: ";
                depthRxLabel.Padding = new Padding(10);
                depthRxLabel.Top = 30;
                depthRxLabel.Left = 10;
                depthRxLabel.Size = new System.Drawing.Size(90, 30);
                depthRxLabel.Font = new System.Drawing.Font("Arial", 12);

                recStateLabel.Text = "●";
                recStateLabel.Padding = new Padding(10);
                recStateLabel.Top = 130;
                recStateLabel.Left = 10;
                recStateLabel.Size = new System.Drawing.Size(32, 32);
                recStateLabel.ForeColor = System.Drawing.Color.Black;
                recStateLabel.Font = new System.Drawing.Font("Arial", 36);

                recButton.Text = "Start Recording Soundings";
                recButton.Padding = new Padding (10);
                recButton.Left = 10;
                recButton.Top = 70;
                recButton.Size = new System.Drawing.Size(110, 60);
                recButton.Click += RecordingButtonClick;

                showLogsButton.Text = "Show Logs";
                showLogsButton.Padding = new Padding(10);
                showLogsButton.Left = 50;
                showLogsButton.Top = 140;
                showLogsButton.Size = new System.Drawing.Size(70, 25);
                showLogsButton.Click += ShowLogsClick;

                recStateLabel.Text = "●";
                recStateLabel.Padding = new Padding(10);
                recStateLabel.Top = 130;
                recStateLabel.Left = 10;
                recStateLabel.Size = new System.Drawing.Size(32, 32);
                recStateLabel.ForeColor = System.Drawing.Color.Black;
                recStateLabel.Font = new System.Drawing.Font("Arial", 36);

                Control[] btn1 = FlightData.instance.tabActionsSimple.Controls.Find("myButton1", false);
                if (btn1 != null)
                {
                    btn1[0].Left = 310;
                }

                Control[] btn2 = FlightData.instance.tabActionsSimple.Controls.Find("myButton2", false);
                if (btn2 != null)
                {
                    btn2[0].Left = 310;
                }


                Control[] btn3 = FlightData.instance.tabActionsSimple.Controls.Find("myButton3", false);
                if(btn3 != null)
                {
                    btn3[0].Left = 310;
                }



                // Create and configure the container panel
                Panel borderContainer = new Panel();
                borderContainer.BorderStyle = BorderStyle.FixedSingle;
                borderContainer.Top = 5;
                borderContainer.Left = 140;
                borderContainer.Size = new System.Drawing.Size(140, 200); // Change size as required
                FlightData.instance.tabActionsSimple.Controls.Add(borderContainer);

                // Add labels to the container and format them
                int topPosition = 5;
                int increment = 20; // Adjust as needed to space the labels correctly

                // Depth Offset Label
                depthOffsetLabel.Text = "Depth Offset: --";
                depthOffsetLabel.Padding = new Padding(10);
                depthOffsetLabel.Top = topPosition;
                depthOffsetLabel.Left = 5;
                depthOffsetLabel.Anchor = AnchorStyles.Left;
                depthOffsetLabel.Size = new System.Drawing.Size(90, 30);
                depthOffsetLabel.Font = new System.Drawing.Font("Arial", 10);
                borderContainer.Controls.Add(depthOffsetLabel);
                topPosition += increment;

                // Range Label
                rangeLabel.Text = "Range: --";
                rangeLabel.Padding = new Padding(10);
                rangeLabel.Top = topPosition;
                rangeLabel.Left = 5;
                rangeLabel.Anchor = AnchorStyles.Left;
                rangeLabel.Size = new System.Drawing.Size(90, 30);
                rangeLabel.Font = new System.Drawing.Font("Arial", 10);
                borderContainer.Controls.Add(rangeLabel);
                topPosition += increment;

                // Ping Label
                pingLabel.Text = "Ping: --";
                pingLabel.Padding = new Padding(10);
                pingLabel.Top = topPosition;
                pingLabel.Left = 5;
                pingLabel.Anchor = AnchorStyles.Left;
                pingLabel.Size = new System.Drawing.Size(90, 30);
                pingLabel.Font = new System.Drawing.Font("Arial", 10);
                borderContainer.Controls.Add(pingLabel);
                topPosition += increment;

                // Pings Per Second Label
                pingsPerSecondLabel.Text = "Pings/Sec: --";
                pingsPerSecondLabel.Padding = new Padding(10);
                pingsPerSecondLabel.Top = topPosition;
                pingsPerSecondLabel.Left = 5;
                pingsPerSecondLabel.Anchor = AnchorStyles.Left;
                pingsPerSecondLabel.Size = new System.Drawing.Size(90, 30);
                pingsPerSecondLabel.Font = new System.Drawing.Font("Arial", 10);
                borderContainer.Controls.Add(pingsPerSecondLabel);
                topPosition += increment;

                // Pulses Per Second Label
                pulsesPerSecondLabel.Text = "Pulses/Sec: --";
                pulsesPerSecondLabel.Padding = new Padding(10);
                pulsesPerSecondLabel.Top = topPosition;
                pulsesPerSecondLabel.Left = 5;
                pulsesPerSecondLabel.Anchor = AnchorStyles.Left;
                pulsesPerSecondLabel.Size = new System.Drawing.Size(90, 30);
                pulsesPerSecondLabel.Font = new System.Drawing.Font("Arial", 10);
                borderContainer.Controls.Add(pulsesPerSecondLabel);
                topPosition += increment;

                // Depth Filter Label
                depthFilterLabel.Text = "Depth Filter: --";
                depthFilterLabel.Padding = new Padding(10);
                depthFilterLabel.Top = topPosition;
                depthFilterLabel.Left = 5;
                depthFilterLabel.Anchor = AnchorStyles.Left;
                depthFilterLabel.Size = new System.Drawing.Size(90, 30);
                depthFilterLabel.Font = new System.Drawing.Font("Arial", 10);
                borderContainer.Controls.Add(depthFilterLabel);
                topPosition += increment;

                // Sample Filter Label
                sampleFilterLabel.Text = "Sample Filter: --";
                sampleFilterLabel.Padding = new Padding(10);
                sampleFilterLabel.Top = topPosition;
                sampleFilterLabel.Left = 5;
                sampleFilterLabel.Anchor = AnchorStyles.Left;
                sampleFilterLabel.Size = new System.Drawing.Size(90, 30);
                sampleFilterLabel.Font = new System.Drawing.Font("Arial", 10);
                borderContainer.Controls.Add(sampleFilterLabel);
                topPosition += increment;

                // Depth Blank Label
                depthBlankLabel.Text = "Depth Blank: --";
                depthBlankLabel.Padding = new Padding(10);
                depthBlankLabel.Top = topPosition;
                depthBlankLabel.Left = 5;
                depthBlankLabel.Anchor = AnchorStyles.Left;
                depthBlankLabel.Size = new System.Drawing.Size(90, 30);
                depthBlankLabel.Font = new System.Drawing.Font("Arial", 10);
                borderContainer.Controls.Add(depthBlankLabel);
                topPosition += increment;

                FlightData.instance.tabActionsSimple.Controls.Add(depthLabel);
                FlightData.instance.tabActionsSimple.Controls.Add(depthRxLabel);
                FlightData.instance.tabActionsSimple.Controls.Add(recStateLabel);
                FlightData.instance.tabActionsSimple.Controls.Add(recButton);
                FlightData.instance.tabActionsSimple.Controls.Add(showLogsButton);

                Application.DoEvents();

            });
            MainV2.comPort.OnPacketReceived += ComPort_RecieveState;
            return true;
        }

        public override bool Loaded()
        {
            loopratehz = 1.0f;
            recStateLabel.ForeColor = System.Drawing.Color.Black;

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
                fileLogger = new BathyLogger();
                Parallel.ForEach(MainV2.Comports, mav =>
                {
                    mav.send_text((byte)MAV_SEVERITY.INFO,"recordDepth:1");
                });
                recButton.Invoke(new Action(() => { recButton.Text = "Starting... Recording"; }));

            }
            else
            {
                Parallel.ForEach(MainV2.Comports, mav =>
                {
                    mav.send_text((byte)MAV_SEVERITY.INFO, "recordDepth:0");
                });
                recButton.Invoke(new Action(() => { recButton.Text = "Stopping... Recording"; }));
                if (fileLogger != null)
                {
                    fileLogger.CloseAndFlush();
                    fileLogger.Dispose();
                    fileLogger = null;
                }
            }
        }

        private void ShowLogsClick(Object sender, EventArgs e)
        {
            // Ensure the directory exists before trying to open it.
            if (Directory.Exists(_logDirectory))
            {
                // Use Process.Start to open the directory in Windows Explorer.
                Process.Start("explorer.exe", _logDirectory);
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
                if (e.compid == 2)
                {
                    DepthRx++;
                    var depth = (MAVLink.mavlink_rangefinder_t)e.data;
                    Host.cs.sonarrange = -depth.distance / 100;
                    Recording = (depth.voltage == 1) ? true : false;

                    //Move all this to fire when private variables update
                    depthRxLabel.Invoke(new Action(() => { depthRxLabel.Text = $"Rx: {(DepthRx).ToString()}"; }));
                    depthLabel.Invoke(new Action(() => { depthLabel.Text = $"{(depth.distance / 100).ToString()} m"; }));
                    recButton.Invoke(new Action(() => { recButton.Text = Recording ? "End Recording" : "Start Recording Soundings"; }));
                    recStateLabel.Invoke(new Action(() => { recStateLabel.ForeColor = Recording ? System.Drawing.Color.Red : System.Drawing.Color.Black; }));;
                    if (fileLogger != null)
                    {
                        fileLogger.WriteLine(Host.cs.datetime.toUnixTimeDouble(), Host.cs.lat, Host.cs.lng, depth.distance / 100, Host.cs.gpsstatus, Host.cs.altasl);
                    }
                }
            }
            else if (e.msgid == (uint)MAVLink.MAVLINK_MSG_ID.STATUSTEXT)
            {
                {
                    if (e.compid == 2)
                    {
                        var status = (MAVLink.mavlink_statustext_t)e.data;
                        string statusText = Encoding.UTF8.GetString(status.text);
                        string[] parts = statusText.Split(':');
                        string key = parts[0];
                        if (parts.Length > 1) { 
                            string value = parts[1];
                            if (!string.IsNullOrEmpty(value))
                            {
                                switch (key)
                                {
                                    case "db":
                                        DepthBlank = Convert.ToDouble(value);
                                        break;
                                    case "df":
                                        DepthFilter = Convert.ToDouble(value);
                                        break;
                                    case "do":
                                        DepthOffset = Convert.ToDouble(value);
                                        break;
                                    case "pm":
                                        Ping = value;
                                        break;
                                    case "pp":
                                        PingsPerSecond = new List<int> { Convert.ToInt32(value) }; // Adjust this if multiple values are expected
                                        break;
                                    case "pu":
                                        PulsesPerSecond = new List<int> { Convert.ToInt32(value) }; // Adjust this if multiple values are expected
                                        break;
                                    case "rg":
                                        Range = value;
                                        break;
                                    case "sf":
                                        SampleFilter = Convert.ToDouble(value);
                                        break;
                                        // Add more cases as needed
                                }
                            }
                        }
                    }
            }
            }
        }
    }
}