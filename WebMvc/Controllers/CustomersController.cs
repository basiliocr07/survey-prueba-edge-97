using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        private static readonly List<string> _availableServices = new List<string>
        {
            "Basic Survey",
            "Premium Survey",
            "Analytics Dashboard",
            "API Access",
            "Custom Branding",
            "Advanced Reports",
            "Email Marketing",
            "Data Export"
        };

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var serviceUsage = await _customerService.GetServiceUsageAnalyticsAsync();

            var viewModel = new CustomerGrowthViewModel
            {
                Customers = customers.Select(c => new CustomerViewModel
                {
                    Id = c.Id,
                    BrandName = c.BrandName,
                    ContactEmail = c.ContactEmail,
                    ContactPhone = c.ContactPhone,
                    AcquiredServices = c.AcquiredServices,
                    CreatedAt = c.CreatedAt,
                    GrowthMetrics = c.GrowthMetrics?.Select(m => new GrowthMetricViewModel
                    {
                        Period = m.Period,
                        Revenue = m.Revenue,
                        UserCount = m.UserCount
                    }).ToList() ?? new List<GrowthMetricViewModel>()
                }).ToList(),
                ServiceUsageData = serviceUsage.Select(kv => new ServiceUsageViewModel
                {
                    Name = kv.Key,
                    Count = kv.Value
                }).OrderByDescending(s => s.Count).ToList(),
                AvailableServices = _availableServices
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Growth()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var serviceUsage = await _customerService.GetServiceUsageAnalyticsAsync();

            var viewModel = new CustomerGrowthViewModel
            {
                Customers = customers.Select(c => new CustomerViewModel
                {
                    Id = c.Id,
                    BrandName = c.BrandName,
                    ContactEmail = c.ContactEmail,
                    ContactPhone = c.ContactPhone,
                    AcquiredServices = c.AcquiredServices,
                    CreatedAt = c.CreatedAt,
                    GrowthMetrics = c.GrowthMetrics?.Select(m => new GrowthMetricViewModel
                    {
                        Period = m.Period,
                        Revenue = m.Revenue,
                        UserCount = m.UserCount
                    }).ToList() ?? new List<GrowthMetricViewModel>()
                }).ToList(),
                ServiceUsageData = serviceUsage.Select(kv => new ServiceUsageViewModel
                {
                    Name = kv.Key,
                    Count = kv.Value
                }).OrderByDescending(s => s.Count).ToList(),
                AvailableServices = _availableServices
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new CreateCustomerViewModel
            {
                AvailableServices = _availableServices
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.AvailableServices = _availableServices;
                return View(viewModel);
            }

            var createCustomerDto = new CreateCustomerDto
            {
                BrandName = viewModel.BrandName,
                ContactEmail = viewModel.ContactEmail,
                ContactPhone = viewModel.ContactPhone,
                AcquiredServices = viewModel.SelectedServices
            };

            await _customerService.CreateCustomerAsync(createCustomerDto);

            TempData["SuccessMessage"] = $"Customer {viewModel.BrandName} has been added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);

                var viewModel = new EditCustomerViewModel
                {
                    Id = customer.Id,
                    BrandName = customer.BrandName,
                    ContactEmail = customer.ContactEmail,
                    ContactPhone = customer.ContactPhone,
                    SelectedServices = customer.AcquiredServices,
                    AvailableServices = _availableServices
                };

                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCustomerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.AvailableServices = _availableServices;
                return View(viewModel);
            }

            var updateCustomerDto = new CreateCustomerDto
            {
                BrandName = viewModel.BrandName,
                ContactEmail = viewModel.ContactEmail,
                ContactPhone = viewModel.ContactPhone,
                AcquiredServices = viewModel.SelectedServices
            };

            try
            {
                await _customerService.UpdateCustomerAsync(viewModel.Id, updateCustomerDto);
                TempData["SuccessMessage"] = $"Customer {viewModel.BrandName} has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                viewModel.AvailableServices = _availableServices;
                return View(viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(id);
                TempData["SuccessMessage"] = "Customer has been deleted successfully.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "Customer not found.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting customer: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
