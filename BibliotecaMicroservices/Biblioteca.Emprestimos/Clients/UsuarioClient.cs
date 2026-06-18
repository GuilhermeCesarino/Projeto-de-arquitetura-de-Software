using Biblioteca.Emprestimos.DTO;
using System.Text;
using System.Text.Json;

namespace Biblioteca.Emprestimos.Clients
{
    public class UsuarioClient
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public UsuarioClient(string baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
        }

        // Integração 1: Busca de dados - consulta usuário no microsserviço de Usuários
        public async Task<UsuarioClienteDTO> BuscarUsuarioPorId(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/Usuarios/{id}");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Usuário com ID {id} não encontrado no serviço de usuários.");

            var json = await response.Content.ReadAsStringAsync();
            var usuario = JsonSerializer.Deserialize<UsuarioClienteDTO>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return usuario!;
        }

        // Integração 3: Alteração de dados - bloqueia usuário com empréstimo atrasado
        public async Task BloquearUsuario(int id)
        {
            var response = await _httpClient.PatchAsync(
                $"{_baseUrl}/api/Usuarios/{id}/bloquear",
                new StringContent("{}", Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Falha ao bloquear usuário com ID {id}.");
        }
    }
}
