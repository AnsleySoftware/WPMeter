using System.Diagnostics;
using System.Runtime.InteropServices;


namespace WPMeter
{
    public partial class Form1 : Form
    {
        private UIController? _ui;
        private TypingController? _typingController;
        private IInputSource? _inputSource;
        private StartupManager? _startupManager;
        private SettingsManager? _settings;
        private readonly string? _iniPath;
        private ColorDialog? colorDialog;
        private readonly LogManager? _logManager;
        private ContextMenuStrip? contextMenu;
        private TextBox? inputBox;
        private readonly Size defaultSize = new Size(150, 90);
        [DllImport("user32.dll")] public static extern bool ReleaseCapture();
        [DllImport("user32.dll")] public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private ToolStripMenuItem? toggleGlobalHookItem;
        private ToolStripMenuItem? addToStartupItem;
        private ToolStripMenuItem? keepLogsItem;
        private ToolStripMenuItem? exitItem;
        private ToolStripMenuItem? showLogsItem;

        public Form1()
        {
            string folder = FileHelpers.EnsureAppDataDirectory("WPMeter");
            Directory.CreateDirectory(folder);
            _iniPath = Path.Combine(folder, "config.ini");
            FileHelpers.EnsureFileExists(_iniPath);
            _settings = new SettingsManager(_iniPath);
            _startupManager = new StartupManager(AppName, AppPath);
            InitializeComponent();

            var logPath = Path.Combine(folder, "logs.txt");
            _logManager = new LogManager(logPath);

            InitializeUIController();
            SetupContextMenu();
            SetupWidgetStyle();
            SetupInputBox();
            LoadSettings();
            SetupColorDialog();
            SetupTransparencyControl();
            FinalizeUIState();
            InitializeTypingController();
            if (_settings!.ShowReadmeOnLaunch)
            {
                ShowReadme();
                _settings.ShowReadmeOnLaunch = false;
            }
        }

