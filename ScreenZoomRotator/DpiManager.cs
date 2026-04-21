// © 荻野 尚志 / says@o-h.co.jp
using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ScreenZoomRotator;

internal static class DpiManager
{
    private const string DesktopKey = @"Control Panel\Desktop";
    private const int DefaultDpi = 96;

    public static int GetSystemScale()
    {
        using var key = Registry.CurrentUser.OpenSubKey(DesktopKey);
        if (key?.GetValue("LogPixels") is int dpi && dpi > 0)
        {
            return DpiToPercent(dpi);
        }

        return 100;
    }

    public static void SetSystemScale(int percent)
    {
        var dpi = PercentToDpi(percent);

        using (var key = Registry.CurrentUser.OpenSubKey(DesktopKey, writable: true)
                          ?? Registry.CurrentUser.CreateSubKey(DesktopKey))
        {
            key.SetValue("LogPixels", dpi, RegistryValueKind.DWord);
            key.SetValue("Win8DpiScaling", percent == 100 ? 0 : 1, RegistryValueKind.DWord);
        }

        BroadcastDpiChange();
    }

    private static int PercentToDpi(int percent) => (int)Math.Round(DefaultDpi * (percent / 100.0));

    private static int DpiToPercent(int dpi) => (int)Math.Round(dpi * 100.0 / DefaultDpi);

    private static void BroadcastDpiChange()
    {
        const int WM_SETTINGCHANGE = 0x001A;
        const int HWND_BROADCAST = 0xFFFF;
        const uint SMTO_ABORTIFHUNG = 0x0002;

        NativeMethods.SendMessageTimeout(
            (IntPtr)HWND_BROADCAST,
            WM_SETTINGCHANGE,
            IntPtr.Zero,
            "WindowMetrics",
            SMTO_ABORTIFHUNG,
            1000,
            out _);
    }
}
