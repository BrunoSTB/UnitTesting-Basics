using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public class TodoItemsRepository : ITodoItemsRepository
    {
        private readonly TodoContext _context;

        public TodoItemsRepository(TodoContext context)
        {
            _context = context;
        }

        public async Task CreateOneAsync(TodoItem item)
        {
            _context.TodoItems
                .Add(item);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TodoItem item)
        {
            _context.TodoItems
                .Remove(item);

            await _context.SaveChangesAsync();
        }

        public async Task<List<TodoItem>> GetAllAsync()
        {
            return await _context.TodoItems
                .ToListAsync();
        }

        public async Task<TodoItem?> GetByIdAsync(long id)
        {
            return await _context.TodoItems
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

        public async Task UpdateOneAsync(TodoItem item)
        {
            _context.TodoItems
                .Update(item);

            await _context.SaveChangesAsync();
        }
    }
}