        private void ShowReadme()
        {
            string readmePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "README.html");
            if (File.Exists(readmePath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = readmePath,
                    UseShellExecute = true
                });
            }
        }

        private void InitializeUIController()
        {
            _ui = new UIController(this, _settings!);
        }

        private void InitializeTypingController()
        {
            _typingController = new TypingController(_logManager!);
            _typingController.WpmUpdated += (wpm) => SafeInvoke(() => UpdateWpmDisplay(wpm));

            if (_settings!.KeepLogs)
            {
                _typingController.EnableLogging();
            }
        }

        private void SetupTransparencyControl()
        {
            var transparency = new TransparencyController(this, _settings!);
            contextMenu!.Items.Insert(contextMenu.Items.IndexOf(exitItem!), transparency.MenuItem!);
        }

        private void FinalizeUIState()
        {
            addToStartupItem!.Checked = _startupManager!.IsEnabled();
            keepLogsItem!.Checked = _settings!.KeepLogs;
        }

        private void LoadSettings()
        {
            this.Location = _settings!.WindowLocation;
            this.Size = _settings!.WindowSize;
            this.BackColor = _settings.BackColor;
            this.Opacity = _settings.Opacity;
            ApplyGlobalCapture(_settings.UseGlobalHook);
        }


        private void SetupColorDialog()
        {
            var changeColorItem = new ToolStripMenuItem("Change Color...");
            changeColorItem.Click += ChangeColorItem_Click;
            contextMenu!.Items.Insert(4, changeColorItem);

            colorDialog = new ColorDialog
            {
                AllowFullOpen = true,
                AnyColor = true,
                SolidColorOnly = false,
                Color = this.BackColor
            };
        }

        private void ChangeColorItem_Click(object? sender, EventArgs e)
        {
            if (colorDialog!.ShowDialog(this) == DialogResult.OK)
            {
                this.BackColor = colorDialog.Color;
            }

            this.BackColor = colorDialog.Color;
            _settings!.BackColor = colorDialog.Color;

        }
        private void SetupInputBox()
        {
            inputBox = new TextBox()
            {
                Dock = DockStyle.Bottom,
                Height = 24,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10),
                Visible = false
            };

            this.Controls.Add(inputBox);
        }


        private void SetupWidgetStyle()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = defaultSize;
            this.TopMost = true;
            this.Opacity = 0.6;
            this.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;

            this.MouseDown += Form_MouseDown;
        }

        private void Form_MouseDown(object? sender, MouseEventArgs e)
        {
            DragForm(e);
        }


        private void Label_MouseDown(object? sender, MouseEventArgs e)
        {
            DragForm(e);
        }

        private void SetupContextMenu()
        {
            contextMenu = new ContextMenuStrip();
            keepLogsItem = new ToolStripMenuItem(
                text: "Keep Logs",
                image: null
                )
            {
                CheckOnClick = true
            };
            keepLogsItem.Click += KeepLogsItem_Click;

            toggleGlobalHookItem = new ToolStripMenuItem(
                text: "Enable Global Key Capture",
                image: null
                )
            {
                Checked = false,
                CheckOnClick = true
            };
            toggleGlobalHookItem.Click += ToggleGlobalHookItem_Click;

            addToStartupItem = new ToolStripMenuItem(
                text: "Add to Startup",
                image: null
                )
            {
                Checked = false,
                CheckOnClick = true
            };
            addToStartupItem.Click += AddToStartupItem_Click;

            showLogsItem = new ToolStripMenuItem("Show Logs");
            showLogsItem.Click += ShowLogsItem_Click;

            exitItem = new ToolStripMenuItem(
                text: "Exit",
                image: null,
                onClick: ExitItem_Click
                );


            contextMenu.Items.Add(keepLogsItem);
            contextMenu.Items.Add(toggleGlobalHookItem);
            contextMenu.Items.Add(addToStartupItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(showLogsItem);
            contextMenu.Items.Add(exitItem);



            this.ContextMenuStrip = contextMenu;
            _ui!.WpmLabel.ContextMenuStrip = contextMenu;

        }

        

        private void ApplyGlobalCapture(bool useGlobal)
        {
            _inputSource?.Dispose();
            if (useGlobal)
            {
                inputBox!.Visible = false;
                _inputSource = new GlobalInputSource();
            }
            else
            {
                inputBox!.Visible = true;
                _inputSource = new TextBoxInputSource(inputBox!);
            }

            _inputSource.KeyPressed += InputSource_KeyDown;
            _inputSource.KeyReleased += InputSource_KeyUp;
            _inputSource.Start();
            toggleGlobalHookItem!.Checked = useGlobal;
            _settings!.UseGlobalHook = useGlobal;
        }

        private void InputSource_KeyDown(object? sender, KeyEventArgs e)
        {
            _typingController!.HandleKeyDown(e.KeyCode);

            if (_inputSource is TextBoxInputSource)
                e.Handled = true;
        }

        private void InputSource_KeyUp(object? sender, KeyEventArgs e)
        {
            _typingController!.HandleKeyUp(e.KeyCode);
        }


        private void SetMenuItemChecked(ToolStripMenuItem item, bool isChecked)
        {
            item.Checked = isChecked;
        }


        private void AddToStartup()
        {
            bool nowEnabled = !_startupManager!.IsEnabled();
            _startupManager.SetEnabled(nowEnabled);
            SetMenuItemChecked(addToStartupItem!, nowEnabled);

        }
        private void UpdateWpmDisplay(double? wpm)
        {
            _ui!.WpmLabel.Text = wpm.HasValue
        ? $"{Math.Round(wpm.Value)} WPM"
        : "-- WPM";

        }

        private void DragForm(MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void SafeInvoke(Action uiAction)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(uiAction);
            else
                uiAction();
        }

        private string AppName => "WPMeter";
        private string AppPath => Application.ExecutablePath;






        private void KeepLogsItem_Click(object? sender, EventArgs e)
        {
            _settings!.KeepLogs = keepLogsItem!.Checked;
            if (keepLogsItem!.Checked)
            {
                if (!_logManager!.LogFileExists())
                    _logManager.CreateLogFile();

                _typingController!.EnableLogging();
            }
            else
            {
                _typingController!.DisableLogging();
            }
        }

        private void ShowLogsItem_Click(object? sender, EventArgs e)
        {
            if (!_logManager!.LogFileExists())
            {
                var ans = MessageBox.Show(
                    "No Log file found. Create one now?",
                    "Keep Logs",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (ans == DialogResult.Yes)
                {
                    _logManager.CreateLogFile();
                    keepLogsItem!.Checked = true;
                    _settings!.KeepLogs = true;
                    _typingController!.EnableLogging();
                }
                else
                {
                    return;
                }
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = "notepad.exe",
                Arguments = _logManager.LogFilePath,
                UseShellExecute = true
            });
        }
        private void ToggleGlobalHookItem_Click(object? sender, EventArgs e) => ApplyGlobalCapture(toggleGlobalHookItem!.Checked);
        private void AddToStartupItem_Click(object? sender, EventArgs e) => AddToStartup();

        private void ExitItem_Click(object? sender, EventArgs e) => this.Close();

        private void Form1_Load(object sender, EventArgs e)
        {
            var grip = _ui!.ResizeGrip;

            Controls.SetChildIndex(grip, 0);
            grip.BringToFront();
            grip.Visible = true;
            grip.BackColor = Color.Red;
            grip.Location = new Point(this.ClientSize.Width - grip.Width, this.ClientSize.Height - grip.Height);
        }
    }
}
