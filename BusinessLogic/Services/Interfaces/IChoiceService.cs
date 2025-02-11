using BusinessLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Interfaces
{
    public interface IChoiceService 
    {
        Task<ResultDto> AddChoiceAsync(ChoiceDto dto);
        Task<ResultDto> DeleteChoiceAsync(int id);

        Task<ResultDto> UpdateChoiceAsync(int id , ChoiceDto dto);

        Task<ResultDto> GetChoiceByIdAsync(int id);

        Task<ResultDto> GetAllChoicesAsync();
    }
}
