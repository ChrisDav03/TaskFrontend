using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Json;
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

        public List<TareaDTO> Tareas { get; set; } = new List<TareaDTO>();

        [BindProperty]
        public TareaDTO NuevaTarea { get; set; } = new TareaDTO();

        [BindProperty]
        public TareaDTO TareaEditada { get; set; } = new TareaDTO();

        public async Task OnGetAsync()
        {
            try
            {
                _authService.SetAuthorizationHeader(_httpClient);

                var response = await _httpClient.GetAsync("http://localhost:5122/api/Task/List");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseDTO>(json);
                    Tareas = apiResponse?.Value ?? new List<TareaDTO>();
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

        public async Task<IActionResult> OnPostCreateAsync()
        {
            try
            {
                _authService.SetAuthorizationHeader(_httpClient);

                // Agregar un usuario asignado a la tarea nueva si es necesario
                if (NuevaTarea.UserAssigned == null)
                {
                    NuevaTarea.UserAssigned = new UserAssignedDTO { Id = 0 }; // O el ID del usuario asignado
                }

                var response = await _httpClient.PostAsJsonAsync("http://localhost:5122/api/Task/Create", NuevaTarea);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear la tarea.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocurrió un error: {ex.Message}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id)
        {
            try
            {
                _authService.SetAuthorizationHeader(_httpClient);

                // Asegurar que la tarea editada tenga un usuario asignado
                if (TareaEditada.UserAssigned == null)
                {
                    TareaEditada.UserAssigned = new UserAssignedDTO { Id = 0 }; // O el ID del usuario asignado
                }

                var response = await _httpClient.PutAsJsonAsync($"http://localhost:5122/api/Task/Update/{id}", TareaEditada);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar la tarea.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocurrió un error: {ex.Message}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                _authService.SetAuthorizationHeader(_httpClient);

                var response = await _httpClient.DeleteAsync($"http://localhost:5122/api/Task/Delete/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al eliminar la tarea.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocurrió un error: {ex.Message}");
            }

            return Page();
        }
    }
    // DTOs to match the API structure
    public class TareaDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public UserAssignedDTO UserAssigned { get; set; }

        // Agregamos las propiedades CreatedAt y UpdatedAt
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } // Asegúrate de que esta propiedad coincida con la estructura
    }

    public class UserAssignedDTO
    {
        public int Id { get; set; }
       
    }


    // API Response DTO
    public class ApiResponseDTO
    {
        public List<TareaDTO> Value { get; set; }
    }
}
