using System;
using System.Windows;
using System.Windows.Controls;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using TippingService;
using Xunit;

public class TippingServiceIntegrationTests
{
    public class TheRequestCalloutDisplayMethod
    {
        [VsFact(UIThread = true)]
        public void Does_Not_Throw()
        {
            var clientId = new Guid("D5D3B674-05BB-4942-B8EC-C3D13B5BD6EE");
            var calloutId = new Guid("63b813cd-9292-4c0f-aa49-ebd888b791f8");
            var title = "Hello";
            var message = "Hello, World!";
            var isDismissable = true;
            var statusBar = FindSccStatusBarHost();
            var serviceProvider = ServiceProvider.GlobalProvider;
            var dte = serviceProvider.GetService(typeof(DTE)) as DTE;
            Assumes.Present(dte);
            var command = dte.Commands.Item("View.URL");
            var commandGuid = new Guid(command.Guid);
            var commandID = (uint)command.ID;
            var target = new VsTippingService(serviceProvider);

            var ex = Record.Exception(() =>
            {
                target.RequestCalloutDisplay(clientId, calloutId, title, message,
                    isDismissable, statusBar, commandGuid, commandID);
            });

            Assert.Null(ex);
        }

        static ContentControl FindSccStatusBarHost()
        {
            var StatusBarPartName = "PART_SccStatusBarHost";
            var mainWindow = Application.Current.MainWindow;
            return mainWindow?.Template?.FindName(StatusBarPartName, mainWindow) as ContentControl;
        }
    }
}
