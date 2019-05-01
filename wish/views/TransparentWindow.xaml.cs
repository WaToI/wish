using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;

namespace wish {
  /// <summary>
  /// TransparentWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class TransparentWindow : Window {

    public bool IsThroughWindow { get; set; } = false;
    WindowInteropHelper winHelper;
    GlobalHooker hooker;
    MouseHookListener mouseListener;
    KeyboardHookListener keyListener;

    public TransparentWindow() {
      InitializeComponent();

      //ウィンドウをマウスで掴んでドラッグ可能
      this.MouseLeftButtonDown += (sender, e) => this.DragMove();

      hooker = new GlobalHooker();
      mouseListener = new MouseHookListener(hooker);
      mouseListener.Enabled = true;
      keyListener = new KeyboardHookListener(hooker);
      keyListener.Enabled = true;

      //ctrl押したときのみスルー
      //this.KeyDown += MainWindow_KeyDown;
      //keyListener.KeyUp += KeyListener_KeyUp;

      //常時スルー。ctrl押したときのみウインドウつかめる
      keyListener.KeyDown += KeyListener_KeyDown;
      this.KeyUp += MainWindow_KeyUp;
    }

    protected readonly int GWL_EXSTYLE = (-20);
    protected int WS_EX_DEFAULT;// = 0xC0100; 
    protected readonly int WS_EX_TRANSPARENT = 0x00000020;

    [DllImport("user32")]
    protected static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32")]
    protected static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwLong);

    protected override void OnSourceInitialized(EventArgs e) {
      base.OnSourceInitialized(e);
      winHelper = new WindowInteropHelper(this);
      WS_EX_DEFAULT = GetWindowLong(winHelper.Handle, GWL_EXSTYLE);
      SetWindowLong(winHelper.Handle, GWL_EXSTYLE, WS_EX_DEFAULT | WS_EX_TRANSPARENT);
    }

    void activeWindow() {
      //this.Topmost = true;
      SetWindowLong(winHelper.Handle, GWL_EXSTYLE, WS_EX_DEFAULT);
    }
    void deactiveWindow() {
      //this.Topmost = false;
      SetWindowLong(winHelper.Handle, GWL_EXSTYLE, WS_EX_DEFAULT | WS_EX_TRANSPARENT);
    }

    void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
      if (e.Key == System.Windows.Input.Key.LeftCtrl) {
        deactiveWindow();
      }
    }
    void KeyListener_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
      if (e.Control) {
        activeWindow();
      }
    }
    void KeyListener_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
      if (e.Control) {
        activeWindow();
      }
    }
    void MainWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) {
      if (e.Key == System.Windows.Input.Key.LeftCtrl) {
        deactiveWindow();
      }
    }
  }//class

  //値の一致を表示状態に変換するコンバータ
  [ValueConversion(typeof(object), typeof(Visibility))]
  public class EqualsToVisibleConveter : IValueConverter {

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      if (value == null) {
        return parameter == null ? Visibility.Visible : Visibility.Collapsed;
      }
      return value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
    }

    //使わないので未実装
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      throw new NotImplementedException();
    }
  }
}//namespace
