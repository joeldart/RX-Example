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
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Concurrency;
using System.Diagnostics;
using System.Threading;

namespace ReactiveExtensionsDemo
{
    public class RequestWithRetryViewModel
    {
        public IWebResponseObservableFactory ResponseObservableFactory { get; set; }
        private IObservable<WebResponse> GetObs()
        {
            return ResponseObservableFactory.GetObservable();
            //TODO: Show debugging code 
        }

        public RequestWithRetryViewModel()
        {
            Names = new ObservableCollection<string>();//I INCLUDED THIS ONLY TO CONFUSE YOU!!!
            ResponseObservableFactory = new WebResponseObservableFactory();
        }

        public void GetData(IScheduler observationScheduler)
        {
            GetObs()
                .Catch((WebException ex1) =>
                {
                    System.Threading.Thread.Sleep(2000);
                    return GetObs()
                        .Catch((WebException ex2) =>
                        {
                            System.Threading.Thread.Sleep(5000);
                            return GetObs();
                        });
                })
            .ObserveOn(observationScheduler)
            //to aid performance, try subscribing with a different scheduler
            .Subscribe(response =>
            {
                var xdoc = XDocument.Load(response.GetResponseStream());
                foreach (var e in xdoc.Root.Descendants())
                {
                    Names.Add(e.Value);
                }
            });
        }

        public ObservableCollection<string> Names { get; set; }
    }
}
