namespace Biblioteca.Livros.DTO
{
    public class LivroDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int AnoPublicacao { get; set; }
        public int QuantidadeTotal { get; set; }
        public int QuantidadeDisponivel { get; set; }
    }

    public class CriarLivroDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int AnoPublicacao { get; set; }
        public int QuantidadeTotal { get; set; }
    }

    public class AtualizarDisponibilidadeDTO
    {
        public int Quantidade { get; set; }
    }
}
