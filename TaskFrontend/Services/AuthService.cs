using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace TaskFrontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string _loginUrl = "http://127.0.0.1:5122/api/Access/login"; // URL de tu API REST

        public AuthService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // Realiza el login y guarda el JWT en una cookie
        public async Task<string> LoginAsync(string email, string password)
        {
            var loginData = new
            {
                email = email,
                password = password
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_loginUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<TokenResponse>(json)?.Token;

                // Almacena el token JWT en una cookie
                if (!string.IsNullOrEmpty(token))
                {
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("jwt_token", token, new CookieOptions
                    {
                        HttpOnly = true,  // Impide que el token sea accedido por JavaScript
                        Secure = true,    // Asegura la cookie en entornos HTTPS
                        SameSite = SameSiteMode.Strict,  // Restringe el acceso de la cookie a tu dominio
                        Expires = DateTime.Now.AddDays(1)  // Configura el tiempo de expiración
                    });
                }

                return token;
            }

            return string.Empty;
        }

        // Configura la cabecera de autorización para las solicitudes
        public void SetAuthorizationHeader(HttpClient _httpClient)
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["jwt_token"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Verifica si el usuario está autenticado mediante el JWT almacenado
        public bool IsAuthenticated()
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["jwt_token"];
            return !string.IsNullOrEmpty(token);
        }

        // Realiza el logout, elimina el JWT y redirige al login
        public void Logout()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("jwt_token");  // Elimina la cookie del token JWT
            _httpContextAccessor.HttpContext.Response.Redirect("/Login");  // Redirige a la página de login
        }
    }

    // Respuesta que se recibe del backend cuando se realiza el login
    public class TokenResponse
    {
        public string Token { get; set; }
    }

}
