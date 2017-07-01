using Kennedy.ManagedHooks;
using Newtonsoft.Json;
using QuickCommander.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Forms = System.Windows.Forms;

namespace QuickCommander
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Output> Outputs { get; set; }

        private bool isVisible;
        private bool isCtrlDown;
        private bool isShiftDown;
        private bool isAltDown;
        private int screenTop;

        public MainWindow(int screenTop)
        {
            if (File.Exists("config.json"))
            {
                var json = File.ReadAllText("config.json");
                var conf = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                ConfigManager.Load(conf);
            }
            else
            {
                ConfigManager.Load();
            }

            InitializeComponent();
            Deactivated += (sender, e) => CloseCommandLine();

            Outputs = new ObservableCollection<Output>();
            DataContext = this;

            this.screenTop = screenTop;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)NativeMethods.GetWindowLong(
                                   wndHelper.Handle,
                                   (int)NativeMethods.GetWindowLongFields.GWL_EXSTYLE
                               );

            exStyle |= (int)NativeMethods.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            NativeMethods.SetWindowLong(
                wndHelper.Handle,
                (int)NativeMethods.GetWindowLongFields.GWL_EXSTYLE,
                (IntPtr)exStyle
            );
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) CloseCommandLine();
            if (e.Key != Key.Enter) return;

            var line = Command.Text.Split(new char[]{' '}, 2);
            var cmd = line[0];
            var args = (line.Length < 2) ? new string[0] : ParseArgs(line[1]);

            Command.Text = "";

            switch (cmd)
            {
                case "exec":
                    {
                        if (args.Length < 1)
                        {
                            IOManager.Out(this, "Usage: exec [name]");
                            break;
                        }

                        var list = ConfigManager.Get("QuickCommander.Exec.List");
                        if (list != null && !(list is Dictionary<string, string>))
                        {
                            IOManager.Out(this, "Type of config 'QuickCommander.Exec.List' is invalid.");
                            break;
                        }

                        if (list == null || !((Dictionary<string, string>)list).ContainsKey(args[0]))
                        {
                            IOManager.Out(this, "Specified name is not registed.");
                            break;
                        }

                        Process.Start(((Dictionary<string, string>)list)[args[0]]);
                    }
                    break;

                case "exec-regist":
                    {
                        if (args.Length < 2)
                        {
                            IOManager.Out(this, "Usage: exec-regist [name] [path to file]");
                            break;
                        }

                        if (!File.Exists(args[1]))
                        {
                            IOManager.Out(this, "The file does not exist.");
                            break;
                        }

                        var list = ConfigManager.Get("QuickCommander.Exec.List");
                        if (list != null && !(list is Dictionary<string, string>))
                        {
                            IOManager.Out(this, "Type of config 'QuickCommander.Exec.List' is invalid.");
                            break;
                        }

                        if (list == null)
                        {
                            ConfigManager.Set("QuickCommander.Exec.List", new Dictionary<string, string>());
                        }

                        ((Dictionary<string, string>)list).Add(args[0], args[1]);
                    }
                    break;

                case "plugin":
                    if (args.Length < 1)
                    {
                        IOManager.Out(this, "Usage: plugin [name]");
                        break;
                    }

                    var plugin = ((App)Application.Current)
                                     .plugins
                                     .Where(p => p.Name == args[0])
                                     .FirstOrDefault();

                    IOManager.Out(this, "Name: " + plugin.Name);
                    IOManager.Out(this, "Description: " + plugin.Description);
                    IOManager.Out(this, "Author: " + plugin.Author);
                    IOManager.Out(this, "Version: " + plugin.Version);
                    IOManager.Out(this, "Location: " + plugin.Location);
                    IOManager.Out(this, "Class: " + plugin.ClassName);
                    break;

                case "plugins":
                    ((App)Application.Current)
                        .plugins
                        .ForEach(
                             p => IOManager.Out(
                                      this,
                                      p.Name + " (" + Path.GetFileName(p.Location) + ")"
                                  )
                         );
                    break;

                case "config":
                    if (args.Length < 2 ||
                        (args[0].ToLower() != "get" && args[0].ToLower() != "set") ||
                        (args[0].ToLower() == "set" && args.Length < 3))
                    {
                        IOManager.Out(this, "Usage: config [get|set] [key] (value: if 'set' operation)");
                        break;
                    }

                    if (args[0].ToLower() == "get")
                    {
                        var value = ConfigManager.Get(args[1]);
                        if (value == null)
                        {
                            IOManager.Out(this, "Value of key " + args[1] + " does not exist or the value is null.");
                        }
                        else
                        {
                            IOManager.Out(this, args[1] + " : " + ConfigManager.Get(args[1]));
                        }
                    }
                    else
                    {
                        ConfigManager.Set(args[1], args[2]);
                        IOManager.Out(this, "Setted.");
                    }
                    break;

                case "echo":
                    if (args.Length < 1)
                    {
                        IOManager.Out(this, "Usage: echo [message]");
                        break;
                    }

                    IOManager.Out(this, args[0]);
                    break;

                case "close":
                    CloseCommandLine();
                    break;

                case "quit":
                case "exit":
                    ((App)Application.Current).Shutdown();
                    break;

                default:
                    if (!API.CommandManager.Execute(cmd, args))
                        IOManager.Out(this, "Unknown command.");
                    break;
            }
        }

        public void OnGlobalKeyDown(KeyboardEvents kEvent, Forms.Keys key)
        {
            if (kEvent == KeyboardEvents.KeyDown || kEvent == KeyboardEvents.SystemKeyDown)
            {
                switch (key)
                {
                    case Forms.Keys.Control:
                        isCtrlDown = true;
                        break;

                    case Forms.Keys.Shift:
                        isShiftDown = true;
                        break;
                        
                    case Forms.Keys.Alt:
                        isAltDown = true;
                        break;

                    case Forms.Keys.C:
                        if (isCtrlDown && isShiftDown) ShowCommandLine();
                        break;
                }
            }
            else
            {
                switch (key)
                {
                    case Forms.Keys.Control:
                        isCtrlDown = false;
                        break;

                    case Forms.Keys.Shift:
                        isShiftDown = false;
                        break;

                    case Forms.Keys.Alt:
                        isAltDown = false;
                        break;
                }
            }
        }

        private string[] ParseArgs(string args)
        {
            var argsList = new List<string>();
            var temp = "";
            char? quotation = null;
            foreach (var c in args.Trim())
            {
                if (c != '"' && c != '\'')
                {
                    if (quotation == null && c == ' ')
                    {
                        argsList.Add(temp);
                        temp = "";
                    }
                    else
                    {
                        temp += c;
                    }
                }
                else
                {
                    if (quotation == null)
                    {
                        quotation = c;
                    }
                    else if(quotation == c)
                    {
                        argsList.Add(temp);
                        temp = "";
                        quotation = null;
                    }
                    else
                    {
                        temp += c;
                    }
                }
            }

            if (temp != "") argsList.Add(temp);

            return argsList.ToArray();
        }

        public void OnOutput(object sender, OutputEventArgs e)
        {
            var pluginName = (!(sender is MainWindow))
                                ? ((App)Application.Current)
                                      .plugins
                                      .Where(p => sender == p.Instance)
                                      .FirstOrDefault()
                                      .Name
                                : "QuickCommander";

            var output = new Output(
                             "[" + pluginName + "] " + e.Message,
                             e.Timeout
                         );

            output.OutputTimeout += OnOutputTimeout;
            Outputs.Add(output);
        }

        public void OnOutputTimeout(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                Outputs.Remove((Output)sender);
            }));
        }

        public void OnConfigSet(object sender, ConfigSettedEventArgs e)
        {
            Task.Run(() =>
            {
                var conf = ConfigManager.GetAll();
                var json = JsonConvert.SerializeObject(conf);
                File.WriteAllText("config.json", json);
            });
        }

        public void ShowCommandLine()
        {
            if (isVisible) return;
            isVisible = true;

            Activate();
            ChangeLocation(screenTop - 2, new Duration(TimeSpan.FromMilliseconds(300)));
            Keyboard.Focus(Command);
        }

        public void CloseCommandLine()
        {
            if (!isVisible) return;
            isVisible = false;

            ChangeLocation(screenTop - 49, new Duration(TimeSpan.FromMilliseconds(300)), () => Command.Text = "");
        }

        private void ChangeLocation(double top, Duration duration, Action onCompleted = null)
        {
            var animation = new DoubleAnimation(top, duration);
            if (onCompleted != null) animation.Completed += (sender, e) => onCompleted();

            BeginAnimation(TopProperty, animation);
        }
    }
}