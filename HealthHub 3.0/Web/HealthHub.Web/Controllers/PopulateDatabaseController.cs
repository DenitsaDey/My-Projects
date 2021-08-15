﻿namespace HealthHub.Web.Controllers
{
    using System.Threading.Tasks;

    using HealthHub.Services;
    using HealthHub.Services.Data.Clinics;
    using HealthHub.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;

    public class PopulateDatabaseController : BaseController
    {
        private readonly ICityAreasScraperService cityAreasScraperService;
        private readonly IInsuranceScraperService insuranceScraperService;
        private readonly IClinicsService clinicsService;
        private readonly IRatingPopulatingService ratingPopulatingService;

        public PopulateDatabaseController(
            ICityAreasScraperService cityAreasScraperService,
            IInsuranceScraperService insuranceScraperService,
            IClinicsService clinicsService,
            IRatingPopulatingService ratingPopulatingService)
        {
            this.cityAreasScraperService = cityAreasScraperService;
            this.insuranceScraperService = insuranceScraperService;
            this.clinicsService = clinicsService;
            this.ratingPopulatingService = ratingPopulatingService;
        }

        public IActionResult Index()
        {
            this.ratingPopulatingService.ImportRatings();

            var viewModel = new HeaderSearchQueryModel();
            viewModel.Clinics = this.clinicsService.GetAll();
            return this.View(viewModel);
        }

        public async Task<IActionResult> Add()
        {
            await this.ratingPopulatingService.ImportRatings();

            // await this.cityAreasScraperService.ImportCityAreas();
            // await this.insuranceScraperService.ImportInsuranceCompanies();
            return this.RedirectToAction("Index", "Home");
        }
    }
}
