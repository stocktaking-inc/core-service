using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoreService.Models;
using CoreService.Interfaces;
using CoreService.DTOs;

[ApiController]
[Route("items")]
//[Authorize]
public class ItemController : ControllerBase
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<ItemController> _logger;

    public ItemController(IItemRepository itemRepository, ILogger<ItemController> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemResponseDto>>> GetAllItems(
        [FromQuery] int page = 1, 
        [FromQuery] int limit = 10)
    {
        try
        {
            var items = await _itemRepository.GetAllAsync(page, limit);
            var response = items.Select(i => i.ToResponseDto()).ToList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all items");
            return BadRequest(new ErrorResponseDto {
                Id = 0,
                Code = "SERVER_ERROR",
                Message = "Error retrieving items",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemResponseDto>> GetItemDetails(int id)
    {
        try
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
            {
                return BadRequest(new ErrorResponseDto {
                    Id = 0,
                    Code = "NOT_FOUND",
                    Message = "Item not found",
                    Timestamp = DateTime.UtcNow
                });
            }
            return Ok(item.ToResponseDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting item with id {id}");
            return BadRequest(new ErrorResponseDto {
                Id = 0,
                Code = "SERVER_ERROR",
                Message = "Error retrieving item",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ItemResponseDto>> AddItem([FromBody] ItemDto itemDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Error { 
                    Code = "INVALID_REQUEST", 
                    Message = ModelState.Values.First().Errors.First().ErrorMessage 
                });
            }

            var item = itemDto.ToModel();
            await _itemRepository.AddAsync(item);
            
            return StatusCode(201, item.ToResponseDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding new item");
            return BadRequest(new ErrorResponseDto {
                Id = 0,
                Code = "SERVER_ERROR",
                Message = "Error adding item",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ItemResponseDto>> UpdateItem(int id, [FromBody] ItemDto itemDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Error { 
                    Code = "INVALID_REQUEST", 
                    Message = ModelState.Values.First().Errors.First().ErrorMessage 
                });
            }

            var existingItem = await _itemRepository.GetByIdAsync(id);
            if (existingItem == null)
            {
                return BadRequest(new ErrorResponseDto {
                    Id = 0,
                    Code = "NOT_FOUND",
                    Message = "Item not found",
                    Timestamp = DateTime.UtcNow
                });
            }

            existingItem.Name = itemDto.Name;
            existingItem.Article = itemDto.Article;
            existingItem.Category = itemDto.Category;
            existingItem.Quantity = itemDto.Quantity;
            existingItem.Location = itemDto.Location;
            existingItem.Status = itemDto.Status;

            await _itemRepository.UpdateAsync(existingItem);
            return Ok(existingItem.ToResponseDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating item with id {id}");
            return BadRequest(new ErrorResponseDto {
                    Id = 0,
                    Code = "SERVER_ERROR",
                    Message = "Error updating item",
                    Timestamp = DateTime.UtcNow
                });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        try
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
            {
                return BadRequest(new ErrorResponseDto {
                    Id = 0,
                    Code = "NOT_FOUND",
                    Message = "Item not found",
                    Timestamp = DateTime.UtcNow
                });
            }

            await _itemRepository.DeleteAsync(item);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting item with id {id}");
            return BadRequest(new ErrorResponseDto {
                    Id = 0,
                    Code = "SERVER_ERROR",
                    Message = "Error deleting item",
                    Timestamp = DateTime.UtcNow
                });
        }
    }
}