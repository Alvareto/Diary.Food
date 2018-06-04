using System.ServiceModel;

namespace Diary.Domain
{
    [ServiceContract]
    public interface IApprovalService
    {
        [OperationContract(IsOneWay = true)]
        void Approve(int id);
        [OperationContract(IsOneWay = true)]
        void Reject(int id);
    }
}