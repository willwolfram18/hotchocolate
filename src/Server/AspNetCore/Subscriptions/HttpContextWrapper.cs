using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
#if ASPNETCLASSIC
using HotChocolate.AspNetClassic;
using HttpContext = Microsoft.Owin.IOwinContext;
#else
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
#endif
namespace HotChocolate.AspNetCore.Subscriptions
{
    internal class HttpContextWrapper
        : IHttpContext
    {
        private readonly HttpContext _context;

        public HttpContextWrapper(
            HttpContext context)
        {
            _context = context;
        }

        public ClaimsPrincipal User
        {
            get
            {
#if ASPNETCLASSIC
                return _context.Authentication.User;
#else
                return _context.User;
#endif
            } 
            set
            {
#if ASPNETCLASSIC
                _context.Authentication.User = value;
#else
                _context.User = value;
#endif
            }
        }

        public CancellationToken RequestAborted =>
            _context.GetCancellationToken();

        public void AddIdentity(ClaimsIdentity identity)
        {
#if ASPNETCLASSIC
            _context.Authentication.User.AddIdentity(identity);
#else
            _context.User.AddIdentity(identity);
#endif
        }

#if !ASPNETCLASSIC
        public IServiceProvider RequestServices =>
            _context.RequestServices;
#endif
    }
}
