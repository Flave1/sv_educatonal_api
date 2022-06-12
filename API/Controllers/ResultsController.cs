using API.Controllers.BaseControllers;
using BLL.MiddleWares;
using Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("api/v1/result/")]
    public class ResultsController : BaseController
    { 
        //private readonly IResultsService service;
        //public ResultsController(IResultsService service)
        //{
        //    this.service = service;
        //}


    }
} 