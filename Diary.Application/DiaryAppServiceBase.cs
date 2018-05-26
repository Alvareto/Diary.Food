using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Diary.Authorization.Users;
using Diary.MultiTenancy;
using Microsoft.AspNet.Identity;

namespace Diary
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class DiaryAppServiceBase : ApplicationService, IDiaryAppService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected DiaryAppServiceBase()
        {
            LocalizationSourceName = DiaryConsts.LocalizationSourceName;
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }

    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class DiaryAppServiceBase<TEntity, TEntityDto>
        : AsyncCrudAppService<TEntity, TEntityDto>
            where TEntity : class, IEntity
            where TEntityDto : IEntityDto //ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected DiaryAppServiceBase(IRepository<TEntity> repository)
            : base(repository)
        {
            LocalizationSourceName = DiaryConsts.LocalizationSourceName;
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        protected virtual List<TEntity> MapToEntityList(List<TEntityDto> input)
        {
            return ObjectMapper.Map<List<TEntity>>(input);
        }

        protected virtual List<TEntityDto> MapToEntityDtoList(List<TEntity> input)
        {
            return ObjectMapper.Map<List<TEntityDto>>(input);
        }
    }
}