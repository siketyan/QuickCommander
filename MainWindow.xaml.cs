﻿using Kennedy.ManagedHooks;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;

using Forms = System.Windows.Forms;

namespace QuickCommander
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isVisible;
        private bool isCtrlDown;
        private bool isShiftDown;
        private bool isAltDown;

        public MainWindow()
        {
            InitializeComponent();
            Deactivated += (sender, e) => CloseCommandLine();
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
            var args = (line.Length < 2) ? new string[0] : line[1].Split(' ');

            switch (cmd)
            {
                case "close":
                    CloseCommandLine();
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

        public void ShowCommandLine()
        {
            if (isVisible) return;
            isVisible = true;

            Storyboard sb = FindResource("ShowAnimation") as Storyboard;
            sb.Completed += (sender, e) => Keyboard.Focus(Command);
            sb.Begin();
        }

        public void CloseCommandLine()
        {
            if (!isVisible) return;
            isVisible = false;

            Storyboard sb = FindResource("CloseAnimation") as Storyboard;
            sb.Completed += (sender, e) => Command.Text = "";
            sb.Begin();
        }
    }
}