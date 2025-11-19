// FILE: Services/IClaimService.cs
using ContractMonthlyClaim.Models;

namespace ContractMonthlyClaim.Services
{
    public interface IClaimService
    {
        Task<List<Claim>> GetClaims();
        Task<int> AddClaim(Claim claim);
        Task<bool> DeleteClaim(int claimId);

        // --- NEW METHODS ---
        Task<List<Claim>> GetPendingClaims();
        Task<Claim?> GetClaimById(int claimId);
        Task<bool> UpdateClaimStatus(int claimId, string newStatus);
        Task<bool> UpdateClaim(Claim updatedClaim);
        Task<List<Claim>> GetApprovedClaims();
        Task<List<Claim>> GetClaimsByLecturerAsync(string lecturerUsername);
    }
}