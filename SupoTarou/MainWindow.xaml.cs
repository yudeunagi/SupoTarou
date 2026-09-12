using System.Windows;
using System.Windows.Threading;
using SupoTarou.Input;
using SupoTarou.Models;
using SupoTarou.Services;

namespace SupoTarou;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly DispatcherTimer _samplingTimer;
    private readonly ScreenColorSampler _screenColorSampler = new();
    private readonly GlobalKeyboardHook _globalKeyboardHook = new();
    private PickerState _pickerState = PickerState.Sampling;
    private ColorData _currentColor = ColorConverter.FromRgb(0, 0, 0);

    public MainWindow()
    {
        InitializeComponent();
        _samplingTimer = new DispatcherTimer(DispatcherPriority.Render)
        {
            Interval = TimeSpan.FromMilliseconds(33)
        };
        _samplingTimer.Tick += OnSamplingTimerTick;

        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    /// <summary>
    /// 起動時処理 (Loaded イベントハンドラ)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _globalKeyboardHook.SpacePressed += OnSpacePressed;
        _globalKeyboardHook.Start();

        UpdateStateText();
        RefreshColorFromCursor();
        _samplingTimer.Start();
    }

    /// <summary>
    /// 終了時処理 (Closed イベントハンドラ)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnClosed(object? sender, EventArgs e)
    {
        _samplingTimer.Stop();
        _samplingTimer.Tick -= OnSamplingTimerTick;
        _globalKeyboardHook.SpacePressed -= OnSpacePressed;
        _globalKeyboardHook.Dispose();
    }

    /// <summary>
    /// サンプリングタイマーの Tick イベントハンドラ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSamplingTimerTick(object? sender, EventArgs e)
    {
        if (_pickerState != PickerState.Sampling)
        {
            return;
        }

        RefreshColorFromCursor();
    }

    /// <summary>
    /// カーソル位置から色を取得して表示を更新する
    /// </summary>
    private void RefreshColorFromCursor()
    {
        if (_screenColorSampler.TrySampleRgb(out var red, out var green, out var blue))
        {
            _currentColor = ColorConverter.FromRgb(red, green, blue);
            UpdateDisplay(_currentColor);
        }
    }

    /// <summary>
    /// Spaceキーが押されたときの処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSpacePressed(object? sender, EventArgs e)
    {
        _ = Dispatcher.BeginInvoke(ToggleSamplingState);
    }

    /// <summary>
    /// 表示色取得状態を切り替える
    /// </summary>
    private void ToggleSamplingState()
    {
        _pickerState = _pickerState == PickerState.Sampling
            ? PickerState.Locked
            : PickerState.Sampling;

        if (_pickerState == PickerState.Sampling)
        {
            RefreshColorFromCursor();
        }

        UpdateStateText();
    }

    /// <summary>
    /// 表示を更新する
    /// </summary>
    /// <param name="colorData"></param>
    private void UpdateDisplay(ColorData colorData)
    {
        HueRing.Hue = colorData.H;
        SvBox.Hue = colorData.H;
        SvBox.Saturation = colorData.S;
        SvBox.Value = colorData.V;

        HSVValueText.Text = $"HSV : {Math.Round(colorData.H):0}°, {Math.Round(colorData.S):0}%, {Math.Round(colorData.V):0}%";
        // 分割したい場合はこっち
        // HueValueText.Text = $"H : {Math.Round(colorData.H):0}°";
        // SaturationValueText.Text = $"S : {Math.Round(colorData.S):0}%";
        // ValueValueText.Text = $"V : {Math.Round(colorData.V):0}%";
    }

    private void UpdateStateText()
    {
        StateText.Text = _pickerState == PickerState.Sampling
            ? "状態：取得中"
            : "状態：固定中";
    }
}

internal enum PickerState
{
    Sampling,
    Locked
}