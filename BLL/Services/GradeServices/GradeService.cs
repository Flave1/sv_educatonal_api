using BLL;
using DAL;
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

        public GradeService(DataContext context)
        {
            this.context = context;
        }

        async Task<APIResponse<EditGradeGroupModel>> IGradeService.UpdateGradeAsync(EditGradeGroupModel request)
        {
            var res = new APIResponse<EditGradeGroupModel>();

            using(var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var gg = await context.GradeGroup.FirstOrDefaultAsync(r => r.GradeGroupId == request.GradeGroupId);

                    var currentSession = context.Session.FirstOrDefault(d => d.IsActive == true);
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
                        res.Message.FriendlyMessage = $"You have successfuly setted up grade setting for { request.GradeGroupName }";
                        return res;

                    }
                    else
                    {
                        //return some messge
                        await transaction.RollbackAsync();
                        res.IsSuccessful = false;
                        res.Message.FriendlyMessage = "There is no session setted up for school";
                        return res;
                    }

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    await context.SaveChangesAsync();
                    res.Message.FriendlyMessage = Messages.FriendlyException;
                    res.Message.TechnicalMessage = ex.ToString();
                    return res;
                }

               
            }
        }


        async Task<APIResponse<AddGradeGroupModel>> IGradeService.CreateGradeAsync(AddGradeGroupModel request)
        {
            var res = new APIResponse<AddGradeGroupModel>();
            if (context.GradeGroup.AsEnumerable().Any(s => UtilTools.ReplaceWhitespace(s.GradeGroupName) == UtilTools.ReplaceWhitespace(request.GradeGroupName)))
            {
                res.Message.FriendlyMessage = "Group Name Already added";
                return res;
            }

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var currentSession = context.Session.FirstOrDefault(d => d.IsActive == true);
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
                        res.Message.FriendlyMessage = "There is no session setted up for school";
                        await transaction.RollbackAsync();
                        return res;
                    }

                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = $"You have successfuly setted up grade setting for { request.GradeGroupName }";
                    return res;

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    res.Message.FriendlyMessage = Messages.FriendlyException;
                    res.Message.TechnicalMessage = ex.ToString();
                    return res;
                }
            }
        }
        //private async Task CreateClassGradeAsync(Guid groupId, List<string> classes)
        //{
        //    var cgList = new List<ClassGrade>();
        //    foreach (var cls in classes)
        //    {
        //        cgList.Add(new ClassGrade
        //        {
        //            GradeGroupId = groupId,
        //            SessionClassId = Guid.Parse(cls),
        //        });
        //        context.AddRange(cgList);
        //    }
        //    await context.SaveChangesAsync();
        //}
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

            var result = await context.GradeGroup
                     .Include(d => d.Classes)
                     .Include(d => d.Grades)
                     .Where(d => d.Deleted == false)
                     .Select(d => new GetGradeGroupModel(d)).ToListAsync();

            res.IsSuccessful = true;
            res.Result = result;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<List<GetSessionClass>>> IGradeService.GetClassesAsync()
        {
            var res = new APIResponse<List<GetSessionClass>>();

            var currentSession = context.Session.FirstOrDefault(d => d.IsActive == true);
            if (currentSession != null)
            {

                var result = await context.SessionClass
                    .Include(rr => rr.Class)
                    .Where(a => a.SessionId == currentSession.SessionId).OrderBy(s => s.Class.Name)
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
                res.Message.FriendlyMessage = "There is no session setted up for school";
                return res;
            }
        }
   
        //private async Task DeleteClassGroupsAsync(Guid groupId)
        //{
        //    var classGroups = await context.ClassGrade.Where(d => d.GradeGroupId == groupId).ToListAsync();
        //    if (classGroups.Any())
        //    {
        //        context.ClassGrade.RemoveRange(classGroups);
        //        await context.SaveChangesAsync();
        //    }
        //}

        private async Task DeleteGroupGradesAsync(Guid groupId)
        {
            var groupGrade = await context.Grade.Where(d => d.GradeGroupId == groupId).ToListAsync();
            if (groupGrade.Any())
            {
                context.Grade.RemoveRange(groupGrade);
                await context.SaveChangesAsync();
            }
        }



    }
}
