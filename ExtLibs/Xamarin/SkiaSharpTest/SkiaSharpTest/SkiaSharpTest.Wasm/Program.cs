﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Foundation;
using Uno.Logging;
using Windows.UI.Popups;

namespace SkiaSharpTest.Wasm
{
    public class Program
    {
        private static App _app;

        static int Main(string[] args)
        {
            ConfigureFilters(LogExtensionPoint.AmbientLoggerFactory);

            Console.WriteLine("Main 1");
            new System.Drawing.android.android();
            var test = System.Drawing.FillMode.Alternate;
            var test2 = System.Drawing.Color.White;

            //new MissionPlanner.Drawing.Common.Common();
            Console.WriteLine("Main 2");
            Windows.UI.Xaml.Application.Start(_ => _app = new App());
            Console.WriteLine("Main 3");
            return 0;
        }
        static void ConfigureFilters(ILoggerFactory factory)
        {
            factory
                .WithFilter(new FilterLoggerSettings
                    {
                        { "Uno", LogLevel.Warning },
                        { "Windows", LogLevel.Warning },
                        { "nVentive.Umbrella", LogLevel.Warning },
                        { "SkiaSharp", LogLevel.Warning },
						// { "ShiaTSInteropMarshaller", LogLevel.Warning },

						// View Navigation related messages
						// { "nVentive.Umbrella.Views.Navigation", LogLevel.Debug },
						// { "nVentive.Umbrella.Presentation.Light.ViewModelRegistry", LogLevel.Debug },
						   
						// All presentation related messages
						// { "nVentive.Umbrella.Presentation.Light", LogLevel.Debug },
						   
						// ViewModel related messages
						// { "nVentive.Umbrella.Presentation.Light.ViewModelBase", LogLevel.Debug },
						   
						// Dynamic properties related messages
						// { "nVentive.Umbrella.Presentation.Light.DynamicProperties", LogLevel.Debug },
						   
						// AVVM/AVP Related messages
						// { "nVentive.Umbrella.Presentation.Light.Extensions.AsyncValueViewModel", LogLevel.Debug },
						// { "nVentive.Umbrella.Views.Controls.AsyncValuePresenter", LogLevel.Debug },
						// { "nVentive.Umbrella.Views.Controls.IfDataContext", LogLevel.Debug },
						   
						// Layouter specific messages
						//{ "Windows.UI.Xaml.Controls", LogLevel.Debug },
						//{ "Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug },
						//{ "Windows.UI.Xaml.Controls.Panel", LogLevel.Debug },
						   
						// Binding related messages
						// { "Windows.UI.Xaml.Data", LogLevel.Debug },
						// { "Windows.UI.Xaml.Data", LogLevel.Debug },
						   
						//  Binder memory references tracking
						// { "ReferenceHolder", LogLevel.Debug },
						   
						// Web Queries related messages
						// gManager.SetLoggerLevel("nVentive.Umbrella.Web", LogLevel.Debug },
						   
						// Location service messages
						// { "nVentive.Umbrella.Services.Location", LogLevel.Debug },
						   
						// Map Component messages
						// { "nVentive.Umbrella.Components.Maps", LogLevel.Debug },
						   
						//  ApplicationViewState
						//  { "nVentive.Umbrella.Views.Services.ApplicationViewState.ApplicationViewStateService", LogLevel.Debug },
					}
                )
                .AddConsole(LogLevel.Debug);
        }
    }
}
