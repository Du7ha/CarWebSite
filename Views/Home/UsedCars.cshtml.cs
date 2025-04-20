using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CarWebsite.Views.Home
{
    public class UsedCars : PageModel
    {
        private readonly ILogger<UsedCars> _logger;

        public UsedCars(ILogger<UsedCars> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}