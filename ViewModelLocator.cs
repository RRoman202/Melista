namespace Melista
{
    public class ViewModelLocator
    {
        private static ServiceProvider _provider;
        public static void Init()
        {
            var services = new ServiceCollection();


            services.AddTransient<MainViewModel>();
            services.AddTransient<StartPageViewModel>();
            services.AddTransient<VideoPlayerViewModel>();

            services.AddSingleton<PageService>();

            _provider = services.BuildServiceProvider();
            foreach (var service in services)
            {
                _provider.GetRequiredService(service.ServiceType);
            }
        }
        public MainViewModel MainViewModel => _provider.GetRequiredService<MainViewModel>();
        public StartPageViewModel StartPageViewModel => _provider.GetRequiredService<StartPageViewModel>();
        public VideoPlayerViewModel VideoPlayerViewModel => _provider.GetRequiredService<VideoPlayerViewModel>();
    }
}
