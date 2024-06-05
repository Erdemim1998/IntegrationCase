using Integration.Common;
using Integration.Backend;
using System.Collections.Concurrent;

namespace Integration.Service;

public sealed class ItemIntegrationService
{
    //This is a dependency that is normally fulfilled externally.
    private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();
    private readonly object _lock = new object();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(100); // adjust the count to control parallelism
    private readonly Dictionary<string, bool> _itemsBeingSaved = new Dictionary<string, bool>();

    public async Task<Result> SaveItem(string itemContent)
    {
        await _semaphore.WaitAsync();
        try
        {
            lock (_lock)
            {
                if (_itemsBeingSaved.ContainsKey(itemContent))
                {
                    return new Result(false, $"Item with content {itemContent} is being saved by another thread.");
                }

                _itemsBeingSaved.Add(itemContent, true);
            }

            try
            {
                // Check the backend to see if the content is already saved.
                if (ItemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0)
                {
                    return new Result(false, $"Duplicate item received with content {itemContent}.");
                }

                var item = ItemIntegrationBackend.SaveItem(itemContent);

                return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
            }

            finally
            {
                lock (_lock)
                {
                    _itemsBeingSaved.Remove(itemContent);
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public List<Item> GetAllItems()
    {
        return ItemIntegrationBackend.GetAllItems();
    }
}