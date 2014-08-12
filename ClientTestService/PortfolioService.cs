using System;
using System.Collections.Generic;
using System.Linq;

namespace ClientTestService
{
    public class Link
    {
        public Link(string rel, string href)
        {
            this.rel = rel;
            this.href = href;
        }

        public string rel { get; set; }
        public string href { get; set; }
    }

    public class Portfolio
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public decimal Balance { get; set; }
        public Link[] Links { get; set; }
    }

    public class PortfolioChanges
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public decimal Balance { get; set; }

        public PortfolioChanges CheckIsValid()
        {
            if (string.IsNullOrEmpty(Name))
                throw new PortfolioInvalidException("Name must be supplied");
            return this;
        }
    }

    public class PortfolioInvalidException : Exception
    {
        public PortfolioInvalidException(string msg) : base(msg)
        {}
    }

    public class PortfolioService
    {
        private Dictionary<int, Portfolio> portfolios;
        private readonly Random rnd = new Random();

        public PortfolioService()
        {
            portfolios = Enumerable.Range(1, 100).Select(CreatePortfolio).ToDictionary(p => p.Id, p => p);
        }

        private Portfolio CreatePortfolio(int id)
        {
            return new Portfolio
            {
                Id = id,
                Name = "Portfolio " + id,
                Location = "London",
                Balance = rnd.Next(-10000, 10000)/100m,
                Links = new[] {CreateSelfLink(id)}
            };
        }

        private static Link CreateSelfLink(int id)
        {
            return new Link("self", "/Portfolio/"+id);
        }

        public Portfolio[] ListPortfolios(int page=0, int size=0)
        {
            IEnumerable<Portfolio> ptfs = portfolios.Values.OrderBy(p=>p.Id);
            if (page > 0 && size > 0)
                ptfs = ptfs.Skip((page - 1)*size).Take(size);
            return ptfs.ToArray();
        }

        public Portfolio CreatePortfolio(PortfolioChanges newValues)
        {
            var newPtf = ApplyChanges(CreatePortfolio(portfolios.Keys.Max() + 1), newValues);
            portfolios[newPtf.Id] = newPtf;
            return newPtf;
        }

        public Portfolio GetPortfolio(int id)
        {
            return portfolios[id];
        }

        public void UpdatePortfolio(int id, PortfolioChanges changes)
        {
            portfolios[id] = ApplyChanges(portfolios[id], changes);
        }

        public void DeletePortfolio(int id)
        {
            portfolios.Remove(id);
        }

        private static Portfolio ApplyChanges(Portfolio ptf, PortfolioChanges changes)
        {
            ptf.Location = changes.Location;
            ptf.Name = changes.Name;
            ptf.Balance = changes.Balance;
            return ptf;
        }
    }
}