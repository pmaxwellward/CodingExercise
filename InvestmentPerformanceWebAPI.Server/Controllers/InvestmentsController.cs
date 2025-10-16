using InvestmentPerformanceWebAPI.Server.Services;  
using InvestmentPerformanceWebAPI.Server.DTOs;     
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace InvestmentPerformanceWebAPI.Server.Controllers {
    [ApiController]
    [Route("api/users/{userId:int}/investments")]
    public sealed class InvestmentsController : ControllerBase {
        private readonly IInvestmentQueryService _service;

        public InvestmentsController(IInvestmentQueryService service) {
            _service = service;
        }

        // GET: /api/users/{userId}/investments
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<InvestmentListItemDto>>> GetInvestments(
            [FromRoute] int userId,
            CancellationToken ct) {
            IReadOnlyList<InvestmentListItemDto> list = await _service.GetInvestmentsForUserAsync(userId, ct);
            return Ok(list);
        }

        // GET: /api/users/{userId}/investments/{investmentId}
        [HttpGet("{investmentId:int}")]
        public async Task<ActionResult<InvestmentDetailsDto>> GetInvestmentDetails(
            [FromRoute] int userId,
            [FromRoute] int investmentId,
            CancellationToken ct) {
            InvestmentDetailsDto? dto = await _service.GetInvestmentDetailsAsync(userId, investmentId, ct);
            if (dto is null) {
                return NotFound();
            }

            return Ok(dto);
        }
    }
}
