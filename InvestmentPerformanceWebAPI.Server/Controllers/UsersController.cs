using InvestmentPerformanceWebAPI.Server.Database.Repository;
using InvestmentPerformanceWebAPI.Server.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentPerformanceWebAPI.Server.Controllers {
    [ApiController]
    [Route("api/users")]
    public sealed class UsersController : ControllerBase {
        private readonly IUserRepository _users;

        public UsersController(IUserRepository users) {
            _users = users;
        }

        // GET: /api/users
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken ct) {
            var list = await _users.GetAllAsync(ct);
            var dtos = list
                .Select(u => new UserDto {
                    Id = u.Id,
                    Name = u.Name
                })
                .ToList();

            return Ok(dtos);
        }

        // GET: /api/users/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById([FromRoute] int id, CancellationToken ct) {
            var user = await _users.GetByIdAsync(id, ct);
            if (user is null) {
                return NotFound();
            }

            return Ok(new UserDto {
                Id = user.Id,
                Name = user.Name
            });
        }
    }
}
