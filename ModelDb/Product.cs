using System;
using System.Collections.Generic;

namespace wapiSeg1BD.ModelDb
{
    public partial class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IdCategory { get; set; }
        public decimal Price { get; set; }
        public bool WithStock { get; set; }
        public DateTime? DeletionDate { get; set; }
        public byte[] RowVersion { get; set; }
        public string Alias { get; set; }

        public virtual Category IdCategoryNavigation { get; set; }
    }
}
