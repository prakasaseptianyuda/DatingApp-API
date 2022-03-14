using DatingApp.WebApi.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Extensions
{
    public static class HttpExtension
    {
        public static void AddPagionationHeader(this HttpResponse response, int currentPage,int itemPerPage,int totalItem,int totalPage)
        {
            var paginationHeader = new PaginationHeader(currentPage,itemPerPage,totalItem,totalPage);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            response.Headers.Add("Pagination",JsonSerializer.Serialize(paginationHeader, options));
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
    }
}
