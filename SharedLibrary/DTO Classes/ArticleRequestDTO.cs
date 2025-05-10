using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DTO_Classes
{
    public class ArticlesRequestDTO
    {
        public int BatchSize { get; set; }
        public int Page { get; set; }
        public ICollection<int>? Tags { get; set; }
        public ICollection<int>? Authors { get; set; }
        public int? SortBy { get; set; }
        public int? SortOrder { get; set; }

    }
}
