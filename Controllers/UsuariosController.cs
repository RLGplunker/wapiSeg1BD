using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wapiSeg1BD.Context;
using wapiSeg1BD.Models;
using wapiSeg1BD.DTOS;
using System.Security.Claims;

namespace wapiSeg1BD.Controllers
{
    [Route("usuarios")]
    public class UsuariosController : MiBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UsuariosController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)            
        {
            _context = context;
            _userManager = userManager;
        }
        
        [HttpPost("AsignarRol")] //<--- si no pongo la barra "/" es ruta relativa 
        public async Task<ActionResult> AsignarRolUsuario (EditarRolDTO editarRolDTO)
        {
            var usuario = await _userManager.FindByIdAsync(editarRolDTO.UserId);
            if (usuario == null)
            {
                return NotFound();
            }
            // La forma de añadir un rol al usuario es añadir un nuevo claim
            // [CON AUTENTICACIÓN CLÁSICA]

            await _userManager.AddClaimAsync(usuario,
                new Claim(ClaimTypes.Role, editarRolDTO.RoleName));

            //También añade AddToRoleAsync 
            // [CON AUTENTICACIÓN JWT]
            await _userManager.AddToRoleAsync(usuario, editarRolDTO.RoleName);
            return Ok();
        }

        [HttpPost("RemoverRol")] //<--- si no pongo la barra "/" es ruta relativa 
        public async Task<ActionResult> RemoverUsuarioRol(EditarRolDTO editarRolDTO)
        {
            var usuario = await _userManager.FindByIdAsync(editarRolDTO.UserId);
            if (usuario == null)
            {
                return NotFound();
            }
            // La forma de añadir un rol al usuario es añadir un nuevo claim
            // [CON AUTENTICACIÓN CLÁSICA]

            await _userManager.RemoveClaimAsync(usuario,
                new Claim(ClaimTypes.Role, editarRolDTO.RoleName));

            //También añade AddToRoleAsync 
            // [CON AUTENTICACIÓN JWT]
            await _userManager.RemoveFromRoleAsync(usuario, editarRolDTO.RoleName);
            return Ok();
        }

       
    }
}