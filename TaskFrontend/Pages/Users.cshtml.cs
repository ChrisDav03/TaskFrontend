using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using TaskFrontend.Services;

namespace TaskFrontend.Pages
{
    public class UsersModel : PageModel
    {
        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;

        public UsersModel(AuthService authService, HttpClient httpClient)
        {
            _authService = authService;
            _httpClient = httpClient;
        }

        // Lista de usuarios que se llenará después de obtener los datos
        public List<User> Users { get; set; } = new List<User>();

        // Método asincrónico para obtener los usuarios
        public async Task OnGetAsync()
        {
            try
            {
                // Establece el encabezado de autorización usando el JWT Token
                _authService.SetAuthorizationHeader(_httpClient);

                // Realiza la solicitud GET a la API de usuarios
                var response = await _httpClient.GetAsync("http://localhost:5122/api/users");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponses>(json);
                    Users = apiResponse?.Value ?? new List<User>();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al obtener los usuarios del servidor.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocurrió un error: {ex.Message}");
            }
        }
    }

    // Clase que representa un usuario
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
    }

    // Clase que representa la respuesta de la API
    public class ApiResponses
    {
        public List<User> Value { get; set; }
    }
}
