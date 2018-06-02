using System.Collections.Generic;

namespace Diary.Domain
{
    public interface IHistory<T>
    { // TODO: tracked properties only, add atribute to mark tracked properties, ...
        ICollection<T> History { get; set; }
    }
}