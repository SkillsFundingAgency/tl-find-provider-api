using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Controllers
{
    [ApiController]
    [Route("[controller]/api")]
    public class FindProvidersController : ControllerBase
    {
        private readonly IProviderDataService _providerDataService;
        private readonly ILogger<FindProvidersController> _logger;

        public FindProvidersController(
            IProviderDataService providerDataService,
            ILogger<FindProvidersController> logger)
        {
            _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Search for providers.
        /// </summary>
        /// <param name="postCode">Postcode that the search should start from.</param>
        /// <param name="qualificationId">Qualification id to filter by. Optional, defaults to null or zero.</param>
        /// <param name="page">Page to be displayed (zero-based).</param>
        /// <param name="pageSize">Number of items to return on a page.</param>
        /// <returns>Json with providers.</returns>
        [HttpGet]
        //[Route("providers/{postCode}/{qualificationId:int?}", Name = "GetProviders")]
        [Route("providers/{postCode}/{qualificationId?}", Name = "GetProviders")]
        [ProducesResponseType(typeof(IEnumerable<Models.Provider>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProviders(
            string postCode, 
            int? qualificationId = null, 
            [FromQuery] int page = 0, 
            [FromQuery] int pageSize = Constants.DefaultPageSize)
        {
            //TODO: Deal with exception or empty result from checking postcode - return a not found with "invalid postcode"?
            var providers = await _providerDataService.FindProviders(postCode);
            return providers != null
                ? Ok(providers)
                : NotFound();
        }

        /// <summary>
        /// Returns a list of all qualifications.
        /// </summary>
        /// <returns>Json with qualifications.</returns>
        [HttpGet]
        [Route("qualifications", Name = "GetQualifications")]
        [ProducesResponseType(typeof(IEnumerable<Qualification>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQualifications()
        {
            var qualifications = await _providerDataService.GetQualifications();
            return qualifications != null
                ? Ok(qualifications)
                : NotFound();
        }
    }
}
