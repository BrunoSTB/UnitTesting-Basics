using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos;

namespace TodoApi.Controllers
{
    public interface ITodoItemsController
    {
        Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems();
        Task<ActionResult<TodoItemDTO>> GetTodoItem(long id);
        Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO);
        Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO);
        Task<IActionResult> DeleteTodoItem(long id);
    }
}
