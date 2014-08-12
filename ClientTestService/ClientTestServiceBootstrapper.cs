using Nancy;
using Nancy.TinyIoc;

namespace ClientTestService
{
    public class ClientTestServiceBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<PortfolioService>().AsSingleton();
        }
    }
}