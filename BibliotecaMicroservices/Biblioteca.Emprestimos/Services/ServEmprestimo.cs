using Biblioteca.Emprestimos.Clients;
using Biblioteca.Emprestimos.DTO;
using Biblioteca.Emprestimos.Infra;
using Biblioteca.Emprestimos.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Emprestimos.Services
{
    public interface IServEmprestimo
    {
        Task<EmprestimoDTO> BuscarPorId(int id);
        Task<List<EmprestimoDTO>> ListarTodos();
        Task<List<EmprestimoDTO>> ListarPorUsuario(int usuarioId);
        Task<EmprestimoDTO> Criar(CriarEmprestimoDTO dto);
        Task<EmprestimoDTO> Devolver(int id);
        Task VerificarAtrasados();
    }

    public class ServEmprestimo : IServEmprestimo
    {
        private DataContext _dataContext;
        private UsuarioClient _usuarioClient;
        private LivroClient _livroClient;

        public ServEmprestimo(string usuariosBaseUrl, string livrosBaseUrl)
        {
            _dataContext = GeradorDeServicos.CarregarContexto();
            _usuarioClient = new UsuarioClient(usuariosBaseUrl);
            _livroClient = new LivroClient(livrosBaseUrl);
        }

        public async Task<EmprestimoDTO> BuscarPorId(int id)
        {
            var emprestimo = _dataContext.Emprestimos.Find(id);
            if (emprestimo == null)
                throw new Exception($"Empréstimo com ID {id} não encontrado.");

            return await EnriquecerEmprestimo(emprestimo);
        }

        public async Task<List<EmprestimoDTO>> ListarTodos()
        {
            var emprestimos = _dataContext.Emprestimos.ToList();
            var resultado = new List<EmprestimoDTO>();

            foreach (var e in emprestimos)
                resultado.Add(await EnriquecerEmprestimo(e));

            return resultado;
        }

        public async Task<List<EmprestimoDTO>> ListarPorUsuario(int usuarioId)
        {
            var emprestimos = _dataContext.Emprestimos
                .Where(e => e.UsuarioId == usuarioId)
                .ToList();

            var resultado = new List<EmprestimoDTO>();
            foreach (var e in emprestimos)
                resultado.Add(await EnriquecerEmprestimo(e));

            return resultado;
        }

        public async Task<EmprestimoDTO> Criar(CriarEmprestimoDTO dto)
        {
            // Integração 1: busca usuário no microsserviço de Usuários
            var usuario = await _usuarioClient.BuscarUsuarioPorId(dto.UsuarioId);

            if (!usuario.Ativo)
                throw new Exception($"Usuário '{usuario.Nome}' está bloqueado e não pode realizar empréstimos.");

            // Integração 2: busca livro no microsserviço de Livros
            var livro = await _livroClient.BuscarLivroPorId(dto.LivroId);

            if (livro.QuantidadeDisponivel <= 0)
                throw new Exception($"Livro '{livro.Titulo}' não possui exemplares disponíveis.");

            var emprestimo = new Emprestimo
            {
                UsuarioId = dto.UsuarioId,
                LivroId = dto.LivroId,
                DataEmprestimo = DateTime.Now,
                DataPrevistaDevolucao = DateTime.Now.AddDays(dto.DiasEmprestimo),
                Status = StatusEmprestimo.Ativo
            };

            _dataContext.Emprestimos.Add(emprestimo);

            // Integração 3 (parte): atualiza disponibilidade no microsserviço de Livros
            await _livroClient.AtualizarDisponibilidade(dto.LivroId, -1);

            _dataContext.SaveChanges();

            return new EmprestimoDTO
            {
                Id = emprestimo.Id,
                UsuarioId = emprestimo.UsuarioId,
                NomeUsuario = usuario.Nome,
                LivroId = emprestimo.LivroId,
                TituloLivro = livro.Titulo,
                DataEmprestimo = emprestimo.DataEmprestimo,
                DataPrevistaDevolucao = emprestimo.DataPrevistaDevolucao,
                Status = emprestimo.Status.ToString()
            };
        }

        public async Task<EmprestimoDTO> Devolver(int id)
        {
            var emprestimo = _dataContext.Emprestimos.Find(id);
            if (emprestimo == null)
                throw new Exception($"Empréstimo com ID {id} não encontrado.");

            if (emprestimo.Status == StatusEmprestimo.Devolvido)
                throw new Exception("Este empréstimo já foi devolvido.");

            emprestimo.DataDevolucao = DateTime.Now;
            emprestimo.Status = StatusEmprestimo.Devolvido;

            // Integração 3 (parte): devolução - atualiza disponibilidade no microsserviço de Livros
            await _livroClient.AtualizarDisponibilidade(emprestimo.LivroId, +1);

            _dataContext.SaveChanges();

            return await EnriquecerEmprestimo(emprestimo);
        }

        // Verifica empréstimos atrasados e bloqueia usuários
        public async Task VerificarAtrasados()
        {
            var atrasados = _dataContext.Emprestimos
                .Where(e => e.Status == StatusEmprestimo.Ativo && e.DataPrevistaDevolucao < DateTime.Now)
                .ToList();

            foreach (var emprestimo in atrasados)
            {
                emprestimo.Status = StatusEmprestimo.Atrasado;

                // Integração 3: alteração de dados - bloqueia usuário no microsserviço de Usuários
                await _usuarioClient.BloquearUsuario(emprestimo.UsuarioId);
            }

            _dataContext.SaveChanges();
        }

        private async Task<EmprestimoDTO> EnriquecerEmprestimo(Emprestimo emprestimo)
        {
            string nomeUsuario = string.Empty;
            string tituloLivro = string.Empty;

            try
            {
                var usuario = await _usuarioClient.BuscarUsuarioPorId(emprestimo.UsuarioId);
                nomeUsuario = usuario.Nome;
            }
            catch { nomeUsuario = $"Usuário #{emprestimo.UsuarioId}"; }

            try
            {
                var livro = await _livroClient.BuscarLivroPorId(emprestimo.LivroId);
                tituloLivro = livro.Titulo;
            }
            catch { tituloLivro = $"Livro #{emprestimo.LivroId}"; }

            return new EmprestimoDTO
            {
                Id = emprestimo.Id,
                UsuarioId = emprestimo.UsuarioId,
                NomeUsuario = nomeUsuario,
                LivroId = emprestimo.LivroId,
                TituloLivro = tituloLivro,
                DataEmprestimo = emprestimo.DataEmprestimo,
                DataPrevistaDevolucao = emprestimo.DataPrevistaDevolucao,
                DataDevolucao = emprestimo.DataDevolucao,
                Status = emprestimo.Status.ToString()
            };
        }
    }
}
