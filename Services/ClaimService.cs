// Returns only claims with a "Pending" status for the approver queue.
using ContractMonthlyClaim.Data;
using ContractMonthlyClaim.Models;
using Microsoft.EntityFrameworkCore;

namespace ContractMonthlyClaim.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ApplicationDBContext _context;

        public ClaimService(ApplicationDBContext context) => _context = context;

        public async Task<int> AddClaim(Claim claim)
        {
            await _context.Claims.AddAsync(claim);
            await _context.SaveChangesAsync();
            return claim.Id;
        }

        public async Task<bool> DeleteClaim(int claimId)
        {
            var claim = await _context.Claims.FirstOrDefaultAsync(x => x.Id == claimId);
            if (claim != null)
            {
                _context.Remove(claim);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Claim>> GetClaims()
        {
            return await _context.Claims.ToListAsync();
        }

        // --- NEW METHOD IMPLEMENTATIONS ---

        public async Task<List<Claim>> GetPendingClaims()
        {
            // Returns only the claims where the status is "Pending"
            return await _context.Claims.Where(c => c.Status == "Pending").ToListAsync();
        }

        public async Task<Claim?> GetClaimById(int claimId)
        {
            // Finds a single claim by its primary key (Id)
            return await _context.Claims.FindAsync(claimId);
        }

        public async Task<bool> UpdateClaimStatus(int claimId, string newStatus)
        {
            // Find the claim first
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
            {
                return false; // Claim not found
            }

            // Update its status and save the change
            claim.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateClaim(Claim updatedClaim)
        {
            var existingClaim = await _context.Claims.FindAsync(updatedClaim.Id);
            if (existingClaim == null)
            {
                return false;
            }

            // Copy properties from the updated model to the one from the database
            existingClaim.LecturerName = updatedClaim.LecturerName;
            existingClaim.Programme = updatedClaim.Programme;
            existingClaim.HoursWorked = updatedClaim.HoursWorked;
            existingClaim.HourlyRate = updatedClaim.HourlyRate;
            existingClaim.Amount = updatedClaim.Amount; // Make sure this is recalculated in the controller
            existingClaim.Notes = updatedClaim.Notes;
            existingClaim.Status = updatedClaim.Status;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Claim>> GetApprovedClaims()
        {
            return await _context.Claims
                .Where(c => c.Status == "Approved")
                .ToListAsync();
        }
        public async Task<List<Claim>> GetClaimsByLecturerAsync(string lecturerUsername)
        {
            return await _context.Claims
                .Where(c => c.LecturerName == lecturerUsername)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }
    }
}