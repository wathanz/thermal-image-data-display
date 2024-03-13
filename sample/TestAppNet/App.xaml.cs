using IntensityMapping.Core.Interface;
using IntensityValueGeneration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using WpfIntensityView.Interface;
using WpfIntensityView.Model;

namespace DemoAppNet;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    public static IHost AppHost { get; private set; }

    public App() {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) => {
                services.AddSingleton<IColorMapping, ColorMapping>();
                services.AddSingleton<IValueGenerationConfig, ValueGenerationConfig>();
                services.AddTransient<IIntensityDataMapBitmap, IntensityDataMapBitmap>();
                services.AddSingleton<MainWindow>();
            }).Build();
    }

    protected override async void OnStartup(StartupEventArgs e) {
        await AppHost.StartAsync();
        var startupWindow = AppHost.Services.GetRequiredService<MainWindow>();
        startupWindow.Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e) {
        await AppHost.StopAsync();
        base.OnExit(e);
    }
}