using Domain.Document;
using Domain.Repository;

namespace Application.Service
{
    public class DailyCashFlowService : BaseUserResourceCrudService<DailyCashFlow>
    {
        public DailyCashFlowService(IRepository<DailyCashFlow> repository) : base(repository)
        {
        }

        public override async Task<DailyCashFlow> Save(DailyCashFlow document)
        {
            var dcf = await Repository.FindOne(d => d.UserId == document.UserId && d.Date == document.Date);
            var previowsDcf = await Repository.FindOne(d => d.UserId == document.UserId && d.Date == document.Date.AddDays(-1));

            if (previowsDcf != null) {
                document.Amount += previowsDcf.Amount;
            }

            if (dcf == null) {
                return await Repository.Save(document);
            }

            document.Id = dcf.Id;

            return await Repository.Update(document);
        }
    }
}