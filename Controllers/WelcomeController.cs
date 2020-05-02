using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using wapiSeg1BD.Interfaces;
using wapiSeg1BD.Services;
using wapiSeg1BD.Models;
using Newtonsoft.Json.Linq;

namespace wapiSeg1BD.Controllers
{
    //[EnableCors("Permitir_apiRequestIo")]
    public class WelcomeController : MiBaseController
    {
        private readonly IDataProtector _dataProtector;
        private readonly IHashService _hashService;
        public WelcomeController(IDataProtectionProvider dataProteccionProvider,
            IHashService hashService)
        {
            _dataProtector = dataProteccionProvider.CreateProtector("valor_unico_y_quizas_secreto");
            _hashService = hashService;
        }

        [Route("/Welcome")]
        [HttpGet]
        public JsonResult Index()
        {
            return new JsonResult("Welcome. How are you?");
        }

        #region Sobre auten. JWT

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [Route("/Conectar")]
        [HttpGet]
        public JsonResult GetInfo()
        {
            return new JsonResult("Has conectado ok con tu token");
        }
        #endregion
        
        #region Sobre CORS

        [EnableCors("Permitir_apiRequestIo")]
        [Route("/TestCors")]
        [HttpPost]
        public ActionResult<TestCors> Get(TestCors testCors)
        {
            var testCorsOut = new TestCors() { Nombre = $"{testCors.Nombre}_out", Edad = 100 };
            return Ok(testCorsOut);
        }
        #endregion
                
        #region Sobre CIFRAR

        [HttpGet]
        [Route("/Cifrar/{textoPlano}")]
        public JsonResult TestCifrar(string textoPlano)
        {
            var textoCifrado = _dataProtector.Protect(textoPlano);
            return new JsonResult($"Texto cifrado: {textoCifrado}");
        }

        [HttpGet]
        [Route("/Descifrar/{textoCifrado}")]
        public JsonResult TestDesCifrar(string textoCifrado)
        {
            try
            {
                var textoPlano = _dataProtector.Unprotect(textoCifrado);
                return new JsonResult($"Texto plano: {textoPlano}");
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpGet]
        [Route("/CifrarTimeLimited/{textoPlano}/{numSegundos}")]
        public JsonResult TestCifrarTimeLimited(string textoPlano, int numSegundos)
        {
            var protectorLimitadoPorTiempo = _dataProtector.ToTimeLimitedDataProtector();

            var textoCifrado = protectorLimitadoPorTiempo.Protect(textoPlano, TimeSpan.FromSeconds(numSegundos));

            Thread.Sleep(numSegundos + 1);
            return new JsonResult($"Texto  Cifrado con límite desfrado en {numSegundos} segundos: {textoCifrado}");
        }

        [HttpGet]
        [Route("/DesCifrarTimeLimited/{textoCifrado}")]
        public JsonResult TestDesCifrarTimeLimited(string textoCifrado)
        {
            try
            {
                var protectorLimitadoPorTiempo = _dataProtector.ToTimeLimitedDataProtector();
                var textoPlano = protectorLimitadoPorTiempo.Unprotect(textoCifrado);
                return new JsonResult($"Texto plano con limite tiempo desfrado es : {textoPlano}");
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }


        }
        #endregion

        #region Sobre HASH

        [Route("/Hash/{textoPlano}")]
        [HttpGet]
        public ActionResult GetHash(string textoPlano)
        {
            var hashResult = _hashService.Hash(textoPlano);
            var hashResult2 = _hashService.Hash(textoPlano);
            return Ok(new {TextoPlano=textoPlano, Hash=hashResult.Hash, Salt=hashResult.Salt, Hash2=hashResult2.Hash, Salt2=hashResult2.Salt });
        }

        [Route("/HashForPassword/{textoPlano}")]
        [HttpGet]
        public ActionResult GetHashForPassword(string textoPlano)
        {
            var hashResult = _hashService.Hash(textoPlano);
            var salt = hashResult.Salt;

            var hashResult2 = _hashService.Hash(textoPlano,salt);
            return Ok(new { TextoPlano = textoPlano, Hash = hashResult.Hash, Salt = hashResult.Salt, Hash2 = hashResult2.Hash, Salt2 = hashResult2.Salt });
        }


        #endregion



    }
}