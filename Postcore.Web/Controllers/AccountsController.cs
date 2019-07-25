using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Postcore.Web.Core.WebModels.Accounts;

namespace Postcore.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public AccountsController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginPost(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email,
                    model.Password, model.RememberMe, false).ConfigureAwait(false);

                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("LoginError", "Invalid email or password.");
            }

            return View("Login", model);
        }

        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
                await _signInManager.SignOutAsync().ConfigureAwait(false);

            return RedirectToAction("Login");
        }

        public IActionResult SignUp()
        {
            var model = new SignUpModel();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var user = _pool.GetUser(model.Email);

            if (user.Status != null)
            { 
                ModelState.AddModelError("SignUpError", "User already exists.");
                return View(model);
            }

            user.Attributes.Add("name", model.Email);
            var createdUser = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

            if (!createdUser.Succeeded)
            {
                ModelState.AddModelError("SignUpError", "Unable to create account.");
                return View(model);
            }

            return RedirectToAction("Confirm");
        }

        [HttpGet]
        public IActionResult Confirm()
        {
            var model = new ConfirmModel();
            return View(model);
        }

        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> ConfirmPost(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
                if (user == null)
                {
                    ModelState.AddModelError("ConfirmError", "Email address not found.");
                    return View(model);
                }

                var result = await ((CognitoUserManager<CognitoUser>)_userManager)
                    .ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("ConfirmError", "Unable to confirm account.");
                    return View(model);
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}