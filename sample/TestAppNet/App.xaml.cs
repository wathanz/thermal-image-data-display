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
    internal static DataGeneratorKind SelectedDataGenerator { get; private set; }

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
        var selection = MessageBox.Show(
            "Choose the sample data source.\n\nYes: bundled video\nNo: moving gradient\nCancel: exit",
            "Select data generator",
            MessageBoxButton.YesNoCancel,
            MessageBoxImage.Question);

        if (selection == MessageBoxResult.Cancel) {
            Shutdown();
            return;
        }

        SelectedDataGenerator = selection == MessageBoxResult.Yes
            ? DataGeneratorKind.BundledVideo
            : DataGeneratorKind.Gradient;

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