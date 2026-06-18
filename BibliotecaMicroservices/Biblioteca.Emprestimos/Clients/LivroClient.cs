using Biblioteca.Emprestimos.DTO;
using System.Text;
using System.Text.Json;

namespace Biblioteca.Emprestimos.Clients
{
    public class LivroClient
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public LivroClient(string baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
        }

        // Integração 2: Busca de dados - consulta livro no microsserviço de Livros
        public async Task<LivroClienteDTO> BuscarLivroPorId(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/Livros/{id}");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Livro com ID {id} não encontrado no serviço de livros.");

            var json = await response.Content.ReadAsStringAsync();
            var livro = JsonSerializer.Deserialize<LivroClienteDTO>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return livro!;
        }

        // Integração 3 (parte): Alteração de dados - atualiza disponibilidade do livro
        public async Task AtualizarDisponibilidade(int id, int quantidade)
        {
            var body = JsonSerializer.Serialize(new { quantidade });
            var response = await _httpClient.PatchAsync(
                $"{_baseUrl}/api/Livros/{id}/disponibilidade",
                new StringContent(body, Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                throw new Exception($"Falha ao atualizar disponibilidade do livro: {erro}");
            }
        }
    }
}
