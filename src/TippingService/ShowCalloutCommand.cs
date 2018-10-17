using System;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace TippingService
{
    internal sealed class ShowCalloutCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("ad85be49-6d55-45a2-a464-3d23a0909db9");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowCalloutCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private ShowCalloutCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ShowCalloutCommand Instance
        {
            get;
            private set;
        }

        IServiceProvider ServiceProvider => package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in ShowCalloutCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new ShowCalloutCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ShowCallout(ServiceProvider);
        }

        [STAThread]
        static void ShowCallout(IServiceProvider serviceProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var tippingService = new VsTippingService(serviceProvider);

            var clientId = new Guid("D5D3B674-05BB-4942-B8EC-C3D13B5BD6EE");
            var calloutId = new Guid("63b813cd-9292-4c0f-aa49-ebd888b791f8");
            var statusBar = FindSccStatusBarHost();

            var dte = serviceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            Assumes.Present(dte);
            var command = dte.Commands.Item("View.URL");
            var commandGuid = new Guid(command.Guid);
            var commandID = (uint)command.ID;
            var commandOptions = "http://testdriven.net";

            tippingService.RequestCalloutDisplay(clientId, calloutId, "foo", "bar", true, statusBar,
                commandGuid, commandID, commandOptions);
        }

        static ContentControl FindSccStatusBarHost()
        {
            var StatusBarPartName = "PART_SccStatusBarHost";
            var mainWindow = Application.Current.MainWindow;
            return mainWindow?.Template?.FindName(StatusBarPartName, mainWindow) as ContentControl;
        }
    }
}
