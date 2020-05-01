using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wapiSeg1BD.DTOS_Db
{
    public class ProductPathDTO
    {
        public string Name { get; set; }        
        public decimal Price { get; set; }
        public bool WithStock { get; set; }
    }
}
