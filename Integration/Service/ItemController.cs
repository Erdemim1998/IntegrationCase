using Integration.Backend;
using Integration.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Integration.Service
{
    public class ItemController:ControllerBase
    {
        private readonly ItemIntegrationContext _context;

        public ItemController(ItemIntegrationContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<string> CreateItem(string content)
        {
            try
            {
                // Check for existing item
                var existingItem = await _context.Items.FirstOrDefaultAsync(i => i.Content == content);

                if (existingItem != null)
                {
                    return $"Creating record '{content}' has already exists.";
                }

                // Create a new item
                var newItem = new Item { Content = content };
                _context.Items.Add(newItem);
                await _context.SaveChangesAsync();

                return "The record was saved successfully. Id:" + newItem.Id;
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
