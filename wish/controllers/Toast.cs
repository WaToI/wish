using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace wish {
  public class Toaster {
    public void showToast() {
      var appId = @"{1AC14E77-02E7-4E5D-B744-2EB1AE5198B7}\WindowsPowerShell\v1.0\powershell.exe";
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
  }
}
