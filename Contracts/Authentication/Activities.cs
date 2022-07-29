using System;
using System.Collections.Generic;

namespace Contracts.Authentication
{
    public class GetRoleActivities
    { 
        public string RoleId { get; set; }
        public string Name { get; set; }
        public List<Guid> Activities { get; set; }
    }
    public class GetActivities
    {
        public string ActivityId { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
    }

    public class GetActivityParent
    {
        public string ParentActivityId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class RoleActivities
    {
        public string ActivityId { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; }
        public string ParentId { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanImport { get; set; }
        public bool CanExport { get; set; }
    }

    public class CreateRoleActivity
    {
        public string Name { get; set; }
        public string[] Activities { get; set; }
    }

    public class RoleActivitiesCommand
    {
        public string ActivityId { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanImport { get; set; }
        public bool CanExport { get; set; }
    }

    public class UpdateRoleActivity
    {
        public string RoleId { get; set; }
        public string Name { get; set; }
        public string[] Activities { get; set; }
    }

    public class NotAddedUserRole
    {
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public List<UserNames> Users { get; set; }
    }

    public class UserNames
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
    }
}
