﻿using BLL;
using SMP.Contracts.PinManagement;
using SMP.Contracts.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.PinManagementService
{
    public interface IPinManagementService
    {

        Task<APIResponse<PreviewResult>> PrintResultAsync(PrintResultRequest request);
    }
}