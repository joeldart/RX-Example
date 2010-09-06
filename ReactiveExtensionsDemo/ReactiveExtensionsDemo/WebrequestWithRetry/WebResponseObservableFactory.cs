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
using System.Linq;

namespace ReactiveExtensionsDemo
{
    public class WebResponseObservableFactory: IWebResponseObservableFactory
    {
        public IObservable<WebResponse> GetObservable()
        {
            //jdart is the name of my machine: this is the same as localhost, but it works through fiddler
            var wr = HttpWebRequest.CreateHttp("http://localhost/examplerestservice/service1");
            wr.AllowReadStreamBuffering = true;
            return Observable.FromAsyncPattern<WebResponse>(wr.BeginGetResponse, wr.EndGetResponse)();
        }
    }
}
