using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface ITodoItemsRepository
    {
        Task<List<TodoItem>> GetAllAsync();
        Task<TodoItem?> GetByIdAsync(long id);
        Task UpdateOneAsync(TodoItem item);
        Task DeleteAsync(TodoItem item);
        Task CreateOneAsync(TodoItem item);
        bool TodoItemExists(long id);
    }
}
