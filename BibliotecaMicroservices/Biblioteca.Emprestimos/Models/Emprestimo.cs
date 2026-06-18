namespace Biblioteca.Emprestimos.Models
{
    public class Emprestimo
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int LivroId { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime DataPrevistaDevolucao { get; set; }
        public DateTime? DataDevolucao { get; set; }
        public StatusEmprestimo Status { get; set; }
    }

    public enum StatusEmprestimo
    {
        Ativo = 1,
        Devolvido = 2,
        Atrasado = 3
    }
}
