namespace MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;

public interface IEnlistmentManager
{
    IReadOnlyDictionary<Guid, TransactionalFileManagementEnlistmentNotification> EnlistmentNotificationDict { get; }

    Guid AddNotification(TransactionalFileManagementEnlistmentNotification notification);
    bool TryRemoveNotification(Guid id);
    bool TryRemoveNotification(TransactionalFileManagementEnlistmentNotification enlistmentNotification);
}