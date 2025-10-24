using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BSC.UI.Services
{
	public class AuthService(ILocalStorageService localStorage, NavigationManager nav, IJSRuntime jsRuntime)
	{
		private readonly ILocalStorageService _localStorage = localStorage;
		private readonly NavigationManager _nav = nav;
		private readonly IJSRuntime _jsRuntime = jsRuntime;
		private const string TokenKey = "authToken";

		// Detecta si JS ya está disponible
		private bool IsJSRuntimeAvailable => _jsRuntime is IJSInProcessRuntime;

		public async Task<bool> IsAuthenticatedAsync()
		{
			// Si aún está prerenderizando, devuelve false y evita el error
			if (!IsJSRuntimeAvailable)
				return false;

			var token = await _localStorage.GetItemAsync<string>(TokenKey);
			return !string.IsNullOrEmpty(token);
		}

		public async Task EnsureAuthenticatedAsync()
		{
			var isAuth = await IsAuthenticatedAsync();
			if (!isAuth)
			{
				_nav.NavigateTo("/login", true);
			}
		}

		public async Task SaveTokenAsync(string token)
		{
			if (IsJSRuntimeAvailable)
				await _localStorage.SetItemAsync(TokenKey, token);
		}

		public async Task LogoutAsync()
		{
			if (IsJSRuntimeAvailable)
				await _localStorage.RemoveItemAsync(TokenKey);

			_nav.NavigateTo("/login", true);
		}

		public async Task<string?> GetTokenAsync()
		{
			if (!IsJSRuntimeAvailable)
				return null;

			return await _localStorage.GetItemAsync<string>(TokenKey);
		}
	}
}
