using Abp.Events.Bus;

namespace Diary.Domain.Events
{
    public class EntityApprovedEventData<TEntity> : EventData
    {
        public TEntity Entity { get; set; }
    }
}