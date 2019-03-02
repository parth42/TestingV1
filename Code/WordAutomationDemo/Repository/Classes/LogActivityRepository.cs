using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Interfaces;

namespace WordAutomationDemo.Repository.Classes
{
    /// <summary>
    /// Created By : Dalvinder Singh
    /// Date : 18 Sep 2017
    /// </summary>
    public class LogActivityRepository : ILogActivity
    {
        private readonly ReadyPortalDBEntities _entities;

        public LogActivityRepository(ReadyPortalDBEntities entities)
        {
            _entities = entities;
        }

        public IQueryable<LogActivityModel> LogActivitylist()
        {   
            var resultList = (from la in _entities.tblLogActivities
                              join ud in _entities.tblUserDepartments on la.CreatedBy equals ud.UserId
                              join lat in _entities.tblLogActivityTypes on la.LogActivityTypeID equals lat.LogActivityTypeID
                              join r in _entities.tblRoles on la.ChangedID equals r.RoleID into rjoin
                              from rleftjoin in rjoin.DefaultIfEmpty()
                              where la.CompanyID == CurrentUserSession.User.CompanyID
                              select new LogActivityModel()
                              {
                                  LogActivityID = la.LogActivityID,
                                  LogActivityTypeID = la.LogActivityTypeID,
                                  LogActivityTypeName = lat.LogActivityName,
                                  ActivityDetails = la.ActivityDetails,
                                  IPAddress = la.IPAddress,
                                  ChangedID = la.ChangedID,
                                  ChangedRoleNameOrID = String.IsNullOrEmpty(rleftjoin.Role) ? (la.ChangedID != null ? SqlFunctions.StringConvert((double)la.ChangedID).Trim() : string.Empty) : rleftjoin.Role,
                                  CreatedBy = la.CreatedBy,
                                  CreatedByName = ud.UserName,
                                  CreatedDate = la.CreatedDate
                              }).OrderByDescending(la => la.LogActivityID).ToList().AsQueryable();

            return resultList;
        }

        public void AddLogActivity(byte logActivityTypeID, string activityDetails, int[] changedID = null)
        {
                if (logActivityTypeID == (byte)Global.LogActivityTypes.UserDeleted)
                {
                    InsertMultipleIds(logActivityTypeID, activityDetails, changedID);
                }
                else
                {
                    InsertLogActivityObject(logActivityTypeID, activityDetails, (changedID != null && changedID.Any() ? changedID.FirstOrDefault() : (int?)null));
                }
        }

        public void AddException(Exception exception, int[] changedID = null)
        {
            var logActivityTypeID = (byte)Global.LogActivityTypes.ExceptionOccured;
            var activityDetails = string.Empty;
            if (exception?.InnerException?.InnerException?.Message?.Contains("The DELETE statement conflicted with the REFERENCE constraint") ?? false)
            {
                activityDetails = "Unable to delete, this record is in use.";
            }
            else
            {
                activityDetails = "Error occured while updating record.";
            }

            InsertMultipleIds(logActivityTypeID, activityDetails, changedID);

        }

        public void AddError(string errorMessage, int[] changedID = null)
        {
            var logActivityTypeID = (byte)Global.LogActivityTypes.ErrorOccured;
            var activityDetails = errorMessage;

            InsertMultipleIds(logActivityTypeID, activityDetails, changedID);

        }

        public void AddInformation(string message, int[] changedID = null)
        {
            var logActivityTypeID = (byte)Global.LogActivityTypes.Information;
            var activityDetails = message;

            InsertMultipleIds(logActivityTypeID, activityDetails, changedID);

        }

        private void InsertMultipleIds(byte logActivityTypeID, string activityDetails, int[] changedID = null)
        {
            if (changedID != null && changedID.Any())
            {
                foreach (var item in changedID)
                {
                    InsertLogActivityObject(logActivityTypeID, activityDetails, item);
                }
            }
            else
            {
                InsertLogActivityObject(logActivityTypeID, activityDetails);
            }
        }

        private void InsertLogActivityObject(int logActivityTypeID, string activityDetails, int? changedID = null, string menuItem = null)
        {
            var objLogActivity = new tblLogActivity()
            {
                LogActivityTypeID = logActivityTypeID,
                ActivityDetails = activityDetails,
                IPAddress = HttpContext.Current.Request.UserHostAddress,
                CreatedBy = CurrentUserSession.UserID > 0 ? CurrentUserSession.UserID : 0,
                CreatedDate = DateTime.Now,
                ChangedID = changedID,
                MenuItem = menuItem,
                CompanyID = CurrentUserSession.User != null ? CurrentUserSession.User.CompanyID : (int?)null
            };

            _entities.tblLogActivities.Add(objLogActivity);
            _entities.SaveChanges();
        }
    }
}