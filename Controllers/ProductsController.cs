using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using wapiSeg1BD.DTOS_Db;
using wapiSeg1BD.ModelDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using wapiSeg1BD.Context;

/// <summary>
/// Ver todas las notas sobre cómo se deben hacer las cosas en
/// D:\___NEW_ORG\LEARN\.NetCore\GETTING_ON\UDEMY_WEB_API\05-ManipulandoRecursos\Notas_01.txt
/// </summary>
namespace wapiSeg1BD.Controllers
{
    [EnableCors("Permitir_Aplic_Mvc_Rosana")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/Products")]
    public class ProductsController : MiBaseController
    {
        private readonly ApplicationDbContext _productsContext;
        private readonly IMapper _mapper;
        public ProductsController(ApplicationDbContext productsContext
            , IMapper mapper)
        {
            _productsContext = productsContext;
            _mapper = mapper;
        }

        [HttpGet("Test")]
        public ActionResult<String> Test()
        {            
            return Ok("Test ok");
        }

        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<ProductListDTO>>> GetListaProducts()
        {
            var products = await  _productsContext.Product.ToListAsync(); //<-- Nota para que aparezca el método ToListAsync() es el ns: using Microsoft.EntityFrameworkCore;
            var productsDTO = _mapper.Map<List<ProductListDTO>>(products);
            return Ok(productsDTO);
        }

        [HttpGet("Get/{id}", Name="GetProduct")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _productsContext.Product.Where(x=>x.Id == id).FirstOrDefaultAsync();
            if (product == null) { return NotFound();}
            var productDTO =_mapper.Map<ProductDTO>(product);

            return Ok(productDTO);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "admin")]
        [HttpPost("Crear")]        
        public async Task<ActionResult<ProductDTO>> Post(ProductCreateDTO productCreateDTO)
        {
            var productDb = _mapper.Map<Product>(productCreateDTO);
            _productsContext.Add<Product>(productDb);
            await _productsContext.SaveChangesAsync();

            return new CreatedAtRouteResult("GetProduct", new { id = productDb.Id}, productCreateDTO);

            //------------------------------------------------------
            // Petición: https://localhost:44328/Products/Crear 
            //------------------------------------------------------
            // Body: 
            //   {
            //   "Name": "Guantes",
            //   "IdCategory": 1,
            //   "Price": 2.20,
            //   "WithStock": true,
            //   "DeletionDate": null,
            //   "Alias":"Guantes"
            //   }
            //------------------------------------------------------
            // Response
            //------------------------------------------------------
            // la cabecera: Location = https://localhost:44328/Products/Get/19
            // Body
                //{
                //    "name": "Guantes",
                //    "idCategory": 1,
                //    "price": 2.20,
                //    "withStock": true,
                //    "deletionDate": null,
                //    "alias": "Guantes"
                //}
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [HttpPut("Actualizar/{id}")]
        public async Task<ActionResult<ProductDTO>> Put(int id, ProductDTO productDTO)
        {
            var product =  await _productsContext.Product.Where(x=>x.Id == id).AsNoTracking().FirstAsync();

            if (product == null) { return NotFound();}

            try
            {
                var productDb = _mapper.Map<Product>(productDTO);
                productDb.Id = id;
                productDb.RowVersion = product.RowVersion; //<-- Si otro usuario modifica el rowVersión => dará error de concurrencia 
                _productsContext.Update<Product>(productDb);
                await _productsContext.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {                
               return Conflict($"Error de concurrencia. Id {id}"); //statuscode = 409    
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [HttpPatch("ActualizarParcial/{id}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<ProductPathDTO> jsonPatchDocument)
        {
            if (jsonPatchDocument == null) { return BadRequest();};

            var productBd = await _productsContext.Product.AsNoTracking().Where(x=>x.Id == id).FirstOrDefaultAsync();
            if (productBd == null) { return NotFound(); }

            var productPathDTO = _mapper.Map<ProductPathDTO>(productBd);

            jsonPatchDocument.ApplyTo(productPathDTO, ModelState);
            
            if (!TryValidateModel(productPathDTO)) {return BadRequest(); };

            _mapper.Map(productPathDTO, productBd); //<-- La clave está aquí: Lo que está en productPathDTO se lleva a productBd

            _productsContext.Entry(productBd).State= EntityState.Modified;
            await _productsContext.SaveChangesAsync();

            return Ok();

            //------------------------------------------------------
            // Petición: https://localhost:44328/Products/ActualizarParcial/19
            // Me da error xq se necesita NewtonSoft como serializer. En cuanto que lo he hecho ok  
            //------------------------------------------------------
            // Body: 
                //[
	               // {
		              //  "op": "replace",
		              //  "path": "/Name",
		              //  "value": "Guantes verdes"

                //    },
	               // {
		              //  "op": "replace",
		              //  "path": "/Price",
		              //  "value": 1.25
	               // },
	               // {
		              //  "op": "replace",
		              //  "path": "/WithStock",
		              //  "value": false
	               // }	
                //]
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [HttpDelete("Eliminar/{id}")]
        public async Task<ActionResult<ProductDTO>> Delete(int id)
        {
            var product = await _productsContext.Product.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (product == null) { return NotFound(); }

            _productsContext.Remove<Product>(product);
            await _productsContext.SaveChangesAsync();

            return Ok();            
        }


    }
}