﻿namespace HealthHub.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using HealthHub.Data.Models;
    using HealthHub.Services;
    using HealthHub.Services.Data;
    using HealthHub.Web.ViewModels.Appointment;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class AppointmentController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDateTimeParserService dateTimeParserService;
        private readonly IServicesService servicesService;
        private readonly IAppointmentsService appointmentService;

        public AppointmentController(
            UserManager<ApplicationUser> userManger,
            IDateTimeParserService dateTimeParserService,
            IServicesService servicesService,
            IAppointmentsService appointmentService)
        {
            this.userManager = userManger;
            this.dateTimeParserService = dateTimeParserService;
            this.servicesService = servicesService;
            this.appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            var patient = await this.userManager.GetUserAsync(this.HttpContext.User);
            var patientId = await this.userManager.GetUserIdAsync(patient);

            var viewModel = new AppointmentListViewModel
            {
                AppointmentList = this.appointmentService.GetUpcomingByPatient(patientId),
            };

            return this.View(viewModel);
        }

        public IActionResult Book()
        {
            var viewModel = new AppointmentInputModel();
            viewModel.ServicesItems = this.servicesService.GetAllServices();
            return this.View(viewModel);
        }

        [HttpPost]
        public IActionResult Book(AppointmentInputModel input)
        {
            //TODO if user is not signed in redirect to login page
            if(!this.servicesService.GetAllServices().Any(s=>s.Id == input.ServiceId))
            {
                this.ModelState.AddModelError(nameof(input.ServiceId), "Service does not exist.");
            }

            if (!this.ModelState.IsValid)
            {
                input.ServicesItems = this.servicesService.GetAllServices();
                return this.View(input);
            }

            //TODO: patient and doctor Id
            //var patientId = this.UserManager.GetId();
            //var doctorId = ...

             //this.appointmentService.AddAppointment(input, doctorId, patientId)
            //TODO return message "You have successfully requested an appointment"
            return this.Redirect("/Appointment/All");
        }

        public IActionResult All(string patientId)
        {
            var viewModel = new AppointmentListViewModel();
            viewModel.AppointmentList = this.appointmentService.GetUpcomingByPatient(patientId);
            return this.View(viewModel);
        }

        public async Task<IActionResult> Details(string appointmentId)
        {
            var model = await this.appointmentService.GetByIdAsync(appointmentId);
            return this.View(model);
        }

        public async Task<IActionResult> Reschedule(string appointmentId, string doctorId)
        {
            var model = await this.appointmentService.GetByIdAsync(appointmentId);
            return this.Redirect($"Doctors/doctorId_{doctorId}");
        }

        public IActionResult Cancel(string appointmentId)
        {
            this.appointmentService.ChangeAppointmentStatusAsync(appointmentId, "Cancelled");
            return this.Redirect("/Appointment/All");
        }

        public IActionResult Edit()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Edit(string appointmentId, string message)
        {
            //TODO if user is not signed in redirect to login page

            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.appointmentService.EditMessageAsync(appointmentId, message);
            //TODO return message "You have successfully edited your appointment"
            return this.Redirect("/Appointment/All");
        }
    }
}
