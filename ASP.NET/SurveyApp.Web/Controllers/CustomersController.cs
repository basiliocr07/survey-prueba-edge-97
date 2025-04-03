
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
using SurveyApp.Application.Customers.Queries.GetServiceUsageData;
using SurveyApp.Web.Models;
using System;
using System.Collections.Generic;
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

        public async Task<IActionResult> Index(string timeRange = "3", string chartType = "services", string customerType = null)
        {
            try
            {
                // Obtener datos de clientes
                var getAllCustomersQuery = new GetAllCustomersQuery();
                var customers = await _mediator.Send(getAllCustomersQuery);

                // Obtener datos de servicios
                var getAllServicesQuery = new GetAllServicesQuery();
                var services = await _mediator.Send(getAllServicesQuery);

                // Obtener datos de uso de servicios
                var getServiceUsageDataQuery = new GetServiceUsageDataQuery 
                { 
                    TimeRange = timeRange, 
                    CustomerType = customerType 
                };
                var serviceUsageData = await _mediator.Send(getServiceUsageDataQuery);

                // Construir el modelo para la vista
                var viewModel = new CustomerViewModel
                {
                    Customers = new List<SurveyApp.Domain.Models.Customer>(customers),
                    Services = new List<SurveyApp.Domain.Models.Service>(services),
                    ServiceUsageData = new List<SurveyApp.Domain.Models.ServiceUsageData>(serviceUsageData),
                    TimeRange = timeRange,
                    ChartType = chartType
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Registrar el error
                TempData["ErrorMessage"] = $"Error cargando datos: {ex.Message}";
                return View(new CustomerViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor complete todos los campos requeridos.";
                return RedirectToAction(nameof(Index));
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

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
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

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(UpdateCustomerViewModel form)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor complete todos los campos requeridos.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var command = new UpdateCustomerCommand
                {
                    Id = form.Id,
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

            return RedirectToAction(nameof(Index));
        }
    }
}
