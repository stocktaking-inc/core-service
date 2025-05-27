using System.ComponentModel.DataAnnotations;
using CoreService.Models;

namespace CoreService.DTOs
{
    public class EmailRequestDto
    {
        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }
    }

    public class EmailRequestResponseDto : EmailRequestDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public EmailStatus Status { get; set; }
    }

    public static class EmailRequestDtoExtensions
    {
        public static EmailRequest ToModel(this EmailRequestDto emailRequestDto)
        {
            return new EmailRequest
            {
                Subject = emailRequestDto.Subject,
                Body = emailRequestDto.Body
            };
        }

        public static EmailRequestResponseDto ToResponseDto(this EmailRequest emailRequest)
        {
            return new EmailRequestResponseDto
            {
                Id = emailRequest.Id,
                Subject = emailRequest.Subject,
                Body = emailRequest.Body,
                CreatedAt = emailRequest.CreatedAt,
                Status = emailRequest.Status
            };
        }
    }
}