using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using System.IO;
using MS.WindowsAPICodePack.Internal;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace wish {

  public class Toaster {

    readonly string appId = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

    public void showToast() {
      TryCreateShortcut();
      var dq = "\"";
      var toastXml = $@"
<toast duration={dq}long{dq}>
    <visual>
        <binding template={dq}ToastText02{dq}>
            <text id={dq}1{dq}>test Title</text>
            <text id={dq}2{dq}>test content</text>
        </binding>
    </visual>
  <actions>
  <input id={dq}snoozeTime{dq} type={dq}selection{dq} defaultInput={dq}15{dq}>
    #5つまで
    <selection id={dq}3{dq} content={dq}3 minutes{dq} />
    <selection id={dq}5{dq} content={dq}5 minutes{dq} />
    <selection id={dq}10{dq} content={dq}10 minutes{dq} />
    <selection id={dq}15{dq} content={dq}15 minutes{dq} />
    <selection id={dq}30{dq} content={dq}30 minutes{dq} />
  </input>
  <action activationType={dq}system{dq} arguments={dq}snooze{dq} hint-inputId={dq}snoozeTime{dq} content={dq}{dq}/>
  <action activationType={dq}system{dq} arguments={dq}dismiss{dq} content={dq}{dq}/>
  </actions>
</toast>
";

      var tileXml = new Windows.Data.Xml.Dom.XmlDocument();
      tileXml.LoadXml(toastXml);
      var toast = new ToastNotification(tileXml);//.Dump();
      var notify = ToastNotificationManager.CreateToastNotifier(appId);
      notify.Show(toast);
    }

    public bool TryCreateShortcut() {
      String shortcutPath = System.IO.Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
          $@"Microsoft\Windows\Start Menu\Programs\{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.lnk");

      if (File.Exists(shortcutPath)) {
        File.Delete(shortcutPath);
      }

      InstallShortcut(shortcutPath);

      return true;
    }

    public void InstallShortcut(String shortcutPath) {

      // Find the path to the current executable
      String exePath = Process.GetCurrentProcess().MainModule.FileName;
      IShellLinkW newShortcut = (IShellLinkW)new CShellLink();

      // Create a shortcut to the exe
      ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
      ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));

      //Add icon to shortcut
      //if (ICON_LOCATION != string.Empty && File.Exists(ICON_LOCATION))
      //  ShellHelper.ErrorHelper.VerifySucceeded(newShortcut.SetIconLocation(ICON_LOCATION, 0));

      // Open the shortcut property store, set the AppUserModelId property
      IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

      using (PropVariant propVariant = new PropVariant(appId)) {
        ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, propVariant));
        ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
      }

      // Commit the shortcut to disk
      IPersistFile newShortcutSave = (IPersistFile)newShortcut;

      ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
    }
  }

  #region ShellHelper
  //from https://code.msdn.microsoft.com/windowsdesktop/sending-toast-notifications-71e230a2/
  internal enum STGM : long {
    STGM_READ = 0x00000000L,
    STGM_WRITE = 0x00000001L,
    STGM_READWRITE = 0x00000002L,
    STGM_SHARE_DENY_NONE = 0x00000040L,
    STGM_SHARE_DENY_READ = 0x00000030L,
    STGM_SHARE_DENY_WRITE = 0x00000020L,
    STGM_SHARE_EXCLUSIVE = 0x00000010L,
    STGM_PRIORITY = 0x00040000L,
    STGM_CREATE = 0x00001000L,
    STGM_CONVERT = 0x00020000L,
    STGM_FAILIFTHERE = 0x00000000L,
    STGM_DIRECT = 0x00000000L,
    STGM_TRANSACTED = 0x00010000L,
    STGM_NOSCRATCH = 0x00100000L,
    STGM_NOSNAPSHOT = 0x00200000L,
    STGM_SIMPLE = 0x08000000L,
    STGM_DIRECT_SWMR = 0x00400000L,
    STGM_DELETEONRELEASE = 0x04000000L,
  }
  internal static class ShellIIDGuid {
    internal const string IShellLinkW = "000214F9-0000-0000-C000-000000000046";
    internal const string CShellLink = "00021401-0000-0000-C000-000000000046";
    internal const string IPersistFile = "0000010b-0000-0000-C000-000000000046";
    internal const string IPropertyStore = "886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99";
  }

  [ComImport,
  Guid(ShellIIDGuid.IShellLinkW),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IShellLinkW {
    UInt32 GetPath(
        [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
        int cchMaxPath,
        //ref _WIN32_FIND_DATAW pfd,
        IntPtr pfd,
        uint fFlags);
    UInt32 GetIDList(out IntPtr ppidl);
    UInt32 SetIDList(IntPtr pidl);
    UInt32 GetDescription(
        [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
        int cchMaxName);
    UInt32 SetDescription(
        [MarshalAs(UnmanagedType.LPWStr)] string pszName);
    UInt32 GetWorkingDirectory(
        [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir,
        int cchMaxPath
        );
    UInt32 SetWorkingDirectory(
        [MarshalAs(UnmanagedType.LPWStr)] string pszDir);
    UInt32 GetArguments(
        [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs,
        int cchMaxPath);
    UInt32 SetArguments(
        [MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
    UInt32 GetHotKey(out short wHotKey);
    UInt32 SetHotKey(short wHotKey);
    UInt32 GetShowCmd(out uint iShowCmd);
    UInt32 SetShowCmd(uint iShowCmd);
    UInt32 GetIconLocation(
        [Out(), MarshalAs(UnmanagedType.LPWStr)] out StringBuilder pszIconPath,
        int cchIconPath,
        out int iIcon);
    UInt32 SetIconLocation(
        [MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
        int iIcon);
    UInt32 SetRelativePath(
        [MarshalAs(UnmanagedType.LPWStr)] string pszPathRel,
        uint dwReserved);
    UInt32 Resolve(IntPtr hwnd, uint fFlags);
    UInt32 SetPath(
        [MarshalAs(UnmanagedType.LPWStr)] string pszFile);
  }

  [ComImport,
  Guid(ShellIIDGuid.IPersistFile),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IPersistFile {
    UInt32 GetCurFile(
        [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile
    );
    UInt32 IsDirty();
    UInt32 Load(
        [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
        [MarshalAs(UnmanagedType.U4)] STGM dwMode);
    UInt32 Save(
        [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
        bool fRemember);
    UInt32 SaveCompleted(
        [MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
  }
  [ComImport]
  [Guid(ShellIIDGuid.IPropertyStore)]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  interface IPropertyStore {
    UInt32 GetCount([Out] out uint propertyCount);
    UInt32 GetAt([In] uint propertyIndex, out PropertyKey key);
    UInt32 GetValue([In] ref PropertyKey key, [Out] PropVariant pv);
    UInt32 SetValue([In] ref PropertyKey key, [In] PropVariant pv);
    UInt32 Commit();
  }

  [ComImport,
  Guid(ShellIIDGuid.CShellLink),
  ClassInterface(ClassInterfaceType.None)]
  internal class CShellLink { }

  internal static class ErrorHelper {
    public static void VerifySucceeded(UInt32 hresult) {
      if (hresult > 1) {
        throw new Exception("Failed with HRESULT: " + hresult.ToString("X"));
      }
    }
  }
  #endregion //ShellHelper
}//namespace
