using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CarWebsite.Views.Cars
{
    public class OfferYourCar : PageModel
    {
        private readonly ILogger<OfferYourCar> _logger;

        public OfferYourCar(ILogger<OfferYourCar> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}