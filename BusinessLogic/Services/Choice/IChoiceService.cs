using BusinessLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Choice
{
    public interface IChoiceService
    {
        Task<ResultDto> AddChoiceAsync(ChoiceAddDto dto);
        Task<ResultDto> DeleteChoiceAsync(int id);

        Task<ResultDto> UpdateChoiceAsync(int id, Data.Models.Choice dto);

        Task<ResultDto> GetChoiceByIdAsync(int id);

        Task<ResultDto> GetAllChoicesAsync();
    }
}
