using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApi.Controllers;
using TodoApi.Dtos;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Tests.UnitTests
{
    public class TodoItemsControllerTests
    {
        private readonly TodoItemsController _cut;
        private readonly Mock<ITodoItemsRepository> _itemRepoMock = new Mock<ITodoItemsRepository>();
        public TodoItemsControllerTests()
        {
            _cut = new TodoItemsController(_itemRepoMock.Object);
        }

        [Fact]
        public async Task GetTodoItem_ShouldReturnItem_IfItemExists()
        {
            // Arrange
            var itemId = 3;
            var itemName = "Mocked Item";
            var itemIsCompleted = false;

            _itemRepoMock.Setup(x => x.GetByIdAsync(itemId))
                .ReturnsAsync(new TodoItem { Id = itemId, Name = itemName, IsComplete = itemIsCompleted });

            // Act
            var item = await _cut.GetTodoItem(itemId);

            // Assert
            Assert.Equal(itemId, item.Value?.Id);
            Assert.Equal(itemName, item.Value?.Name);
            Assert.Equal(itemIsCompleted, item.Value?.IsComplete);
        }

        [Fact]
        public async Task GetTodoItem_ShouldReturnNull_WhenItemDoesntExist()
        {
            // Arrange
            var rnd = new Random();
            var id = rnd.Next(1, 999);

            _itemRepoMock.Setup(x => x.GetByIdAsync( It.IsAny<long>() ))
                .ReturnsAsync( () => null );

            // Act
            var item = await _cut.GetTodoItem(id);

            // Assure
            Assert.Null(item.Value);
        }

        [Fact]
        public async Task GetTodoItems_ShouldReturnItemList_IfItemsExist()
        {
            // Arrange
            var totalItems = 2;
            var mockedItemsList = new List<TodoItem>();

            for (int i = 0; i < totalItems; i++)
            {
                mockedItemsList.Add(new TodoItem
                {
                    Id = i + 1,
                    Name = "Item " + (i + 1).ToString(),
                    IsComplete = true
                });
            }

            _itemRepoMock.Setup( x => x.GetAllAsync() )
                .ReturnsAsync( mockedItemsList );

            // Act
            var itemList = (await _cut.GetTodoItems()).Value;

            // Assert
            Assert.Equal(totalItems, itemList?.Count());

            for (int i = 0; i < totalItems; i++)
            {
                Assert.Contains(itemList, item => item.Id == mockedItemsList[i].Id);
                Assert.Contains(itemList, item => item.Name == mockedItemsList[i].Name);
                Assert.Contains(itemList, item => item.IsComplete == mockedItemsList[i].IsComplete);
            }
        }

        [Fact]
        public async Task GetTodoItems_ShouldReturnEmptyList_IfItemsDontExist()
        {
            // Arrange
            _itemRepoMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync( new List<TodoItem>() );

            // Act
            var itemList = (await _cut.GetTodoItems()).Value;

            // Assert
            Assert.Empty(itemList);
        }

        [Fact]
        public async Task UpdateTodoItem_ShouldUpdateItem_IfItemExists()
        {
            // Arrange
            var mockedId = 1;
            var mockedName = "Mocked Name";
            var mockedIsComplete = false;

            var mockedTodoItem = new TodoItem 
            { 
                Id = mockedId, 
                Name = mockedName, 
                IsComplete = mockedIsComplete 
            };


            var updatedName = "Update Name";
            var updatedIsComplete = true;

            var updatedTodoItem = new TodoItemDTO
            {
                Id = mockedId,
                Name = updatedName,
                IsComplete = updatedIsComplete
            };


            _itemRepoMock.Setup(x => x.GetByIdAsync(mockedId))
                .ReturnsAsync(mockedTodoItem);

            _itemRepoMock.Setup(x => x.UpdateOneAsync( It.IsAny<TodoItem>() ))
                .Verifiable();


            // Act
            await _cut.UpdateTodoItem(mockedId, updatedTodoItem);

            // Assert
            _itemRepoMock.Verify(x => x.UpdateOneAsync(It.IsAny<TodoItem>()), Times.Once());
        }

        [Fact]
        public async Task UpdateTodoItem_ShouldNotUpdateItem_IfItemDoesntExist()
        {
            // Arrange
            var id = 1;
            var updatedName = "Update Name";
            var updatedIsComplete = true;

            var updatedTodoItem = new TodoItemDTO
            {
                Id = id,
                Name = updatedName,
                IsComplete = updatedIsComplete
            };

            _itemRepoMock.Setup(x => x.GetByIdAsync( It.IsAny<long>() ))
                .ReturnsAsync( () => null );

            _itemRepoMock.Setup(x => x.UpdateOneAsync( It.IsAny<TodoItem>() ))
                .Verifiable();

            // Act
            await _cut.UpdateTodoItem(id, updatedTodoItem);

            // Assert
            _itemRepoMock.Verify(x => x.UpdateOneAsync(It.IsAny<TodoItem>()), Times.Never());
        }

        [Fact]
        public async Task CreateTodoItem_ShouldCreateTodoItem_GivenValidInput()
        {
            // Arrange
            var name = "Random Name";
            var isComplete = false;

            var newItemDTO = new TodoItemDTO
            {
                Name = name,
                IsComplete = isComplete
            };

            _itemRepoMock.Setup(x => x.CreateOneAsync( It.IsAny<TodoItem>() ))
                .Verifiable();

            // Act
            var createdItem = ((await _cut.CreateTodoItem(newItemDTO)).Result as CreatedAtActionResult)!.Value as TodoItemDTO;
            
            // Assert
            _itemRepoMock.Verify(x => x.CreateOneAsync(It.IsAny<TodoItem>()), Times.Once());
            Assert.NotNull(createdItem);
            Assert.Equal(name, createdItem!.Name);
            Assert.Equal(isComplete, createdItem!.IsComplete);
        }

        [Fact]
        public async Task CreateTodoItem_ShouldReturnNull_GivenInvalidInput()
        {
            // Arrange
            var newItemDTO = new TodoItemDTO();

            _itemRepoMock.Setup(x => x.CreateOneAsync(It.IsAny<TodoItem>()))
                .Verifiable();

            // Act
            var response = (await _cut.CreateTodoItem(newItemDTO)).Result as BadRequestResult;
            
            // Assert
            Assert.Equal(400, response!.StatusCode);
            _itemRepoMock.Verify(x => x.CreateOneAsync(It.IsAny<TodoItem>()), Times.Never());
        }

        [Fact]
        public async Task DeleteTodoItem_ShouldDeleteItem_IfItemExists()
        {
            // Arrange
            var mockedId = 1;
            var mockedName = "Mocked Name";
            var mockedIsComplete = false;

            var mockedTodoItem = new TodoItem
            {
                Id = mockedId,
                Name = mockedName,
                IsComplete = mockedIsComplete
            };

            _itemRepoMock.Setup(x => x.GetByIdAsync(mockedId))
                .ReturnsAsync(mockedTodoItem);

            _itemRepoMock.Setup(x => x.DeleteAsync(mockedTodoItem))
                .Verifiable();

            // Act
            var result = await _cut.DeleteTodoItem(mockedId);

            // Assert
            _itemRepoMock.Verify(x => x.DeleteAsync(It.IsAny<TodoItem>()), Times.Once());
        }

        [Fact]
        public async Task DeleteTodoItem_ShouldNotDeleteItem_IfItemDoesntExist()
        {
            // Arrange
            var mockedId = 1;
            var mockedName = "Mocked Name";
            var mockedIsComplete = false;

            var mockedTodoItem = new TodoItem
            {
                Id = mockedId,
                Name = mockedName,
                IsComplete = mockedIsComplete
            };

            _itemRepoMock.Setup(x => x.GetByIdAsync( It.IsAny<long>() ))
                .ReturnsAsync( () => null );

            _itemRepoMock.Setup(x => x.DeleteAsync(mockedTodoItem))
                .Verifiable();

            // Act
            await _cut.DeleteTodoItem(mockedId);

            // Assert
            _itemRepoMock.Verify(x => x.DeleteAsync(It.IsAny<TodoItem>()), Times.Never());
        }
    }
}
