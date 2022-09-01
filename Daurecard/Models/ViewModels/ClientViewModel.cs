using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Daurecard.Models.ViewModels
{
    public class ClientViewModel
    {
        public ClientViewModel()
        {
        }

        public Client Client { get; set; }

        public IEnumerable<SelectListItem> EntreprisesList { get; set; }
    }
}
