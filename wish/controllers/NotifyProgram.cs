using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wish {

  public sealed class NotifyProgram {
    private NotifyIcon notifyIcon;
    private ContextMenu notificationMenu;

    #region Initialize icon and menu
    public NotifyProgram() {
      notifyIcon = new NotifyIcon();
      notificationMenu = new ContextMenu(InitializeMenu());

      notifyIcon.DoubleClick += IconDoubleClick;
      notifyIcon.Icon = Properties.Resources.wish;
      notifyIcon.ContextMenu = notificationMenu;
    }

    private MenuItem[] InitializeMenu() {
      MenuItem[] menu = new MenuItem[] {
        new MenuItem("About", menuAboutClick),
        new MenuItem("Exit", menuExitClick)
      };
      return menu;
    }
    #endregion

    #region main
    [STAThread]
    public static void Main(string[] args) {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      bool isFirstInstance;
      using (Mutex mtx = new Mutex(true, "notify", out isFirstInstance)) {
        if (isFirstInstance) {
          var notificationIcon = new NotifyProgram();
          notificationIcon.notifyIcon.Visible = true;
          Application.Run();
          notificationIcon.notifyIcon.Dispose();
        } else {
          // The application is already running
          // TODO: Display message box or change focus to existing application instance
        }
      } // releases the Mutex
    }
    #endregion

    #region Event Handlers
    private void menuAboutClick(object sender, EventArgs e) {
      MessageBox.Show("About This Application");
    }

    private void menuExitClick(object sender, EventArgs e) {
      Application.Exit();
    }

    private void IconDoubleClick(object sender, EventArgs e) {
      var wind = new TransparentWindow();
      wind.Topmost = true;
      wind.Show();
    }
    #endregion
  }
}
