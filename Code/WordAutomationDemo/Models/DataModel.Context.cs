﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WordAutomationDemo.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ReadyPortalDBEntities : DbContext
    {
        public ReadyPortalDBEntities()
            : base("name=ReadyPortalDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblAssignedWordPage> tblAssignedWordPages { get; set; }
        public virtual DbSet<tblAssignedExcelSheet> tblAssignedExcelSheets { get; set; }
        public virtual DbSet<tblAssignedPPTSlide> tblAssignedPPTSlides { get; set; }
        public virtual DbSet<tblAssignmentHistory> tblAssignmentHistories { get; set; }
        public virtual DbSet<tblAssignmentLog> tblAssignmentLogs { get; set; }
        public virtual DbSet<tblAssignmentMember> tblAssignmentMembers { get; set; }
        public virtual DbSet<tblAssignment> tblAssignments { get; set; }
        public virtual DbSet<tblChatMessage> tblChatMessages { get; set; }
        public virtual DbSet<tblCompany> tblCompanies { get; set; }
        public virtual DbSet<tblDateFormat> tblDateFormats { get; set; }
        public virtual DbSet<tblDocument> tblDocuments { get; set; }
        public virtual DbSet<tblExcelRowMap> tblExcelRowMaps { get; set; }
        public virtual DbSet<tblExcelSheet> tblExcelSheets { get; set; }
        public virtual DbSet<tblLogActivity> tblLogActivities { get; set; }
        public virtual DbSet<tblMenuItem> tblMenuItems { get; set; }
        public virtual DbSet<tblMenuItem1> tblMenuItems1 { get; set; }
        public virtual DbSet<tblPPTSlide> tblPPTSlides { get; set; }
        public virtual DbSet<tblProjectDocument> tblProjectDocuments { get; set; }
        public virtual DbSet<tblProjectMember> tblProjectMembers { get; set; }
        public virtual DbSet<tblProject> tblProjects { get; set; }
        public virtual DbSet<tblRole> tblRoles { get; set; }
        public virtual DbSet<tblRolePrivilage> tblRolePrivilages { get; set; }
        public virtual DbSet<tblSectionMaster> tblSectionMasters { get; set; }
        public virtual DbSet<tblTemplateMaster> tblTemplateMasters { get; set; }
        public virtual DbSet<tblUserDepartment> tblUserDepartments { get; set; }
        public virtual DbSet<tblLogActivityType> tblLogActivityTypes { get; set; }
        public virtual DbSet<vwRolePrivilege> vwRolePrivileges { get; set; }
    
        public virtual int RolePrev(Nullable<int> roleID)
        {
            var roleIDParameter = roleID.HasValue ?
                new ObjectParameter("RoleID", roleID) :
                new ObjectParameter("RoleID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("RolePrev", roleIDParameter);
        }
    }
}
