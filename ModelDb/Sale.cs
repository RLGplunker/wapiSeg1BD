using System;
using System.Collections.Generic;

namespace wapiSeg1BD.ModelDb
{
    public partial class Sale
    {
        public int Id { get; set; }
        public int IdProduct { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal? SalePrice { get; set; }
    }
}
