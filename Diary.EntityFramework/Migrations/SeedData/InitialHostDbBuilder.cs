using Diary.EntityFramework;
using EntityFramework.DynamicFilters;

namespace Diary.Migrations.SeedData
{
    public class InitialHostDbBuilder
    {
        private readonly DiaryDbContext _context;

        public InitialHostDbBuilder(DiaryDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            _context.DisableAllFilters();

            new DefaultEditionsCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
        }
    }
}
