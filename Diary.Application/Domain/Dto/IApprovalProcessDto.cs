using Diary.Domain.Models;

namespace Diary.Domain.Dto
{
    public interface IApprovalProcessDto
    {
        ApprovalStatus Status { get; set; }

        //long ApproverUserId { get; set; }
    }
}