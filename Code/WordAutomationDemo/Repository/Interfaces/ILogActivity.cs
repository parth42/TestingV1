using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Repository.Interfaces
{
    /// <summary>
    /// Created By : Dalvinder Singh
    /// Date : 18 Sep 2017
    /// </summary>
    public interface ILogActivity
    {
        /// <summary>
        /// Get all log activity list
        /// </summary>
        /// <returns>Return list of log activities</returns>
        IQueryable<LogActivityModel> LogActivitylist();

        /// <summary>
        /// Add a new log activity
        /// </summary>
        /// <param name="logActivityTypeID">Log Activity Type Id</param>
        /// <param name="activityDetails">Log Activity Details</param>
        /// <param name="changedID">Array of Changed Ids</param>
        void AddLogActivity(byte logActivityTypeID, string activityDetails, int[] changedID = null);

        /// <summary>
        /// Add a new exception log
        /// </summary>
        /// <param name="exception">Exception Occured</param>
        /// <param name="changedID">Array of changed Ids</param>
        void AddException(Exception exception, int[] changedID = null);

        /// <summary>
        /// Add a new error log
        /// </summary>
        /// <param name="errorMessage">Error Message</param>
        /// <param name="changedID">Array of changed Ids</param>
        void AddError(string errorMessage, int[] changedID = null);

        /// <summary>
        /// Add information
        /// </summary>
        /// <param name="message">Information message</param>
        /// <param name="changedID">Changed Ids</param>
        void AddInformation(string message, int[] changedID = null);
    }
}