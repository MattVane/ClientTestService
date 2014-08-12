using System.Collections.Generic;
using Nancy;
using Nancy.ModelBinding;

namespace ClientTestService
{
    public class PortfolioModule : NancyModule
    {
        private readonly PortfolioService service;

        public PortfolioModule(PortfolioService service)
        {
            this.service = service;

            Get["/Portfolios"] = _ =>
            {
                var page = Request.Query.page.HasValue ? (int)Request.Query.page : 0;
                var size = Request.Query.size.HasValue ? (int)Request.Query.size : 0;
                return Negotiate.WithStatusCode(HttpStatusCode.OK).WithModel(new
                {
                    Portfolios = service.ListPortfolios(page, size),
                    Links = new[] {new Link("add", "/Portfolios/new")}
                });
            };

            Post["/Portfolios/new"] = _ =>
            {
                try
                {
                    return Negotiate.WithStatusCode(HttpStatusCode.OK)
                        .WithModel(new {Id = service.CreatePortfolio(this.Bind<PortfolioChanges>().CheckIsValid()).Id});

                }
                catch (PortfolioInvalidException e)
                {
                    return Negotiate.WithStatusCode(HttpStatusCode.NotAcceptable).WithModel(new {Failure = e.Message});
                }
            };

            Get["/Portfolio/{id}"] = parameters =>
            {
                try
                {
                    return
                        Negotiate.WithStatusCode(HttpStatusCode.OK).WithModel(service.GetPortfolio((int) parameters.id));
                }
                catch (KeyNotFoundException)
                {
                    return HttpStatusCode.NotFound;
                }
            };

            Put["Portfolio/{id}"] = parameters =>
            {
                try
                {
                    service.UpdatePortfolio((int) parameters.id, this.Bind<PortfolioChanges>());
                    return HttpStatusCode.OK;
                }
                catch (PortfolioInvalidException e)
                {
                    return Negotiate.WithStatusCode(HttpStatusCode.NotAcceptable).WithModel(new {Failure = e.Message});
                }
                catch (KeyNotFoundException)
                {
                    return HttpStatusCode.NotFound;
                }
            };

            Delete["/Portfolio/{id}"] = parameters =>
            {
                try
                {
                    service.DeletePortfolio((int)parameters.id);
                    return HttpStatusCode.OK;
                }
                catch (KeyNotFoundException)
                {
                    return HttpStatusCode.NotFound;
                }
            };
        }
    }
}