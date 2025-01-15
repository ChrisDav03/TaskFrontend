using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskFrontend.Services;

namespace TaskFrontend.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AuthService _authService;

        public LoginModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public async Task<IActionResult>
        OnPostAsync()
        {
            var token = await _authService.LoginAsync(Email, Password);

            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Tasks"); // Redirige a la página principal
            }

            // Si la autenticación falla
            ModelState.AddModelError(string.Empty, "Credenciales incorrectas");
            return Page();
        }
        public void OnGet()
        {
        }
    }
}
