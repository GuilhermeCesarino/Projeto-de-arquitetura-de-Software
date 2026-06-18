using Biblioteca.Livros.DTO;
using Biblioteca.Livros.Services;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Livros.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LivrosController : Controller
    {
        private IServLivro _servLivro;

        public LivrosController()
        {
            _servLivro = new ServLivro();
        }

        [HttpGet]
        public IActionResult ListarTodos()
        {
            try
            {
                var livros = _servLivro.ListarTodos();
                return Ok(livros);
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
                var livroDto = _servLivro.BuscarPorId(id);
                return Ok(livroDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IActionResult Criar([FromBody] CriarLivroDTO dto)
        {
            try
            {
                var livroDto = _servLivro.Criar(dto);
                return Ok(livroDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("{id}")]
        [HttpPut]
        public IActionResult Atualizar(int id, [FromBody] CriarLivroDTO dto)
        {
            try
            {
                var livroDto = _servLivro.Atualizar(id, dto);
                return Ok(livroDto);
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
                _servLivro.Deletar(id);
                return Ok("Livro removido com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Endpoint chamado pelo microsserviço de Empréstimos para alterar disponibilidade
        // quantidade negativa = retirada (empréstimo), positiva = devolução
        [Route("{id}/disponibilidade")]
        [HttpPatch]
        public IActionResult AtualizarDisponibilidade(int id, [FromBody] AtualizarDisponibilidadeDTO dto)
        {
            try
            {
                var livroDto = _servLivro.AtualizarDisponibilidade(id, dto.Quantidade);
                return Ok(livroDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
