﻿using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using Task = System.Threading.Tasks.Task;

namespace EmuliciousDebuggerPackage.Debugger.VisualStudio
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    // [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [PackageRegistration(UseManagedResourcesOnly = false)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(EmuliciousPackage.PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class EmuliciousPackage : Package
    {
        public static string DebugAdapterPath { get; set; }

        /// <summary>
        /// EmuliciousPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "efed050d-270a-4a5a-a28c-d008bda32b8e";

        /// <summary>
        /// Initializes a new instance of the <see cref="EmuliciousPackage"/> class.
        /// </summary>
        public EmuliciousPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            // Attempt to query the registry for info.
            try
            {
                AppendLogText("Application Root: " + ApplicationRegistryRoot.Name);
                var regInfo =
                    ApplicationRegistryRoot.OpenSubKey(@"AD7Metrics\Engine\{BE99C8E2-969A-450C-8FAB-73BECCC53DF4}");

                if (regInfo != null)
                {
                    AppendLogText("SubKey: " + regInfo.Name);
                    DebugAdapterPath = regInfo.GetValue("Adapter").ToString();

                    AppendLogText("Adapter: " + DebugAdapterPath);
                }
            }
            catch (Exception err)
            {

                AppendLogText("Applciation Path Exception:\n" + err.ToString());
            }

            // Attempt to query the registry for info.
            if (string.IsNullOrEmpty(DebugAdapterPath))
            {
                try
                {
                    AppendLogText("User Root: " + UserRegistryRoot.Name);
                    var regInfo =
                        UserRegistryRoot.OpenSubKey(@"AD7Metrics\Engine\{BE99C8E2-969A-450C-8FAB-73BECCC53DF4}");

                    if (regInfo != null)
                    {
                        AppendLogText("SubKey: " + regInfo.Name);
                        DebugAdapterPath = regInfo.GetValue("Adapter").ToString();

                        AppendLogText("Adapter: " + DebugAdapterPath);
                    }
                }
                catch (Exception err)
                {
                    AppendLogText("User Path Exception:\n" + err.ToString());
                }
            }
        }

        private void AppendLogText(string text)
        {
            File.AppendAllText(@"C:\Development\Development\Projects\GBDKProjects\GBDKEngine\Debug\Package.log", text + "\n");
        }
    }
}
