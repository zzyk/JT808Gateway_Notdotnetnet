using MediatR;

namespace JT808Server.WinForm.MediatorExpand
{
    public sealed class DefaultMediator : Mediator
    {
        public DefaultMediator(ServiceFactory serviceFactory) : base(serviceFactory) { }
        //PS: 如果此处整体是不等待，那么各个Handler中所依赖的服务要么是单例 要么重新创建作用域进行解析 否则会随着当前作用域释放会出现错误。（此处是可等待可忽略）
        protected override Task PublishCore(IEnumerable<Func<INotification, CancellationToken, Task>> allHandlers, INotification notification, CancellationToken cancellationToken)
        {
            List<Task> tasks = new();
            foreach (var handler in allHandlers)
            {
                tasks.Add(handler(notification, cancellationToken));
            }
            return Task.WhenAll(tasks);
        }
    }
}