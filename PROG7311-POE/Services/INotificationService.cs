using PROG7311_POE.Models;

namespace PROG7311_POE.Services
{
    // checks when a contracts status has changed
    public interface INotificationService
    {
        // called when the status has changed
        void NotifyContractStatusChanged(Contract contract, ContractStatus oldStatus, ContractStatus newStatus);
    }
}