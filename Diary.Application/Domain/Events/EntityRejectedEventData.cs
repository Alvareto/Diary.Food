using Abp.Events.Bus;

namespace Diary.Domain.Events
{
    public class EntityRejectedEventData<TEntity> : EventData
    {
        public TEntity Entity { get; set; }
    }
}