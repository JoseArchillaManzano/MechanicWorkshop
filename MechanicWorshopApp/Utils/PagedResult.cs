using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Utils
{
    public class PagedResult<T>
    {
        public int CurrentPage { get; set; } // Página actual
        public int TotalPages { get; set; } // Total de páginas
        public int PageSize { get; set; } // Tamaño de la página
        public int TotalCount { get; set; } // Total de registros
        public IEnumerable<T> Items { get; set; } // Elementos de la página

        public PagedResult(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }
}