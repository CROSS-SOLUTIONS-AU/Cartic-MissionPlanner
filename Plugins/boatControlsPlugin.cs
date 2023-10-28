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
using System.ComponentModel;
using Org.BouncyCastle.Asn1.Crmf;
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

    public class SonarSettings : INotifyPropertyChanged
    {
        private int? depthOffset;  // DOFFSET
        private string range;        // RANGE
        private string ping;         // PING
        private List<int> pingsPerSecond; // PINGSPS
        private List<int> pulsesPerSecond; // PULSESPP
        private int? depthFilter;  // DFILTER
        private int? sampleFilter; // SFILTER
        private int? depthBlank;   // DBLANK

        public int? DepthOffset
        {
            get { return depthOffset; }
            set
            {
                depthOffset = value;
                OnPropertyChanged("DepthOffset");
            }
        }

        public string Range
        {
            get { return range; }
            set
            {
                if (range != value)
                {
                    range = value;
                    OnPropertyChanged("Range");
                }
            }
        }

        public string Ping
        {
            get { return ping; }
            set
            {
                if (ping != value)
                {
                    ping = value;
                    OnPropertyChanged("Ping");
                }
            }
        }

        public List<int> PingsPerSecond
        {
            get { return pingsPerSecond; }
            set
            {
                if (pingsPerSecond != value)
                {
                    pingsPerSecond = value;
                    OnPropertyChanged("PingsPerSecond");
                }
            }
        }
        public string PingsPerSecondString
        {
            get
            {
                if(PingsPerSecond == null) {
                    return "";
                }
                return string.Join(", ", PingsPerSecond);
            }
        }

        public List<int> PulsesPerSecond
        {
            get { return pulsesPerSecond; }
            set
            {
                if (pingsPerSecond != value)
                {
                    pingsPerSecond = value;
                    OnPropertyChanged("PulsesPerSecond");
                }
            }
        }

        // This is the new property that returns the list as a formatted string
        public string PulsesPerSecondString
        {
            get
            {
                if (PulsesPerSecond == null)
                {
                    return "";
                }
                return string.Join(", ", PulsesPerSecond);
            }
        }

        public int? DepthFilter
        {
            get { return depthFilter; }
            set
            {
                if (depthFilter != value)
                {
                    depthFilter = value;
                    OnPropertyChanged("DepthFilter");
                }
            }
        }
        public int? SampleFilter
        {
            get { return sampleFilter; }
            set
            {
                if (sampleFilter != value)
                {
                    sampleFilter = value;
                    OnPropertyChanged("SampleFilter");
                }
            }
        }

        public int? DepthBlank
        {
            get { return depthBlank; }
            set
            {
                if (depthBlank != value)
                {
                    depthBlank = value;
                    OnPropertyChanged("DepthBlank");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BoatControlsPlugin : MissionPlanner.Plugin.Plugin
    {
        private int _depthRx = 0;
        private bool _recording = false;
        private string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bathy_Logs");
        private BathyLogger fileLogger = null;
        private SonarSettings sonarSettings = new SonarSettings();

        private TableLayoutPanel sonarParamLayout = new System.Windows.Forms.TableLayoutPanel();

        private Control recButton = new MissionPlanner.Controls.MyButton();
        private Control showLogsButton = new MissionPlanner.Controls.MyButton();
        private Control depthLabel = new MissionPlanner.Controls.MyLabel();
        private Control recStateLabel = new MissionPlanner.Controls.MyLabel();
        private Control depthRxLabel = new MissionPlanner.Controls.MyLabel();

        //Value Labels
        private Control depthOffsetVLabel = new MissionPlanner.Controls.MyLabel();
        private Control rangeVLabel = new MissionPlanner.Controls.MyLabel();
        private Control pingVLabel = new MissionPlanner.Controls.MyLabel();
        private Control pingsPerSecondVLabel = new MissionPlanner.Controls.MyLabel();
        private Control pulsesPerSecondVLabel = new MissionPlanner.Controls.MyLabel();
        private Control depthFilterVLabel = new MissionPlanner.Controls.MyLabel();
        private Control sampleFilterVLabel = new MissionPlanner.Controls.MyLabel();
        private Control depthBlankVLabel = new MissionPlanner.Controls.MyLabel();

        //Labels
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
                recButton.Padding = new Padding(10);
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
                    FlightData.instance.tabActionsSimple.Controls.Remove(btn1[0]);
                }

                Control[] btn2 = FlightData.instance.tabActionsSimple.Controls.Find("myButton2", false);
                if (btn2 != null)
                {
                    FlightData.instance.tabActionsSimple.Controls.Remove(btn2[0]);
                }


                Control[] btn3 = FlightData.instance.tabActionsSimple.Controls.Find("myButton3", false);
                if(btn3 != null)
                {
                    FlightData.instance.tabActionsSimple.Controls.Remove(btn3[0]);
                }

                sonarParamLayout.Name = "sonarParamLayout";
                sonarParamLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                sonarParamLayout.Size = new System.Drawing.Size (220, 280);
                sonarParamLayout.SuspendLayout();

                int numberOfColumns = 3; // For example, set it to 4 columns. Adjust as needed.

                // Set the number of columns for your TableLayoutPanel
                sonarParamLayout.ColumnCount = numberOfColumns;

                for (int i = 0; i < numberOfColumns; i++)
                {
                    if (i == 0) { sonarParamLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F)); }
                    if (i == 1) { sonarParamLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F)); }
                    if (i == 2) { sonarParamLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F)); }
                }

                int numberOfRows = 8; // For example, set it to 4 columns. Adjust as needed.
                // Set the number of columns for your TableLayoutPanel
                sonarParamLayout.RowCount = numberOfRows;

                for (int i = 0; i < numberOfRows; i++)
                {
                    sonarParamLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 12.5F));
                }

                // Create and configure the container panel
                Panel borderContainer = new Panel();
                borderContainer.BorderStyle = BorderStyle.FixedSingle;
                borderContainer.Top = 5;
                borderContainer.Left = 140;
                borderContainer.Size = new System.Drawing.Size(220, 280); // Change size as required
                FlightData.instance.tabActionsSimple.Controls.Add(borderContainer);
                borderContainer.Controls.Add(sonarParamLayout);
                
                // Add labels to the container and format them

                // Depth Offset Label
                depthOffsetLabel.Text = "Depth Offset: ";
                depthOffsetLabel.Size = new System.Drawing.Size(90, 16);
                depthOffsetLabel.Font = new System.Drawing.Font("Arial", 10);
                sonarParamLayout.Controls.Add(depthOffsetLabel, 0, 0);

                // Range Label
                rangeLabel.Text = "Range: ";
                rangeLabel.Size = new System.Drawing.Size(90, 16);
                rangeLabel.Font = new System.Drawing.Font("Arial", 10);
                sonarParamLayout.Controls.Add(rangeLabel, 0, 1);

                // Ping Label
                pingLabel.Text = "Ping: ";
                pingLabel.Size = new System.Drawing.Size(90, 16);
                pingLabel.Font = new System.Drawing.Font("Arial", 10);
                sonarParamLayout.Controls.Add(pingLabel, 0, 2);

                // Pings Per Second Label
                pingsPerSecondLabel.Text = "Pings/Sec: ";
                pingsPerSecondLabel.Size = new System.Drawing.Size(90, 16);
                pingsPerSecondLabel.Font = new System.Drawing.Font("Arial", 10);
                sonarParamLayout.Controls.Add(pingsPerSecondLabel, 0, 3);

                // Pulses Per Second Label
                pulsesPerSecondLabel.Text = "Pulses/Sec: ";
                pulsesPerSecondLabel.Size = new System.Drawing.Size(90, 16);
                pulsesPerSecondLabel.Font = new System.Drawing.Font("Arial", 10);
                sonarParamLayout.Controls.Add(pulsesPerSecondLabel, 0, 4);

                // Depth Filter Label
                depthFilterLabel.Text = "Depth Filter: ";
                depthFilterLabel.Size = new System.Drawing.Size(90, 16);
                depthFilterLabel.Font = new System.Drawing.Font("Arial", 10);
                sonarParamLayout.Controls.Add(depthFilterLabel, 0, 5);

                // Sample Filter Label
                sampleFilterLabel.Text = "Sample Filter: ";
                sampleFilterLabel.Size = new System.Drawing.Size(90, 16);
                sampleFilterLabel.Font = new System.Drawing.Font("Arial", 10);
                sonarParamLayout.Controls.Add(sampleFilterLabel, 0, 6);

                // Depth Blank Label
                depthBlankLabel.Text = "Depth Blank: ";
                depthBlankLabel.Size = new System.Drawing.Size(90, 16);
                depthBlankLabel.Font = new System.Drawing.Font("Arial", 10);
                sonarParamLayout.Controls.Add(depthBlankLabel, 0, 7);

                /////////// VALUE LABELS

                // Depth Offset Label
                depthOffsetVLabel.Text = "Depth Offset: --";
                depthOffsetLabel.Size = new System.Drawing.Size(90, 16);
                depthOffsetVLabel.Font = new System.Drawing.Font("Arial", 10);
                depthOffsetVLabel.DataBindings.Add("Text", sonarSettings, "DepthOffset");
                sonarParamLayout.Controls.Add(depthOffsetVLabel, 1, 0);

                // Range VLabel
                rangeVLabel.Text = "Range: --";
                rangeVLabel.Size = new System.Drawing.Size(90, 16);
                rangeVLabel.Font = new System.Drawing.Font("Arial", 10);
                rangeVLabel.DataBindings.Add("Text", sonarSettings, "Range");
                sonarParamLayout.Controls.Add(rangeVLabel, 1, 1);

                // Ping VLabel
                pingVLabel.Text = "Ping: --";
                pingVLabel.Size = new System.Drawing.Size(90, 16);
                pingVLabel.Font = new System.Drawing.Font("Arial", 10);
                pingVLabel.DataBindings.Add("Text", sonarSettings, "Ping");
                sonarParamLayout.Controls.Add(pingVLabel, 1, 2);

                // Pings Per Second VLabel
                pingsPerSecondVLabel.Text = "Pings/Sec: --";
                pingsPerSecondVLabel.Size = new System.Drawing.Size(90, 16);
                pingsPerSecondVLabel.Font = new System.Drawing.Font("Arial", 10);
                pingsPerSecondVLabel.DataBindings.Add("Text", sonarSettings, "PingsPerSecondString");
                sonarParamLayout.Controls.Add(pingsPerSecondVLabel, 1, 3);

                // Pulses Per Second VLabel
                pulsesPerSecondVLabel.Text = "Pulses/Sec: --";
                pulsesPerSecondVLabel.Size = new System.Drawing.Size(90, 16);
                pulsesPerSecondVLabel.Font = new System.Drawing.Font("Arial", 10);
                pulsesPerSecondVLabel.DataBindings.Add("Text", sonarSettings, "PulsesPerSecondString");
                sonarParamLayout.Controls.Add(pulsesPerSecondVLabel, 1, 4);

                // Depth Filter VLabel
                depthFilterVLabel.Text = "Depth Filter: --";
                depthFilterVLabel.Size = new System.Drawing.Size(90, 16);
                depthFilterVLabel.Font = new System.Drawing.Font("Arial", 10);
                depthFilterVLabel.DataBindings.Add("Text", sonarSettings, "DepthFilter");
                sonarParamLayout.Controls.Add(depthFilterVLabel, 1, 5);

                // Sample Filter VLabel
                sampleFilterVLabel.Text = "Sample Filter: --";
                sampleFilterVLabel.Size = new System.Drawing.Size(90, 16);
                sampleFilterVLabel.Font = new System.Drawing.Font("Arial", 10);
                sampleFilterVLabel.DataBindings.Add("Text", sonarSettings, "SampleFilter");
                sonarParamLayout.Controls.Add(sampleFilterVLabel, 1, 6);

                // Depth Blank VLabel
                depthBlankVLabel.Text = "Depth Blank: --";
                depthBlankVLabel.Size = new System.Drawing.Size(90, 16);
                depthBlankVLabel.Font = new System.Drawing.Font("Arial", 10);
                depthBlankVLabel.DataBindings.Add("Text", sonarSettings, "DepthBlank");
                sonarParamLayout.Controls.Add(depthBlankVLabel, 1, 7);

                sonarParamLayout.ResumeLayout(false);

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
                                        sonarSettings.DepthBlank = Convert.ToInt32(value.TrimEnd('\0'));
                                        break;
                                    case "df":
                                        sonarSettings.DepthFilter = Convert.ToInt32(value.TrimEnd('\0'));
                                        break;
                                    case "do":
                                        sonarSettings.DepthOffset = Convert.ToInt32(value.TrimEnd('\0'));
                                        break;
                                    case "pm":
                                        sonarSettings.Ping = value.TrimEnd('\0');
                                        break;
                                    case "pp":
                                        Console.WriteLine(value);
                                        List<int> pingps = value.TrimEnd('\0').Split(',')
                                           .Select(s => int.Parse(s.Trim()))
                                           .ToList();
                                        sonarSettings.PingsPerSecond = pingps;
                                        Console.WriteLine(string.Join(",", sonarSettings.PulsesPerSecond));// Adjust this if multiple values are expected
                                        break;
                                    case "pu":
                                        Console.WriteLine(value);
                                        List<int> pps = value.TrimEnd('\0').Split(',')
                                            .Select(s => int.Parse(s.Trim()))
                                            .ToList();
                                        sonarSettings.PulsesPerSecond = pps; // Adjust this if multiple values are expected
                                        Console.WriteLine(string.Join(",", sonarSettings.PulsesPerSecond));
                                        break;
                                    case "rg":
                                        sonarSettings.Range = value.TrimEnd('\0');
                                        break;
                                    case "sf":
                                        sonarSettings.SampleFilter = Convert.ToInt32(value.TrimEnd('\0'));
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