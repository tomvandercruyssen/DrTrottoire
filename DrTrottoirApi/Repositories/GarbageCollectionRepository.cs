using System.Globalization;
using DrTrottoirApi.Entities;
using DrTrottoirApi.Exceptions;
using DrTrottoirApi.Extensions;
using DrTrottoirApi.Helpers;
using DrTrottoirApi.Models;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Repositories
{
    public class GarbageCollectionRepository : IGarbageCollectionRepository
    {
        private readonly DrTrottoirDbContext _context;

        public GarbageCollectionRepository(DrTrottoirDbContext context)
        {
            _context = context;
        }
        public async Task CreateGarbageCollection(CreateGarbageCollectionRequest request)
        {
            if (request.GarbageTypes == null)
                throw new ArgumentNullException(nameof(request.GarbageTypes));

            if (request.CollectionTime < DateTime.UtcNow)
                throw new InvalidDateException();

            var validGarbageTypes = (await ValidGarbageTypes(request.GarbageTypes));

            if (!validGarbageTypes)
                throw new InvalidGarbageTypesException();

            await SetPlanningForGarbageCollectionGarbageType(request);
        }
        public async Task DeleteGarbageCollection(DeleteGarbageCollectionRequest request)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == request.CompanyId);
            if (company == null)
                throw new CompanyNotFoundException();
            
            var allGarbageCollections = await _context.GarbageCollections.Where(g => g.CollectionTime == request.CollectionTime).ToListAsync();

            var garbageCollectionsForThatCompany =
                await _context.CompanyGarbageCollections.FirstOrDefaultAsync(cg => cg.CompanyId == company.Id && allGarbageCollections.Any(c => c.Id == cg.GarbageCollection.Id));

            if (garbageCollectionsForThatCompany == null)
                throw new NoGarbageCollectionsException();

            var garbageCollectionsGarbageTypes = await _context.GarbageCollectionGarbageTypes.Where(g =>
                g.GarbageCollectionId == garbageCollectionsForThatCompany.GarbageCollectionId).ToListAsync();

            var garbageCollection = await _context.GarbageCollections.FirstOrDefaultAsync(g => g.Id == garbageCollectionsForThatCompany.GarbageCollectionId);

            _context.CompanyGarbageCollections.Remove(garbageCollectionsForThatCompany);
            _context.GarbageCollectionGarbageTypes.RemoveRange(garbageCollectionsGarbageTypes);
            _context.GarbageCollections.Remove(garbageCollection);
            await _context.SaveChangesAsync();
        }
        public async Task<BaseGarbageCollectionResponse> GetGarbageCollectionsForWeek(GetGarbageCollectionRequest request)
        {
            var weekOfDate = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(request.Date,
                CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, DayOfWeek.Monday);

            var garbageCollectionsForCompany =
                await _context.CompanyGarbageCollections.Where(cg => cg.CompanyId == request.CompanyId).ToListAsync();

            var garbageCollectionsForWeek = new List<DateTime>();

            foreach (var garbageCollectionForCompany in garbageCollectionsForCompany)
            {
                var weekOfCollectionDate = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    garbageCollectionForCompany.GarbageCollection.CollectionTime,
                    CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, DayOfWeek.Monday);

                if(weekOfCollectionDate == weekOfDate)
                    garbageCollectionsForWeek.Add(garbageCollectionForCompany.GarbageCollection.CollectionTime);
            }

            return new BaseGarbageCollectionResponse() { CollectionTimes = garbageCollectionsForWeek };
        }
        public async Task<IList<GarbageCollectionGarbageTypeResponse>> GetGarbageCollectionsWithGarbageTypesForWeek(GetGarbageCollectionRequest request)
        {
            var weekOfDate = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(request.Date,
                CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, DayOfWeek.Monday);

            var garbageCollectionsForCompany =
                await _context.CompanyGarbageCollections.Where(cg => cg.CompanyId == request.CompanyId).ToListAsync();

            var garbageCollectionsForWeek = new List<GarbageCollectionGarbageTypeResponse>();

            foreach (var garbageCollectionForCompany in garbageCollectionsForCompany)
            {
                var weekOfCollectionDate = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    garbageCollectionForCompany.GarbageCollection.CollectionTime,
                    CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, DayOfWeek.Monday);

                if (weekOfCollectionDate != weekOfDate) continue;

                var garbageTypesIds= await _context.GarbageCollectionGarbageTypes
                    .Where(g => g.GarbageCollectionId == garbageCollectionForCompany.GarbageCollectionId)
                    .Select(g => g.GarbageTypeId)
                    .ToListAsync();

                var garbageTypes = await _context.GarbageTypes.Where(g => garbageTypesIds.Contains(g.Id))
                    .Select(g => g.Name)
                    .ToListAsync();

                garbageCollectionsForWeek.Add(new GarbageCollectionGarbageTypeResponse()
                {
                    CollectionTime = garbageCollectionForCompany.GarbageCollection.CollectionTime,
                    HasToBeBroughtOutside = garbageCollectionForCompany.GarbageCollection.HasToBeBroughtOutside,
                    GarbageTypes = garbageTypes
                });
            }

            return garbageCollectionsForWeek;
        }
        public async Task<GarbageCollectionGarbageTypeResponse> GetGarbageCollectionsWithGarbageTypesForTimeSlot(GetGarbageCollectionRequest request)
        {
            var garbageCollectionForCompany =
                await _context.CompanyGarbageCollections.FirstOrDefaultAsync(cg =>
                    cg.CompanyId == request.CompanyId && cg.GarbageCollection.CollectionTime.Equals(request.Date));

            if (garbageCollectionForCompany == null)
                throw new NoGarbageCollectionsException();

            var garbageTypesIds = await _context.GarbageCollectionGarbageTypes
                .Where(g => g.GarbageCollectionId == garbageCollectionForCompany.GarbageCollectionId)
                .Select(g => g.GarbageTypeId)
                .ToListAsync();

            var garbageTypes = await _context.GarbageTypes.Where(g => garbageTypesIds.Contains(g.Id))
                .Select(g => g.Name)
                .ToListAsync();

            return new GarbageCollectionGarbageTypeResponse()
            {
                CollectionTime = request.Date,
                HasToBeBroughtOutside = garbageCollectionForCompany.GarbageCollection.HasToBeBroughtOutside,
                GarbageTypes = garbageTypes
            };
        }
        private async Task SetPlanningForGarbageCollectionGarbageType(CreateGarbageCollectionRequest request)
        {
            var garbageTypes = request.GarbageTypes.Select(g => new GarbageType() { Name = g }).ToList();
            var garbageCollection = new GarbageCollection() { CollectionTime = request.CollectionTime, HasToBeBroughtOutside = request.HasToBeBroughtOutside};
            var garbageCollectionGarbageType = new List<GarbageCollectionGarbageType>();

            _context.GarbageCollections.Add(garbageCollection);
            await _context.SaveChangesAsync();

            foreach (var garbageType in garbageTypes)
                garbageCollectionGarbageType.Add(new GarbageCollectionGarbageType() { GarbageCollectionId = garbageCollection.Id, GarbageTypeId = garbageType.Id, GarbageCollection = garbageCollection, GarbageType = garbageType });

            _context.CompanyGarbageCollections.Add(new CompanyGarbageCollection()
                { CompanyId = request.CompanyId, GarbageCollection = garbageCollection });
            _context.GarbageCollectionGarbageTypes.AddRange(garbageCollectionGarbageType);
            await _context.SaveChangesAsync();
        }
        private async Task<bool> ValidGarbageTypes(List<string> garbageTypes)
        {
            var allGarbageTypes = await _context.GarbageTypes.Select(g => g.Name).ToListAsync();

            foreach (var garbageType in garbageTypes)
            {
                if (!allGarbageTypes.Contains(garbageType))
                    return false;
            }

            return true;
        }
    }
}
