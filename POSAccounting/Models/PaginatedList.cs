using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{

    public class PaginatedList<T>
    {
        public ObservableCollection<T> Models { get; set; }
       // public List<T> Entities { get; set; }
        public int Count { get; set; }
        public decimal Amount { get; set; }
        public decimal Credit { get; set; }
        public decimal Debt { get; set; }
        //public static PaginatedList<T> CreateAsync<K>(IQueryable<T> source, Expression<Func<T, K>> orderBy,
        //    int pageIndex, int pageSize)
        //{
        //    int count = source.Count();
        //    var items = source.OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        //    //return new PaginatedList<T>(items, count, pageIndex, pageSize);
        //    return new PaginatedList<T>(items, count);
        //}

        ////public PaginatedList(List<T> items, int count)
        ////{
        ////    this.Count = count;
        ////    this.Entities = items;
        ////}

        public PaginatedList()
        {
        }

        //public int PageIndex { get; private set; }
        //public int TotalPages { get; private set; }

        //public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        //{
        //    PageIndex = pageIndex;
        //    TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        //    this.AddRange(items);
        //}

        //public bool HasPreviousPage
        //{
        //    get
        //    {
        //        return (PageIndex > 1);
        //    }
        //}

        //public bool HasNextPage
        //{
        //    get
        //    {
        //        return (PageIndex < TotalPages);
        //    }
        //}
    }

}
