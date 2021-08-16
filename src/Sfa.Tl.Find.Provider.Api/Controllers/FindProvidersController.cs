using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Models.Exceptions;

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
        /// <param name="postcode">Postcode that the search should start from.</param>
        /// <param name="qualificationId">Qualification id to filter by. Optional, defaults to null or zero.</param>
        /// <param name="page">Page to be displayed (zero-based).</param>
        /// <param name="pageSize">Number of items to return on a page.</param>
        /// <returns>Json with providers.</returns>
        [HttpGet]
        [Route("providers/{postcode}", Name = "GetProviders")]
        [ProducesResponseType(typeof(IEnumerable<ProviderSearchResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProviders(
            string postcode,
            [FromQuery] int? qualificationId = null,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = Constants.DefaultPageSize)
        {
            _logger.LogDebug($"GetProviders called with qualificationId={qualificationId}, " +
                             $"page={page}, pageSize={pageSize}");

            try
            {
                var providers = await _providerDataService.FindProviders(postcode);
                return providers != null
                    ? Ok(providers)
                    : NotFound();
            }
            catch (PostcodeNotFoundException pex)
            {
                _logger.LogError(pex, $"Postcode {pex.Postcode} was not found. Returning a Not Found result.");
                return NotFound(pex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred. Returning error result.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
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
