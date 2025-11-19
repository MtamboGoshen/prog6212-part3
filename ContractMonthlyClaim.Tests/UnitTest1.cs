using Xunit;
using ContractMonthlyClaim.Data;
using ContractMonthlyClaim.Models;
using ContractMonthlyClaim.Services;
using Microsoft.EntityFrameworkCore;

namespace ContractMonthlyClaim.Tests
{
    public class ClaimServiceTests
    {
        private readonly DbContextOptions<ApplicationDBContext> _dbContextOptions;

        public ClaimServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        // --- EXISTING PART 2 TESTS ---

        [Fact]
        public async Task GetPendingClaims_ShouldReturnOnlyPendingClaims()
        {
            await using var context = new ApplicationDBContext(_dbContextOptions);
            context.Claims.AddRange(
                new Claim { Status = "Pending" },
                new Claim { Status = "Approved" },
                new Claim { Status = "Pending" }
            );
            await context.SaveChangesAsync();
            var service = new ClaimService(context);

            var result = await service.GetPendingClaims();

            Assert.Equal(2, result.Count);
            Assert.All(result, claim => Assert.Equal("Pending", claim.Status));
        }

        [Fact]
        public async Task AddClaim_ShouldAddClaimToDatabase()
        {
            await using var context = new ApplicationDBContext(_dbContextOptions);
            var service = new ClaimService(context);
            var newClaim = new Claim { LecturerName = "Test Lecturer", Status = "Pending" };

            await service.AddClaim(newClaim);

            Assert.Equal(1, await context.Claims.CountAsync());
            Assert.Equal("Test Lecturer", (await context.Claims.FirstAsync()).LecturerName);
        }

        [Fact]
        public async Task UpdateClaimStatus_ShouldChangeStatusToApproved()
        {
            await using var context = new ApplicationDBContext(_dbContextOptions);
            var testClaim = new Claim { Id = 1, Status = "Pending" };
            context.Claims.Add(testClaim);
            await context.SaveChangesAsync();
            var service = new ClaimService(context);

            await service.UpdateClaimStatus(1, "Approved");

            var updatedClaim = await context.Claims.FindAsync(1);
            Assert.NotNull(updatedClaim);
            Assert.Equal("Approved", updatedClaim.Status);
        }

        [Fact]
        public async Task DeleteClaim_ShouldRemoveClaimFromDatabase()
        {
            await using var context = new ApplicationDBContext(_dbContextOptions);
            var claimToDelete = new Claim { Id = 1, Status = "Pending" };
            context.Claims.Add(claimToDelete);
            await context.SaveChangesAsync();
            var service = new ClaimService(context);

            var result = await service.DeleteClaim(1);

            Assert.True(result);
            Assert.Equal(0, await context.Claims.CountAsync());
        }

        [Fact]
        public async Task GetClaimById_ShouldReturnCorrectClaim_WhenExists()
        {
            await using var context = new ApplicationDBContext(_dbContextOptions);
            context.Claims.AddRange(
                new Claim { Id = 1, LecturerName = "Lecturer A" },
                new Claim { Id = 2, LecturerName = "Lecturer B" }
            );
            await context.SaveChangesAsync();
            var service = new ClaimService(context);

            var result = await service.GetClaimById(2);

            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("Lecturer B", result.LecturerName);
        }

        // --- NEW PART 3 TESTS ---

        [Fact]
        public async Task GetApprovedClaims_ShouldReturnOnlyApprovedClaims()
        {
            // Used for HR Payment Report
            await using var context = new ApplicationDBContext(_dbContextOptions);
            context.Claims.AddRange(
                new Claim { Status = "Approved" },
                new Claim { Status = "Rejected" },
                new Claim { Status = "Approved" }
            );
            await context.SaveChangesAsync();
            var service = new ClaimService(context);

            var result = await service.GetApprovedClaims();

            Assert.Equal(2, result.Count);
            Assert.All(result, claim => Assert.Equal("Approved", claim.Status));
        }

        [Fact]
        public async Task GetClaimsByLecturerAsync_ShouldReturnClaimsForSpecificLecturer()
        {
            // Used for Lecturer "My Claims" view
            await using var context = new ApplicationDBContext(_dbContextOptions);
            context.Claims.AddRange(
                new Claim { LecturerName = "lecturer@test.com", Amount = 100 },
                new Claim { LecturerName = "other@test.com", Amount = 200 },
                new Claim { LecturerName = "lecturer@test.com", Amount = 300 }
            );
            await context.SaveChangesAsync();
            var service = new ClaimService(context);

            var result = await service.GetClaimsByLecturerAsync("lecturer@test.com");

            Assert.Equal(2, result.Count);
            Assert.All(result, c => Assert.Equal("lecturer@test.com", c.LecturerName));
        }

        [Fact]
        public async Task UpdateClaim_ShouldUpdateExistingDetails()
        {
            // Used for HR/Admin Edit feature
            await using var context = new ApplicationDBContext(_dbContextOptions);
            var originalClaim = new Claim
            {
                Id = 1,
                LecturerName = "Old Name",
                HoursWorked = 10,
                HourlyRate = 50
            };
            context.Claims.Add(originalClaim);
            await context.SaveChangesAsync();
            var service = new ClaimService(context);

            var updatedData = new Claim
            {
                Id = 1,
                LecturerName = "New Name",
                HoursWorked = 20,
                HourlyRate = 60
            };

            var result = await service.UpdateClaim(updatedData);

            Assert.True(result);
            var dbClaim = await context.Claims.FindAsync(1);
            Assert.NotNull(dbClaim);
            Assert.Equal("New Name", dbClaim.LecturerName);
            Assert.Equal(20, dbClaim.HoursWorked);
        }
    }
}