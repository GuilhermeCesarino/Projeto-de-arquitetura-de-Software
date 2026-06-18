using Biblioteca.Usuarios.DTO;
using Biblioteca.Usuarios.Infra;
using Biblioteca.Usuarios.Models;

namespace Biblioteca.Usuarios.Services
{
    public interface IServUsuario
    {
        UsuarioDTO BuscarPorId(int id);
        List<UsuarioDTO> ListarTodos();
        UsuarioDTO Criar(CriarUsuarioDTO dto);
        UsuarioDTO Atualizar(int id, CriarUsuarioDTO dto);
        void Deletar(int id);
        UsuarioDTO BloquearUsuario(int id);
    }

    public class ServUsuario : IServUsuario
    {
        private DataContext _dataContext;

        public ServUsuario()
        {
            _dataContext = GeradorDeServicos.CarregarContexto();
        }

        public UsuarioDTO BuscarPorId(int id)
        {
            var usuario = _dataContext.Usuarios.Find(id);
            if (usuario == null)
                throw new Exception($"Usuário com ID {id} não encontrado.");

            return MapearParaDTO(usuario);
        }

        public List<UsuarioDTO> ListarTodos()
        {
            return _dataContext.Usuarios
                .Select(u => MapearParaDTO(u))
                .ToList();
        }

        public UsuarioDTO Criar(CriarUsuarioDTO dto)
        {
            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Telefone = dto.Telefone,
                DataCadastro = DateTime.Now,
                Ativo = true
            };

            _dataContext.Usuarios.Add(usuario);
            _dataContext.SaveChanges();

            return MapearParaDTO(usuario);
        }

        public UsuarioDTO Atualizar(int id, CriarUsuarioDTO dto)
        {
            var usuario = _dataContext.Usuarios.Find(id);
            if (usuario == null)
                throw new Exception($"Usuário com ID {id} não encontrado.");

            usuario.Nome = dto.Nome;
            usuario.Email = dto.Email;
            usuario.Telefone = dto.Telefone;

            _dataContext.SaveChanges();

            return MapearParaDTO(usuario);
        }

        public void Deletar(int id)
        {
            var usuario = _dataContext.Usuarios.Find(id);
            if (usuario == null)
                throw new Exception($"Usuário com ID {id} não encontrado.");

            _dataContext.Usuarios.Remove(usuario);
            _dataContext.SaveChanges();
        }

        public UsuarioDTO BloquearUsuario(int id)
        {
            var usuario = _dataContext.Usuarios.Find(id);
            if (usuario == null)
                throw new Exception($"Usuário com ID {id} não encontrado.");

            usuario.Ativo = false;
            _dataContext.SaveChanges();

            return MapearParaDTO(usuario);
        }

        private static UsuarioDTO MapearParaDTO(Usuario u) => new UsuarioDTO
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            Telefone = u.Telefone,
            DataCadastro = u.DataCadastro,
            Ativo = u.Ativo
        };
    }
}
