using ITTP.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ITTP.Services.AuthService.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly bool _isOnlyAdminAllowed;

        public AuthorizeAttribute(bool isOnlyAdminAllowed = false)
        {
            _isOnlyAdminAllowed = isOnlyAdminAllowed;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var user = (User)filterContext.HttpContext.Items[nameof(AuthData)]!;

            if (user == null)
            {
                filterContext.Result = new JsonResult(new { message = "Неавторизованный" })
                { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                if (_isOnlyAdminAllowed == true && user.IsAdmin == false)
                {
                    filterContext.Result = new JsonResult(new { message = "Метод не допускается" })
                    { StatusCode = StatusCodes.Status405MethodNotAllowed };
                }
            }
        }
    }
}
