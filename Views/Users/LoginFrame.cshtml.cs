using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CarWebsite.Views.Users
{
    public class LoginFrame : PageModel
    {
        private readonly ILogger<LoginFrame> _logger;

        public LoginFrame(ILogger<LoginFrame> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}