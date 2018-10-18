using System;
using System.Windows;

namespace TippingService
{
    public interface IVsTippingService
    {
        void RequestCalloutDisplay(Guid calloutId, string title, string message,
            bool isPermanentlyDismissible, FrameworkElement targetElement, Guid vsCommandGroupId, uint vsCommandId);
    }
}