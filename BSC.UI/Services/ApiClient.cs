using Blazored.LocalStorage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;


namespace BSC.UI.Services
{
	public class ApiClient(HttpClient httpClient, AuthService auth)
	{
		private readonly HttpClient _httpClient = httpClient;
		private readonly AuthService _authService = auth;
		private readonly JsonSerializerOptions _jsonOptions = new()
		{
			PropertyNameCaseInsensitive = true
		};

		// 🔹 Aplica el token al header si existe
		private async Task EnsureTokenAsync()
		{
			var token = await _authService.GetTokenAsync();
			if (!string.IsNullOrEmpty(token))
			{
				_httpClient.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Bearer", token);
			}
			else
			{
				// Si no hay token, fuerza logout (redirige al login)
				await _authService.LogoutAsync();
			}
		}

		// 🔹 POST genérico
		public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
		{
			await EnsureTokenAsync();

			var response = await _httpClient.PostAsJsonAsync(url, data);

			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				await _authService.LogoutAsync();
				return default;
			}

			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
		}

		// 🔹 GET genérico
		public async Task<TResponse?> GetAsync<TResponse>(string url)
		{
			await EnsureTokenAsync();

			var response = await _httpClient.GetAsync(url);

			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				await _authService.LogoutAsync();
				return default;
			}

			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
		}

		// 🔹 Login (sin token requerido)
		public async Task<TResponse?> LoginAsync<TRequest, TResponse>(string url, TRequest credentials)
		{
			var response = await _httpClient.PostAsJsonAsync(url, credentials);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
			}

			var error = await response.Content.ReadAsStringAsync();
			throw new HttpRequestException($"Error en login: {response.StatusCode} - {error}");
		}

		// 🔹 Logout manual (opcional)
		public async Task LogoutAsync()
		{
			await _authService.LogoutAsync();
		}
	}
}
