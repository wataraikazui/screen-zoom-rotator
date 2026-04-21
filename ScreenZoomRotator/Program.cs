// © 荻野 尚志 / says@o-h.co.jp
using System;
using System.Threading;
using System.Windows.Forms;

namespace ScreenZoomRotator;

internal static class Program
{
    private const string MutexName = "ScreenZoomRotator_SingleInstance_Mutex";

    [STAThread]
    private static void Main()
    {
        using var mutex = new Mutex(true, MutexName, out var createdNew);
        if (!createdNew)
        {
            MessageBox.Show(
                "すでに起動しています。タスクトレイのアイコンをご確認ください。",
                "Screen Zoom Rotator",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new TrayAppContext());
    }
}
