using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wish {

  public sealed class Notify {
    private NotifyIcon notifyIcon;
    private ContextMenu notificationMenu;

    #region Initialize icon and menu
    public Notify() {
      notifyIcon = new NotifyIcon();
      notificationMenu = new ContextMenu(InitializeMenu());

      notifyIcon.DoubleClick += IconDoubleClick;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Notify));
      //notifyIcon.Icon = (Icon)resources.GetObject("$this.Icon");
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
    public static void Main2(string[] args) {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      bool isFirstInstance;
      // Please use a unique name for the mutex to prevent conflicts with other programs
      using (Mutex mtx = new Mutex(true, "notify", out isFirstInstance)) {
        if (isFirstInstance) {
          var notificationIcon = new Notify();
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
      MessageBox.Show("The icon was double clicked");
    }
    #endregion
  }
}
