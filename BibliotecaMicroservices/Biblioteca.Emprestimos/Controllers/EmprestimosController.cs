using Biblioteca.Emprestimos.DTO;
using Biblioteca.Emprestimos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Emprestimos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmprestimosController : Controller
    {
        private IServEmprestimo _servEmprestimo;

        public EmprestimosController(IConfiguration configuration)
        {
            var usuariosUrl = configuration["Microservices:UsuariosUrl"]!;
            var livrosUrl = configuration["Microservices:LivrosUrl"]!;
            _servEmprestimo = new ServEmprestimo(usuariosUrl, livrosUrl);
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodos()
        {
            try
            {
                var emprestimos = await _servEmprestimo.ListarTodos();
                return Ok(emprestimos);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            try
            {
                var emprestimoDto = await _servEmprestimo.BuscarPorId(id);
                return Ok(emprestimoDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("usuario/{usuarioId}")]
        [HttpGet]
        public async Task<IActionResult> ListarPorUsuario(int usuarioId)
        {
            try
            {
                var emprestimos = await _servEmprestimo.ListarPorUsuario(usuarioId);
                return Ok(emprestimos);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarEmprestimoDTO dto)
        {
            try
            {
                var emprestimoDto = await _servEmprestimo.Criar(dto);
                return Ok(emprestimoDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("{id}/devolver")]
        [HttpPatch]
        public async Task<IActionResult> Devolver(int id)
        {
            try
            {
                var emprestimoDto = await _servEmprestimo.Devolver(id);
                return Ok(emprestimoDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("verificar-atrasados")]
        [HttpPost]
        public async Task<IActionResult> VerificarAtrasados()
        {
            try
            {
                await _servEmprestimo.VerificarAtrasados();
                return Ok("Verificação de atrasos concluída.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
