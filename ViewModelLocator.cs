﻿namespace Melista
{
    public class ViewModelLocator
    {
        private static ServiceProvider _provider;
        public static IConfiguration Configuration { get; private set; }
        public static void Init()
        {
            var services = new ServiceCollection();


            services.AddTransient<MainViewModel>();
            services.AddTransient<StartPageView>();

            services.AddSingleton<PageService>();

            _provider = services.BuildServiceProvider();
            foreach (var service in services)
            {
                _provider.GetRequiredService(service.ServiceType);
            }
        }
        public MainViewModel MainViewModel => _provider.GetRequiredService<MainViewModel>();
        public StartPageViewModel StartPageViewModel => _provider.GetRequiredService<StartPageViewModel>();
    }
}
