using Biblioteca.Usuarios.DTO;
using Biblioteca.Usuarios.Services;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Usuarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private IServUsuario _servUsuario;

        public UsuariosController()
        {
            _servUsuario = new ServUsuario();
        }

        [HttpGet]
        public IActionResult ListarTodos()
        {
            try
            {
                var usuarios = _servUsuario.ListarTodos();
                return Ok(usuarios);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult BuscarPorId(int id)
        {
            try
            {
                var usuarioDto = _servUsuario.BuscarPorId(id);
                return Ok(usuarioDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IActionResult Criar([FromBody] CriarUsuarioDTO dto)
        {
            try
            {
                var usuarioDto = _servUsuario.Criar(dto);
                return Ok(usuarioDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("{id}")]
        [HttpPut]
        public IActionResult Atualizar(int id, [FromBody] CriarUsuarioDTO dto)
        {
            try
            {
                var usuarioDto = _servUsuario.Atualizar(id, dto);
                return Ok(usuarioDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public IActionResult Deletar(int id)
        {
            try
            {
                _servUsuario.Deletar(id);
                return Ok("Usuário removido com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Endpoint chamado pelo microsserviço de Empréstimos para bloquear usuário com atraso
        [Route("{id}/bloquear")]
        [HttpPatch]
        public IActionResult BloquearUsuario(int id)
        {
            try
            {
                var usuarioDto = _servUsuario.BloquearUsuario(id);
                return Ok(usuarioDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
