using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using Diary.EntityFramework;

namespace Diary.Migrator
{
    [DependsOn(typeof(DiaryDataModule))]
    public class DiaryMigratorModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer<DiaryDbContext>(null);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}