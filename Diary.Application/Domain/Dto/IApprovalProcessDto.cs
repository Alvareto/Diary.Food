using Diary.Domain.Models;
using Diary.Users.Dto;

namespace Diary.Domain.Dto
{
    public interface IApprovalProcessDto
    {
        ApprovalStatus Status { get; set; }
        UserDto ApproverUser { get; set; }
        //long ApproverUserId { get; set; }
    }
}