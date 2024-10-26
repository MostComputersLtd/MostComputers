using System.Collections.Concurrent;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;

namespace MOSTComputers.Services.TransactionalFileManagement.Services;

public class EnlistmentManager : IEnlistmentManager
{
    private readonly ConcurrentDictionary<Guid, TransactionalFileManagementEnlistmentNotification> _enlistmentNotificationDictionary = new();

    public IReadOnlyDictionary<Guid, TransactionalFileManagementEnlistmentNotification> EnlistmentNotificationDict => _enlistmentNotificationDictionary;

    public virtual Guid AddNotification(TransactionalFileManagementEnlistmentNotification notification)
    {
        Guid guid = Guid.NewGuid();

        while (_enlistmentNotificationDictionary.ContainsKey(guid))
        {
            guid = Guid.NewGuid();
        }

        _enlistmentNotificationDictionary.TryAdd(guid, notification);

        return guid;
    }

    public virtual bool TryRemoveNotification(Guid id)
    {
        foreach (KeyValuePair<Guid, TransactionalFileManagementEnlistmentNotification> kvp in _enlistmentNotificationDictionary)
        {
            if (kvp.Key == id)
            {
                return _enlistmentNotificationDictionary.TryRemove(kvp);
            }
        }

        return false;
    }

    public virtual bool TryRemoveNotification(TransactionalFileManagementEnlistmentNotification enlistmentNotification)
    {
        foreach (KeyValuePair<Guid, TransactionalFileManagementEnlistmentNotification> kvp in _enlistmentNotificationDictionary)
        {
            if (kvp.Value == enlistmentNotification)
            {
                return _enlistmentNotificationDictionary.TryRemove(kvp);
            }
        }

        return false;
    }
}