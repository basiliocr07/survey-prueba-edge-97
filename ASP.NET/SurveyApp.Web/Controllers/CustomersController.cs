
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Customers.Commands.AddCustomer;
using SurveyApp.Application.Customers.Commands.DeleteCustomer;
using SurveyApp.Application.Customers.Commands.UpdateCustomer;
using SurveyApp.Application.Customers.Queries.GetAllCustomers;
using SurveyApp.Application.Customers.Queries.GetAllServices;
using SurveyApp.Application.Customers.Queries.GetCustomerById;
using SurveyApp.Application.Customers.Queries.GetCustomersByType;
using SurveyApp.Application.Customers.Queries.GetCustomerEmails;
using SurveyApp.Application.Customers.Queries.GetCustomerGrowthData;
using SurveyApp.Web.Models;
using System;
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

        public async Task<IActionResult> Growth(string timeRange = "3", string chartType = "services", string customerType = null)
        {
            try
            {
                int? timeRangeInMonths = timeRange != "all" ? int.Parse(timeRange) : null;

                var query = new GetCustomerGrowthDataQuery
                {
                    TimeRangeInMonths = timeRangeInMonths,
                    CustomerType = customerType
                };

                var growthData = await _mediator.Send(query);
                
                var viewModel = new CustomerGrowthViewModel
                {
                    Customers = growthData.Customers,
                    Services = growthData.Services,
                    ServiceUsageData = growthData.ServiceUsageData,
                    MonthlyGrowthData = growthData.MonthlyGrowthData,
                    BrandGrowthData = growthData.BrandGrowthData,
                    SelectedTimeRange = timeRange,
                    SelectedChartType = chartType
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
                    CustomerType = form.CustomerType,
                    AcquiredServices = form.SelectedServices ?? new List<string>()
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

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var command = new DeleteCustomerCommand { Id = id };
                var result = await _mediator.Send(command);

                if (result)
                {
                    TempData["SuccessMessage"] = "Cliente eliminado exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se pudo eliminar el cliente.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar cliente: {ex.Message}";
            }

            return RedirectToAction(nameof(Growth));
        }

        [HttpPost]
        public async Task<IActionResult> Update(CustomerFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor complete todos los campos requeridos.";
                return RedirectToAction(nameof(Growth));
            }

            try
            {
                var command = new UpdateCustomerCommand
                {
                    Id = int.Parse(form.Id),
                    BrandName = form.BrandName,
                    ContactName = form.ContactName,
                    ContactEmail = form.ContactEmail,
                    ContactPhone = form.ContactPhone,
                    CustomerType = form.CustomerType,
                    AcquiredServices = form.SelectedServices ?? new List<string>()
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

            return RedirectToAction(nameof(Growth));
        }
    }
}
