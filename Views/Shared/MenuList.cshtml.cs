using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CarWebsite.Views.Shared
{
    public class MenuList : PageModel
    {
        private readonly ILogger<MenuList> _logger;

        public MenuList(ILogger<MenuList> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}