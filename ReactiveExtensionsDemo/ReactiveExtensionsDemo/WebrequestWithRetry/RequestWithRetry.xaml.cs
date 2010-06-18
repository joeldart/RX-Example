using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Concurrency;

namespace ReactiveExtensionsDemo
{
    public partial class RequestWithRetry : Page
    {
        public RequestWithRetry()
        {
            InitializeComponent();
            var vm = new RequestWithRetryViewModel();
            this.DataContext = vm;
            vm.GetData(Scheduler.Dispatcher);//initialize and call out to get data
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

    }
}
