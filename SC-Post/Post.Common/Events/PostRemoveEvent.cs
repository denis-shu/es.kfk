using CQRS.Core.Events;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Common.Events
{
    public class PostRemoveEvent: BaseEvent
    {
        public PostRemoveEvent(): base(nameof(PostRemoveEvent))
        {

        }
    }
}
