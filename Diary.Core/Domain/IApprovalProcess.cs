using Diary.Authorization.Users;
using Diary.Domain.Models;

namespace Diary.Domain
{
    public interface IApprovalProcess
    {
        ApprovalStatus Status { get; set; }
        
        User ApproverUser { get; set; }

        void Approve();
        void Reject();
    }
}