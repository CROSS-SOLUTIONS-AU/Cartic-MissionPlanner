using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using static MAVLink;
using MissionPlanner.Controls;
using MissionPlanner.GCSViews;
using System.ComponentModel;
using OpenRGB.NET;
using OpenRGB.NET.Models;
using System.CodeDom;
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
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bathy_Logs", fileName);
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
                    OnPropertyChanged("PingsPerSecondString");
                }
            }
        }
        public string PingsPerSecondString
        {
            get
            {
                if (PingsPerSecond == null)
                {
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
                if (pulsesPerSecond != value)
                {
                    pulsesPerSecond = value;
                    OnPropertyChanged("PulsesPerSecond");
                    OnPropertyChanged("PulsesPerSecondString");
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

    public class SonarButton : MyButton
    {
        private readonly Action<string> externalFunction;
        public SonarButton(string buttonName, Action<string> functionToCall)
        {
            this.Name = buttonName;
            this.Text = "Set";
            this.Size = new System.Drawing.Size(35, 25);
            this.Padding = new Padding(3);

            // Attach the custom OnClick event handler

            // Assign the external function to the local delegate
            this.externalFunction = functionToCall;

            // Attach the custom OnClick event handler
            this.Click += SonarButton_Click;
        }

        private void SonarButton_Click(object sender, EventArgs e)
        {
            // Call the custom function here
            externalFunction?.Invoke(this.Name);
        }
    }

    public class SonarSetDialog : Form
    {
        private Label lblInput;
        private TextBox txtInput;
        private Button btnAccept;
        private Button btnCancel;

        public string InputValue => txtInput.Text;

        private readonly Action<string> externalFunction;

        public SonarSetDialog(string Command, Action<string> ActionToCall)
        {
            this.Name = Command;
            this.externalFunction = ActionToCall;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            lblInput = new Label();
            txtInput = new TextBox();
            btnAccept = new Button();
            btnCancel = new Button();

            string cmdType = BoatControlsPlugin.GetReadableSonarPropertyName(this.Name.RemoveFromEnd("Set"));

            lblInput.Text = $"Enter a new value for {cmdType}:";
            lblInput.Location = new System.Drawing.Point(10, 10);
            lblInput.AutoSize = true;

            txtInput.Location = new System.Drawing.Point(10, 40);
            txtInput.Width = 300;

            btnAccept.Text = "Accept";
            btnAccept.Location = new System.Drawing.Point(10, 70);
            btnAccept.Click += BtnAccept_Click;

            btnCancel.Text = "Cancel";
            btnCancel.Location = new System.Drawing.Point(120, 70);
            btnCancel.Click += BtnCancel_Click;

            this.Controls.Add(lblInput);
            this.Controls.Add(txtInput);
            this.Controls.Add(btnAccept);
            this.Controls.Add(btnCancel);

            this.Text = $"Set {cmdType} for the Sonar";
            this.Size = new System.Drawing.Size(350, 150);
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            // You can handle the Accept logic here, like raising an event or setting a property
            externalFunction?.Invoke($"{this.Name}:{txtInput.Text.TrimEnd()}");
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            // Handle the Cancel logic
            this.Close();
        }
    }

    public class BoatControlsPlugin : MissionPlanner.Plugin.Plugin
    {
        private int _depthRx = 0;
        private bool _recording = false;
        private string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bathy_Logs");
        private BathyLogger fileLogger = null;
        private SonarSettings sonarSettings = new SonarSettings();
        private Process rgbProcess = null;
        private Task rgbTask = null;
        private OpenRGBClient openRgbClient = null;

        private Color _LeftLED0 = new Color(55, 255, 55);
        private Color _LeftLED1 = new Color(55, 255, 55);
        private Color _RightLED0 = new Color(55, 255, 55);
        private Color _RightLED1 = new Color(55, 255, 55);

        private TableLayoutPanel sonarParamLayout = new System.Windows.Forms.TableLayoutPanel();
        private SonarSetDialog sonarSetDialog;

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

        private Control depthOffsetButton;
        private Control rangeButton;
        private Control pingButton;
        private Control pingsPerSecondButton;
        private Control pulsesPerSecondButton;
        private Control depthFilterButton;
        private Control sampleFilterButton;
        private Control depthBlankButton;

        public event EventHandler DepthRecieved;
        public event EventHandler RecordingChanged;

        public override string Name
        {
            get { return "Cartic Boat Controls"; }
        }

        public override string Version
        {
            get { return "1.0"; }
        }

        public override string Author
        {
            get { return "Cartic Pty Ltd - JPiperGreen"; }
        }

        //STATIC FUNCTIONS

        public static string GetReadableSonarPropertyName(string code)
        {
            switch (code)
            {
                case "do":
                    return "Depth Offset";
                case "rg":
                    return "Range";
                case "pm":
                    return "Ping Mode";
                case "pp":
                    return "Pings Per Second";
                case "pu":
                    return "Pulses Per Second";
                case "df":
                    return "Depth Filter";
                case "sf":
                    return "Sample Filter";
                case "db":
                    return "Depth Blank";
                default:
                    return "Unknown Property";
            }
        }

        public static Process ExecuteCommand(string command)
        {
            Process process = new Process();

            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C {command}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            Task.Run(() =>
            {
                string output = process.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
            });

            return process;
        }

        public static void UpdateRGB(OpenRGBClient client, Color LeftLED0, Color LeftLED1, Color RightLED0, Color RightLED1)
        {
            try
            {
                if (Directory.Exists("C:\\OpenRGB\\"))
                {
                    if (client != null)
                    {
                        Color[] colours = new Color[4];
                        colours[0] = LeftLED0;
                        colours[1] = LeftLED1;
                        colours[2] = RightLED0;
                        colours[3] = RightLED1;
                        client.UpdateLeds(0, colours);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set colours of LED's: {ex.Message}");
            }

        }

        //VIRTUAL FUNCTIONS

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
            get { return _depthRx; }
            private set
            {
                _depthRx = value;
            }
        }

        public Color LeftLED0
        {
            get { return _LeftLED0;  }
            private set
            {
                _LeftLED0 = value;
            }
        }
        public Color LeftLED1
        {
            get { return _LeftLED1; }
            private set
            {
                _LeftLED1 = value;
            }
        }
        public Color RightLED0
        {
            get { return _RightLED0; }
            private set
            {
                _RightLED0 = value;
            }
        }
        public Color RightLED1
        {
            get { return _RightLED1; }
            private set
            {
                _RightLED1 = value;
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
                    UpdateRGB(this.openRgbClient, this.LeftLED0, this.LeftLED1, this.RightLED0, this.RightLED1);
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

                depthRxLabel.Text = "Rx: ";
                depthRxLabel.Padding = new Padding(10);
                depthRxLabel.Top = 30;
                depthRxLabel.Left = 10;
                depthRxLabel.Size = new System.Drawing.Size(90, 30);

                recStateLabel.Text = "●";
                recStateLabel.Padding = new Padding(10);
                recStateLabel.Top = 130;
                recStateLabel.Left = 10;
                recStateLabel.Size = new System.Drawing.Size(32, 32);
                recStateLabel.ForeColor = System.Drawing.Color.Black;

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

                depthOffsetButton = new SonarButton("doSet", SetSonarParam);
                rangeButton = new SonarButton("rgSet", SetSonarParam);
                pingButton = new SonarButton("pmSet", SetSonarParam);
                pingsPerSecondButton = new SonarButton("ppSet", SetSonarParam);
                pulsesPerSecondButton = new SonarButton("puSet", SetSonarParam);
                depthFilterButton = new SonarButton("dfSet", SetSonarParam);
                sampleFilterButton = new SonarButton("sfSet", SetSonarParam);
                depthBlankButton = new SonarButton("dbSet", SetSonarParam);

                //Remove default buttons for the simpleActionTab
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
                if (btn3 != null)
                {
                    FlightData.instance.tabActionsSimple.Controls.Remove(btn3[0]);
                }

                sonarParamLayout.Name = "sonarParamLayout";
                sonarParamLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                sonarParamLayout.Size = new System.Drawing.Size(225, 250);
                sonarParamLayout.SuspendLayout();

                int numberOfColumns = 3; // For example, set it to 4 columns. Adjust as needed.

                // Set the number of columns for your TableLayoutPanel
                sonarParamLayout.ColumnCount = numberOfColumns;

                for (int i = 0; i < numberOfColumns; i++)
                {
                    if (i == 0) { sonarParamLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F)); }
                    if (i == 1) { sonarParamLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F)); }
                    if (i == 2) { sonarParamLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F)); }
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
                borderContainer.Left = 135;
                borderContainer.Size = new System.Drawing.Size(225, 250); // Change size as required
                FlightData.instance.tabActionsSimple.Controls.Add(borderContainer);
                borderContainer.Controls.Add(sonarParamLayout);

                // Add labels to the container and format them

                // Depth Offset Label
                depthOffsetLabel.Text = "Depth Offset: ";
                depthOffsetLabel.Size = new System.Drawing.Size(90, 16);
                sonarParamLayout.Controls.Add(depthOffsetLabel, 0, 0);

                // Range Label
                rangeLabel.Text = "Range: ";
                rangeLabel.Size = new System.Drawing.Size(90, 16);
                sonarParamLayout.Controls.Add(rangeLabel, 0, 1);

                // Ping Label
                pingLabel.Text = "Ping: ";
                pingLabel.Size = new System.Drawing.Size(90, 16);
                sonarParamLayout.Controls.Add(pingLabel, 0, 2);

                // Pings Per Second Label
                pingsPerSecondLabel.Text = "Pings/Sec: ";
                pingsPerSecondLabel.Size = new System.Drawing.Size(90, 16);
                sonarParamLayout.Controls.Add(pingsPerSecondLabel, 0, 3);

                // Pulses Per Second Label
                pulsesPerSecondLabel.Text = "Pulses/Sec: ";
                pulsesPerSecondLabel.Size = new System.Drawing.Size(90, 16);
                sonarParamLayout.Controls.Add(pulsesPerSecondLabel, 0, 4);

                // Depth Filter Label
                depthFilterLabel.Text = "Depth Filter: ";
                depthFilterLabel.Size = new System.Drawing.Size(90, 16);
                sonarParamLayout.Controls.Add(depthFilterLabel, 0, 5);

                // Sample Filter Label
                sampleFilterLabel.Text = "Sample Filter: ";
                sampleFilterLabel.Size = new System.Drawing.Size(90, 16);
                sonarParamLayout.Controls.Add(sampleFilterLabel, 0, 6);

                // Depth Blank Label
                depthBlankLabel.Text = "Depth Blank: ";
                depthBlankLabel.Size = new System.Drawing.Size(90, 16);
                sonarParamLayout.Controls.Add(depthBlankLabel, 0, 7);

                /////////// VALUE LABELS

                // Depth Offset Label
                depthOffsetVLabel.Text = "--";
                depthOffsetVLabel.Size = new System.Drawing.Size(90, 16);
                depthOffsetVLabel.DataBindings.Add("Text", sonarSettings, "DepthOffset");
                sonarParamLayout.Controls.Add(depthOffsetVLabel, 1, 0);
                sonarParamLayout.Controls.Add(depthOffsetButton, 2, 0);

                // Range VLabel
                rangeVLabel.Text = "--";
                rangeVLabel.Size = new System.Drawing.Size(90, 16);
                rangeVLabel.DataBindings.Add("Text", sonarSettings, "Range");
                sonarParamLayout.Controls.Add(rangeVLabel, 1, 1);
                sonarParamLayout.Controls.Add(rangeButton, 2, 1);

                // Ping VLabel
                pingVLabel.Text = "--";
                pingVLabel.Size = new System.Drawing.Size(90, 16);
                pingVLabel.DataBindings.Add("Text", sonarSettings, "Ping");
                sonarParamLayout.Controls.Add(pingVLabel, 1, 2);
                sonarParamLayout.Controls.Add(pingButton, 2, 2);

                // Pings Per Second VLabel
                pingsPerSecondVLabel.Text = "--";
                pingsPerSecondVLabel.Size = new System.Drawing.Size(90, 16);
                pingsPerSecondVLabel.DataBindings.Add("Text", sonarSettings, "PingsPerSecondString");
                sonarParamLayout.Controls.Add(pingsPerSecondVLabel, 1, 3);
                sonarParamLayout.Controls.Add(pingsPerSecondButton, 2, 3);

                // Pulses Per Second VLabel
                pulsesPerSecondVLabel.Text = "--";
                pulsesPerSecondVLabel.Size = new System.Drawing.Size(90, 16);
                pulsesPerSecondVLabel.DataBindings.Add("Text", sonarSettings, "PulsesPerSecondString");
                sonarParamLayout.Controls.Add(pulsesPerSecondVLabel, 1, 4);
                sonarParamLayout.Controls.Add(pulsesPerSecondButton, 2, 4);

                // Depth Filter VLabel
                depthFilterVLabel.Text = "--";
                depthFilterVLabel.Size = new System.Drawing.Size(90, 16);
                depthFilterVLabel.DataBindings.Add("Text", sonarSettings, "DepthFilter");
                sonarParamLayout.Controls.Add(depthFilterVLabel, 1, 5);
                sonarParamLayout.Controls.Add(depthFilterButton, 2, 5);

                // Sample Filter VLabel
                sampleFilterVLabel.Text = "--";
                sampleFilterVLabel.Size = new System.Drawing.Size(90, 16);
                sampleFilterVLabel.DataBindings.Add("Text", sonarSettings, "SampleFilter");
                sonarParamLayout.Controls.Add(sampleFilterVLabel, 1, 6);
                sonarParamLayout.Controls.Add(sampleFilterButton, 2, 6);

                // Depth Blank VLabel
                depthBlankVLabel.Text = "--";
                depthBlankVLabel.Size = new System.Drawing.Size(90, 16);
                depthBlankVLabel.DataBindings.Add("Text", sonarSettings, "DepthBlank");
                sonarParamLayout.Controls.Add(depthBlankVLabel, 1, 7);
                sonarParamLayout.Controls.Add(depthBlankButton, 2, 7);

                sonarParamLayout.ResumeLayout(false);

                FlightData.instance.tabActionsSimple.Controls.Add(depthLabel);
                FlightData.instance.tabActionsSimple.Controls.Add(depthRxLabel);
                FlightData.instance.tabActionsSimple.Controls.Add(recStateLabel);
                FlightData.instance.tabActionsSimple.Controls.Add(recButton);
                FlightData.instance.tabActionsSimple.Controls.Add(showLogsButton);

                Application.DoEvents();

            });

            if (Directory.Exists("C:\\OpenRGB\\"))
            {
                if (this.rgbProcess == null)
                {
                    this.rgbTask = Task.Run(() =>
                    {
                        this.rgbProcess = ExecuteCommand($"C:\\OpenRGB\\OpenRGB.exe --noautoconnect --server");
                    });
                    this.openRgbClient = new OpenRGBClient();
                    this.openRgbClient.Connect();
                }
            }

            MissionPlanner.MainV2.comPort.OnPacketReceived += ComPort_RecieveState;
            return true;
        }

        public override bool Loaded()
        {
            loopratehz = 0.5f;
            recStateLabel.ForeColor = System.Drawing.Color.Black;

            //Color[] colours = new Color[4];
            //colours[0] = new Color(55, 255, 55);
            //colours[1] = new Color(55, 255, 55);
            //colours[2] = new Color(55, 255, 55);
            //colours[3] = new Color(55, 255, 55);

            UpdateRGB(this.openRgbClient, LeftLED0, LeftLED1, RightLED0, RightLED1);
            //this.openRgbClient.UpdateLeds(0, colours);

            return true;
        }

        public override bool Loop()
        {

            return true;
        }

        public override bool Exit()
        {
            if (rgbProcess != null)
            {
                this.rgbProcess.Close();
            }
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
                Parallel.ForEach(MissionPlanner.MainV2.Comports, mav =>
                {
                    mav.send_text((byte)MAV_SEVERITY.INFO, "recordDepth:1");
                });
                recButton.Invoke(new Action(() => { recButton.Text = "Starting... Recording"; }));

            }
            else
            {
                Parallel.ForEach(MissionPlanner.MainV2.Comports, mav =>
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

        public void SetSonarParam(string Param)
        {
            sonarSetDialog = new SonarSetDialog(Param, SendMessage);
            sonarSetDialog.Show();
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
            if (MissionPlanner.MainV2.comPort.BaseStream.IsOpen)
            {
                // Get current MAV state
                MissionPlanner.MAVState mavState = MissionPlanner.MainV2.comPort.MAV;
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
                //MissionPlanner.MainV2.comPort.sendPacket(statusText, hostsysId, hostcompId);
                MissionPlanner.MainV2.comPort.send_text((byte)MAV_SEVERITY.INFO, message);
            }
        }

        public void ComPort_RecieveState(object sender, MAVLink.MAVLinkMessage e)
        {
            try
            {
                if (e != null)
                {
                    var eCopy = new MAVLink.MAVLinkMessage();
                    //make threadsafe copy of message, just in case.
                    lock (e)
                    {
                        eCopy = e;
                    }
                    if (eCopy != null && eCopy.msgid == (uint)MAVLink.MAVLINK_MSG_ID.RANGEFINDER)
                    {
                        if (eCopy.compid == 2)
                        {

                            DepthRx++;
                            var depth = (MAVLink.mavlink_rangefinder_t)eCopy.data;

                            Host.cs.sonarrange = -depth.distance / 100;
                            Recording = (depth.voltage == 1) ? true : false;

                            Action updateUI = new Action(() =>
                            {
                                if (depthRxLabel.IsHandleCreated)
                                {
                                    depthRxLabel.Text = $"Rx: {DepthRx}";
                                }
                                if (depthLabel.IsHandleCreated)
                                {
                                    depthLabel.Text = $"{(depth.distance / 100)} m";
                                }
                                if (recButton.IsHandleCreated)
                                {
                                    recButton.Text = Recording ? "End Recording" : "Start Recording Soundings";
                                }
                                if (recStateLabel.IsHandleCreated)
                                {
                                    recStateLabel.ForeColor = Recording ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                                }
                            });

                            // Use BeginInvoke on one of the controls
                            if (depthRxLabel.InvokeRequired)
                            {
                                depthRxLabel.BeginInvoke(updateUI);
                            }
                            else
                            {
                                updateUI();
                            }

                            this.LeftLED0 = Recording ? this.LeftLED0 : new Color(255, 0, 0);
                            this.LeftLED1 = Recording ? this.LeftLED1 : new Color(255, 0, 0);
                            if (fileLogger != null)
                            {
                                fileLogger.WriteLine(Host.cs.datetime.toUnixTimeDouble(), Host.cs.lat, Host.cs.lng, depth.distance / 100, Host.cs.gpsstatus, Host.cs.altasl);
                            }

                        }
                    }
                    else if (eCopy.msgid == (uint)MAVLink.MAVLINK_MSG_ID.STATUSTEXT)
                    {
                        if (eCopy != null && eCopy.compid == (byte)2)
                        {
                            var status = (MAVLink.mavlink_statustext_t)e.data;
                            string statusText = Encoding.UTF8.GetString(status.text);
                            string[] parts = statusText.Split(':');
                            string key = parts[0];

                            if (parts.Length > 1)
                            {
                                string value = parts[1].TrimEnd('\0');
                                if (!string.IsNullOrEmpty(value))
                                {
                                    switch (key)
                                    {
                                        case "db":
                                            if (Int32.TryParse(value, out int dbResult))
                                            {
                                                sonarSettings.DepthBlank = dbResult;
                                            }
                                            break;
                                        case "df":
                                            if (Int32.TryParse(value, out int dfResult))
                                            {
                                                sonarSettings.DepthFilter = dfResult;
                                            }
                                            break;
                                        case "do":
                                            if (Int32.TryParse(value, out int doResult))
                                            {
                                                sonarSettings.DepthOffset = doResult;
                                            }
                                            break;
                                        case "pm":
                                            sonarSettings.Ping = value;
                                            break;
                                        case "pp":
                                            string ppValueSantized = value.TrimStart('[').TrimEnd(']');
                                            List<int> pingps = ppValueSantized.Split(',')
                                                .Select(s => int.Parse(s.Trim()))
                                                .ToList();
                                            sonarSettings.PingsPerSecond = pingps;
                                            break;
                                        case "pu":
                                            string puValueSantized = value.TrimStart('[').TrimEnd(']');
                                            List<int> pps = puValueSantized.Split(',')
                                                .Select(s => int.Parse(s.Trim()))
                                                .ToList();
                                            sonarSettings.PulsesPerSecond = pps;
                                            break;
                                        case "rg":
                                            sonarSettings.Range = value;
                                            break;
                                        case "sf":
                                            if (Int32.TryParse(value, out int sfResult))
                                            {
                                                sonarSettings.SampleFilter = sfResult;
                                            }
                                            break;
                                            // Add more cases as needed
                                    }
                                    UpdateRGB(this.openRgbClient, this.LeftLED0, this.LeftLED1, this.RightLED0, this.RightLED1);
                                }
                            }
                        }
                    }

                }
                else
                {
                    Console.WriteLine("Mavlink Msg (e) is Null");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("-=------=- MAVLINK MSG ERROR -=------=- \n" + ex.Message);
            }
        }
    }
}