using Application.Service;
using Domain.Document;
using Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Transactions.Controllers
{
    [ApiController]
    [Route("")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly IUserResourceCrudService<Transaction> TransactionService;

        private readonly UserContext UserContext;

        public TransactionsController(IUserResourceCrudService<Transaction> transactionService, UserContext userContext)
        {
            TransactionService = transactionService;
            UserContext = userContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Transaction>>> ListForUser(string orderBy = "Id", int page = 1, int limit = 30)
        {
            return await TransactionService.GetListForUser(UserContext.UserId, null, orderBy, page, limit);
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> Save([FromBody] Transaction transaction)
        {
            var userId = UserContext.UserId;

            transaction.UserId = userId;

            return await TransactionService.Save(transaction);
        }
    }
}