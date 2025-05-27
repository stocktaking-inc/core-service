using System.ComponentModel.DataAnnotations;
using CoreService.Models;

namespace CoreService.DTOs
{
    public class ErrorDto
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Message { get; set; }
    }

    public class ErrorResponseDto : ErrorDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public static class ErrorDtoExtensions
    {
        public static Error ToModel(this ErrorDto errorDto)
        {
            return new Error
            {
                Code = errorDto.Code,
                Message = errorDto.Message
            };
        }

        public static ErrorResponseDto ToResponseDto(this Error error)
        {
            return new ErrorResponseDto
            {
                Id = error.Id,
                Code = error.Code,
                Message = error.Message,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}