using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services
{
    public class LogService:ILogServices
    {
        private readonly CulturalHeritageDbContext _dbContext;

        public LogService(CulturalHeritageDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<LogDto>> GetLogs(int count)
        {
            return await _dbContext.Logs
                .OrderByDescending(log => log.Timestamp)
                .Take(count)
                .Select(log => new LogDto
                {
                    LogID = log.LogId,
                    Message = log.Message,
                    Timestamp = log.Timestamp,
                    UserID = log.UserId
                })
                .ToListAsync();
        }
        public async Task<int> GetLogCount()
        {
            return await _dbContext.Logs.CountAsync();
        }
        public async Task AddLog(string message, int userId)
        {
            var log = new Log
            {
                Message = message,
                Timestamp = DateTime.UtcNow,
                UserId = userId
            };

            _dbContext.Logs.Add(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}

