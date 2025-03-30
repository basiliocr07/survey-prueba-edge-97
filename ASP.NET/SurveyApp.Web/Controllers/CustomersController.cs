
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Customers.Commands.AddCustomer;
using SurveyApp.Application.Customers.Commands.UpdateCustomer;
using SurveyApp.Application.Customers.Commands.DeleteCustomer;
using SurveyApp.Application.Customers.Queries.GetCustomerGrowthData;
using SurveyApp.Application.Customers.Queries.GetCustomerById;
using SurveyApp.Web.Models;
using System.Threading.Tasks;

namespace SurveyApp.Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Growth(string userType = "client")
        {
            try
            {
                var isAdmin = HttpContext.User.IsInRole("Admin") || userType == "admin";
                
                var query = new GetCustomerGrowthDataQuery { UserType = userType };
                var growthData = await _mediator.Send(query);
                
                var viewModel = new CustomerGrowthViewModel
                {
                    Customers = growthData.Customers,
                    Services = growthData.Services,
                    ServiceUsageData = growthData.ServiceUsageData,
                    IsAdmin = isAdmin,
                    UserType = userType
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the error
                TempData["ErrorMessage"] = $"Error cargando datos: {ex.Message}";
                return View(new CustomerGrowthViewModel { UserType = userType });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor complete todos los campos requeridos.";
                return RedirectToAction(nameof(Growth), new { userType = form.UserType });
            }

            try
            {
                var command = new AddCustomerCommand
                {
                    BrandName = form.BrandName,
                    ContactName = form.ContactName,
                    ContactEmail = form.ContactEmail,
                    ContactPhone = form.ContactPhone,
                    AcquiredServices = form.SelectedServices,
                    UserType = form.UserType
                };

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Cliente añadido exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al añadir cliente: {ex.Message}";
            }

            return RedirectToAction(nameof(Growth), new { userType = form.UserType });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(CustomerFormViewModel form, int id)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor complete todos los campos requeridos.";
                return RedirectToAction(nameof(Growth), new { userType = form.UserType });
            }

            try
            {
                var command = new UpdateCustomerCommand
                {
                    Id = id,
                    BrandName = form.BrandName,
                    ContactName = form.ContactName,
                    ContactEmail = form.ContactEmail,
                    ContactPhone = form.ContactPhone,
                    AcquiredServices = form.SelectedServices,
                    UserType = form.UserType
                };

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Cliente actualizado exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al actualizar cliente: {ex.Message}";
            }

            return RedirectToAction(nameof(Growth), new { userType = form.UserType });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id, string userType = "client")
        {
            try
            {
                var command = new DeleteCustomerCommand
                {
                    Id = id,
                    UserType = userType
                };

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Cliente eliminado exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar cliente: {ex.Message}";
            }

            return RedirectToAction(nameof(Growth), new { userType });
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomer(int id)
        {
            try
            {
                var query = new GetCustomerByIdQuery { Id = id };
                var customer = await _mediator.Send(query);

                if (customer == null)
                {
                    return NotFound();
                }

                return Json(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
