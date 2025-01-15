using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using TaskFrontend.Services;

namespace TaskFrontend.Pages
{
    public class TasksModel : PageModel
    {
        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;

        public TasksModel(AuthService authService, HttpClient httpClient)
        {
            _authService = authService;
            _httpClient = httpClient;
        }

        public List<Tarea> Tareas { get; set; } = new List<Tarea>();

        public async Task OnGetAsync()
        {
            try
            {
                // Establece el encabezado de autorización usando el JWT Token
                _authService.SetAuthorizationHeader(_httpClient);

                // Realiza la solicitud GET a la API
                var response = await _httpClient.GetAsync("http://localhost:5122/api/Task/List");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(json);
                    Tareas = apiResponse?.Value ?? new List<Tarea>();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al obtener las tareas del servidor.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocurrió un error: {ex.Message}");
            }
        }
    }

    public class Tarea
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public UserAssigned UserAssigned { get; set; }  // Change here
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UserAssigned  // Add this class to represent the userAssigned object
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Role { get; set; }  // Or specify the correct type if available
    }

    public class ApiResponse
    {
        public List<Tarea> Value { get; set; }
    }
}
