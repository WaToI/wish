using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.Devices.Bluetooth.Advertisement;

namespace wish {
  public class BLE : IDisposable {
    public BluetoothLEAdvertisementWatcher Watcher { get; set; }
    public BluetoothLEAdvertisementReceivedEventArgs ReceiveArgs{ get; set; }

    public BLE(bool startWatch=false) {
      if (startWatch) {
        StartWatch();
      }

    }

    public void StartWatch() {
      Watcher = new BluetoothLEAdvertisementWatcher();
      Watcher.ScanningMode = BluetoothLEScanningMode.Active;
      Watcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;
      Watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;
      Watcher.Received += OnAdvertisementReceived;
      Watcher.Start();
    } 

    public void Dispose() {
      Watcher.Received -= OnAdvertisementReceived;
      Watcher.Stop();
      Watcher = null;
    } 

    private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs) {
      ReceiveArgs = eventArgs;
      Debug.WriteLine($@"
Advertisement:
  BT_ADDR: {eventArgs.BluetoothAddress}
  AdvertisementType: {eventArgs.AdvertisementType}
");
    }
  }
}
