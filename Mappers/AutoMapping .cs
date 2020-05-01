using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using wapiSeg1BD.ModelDb;
using wapiSeg1BD.DTOS_Db;


namespace wapiSeg1BD.Mappers
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Product, ProductListDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<ProductCreateDTO, Product>().ReverseMap();
            CreateMap<Product, ProductPathDTO>().ReverseMap();

        }
    }
}
