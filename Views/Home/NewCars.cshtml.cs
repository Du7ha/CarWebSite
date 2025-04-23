using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CarWebsite.Views.Home
{
    public class NewCars : PageModel
    {
        private readonly ILogger<NewCars> _logger;

        public NewCars(ILogger<NewCars> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}