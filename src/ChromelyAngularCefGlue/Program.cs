// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Chromely Projects">
//   Copyright (c) 2017-2018 Chromely Projects
// </copyright>
// <license>
//      See the LICENSE.md file in the project root for more information.
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace ChromelyAngularCefGlue
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Chromely.CefGlue.Winapi;
    using Chromely.CefGlue.Winapi.ChromeHost;
    using Chromely.Core;
    using Chromely.Core.Infrastructure;
    using WinApi.Windows;

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                HostHelpers.SetupDefaultExceptionHandlers();

                string startUrl = "local://dist/index.html";

                ChromelyConfiguration config = ChromelyConfiguration
                                              .Create()
                                              .WithAppArgs(args)
                                              .WithHostSize(1200, 900)
                                              .WithLogFile("logs\\chromely.cef_new.log")
                                              .WithStartUrl(startUrl)
                                              .WithLogSeverity(LogSeverity.Info)
                                              .UseDefaultLogger("logs\\chromely_new.log")
                                              .UseDefaultResourceSchemeHandler("local", string.Empty)
                                              .UseDefaultHttpSchemeHandler("http", "chromely.com");

                var factory = WinapiHostFactory.Init("chromely.ico");
                using (var window = factory.CreateWindow(
                    () => new CefGlueBrowserHost(config),
                    "chromely",
                    constructionParams: new FrameWindowConstructionParams()))
                {
                    // Register external url schems
                    window.RegisterUrlScheme(new UrlScheme("https://github.com/mattkol/Chromely", true));

                    // window.RegisterUrlScheme(new UrlScheme("https://google.com", true));

                    /*
                     * Register service assemblies
                     * Uncomment relevant part to register assemblies
                     */

                    // 1. Register current/local assembly:
                    window.RegisterServiceAssembly(Assembly.GetExecutingAssembly());

                    // 2. Register external assembly with file name:
                    // string serviceAssemblyFile = @"C:\ChromelyDlls\Chromely.Service.Demo.dll";
                    // window.RegisterServiceAssembly(serviceAssemblyFile);

                    // 3. Register external assemblies with list of filenames:
                    // string serviceAssemblyFile1 = @"C:\ChromelyDlls\Chromely.Service.Demo.dll";
                    // List<string> filenames = new List<string>();
                    // filenames.Add(serviceAssemblyFile1);
                    // window.RegisterServiceAssemblies(filenames);

                    // 4. Register external assemblies directory:
                    string serviceAssembliesFolder = @"C:\ChromelyDlls";
                    window.RegisterServiceAssemblies(serviceAssembliesFolder);

                    // Scan assemblies for Controller routes 
                    window.ScanAssemblies();

                    window.SetSize(config.HostWidth, config.HostHeight);
                    window.CenterToScreen();
                    window.Show();
                    return new EventLoop().Run(window);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }

            return 0;
        }
    }
}

