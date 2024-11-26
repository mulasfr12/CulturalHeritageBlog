using WebAPI.DTOs;

namespace WebAPI.Services.Interfaces
{
    public interface ILogServices
    {
        Task<IEnumerable<LogDto>> GetLogs(int count);
        Task<int> GetLogCount(); // New method for counting logs
        Task AddLog(string message, int userId);
    }
}
