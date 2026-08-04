using Microsoft.Extensions.DependencyInjection;

namespace TripleTwenty
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = default!;

        public App(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}