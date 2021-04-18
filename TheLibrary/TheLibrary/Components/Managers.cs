using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheLibrary.Data;

namespace TheLibrary.Components
{
    public class Managers : ViewComponent
    {
        private readonly AppDbContext _context;

        public Managers(AppDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int? id)
        {
            var managers = _context.Employees.Where(x => x.IsManager).ToList();

            return View(managers);
        }
    }
}
