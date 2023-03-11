using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infractructure.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();
        public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
        {
            if (_handlers.ContainsKey(typeof(T))) throw new IndexOutOfRangeException("command twice");

            _handlers.Add(typeof(T), x => handler((T)x));
        }

        public async Task SendAsync(BaseCommand cmd)
        {
            if (_handlers.TryGetValue(cmd.GetType(), out Func<BaseCommand, Task> handler))
                await handler(cmd);
            else
                throw new ArgumentNullException("no cmd handler");
        }
    }
}
