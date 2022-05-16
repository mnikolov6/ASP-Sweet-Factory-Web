using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Sladko_Izkushenie.Data;

namespace Sladko_Izkushenie.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Имейла е задължително поле!")]
            [EmailAddress]
            [Display(Name = "Имейл")]

            public string Email { get; set; }

            [Required(ErrorMessage = "Потребителското име е задължително поле!")]
            
            [Display(Name = "Потребителско име")]

            public string UserName { get; set; }

            [Required(ErrorMessage = "Пълното Ви име е задължително поле!")]
            [DataType(DataType.Text)]
            [Display(Name = "Име и фамилия")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Телефонният Ви номер е задължително поле!")]
            [StringLength(10, ErrorMessage = "{0} трябва да бъде {1} цифри!", MinimumLength = 10)]
            [DataType((DataType.PhoneNumber))]
            [Display(Name = "Телефон")]
            public string PhoneNumber { get; set; }
            [Required(ErrorMessage = "Паролата е задължително поле!")]
            [StringLength(100, ErrorMessage = "{0} трябва да бъде поне {2} и най-много {1} символа.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Парола")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Трябва да потвърдите паролата си!")]
            [DataType(DataType.Password)]
            [Display(Name = "Потвърждаване на парола")]
            [Compare("Password", ErrorMessage = "Паролата и полето за потвърждение не съответстват!")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new User
                { UserName = Input.UserName,
                  Email = Input.Email,
                  FullName = Input.FullName,
                  PhoneNumber = Input.PhoneNumber};
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Потребителя създаде нов акаунт с парола.");
                    var result1 = await _userManager.AddToRoleAsync(user, "User");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Потвърдете Вашия имейл",
                        $"Моля потвърдете Вашия акаунт като <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>кликнете тук.</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
