using BLL;
using BLL.LoggerService;
using Contracts.Common;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Utilities;
using SMP.Contracts.GradeModels;
using SMP.DAL.Models.GradeEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.GradeServices
{
    public class GradeService : IGradeService
    {
        private readonly DataContext context;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;

        public GradeService(DataContext context, IHttpContextAccessor accessor, ILoggerService loggerService)
        {
            this.context = context;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }

        async Task<APIResponse<EditGradeGroupModel>> IGradeService.UpdateGradeAsync(EditGradeGroupModel request)
        {
            var res = new APIResponse<EditGradeGroupModel>();

            using(var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var gg = await context.GradeGroup.FirstOrDefaultAsync(r => r.GradeGroupId == request.GradeGroupId && r.ClientId == smsClientId);

                    var currentSession = context.Session.FirstOrDefault(d => d.ClientId == smsClientId && d.IsActive == true);
                    if (currentSession != null)
                    {
                        gg.GradeGroupName = request.GradeGroupName;
                        gg.SessionId = currentSession.SessionId;
                        await context.SaveChangesAsync();

                        //await DeleteClassGroupsAsync(request.GradeGroupId);
                        await DeleteGroupGradesAsync(request.GradeGroupId);

                        //await CreateClassGradeAsync(gg.GradeGroupId, request.Classes);
                        await CreateGradeAsync(gg.GradeGroupId, request.Grades);

                        await transaction.CommitAsync();
                        res.Result = request;
                        res.IsSuccessful = true;
                        res.Message.FriendlyMessage = $"You have successfuly set up grade setting for { request.GradeGroupName }";
                        return res;

                    }
                    else
                    {
                        //return some messge
                        await transaction.RollbackAsync();
                        res.IsSuccessful = false;
                        res.Message.FriendlyMessage = "There is no session set up for school";
                        return res;
                    }

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    await context.SaveChangesAsync();
                    loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                    res.Message.FriendlyMessage = Messages.FriendlyException;
                    res.Message.TechnicalMessage = ex.ToString();
                    return res;
                }

               
            }
        }


        async Task<APIResponse<AddGradeGroupModel>> IGradeService.CreateGradeAsync(AddGradeGroupModel request)
        {
            var res = new APIResponse<AddGradeGroupModel>();
            if (context.GradeGroup.Where(x=>x.ClientId == smsClientId).AsEnumerable().Any(s => s.GradeGroupName.ToLower() == request.GradeGroupName.ToLower()))
            {
                res.Message.FriendlyMessage = "Group Name Already added";
                return res;
            }

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var currentSession = context.Session.FirstOrDefault(d => d.ClientId == smsClientId && d.IsActive == true);
                    if (currentSession != null)
                    {
                        var gg = new GradeGroup();
                        gg.GradeGroupName = request.GradeGroupName;
                        gg.SessionId = currentSession.SessionId;
                        context.GradeGroup.Add(gg);
                        await context.SaveChangesAsync();

                        //await CreateClassGradeAsync(gg.GradeGroupId, request.Classes);
                        await CreateGradeAsync(gg.GradeGroupId, request.Grades);

                        await transaction.CommitAsync();
                        res.Result = request;

                    }
                    else
                    {
                        res.Message.FriendlyMessage = "There is no session set up for school";
                        await transaction.RollbackAsync();
                        return res;
                    }

                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = $"You have successfuly set up grade setting for { request.GradeGroupName }";
                    return res;

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                    res.Message.FriendlyMessage = Messages.FriendlyException;
                    res.Message.TechnicalMessage = ex.ToString();
                    return res;
                }
            }
        }
          
        private async Task CreateGradeAsync(Guid groupId, GradesModel[] grades)
        {
            var gList = new List<Grade>();
            foreach (var g in grades)
            {
                gList.Add(new Grade
                {
                    GradeGroupId = groupId,
                    GradeName = g.GradeName,
                    LowerLimit = g.LowerLimit,
                    Remark = g.Remark,
                    UpperLimit = g.UpperLimit,
                });
                context.AddRange(gList);
            }
            await context.SaveChangesAsync();
        }

        async Task<APIResponse<List<GetGradeGroupModel>>> IGradeService.GetGradeSettingAsync()
        {
            var res = new APIResponse<List<GetGradeGroupModel>>();

            var result = await context.GradeGroup.Where(x=>x.ClientId == smsClientId && x.Deleted == false)
                     .Include(d => d.Classes)
                     .Include(d => d.Grades)
                     .Select(d => new GetGradeGroupModel(d)).ToListAsync();

            res.IsSuccessful = true;
            res.Result = result;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<List<GetSessionClass>>> IGradeService.GetClassesAsync()
        {
            var res = new APIResponse<List<GetSessionClass>>();

            var currentSession = context.Session.FirstOrDefault(d => d.IsActive == true && d.ClientId == smsClientId);
            if (currentSession != null)
            {

                var result = await context.SessionClass.Where(d=> d.SessionId == currentSession.SessionId && d.ClientId == smsClientId)
                    .Include(rr => rr.Class)
                    .OrderBy(s => s.Class.Name)
                    .Select(d => new GetSessionClass(d)).ToListAsync();

                              //select new GetSessionClass(a)).ToList();

                res.IsSuccessful = true;
                res.Result = result;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            else
            {
                //return some messge
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = "There is no session set up for school";
                return res;
            }
        }
   
     
        private async Task DeleteGroupGradesAsync(Guid groupId)
        {
            var groupGrade = await context.Grade.Where(d => d.GradeGroupId == groupId && d.ClientId == smsClientId).ToListAsync();
            if (groupGrade.Any())
            {
                context.Grade.RemoveRange(groupGrade);
                await context.SaveChangesAsync();
            }
        }

        async Task<APIResponse<SingleDelete>> IGradeService.DeleteGradeAsync(SingleDelete request)
        {
            var res = new APIResponse<SingleDelete>();

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var gg = await context.GradeGroup.FirstOrDefaultAsync(r => r.GradeGroupId == Guid.Parse(request.Item) && r.ClientId == smsClientId);

                    if (gg == null)
                    {
                        res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                        return res;
                    }

                    gg.Deleted = true;
                    context.SaveChanges();
                    await transaction.CommitAsync();
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = Messages.DeletedSuccess;
                    return res;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                    res.Message.FriendlyMessage = Messages.FriendlyException;
                    res.Message.TechnicalMessage = ex.ToString();
                    return res;
                }


            }
        }


    }
}
