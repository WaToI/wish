using NUnit.Framework;
using wish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace wish.Tests {
  [TestFixture()]
  public class BLETests {
    DirectoryInfo cDi = new DirectoryInfo("./");
    [OneTimeSetUp]
    public void RunBeforeAnyTests() {
      Environment.CurrentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
      cDi = new DirectoryInfo(Environment.CurrentDirectory);
      Console.WriteLine($@"
setupTests
  pwd: {Environment.CurrentDirectory}
");
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests() {
    }

    [Test()]
    public void BLETest() {
      using (var t = new BLE(startWatch:true)) {
      }
      //Assert.Fail();
    }

    [Test()]
    public void DisposeTest() {
      //Assert.Fail();
    }
  }
}