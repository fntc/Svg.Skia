﻿using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Avalonia.Svg.Skia;

namespace TestApp
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp()
        {
            GC.KeepAlive(typeof(SvgImageExtension).Assembly);
            GC.KeepAlive(typeof(Avalonia.Svg.Skia.Svg).Assembly);

            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .With(new X11PlatformOptions
                {
                })
                .LogToTrace()
                .UseSkia()
                .UseReactiveUI();
        }
    }
}
