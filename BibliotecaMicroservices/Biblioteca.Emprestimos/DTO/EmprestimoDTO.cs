namespace Biblioteca.Emprestimos.DTO
{
    public class EmprestimoDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public int LivroId { get; set; }
        public string TituloLivro { get; set; } = string.Empty;
        public DateTime DataEmprestimo { get; set; }
        public DateTime DataPrevistaDevolucao { get; set; }
        public DateTime? DataDevolucao { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CriarEmprestimoDTO
    {
        public int UsuarioId { get; set; }
        public int LivroId { get; set; }
        public int DiasEmprestimo { get; set; } = 7;
    }

    // DTOs para consumo dos outros microsserviços
    public class UsuarioClienteDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Ativo { get; set; }
    }

    public class LivroClienteDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int QuantidadeDisponivel { get; set; }
    }
}
