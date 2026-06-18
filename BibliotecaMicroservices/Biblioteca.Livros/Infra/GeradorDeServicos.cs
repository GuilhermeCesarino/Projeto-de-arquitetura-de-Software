using Biblioteca.Livros;

namespace Biblioteca.Livros.Infra
{
    public static class GeradorDeServicos
    {
        public static ServiceProvider ServiceProvider;

        public static DataContext CarregarContexto()
        {
            return ServiceProvider.GetService<DataContext>();
        }
    }
}
