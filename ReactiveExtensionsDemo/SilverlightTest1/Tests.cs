using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveExtensionsDemo;
using System.Linq;
using System.IO;
using System.Concurrency;

namespace SilverlightTest1
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var vm = new RequestWithRetryViewModel();
            var mock = new MockWebRequestObservableFactory();
            vm.ResponseObservableFactory = mock;
            vm.GetData(Scheduler.CurrentThread);
            while (!mock.completed)
            {
                System.Threading.Thread.Sleep(100);
            }
            Assert.AreEqual(4, vm.Names.Count, "There should be 4 names in the list!"); 
        }

        public class MockWebRequestObservableFactory : IWebResponseObservableFactory
        {
            private int counter = 0;
            public bool completed = false;
            public IObservable<WebResponse> GetObservable()
            {
                counter++;
                if (counter < 3)
                    return Observable.Create<WebResponse>(observable =>
                    {
                        observable.OnError(new WebException());
                        return () => { };
                    });
                else
                    return Observable.Create<WebResponse>(observable =>
                    {
                        observable.OnNext(new MyResponse());
                        observable.OnCompleted();
                        completed = true;
                        return () => { };
                    });

            }
        }
        private class MyResponse : WebResponse
        {
            public override Stream GetResponseStream()
            {
                var text = "<ArrayOfstring xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><string>John</string><string>Ellen</string><string>Tom</string><string>Olivia</string></ArrayOfstring>";
                return new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(text));
            }

            public override void Close()
            {
                throw new NotImplementedException();
            }

            public override long ContentLength
            {
                get { throw new NotImplementedException(); }
            }

            public override string ContentType
            {
                get { throw new NotImplementedException(); }
            }

            public override Uri ResponseUri
            {
                get { throw new NotImplementedException(); }
            }
        }

    }
}