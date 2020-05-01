using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wapiSeg1BD.DTOS_Db
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IdCategory { get; set; }
        public decimal Price { get; set; }
        public bool WithStock { get; set; }
        public DateTime? DeletionDate { get; set; }        
        public string Alias { get; set; }
    }
}
