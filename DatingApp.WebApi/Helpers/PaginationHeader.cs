using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Helpers
{
    public class PaginationHeader
    {
        public PaginationHeader(int currentPage, int itemPerPage, int totalItem, int totalPage)
        {
            CurrentPage = currentPage;
            ItemPerPage = itemPerPage;
            TotalItem = totalItem;
            TotalPage = totalPage;
        }

        public int CurrentPage { get; set; }
        public int ItemPerPage { get; set; }
        public int TotalItem { get; set; }
        public int TotalPage { get; set; }
    }
}
