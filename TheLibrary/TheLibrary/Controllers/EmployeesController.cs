using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheLibrary.Data;
using TheLibrary.Models;

namespace TheLibrary.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // Get All Employees
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.ToListAsync());
        }

        // Detail page for Employees
        public async Task<IActionResult> EmployeeDetails(int? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        #region Create Employee
        public IActionResult AddEmployee()
        {
            return View();
        }

        // Create Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee([Bind("Id,FirstName,LastName,Salary,IsCEO,IsManager,ManagerId")] Employee employee)
        {
            //Check if employee.IsCEO already is true
            if (employee.IsCEO)
            {
                Employee CeoExist = _context.Employees.FirstOrDefault(
                c => c.IsCEO);
                //Check if an CEO already exist, if it does it will return an error
                if (CeoExist != null)
                {
                    ModelState.AddModelError(string.Empty, "an CEO Already Exist");
                    return View(employee);
                }
            }
            //Employee rank multiplied with salary coefficent 
            var coefficent = (employee.IsCEO) ? 2.725 : (employee.IsManager) ? 1.725 : 1.125;
            employee.Salary = Convert.ToDecimal(coefficent) * employee.Salary;

            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
        #endregion

        #region Edit Employee
        public async Task<IActionResult> EditEmployee(int? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // Edit Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployee(int id, [Bind("Id,FirstName,LastName,Salary,IsCEO,IsManager,ManagerId")] Employee employee)
        {
            bool EmployeeExists(int id)
            {
                return _context.Employees.Any(e => e.Id == id);
            }

            if (employee.IsCEO)
            {
                Employee CeoExist = _context.Employees.FirstOrDefault(
                c => c.IsCEO);
                //Check if an CEO already exist, if it does it will return an error
                if (CeoExist != null)
                {
                    ModelState.AddModelError(string.Empty, "an CEO Already Exist");
                    return View(employee);
                }
            }

            if (id != employee.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    //try to update existing employee
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
        #endregion

        #region Delete Employee
        // Delete Employees
        public async Task<IActionResult> RemoveEmployee(int? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("RemoveEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee.IsManager || employee.IsCEO)
            {
                //Check if Ceo or Manager is managing another employee
                var employeeList = _context.Employees.Count(x => x.ManagerId == id);
                if (employeeList > 0 || (employee.ManagerId > 0 && employee.IsCEO))
                {
                    ModelState.AddModelError(string.Empty, "The CEO or Manager is Still Managing an Employee");
                    return View();
                }
            }
            //Removing Employee from Database
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Managing Employees
        //Code to manage employees
        [HttpGet("ManageEmployees/{id}")]
        public async Task<IActionResult> ManageEmployees(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            var employees = new List<Employee>(); // Employee List
            if(employee.IsManager)
            {
                employees = _context.Employees.Where(x => !x.IsCEO && x.Id != id).ToList();
            }
            else if(employee.IsCEO)
            {
                employees = _context.Employees.Where(x => x.IsManager && x.Id != id).ToList();
            }
            return View(employees);
        }
        #endregion
    }


}
