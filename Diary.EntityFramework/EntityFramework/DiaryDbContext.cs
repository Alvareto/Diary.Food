using System.Data.Common;
using System.Data.Entity;
using Abp.Zero.EntityFramework;
using Diary.Authorization.Roles;
using Diary.Authorization.Users;
using Diary.Domain.Models;
using Diary.MultiTenancy;

namespace Diary.EntityFramework
{
    public class DiaryDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        //TODO: Define an IDbSet for your Entities...
        public virtual IDbSet<NutritionFact> NutritionFacts { get; set; }
        public virtual IDbSet<Ingredient> Ingredients { get; set; }
        public virtual IDbSet<Meal> Meals { get; set; }

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public DiaryDbContext()
            : base("Default")
        {
            this.Database.CommandTimeout = 60;
        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in DiaryDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of DiaryDbContext since ABP automatically handles it.
         */
        public DiaryDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        //This constructor is used in tests
        public DiaryDbContext(DbConnection existingConnection)
         : base(existingConnection, false)
        {

        }

        public DiaryDbContext(DbConnection existingConnection, bool contextOwnsConnection)
         : base(existingConnection, contextOwnsConnection)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ingredient>()
                .HasOptional<User>(s => s.ApproverUser)
                .WithMany()
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
