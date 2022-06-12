using BLL;
using DAL;
using Microsoft.AspNetCore.Http;
using SMP.Contracts.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ResultServices
{
    public class ResultsService: IResultsService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;

        public ResultsService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }

        //public async Task<APIResponse<List<GetClasses>>> IResultsService.GetCurrentStaffClassesAsync()
        //{

        //}
    }
}
