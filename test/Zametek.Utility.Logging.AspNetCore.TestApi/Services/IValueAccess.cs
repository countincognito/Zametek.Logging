using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging.AspNetCore.TestApi
{
    public interface IValueAccess
    {
        Task<string> AddAsync(RequestDto requestDto);
        Task<IList<ResponseDto>> GetAsync(string random, string password);
    }
}