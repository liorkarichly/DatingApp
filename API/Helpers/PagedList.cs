using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using System;
using System.Threading.Tasks;
using System.Linq;

namespace API.Helpers
{

    public class PagedList<T>: List<T>//Generics, we want to create the paged list
    {
        
        public PagedList(IEnumerable<T> items, int count, int pageNumber
                       , int pageSize)
        {

            CurrentPage = pageNumber;       //10            5 //going to work out that we've got 2 pages from this query.
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);//Ceiling - Returns the smallest integral value that is greater than or equal to the specified double-precision floating-point number.
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);// AddRange allows us to add multiple entities at once rather than the 'Add' method that only allows us to add a single entit

        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source
                                                    , int pageNumber
                                                    , int pageSize)
        {

            var count = await source.CountAsync();
                                                //(Number - 1) * size of page = Like page number 0 * count of page size. take from page the size like 20 
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
            
        }

    }

}