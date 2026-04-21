// © 荻野 尚志 / says@o-h.co.jp
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ScreenZoomRotator;

internal sealed class TrayAppContext : ApplicationContext
{
    private static readonly int[] ZoomLevels = { 100, 150, 200 };

    private readonly NotifyIcon _notifyIcon;
    private readonly ToolStripMenuItem[] _zoomMenuItems;

    public TrayAppContext()
    {
        _zoomMenuItems = new ToolStripMenuItem[ZoomLevels.Length];

        var menu = new ContextMenuStrip();
        for (var i = 0; i < ZoomLevels.Length; i++)
        {
            var level = ZoomLevels[i];
            var item = new ToolStripMenuItem($"{level}%")
            {
                Tag = level,
            };
            item.Click += (_, _) => ApplyZoom(level);
            _zoomMenuItems[i] = item;
            menu.Items.Add(item);
        }

        menu.Items.Add(new ToolStripSeparator());

        var aboutItem = new ToolStripMenuItem("バージョン情報");
        aboutItem.Click += (_, _) => ShowAbout();
        menu.Items.Add(aboutItem);

        var exitItem = new ToolStripMenuItem("終了");
        exitItem.Click += (_, _) => ExitApplication();
        menu.Items.Add(exitItem);

        _notifyIcon = new NotifyIcon
        {
            Icon = CreateTrayIcon(GetCurrentZoomPercent()),
            ContextMenuStrip = menu,
            Visible = true,
            Text = BuildTooltip(GetCurrentZoomPercent()),
        };

        _notifyIcon.MouseClick += OnTrayIconMouseClick;

        RefreshMenuState();
    }

    private void OnTrayIconMouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            RotateZoom();
        }
    }

    private void RotateZoom()
    {
        var current = GetCurrentZoomPercent();
        var index = Array.IndexOf(ZoomLevels, current);
        var nextIndex = (index + 1) % ZoomLevels.Length;
        if (index < 0)
        {
            nextIndex = 0;
        }

        ApplyZoom(ZoomLevels[nextIndex]);
    }

    private void ApplyZoom(int percent)
    {
        try
        {
            DpiManager.SetSystemScale(percent);
            UpdateIcon(percent);
            _notifyIcon.ShowBalloonTip(
                2000,
                "Screen Zoom Rotator",
                $"画面倍率を {percent}% に変更しました。\n一部のアプリは再起動すると反映されます。",
                ToolTipIcon.Info);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"倍率の変更に失敗しました。\n{ex.Message}",
                "Screen Zoom Rotator",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private int GetCurrentZoomPercent()
    {
        try
        {
            return DpiManager.GetSystemScale();
        }
        catch
        {
            return 100;
        }
    }

    private void UpdateIcon(int percent)
    {
        var oldIcon = _notifyIcon.Icon;
        _notifyIcon.Icon = CreateTrayIcon(percent);
        _notifyIcon.Text = BuildTooltip(percent);
        oldIcon?.Dispose();
        RefreshMenuState();
    }

    private void RefreshMenuState()
    {
        var current = GetCurrentZoomPercent();
        foreach (var item in _zoomMenuItems)
        {
            if (item.Tag is int level)
            {
                item.Checked = level == current;
            }
        }
    }

    private static string BuildTooltip(int percent) => $"Screen Zoom Rotator - 現在 {percent}%（左クリックで切替）";

    private static Icon CreateTrayIcon(int percent)
    {
        const int size = 32;
        using var bmp = new Bitmap(size, size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.Clear(Color.Transparent);

            using var bg = new SolidBrush(Color.FromArgb(91, 138, 114));
            g.FillEllipse(bg, 0, 0, size - 1, size - 1);

            var text = percent.ToString();
            using var font = new Font("Segoe UI", 12f, FontStyle.Bold, GraphicsUnit.Pixel);
            var textSize = g.MeasureString(text, font);
            using var fg = new SolidBrush(Color.White);
            g.DrawString(
                text,
                font,
                fg,
                (size - textSize.Width) / 2f,
                (size - textSize.Height) / 2f);
        }

        var hIcon = bmp.GetHicon();
        var icon = (Icon)Icon.FromHandle(hIcon).Clone();
        NativeMethods.DestroyIcon(hIcon);
        return icon;
    }

    private static void ShowAbout()
    {
        MessageBox.Show(
            "Screen Zoom Rotator\n\n"
            + "ワンクリックで画面倍率を 100% → 150% → 200% に切り替えます。\n\n"
            + "© 荻野 尚志 / says@o-h.co.jp",
            "バージョン情報",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void ExitApplication()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        ExitThread();
    }
}
