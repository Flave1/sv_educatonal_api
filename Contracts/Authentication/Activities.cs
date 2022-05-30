﻿using System.Collections.Generic;

namespace Contracts.Authentication
{
    public class GetRoleActivities
    { 
        public string RoleId { get; set; }
        public string Name { get; set; }
        public List<RoleActivities> Activities { get; set; } = new List<RoleActivities>();
    }
    public class GetActivities
    {
        public string ActivityId { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
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
        public RoleActivitiesCommand[] Activities { get; set; }
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
        public RoleActivitiesCommand[] Activities { get; set; }
    }
}