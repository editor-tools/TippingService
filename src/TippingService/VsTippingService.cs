using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft;
using Microsoft.VisualStudio.Shell;

namespace TippingService
{
    [Export]
    public class VsTippingService
    {
        readonly IServiceProvider serviceProvider;

        [ImportingConstructor]
        public VsTippingService([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void RequestCalloutDisplay(Guid clientId, Guid calloutId, string title, string message,
            bool isPermanentlyDismissible, UIElement targetElement,
            Guid vsCommandGroupId, uint vsCommandId, object commandOption)
        {
            var tippingService = serviceProvider.GetService(typeof(SVsTippingService));
            Assumes.Present(tippingService);
            var currentMethod = MethodBase.GetCurrentMethod();
            var parameterTypes = currentMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            var method = tippingService.GetType().GetMethod(currentMethod.Name, parameterTypes);
            var arguments = new object[] { clientId, calloutId, title, message, isPermanentlyDismissible, targetElement,
                    vsCommandGroupId, vsCommandId, commandOption };
            method.Invoke(tippingService, arguments);
        }
    }

    [Guid("DCCC6A2B-F300-4DA1-92E1-8BF4A5BCA795")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [TypeIdentifier]
    [ComImport]
    public interface SVsTippingService
    {
    }
}
