using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CarWebsite.Views.Cars
{
    public class CarDetails : PageModel
    {
        private readonly ILogger<CarDetails> _logger;

        public CarDetails(ILogger<CarDetails> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}