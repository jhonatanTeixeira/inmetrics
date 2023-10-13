using Application.Service;
using Domain.Document;
using Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyCashFlow.Controllers
{
    [ApiController]
    [Route("")]
    [Authorize]
    public class DailyCashFlowController : ControllerBase
    {
        private UserContext UserContext;

        private IUserResourceCrudService<Domain.Document.DailyCashFlow> Service;

        public DailyCashFlowController(UserContext userContext, IUserResourceCrudService<Domain.Document.DailyCashFlow> service)
        {
            UserContext = userContext;
            Service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Domain.Document.DailyCashFlow>>> ListForUser(string orderBy = "Id", int page = 1, int limit = 30)
        {
            return await Service.GetListForUser(UserContext.UserId, null, orderBy, page, limit);
        }
    }
}