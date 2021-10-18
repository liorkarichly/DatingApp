using Microsoft.AspNetCore.Http;
using API.Helpers;
using System.Text.Json;

namespace API.Extensions
{
    public static class HttpExtensions
    {

        public static void AddPaginationHeader(this HttpResponse response
                                              , int currentPage, int itemsPerPage
                                              , int totalItems, int totalPages)
        {

                var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

                var options = new JsonSerializerOptions {

                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader,options));
                response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
/*And then what we need to do, because we're adding a custom header, we need to add a cause header into this to make this header available.
And then we specify the name of the header that we're exposing and the name is Pagination.*/
                

        }
        
    }
}