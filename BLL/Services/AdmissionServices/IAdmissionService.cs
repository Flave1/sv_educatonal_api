﻿using BLL;
using BLL.Filter;
using BLL.Wrappers;
using SMP.Contracts.Admissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AdmissionServices
{
    public interface IAdmissionService
    {
        public Task<APIResponse<PagedResponse<List<SelectAdmission>>>> GetAllAdmission(PaginationFilter filter);
        public Task<APIResponse<SelectAdmission>> GetAdmission(string admissionId);
        public Task<APIResponse<bool>> EnrollCandidate(string admissionId);
    }
}
