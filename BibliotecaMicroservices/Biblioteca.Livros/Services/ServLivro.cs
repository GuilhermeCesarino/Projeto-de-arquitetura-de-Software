using Biblioteca.Livros.DTO;
using Biblioteca.Livros.Infra;
using Biblioteca.Livros.Models;

namespace Biblioteca.Livros.Services
{
    public interface IServLivro
    {
        LivroDTO BuscarPorId(int id);
        List<LivroDTO> ListarTodos();
        LivroDTO Criar(CriarLivroDTO dto);
        LivroDTO Atualizar(int id, CriarLivroDTO dto);
        void Deletar(int id);
        LivroDTO AtualizarDisponibilidade(int id, int quantidade);
    }

    public class ServLivro : IServLivro
    {
        private DataContext _dataContext;

        public ServLivro()
        {
            _dataContext = GeradorDeServicos.CarregarContexto();
        }

        public LivroDTO BuscarPorId(int id)
        {
            var livro = _dataContext.Livros.Find(id);
            if (livro == null)
                throw new Exception($"Livro com ID {id} não encontrado.");

            return MapearParaDTO(livro);
        }

        public List<LivroDTO> ListarTodos()
        {
            return _dataContext.Livros
                .Select(l => MapearParaDTO(l))
                .ToList();
        }

        public LivroDTO Criar(CriarLivroDTO dto)
        {
            var livro = new Livro
            {
                Titulo = dto.Titulo,
                Autor = dto.Autor,
                ISBN = dto.ISBN,
                AnoPublicacao = dto.AnoPublicacao,
                QuantidadeTotal = dto.QuantidadeTotal,
                QuantidadeDisponivel = dto.QuantidadeTotal
            };

            _dataContext.Livros.Add(livro);
            _dataContext.SaveChanges();

            return MapearParaDTO(livro);
        }

        public LivroDTO Atualizar(int id, CriarLivroDTO dto)
        {
            var livro = _dataContext.Livros.Find(id);
            if (livro == null)
                throw new Exception($"Livro com ID {id} não encontrado.");

            livro.Titulo = dto.Titulo;
            livro.Autor = dto.Autor;
            livro.ISBN = dto.ISBN;
            livro.AnoPublicacao = dto.AnoPublicacao;
            livro.QuantidadeTotal = dto.QuantidadeTotal;

            _dataContext.SaveChanges();

            return MapearParaDTO(livro);
        }

        public void Deletar(int id)
        {
            var livro = _dataContext.Livros.Find(id);
            if (livro == null)
                throw new Exception($"Livro com ID {id} não encontrado.");

            _dataContext.Livros.Remove(livro);
            _dataContext.SaveChanges();
        }

        // Chamado pelo microsserviço de Empréstimos ao emprestar/devolver livro
        public LivroDTO AtualizarDisponibilidade(int id, int quantidade)
        {
            var livro = _dataContext.Livros.Find(id);
            if (livro == null)
                throw new Exception($"Livro com ID {id} não encontrado.");

            var novaQtd = livro.QuantidadeDisponivel + quantidade;

            if (novaQtd < 0)
                throw new Exception($"Quantidade insuficiente. Disponível: {livro.QuantidadeDisponivel}");
            if (novaQtd > livro.QuantidadeTotal)
                throw new Exception($"Quantidade ultrapassa o total de exemplares.");

            livro.QuantidadeDisponivel = novaQtd;
            _dataContext.SaveChanges();

            return MapearParaDTO(livro);
        }

        private static LivroDTO MapearParaDTO(Livro l) => new LivroDTO
        {
            Id = l.Id,
            Titulo = l.Titulo,
            Autor = l.Autor,
            ISBN = l.ISBN,
            AnoPublicacao = l.AnoPublicacao,
            QuantidadeTotal = l.QuantidadeTotal,
            QuantidadeDisponivel = l.QuantidadeDisponivel
        };
    }
}
