using System;
using PROG7311_POE.Models;

namespace PROG7311_POE.Services
{
    // helper class, sends notifications here
    // NO FUNCTIONALITY YET
    public class EmailNotificationService : INotificationService
    {
        // checks if contracts status changed
        public void NotifyContractStatusChanged(Contract contract, ContractStatus oldStatus, ContractStatus newStatus)
        {
            // creates message about what changed
            var message = $"[NOTIFICATION] Contract #{contract.Id} for client #{contract.ClientId} changed from {oldStatus} to {newStatus}.";
            //outputs message to console currently
            Console.WriteLine(message);
        }
    }
}