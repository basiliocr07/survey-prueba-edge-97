
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Customers.Commands.AddCustomer;
using SurveyApp.Application.Customers.Queries.GetCustomerGrowthData;
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

        public async Task<IActionResult> Growth()
        {
            try
            {
                var growthData = await _mediator.Send(new GetCustomerGrowthDataQuery());
                
                var viewModel = new CustomerGrowthViewModel
                {
                    Customers = growthData.Customers,
                    Services = growthData.Services,
                    ServiceUsageData = growthData.ServiceUsageData
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the error
                TempData["ErrorMessage"] = $"Error cargando datos: {ex.Message}";
                return View(new CustomerGrowthViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor complete todos los campos requeridos.";
                return RedirectToAction(nameof(Growth));
            }

            try
            {
                var command = new AddCustomerCommand
                {
                    BrandName = form.BrandName,
                    ContactName = form.ContactName,
                    ContactEmail = form.ContactEmail,
                    ContactPhone = form.ContactPhone,
                    AcquiredServices = form.SelectedServices
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

            return RedirectToAction(nameof(Growth));
        }
    }
}
