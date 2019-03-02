using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kendo.Mvc;
using System.Collections;
using Kendo.Mvc.Infrastructure;
using System.ComponentModel;
using WordAutomationDemo.Models;
using System.Data.Entity.SqlServer;
using WordAutomationDemo.Common;
using System.Data.Entity;

namespace WordAutomationDemo.Controllers
{
    public static class AssignmentCustomBinding
    {
        public enum AssignmentFields
        {
            DateTime,
            dtDueDate,
            dtCompletedDate,
            Remarks,
            AssignedBy,
            AssignedTo,
            ProjectName,
            StatusButtonContentForMerge,
            TaskName,
        }

        public static IQueryable<AssignmentModel> ApplyFiltering(this IQueryable<AssignmentModel> data, IList<IFilterDescriptor> filterDescriptors)
        {
            if (filterDescriptors.Any())
            {
                foreach (IFilterDescriptor filterDescriptor in filterDescriptors)
                {
                    data = ApplyFilter(data, filterDescriptor);
                }
            }
            return data;
        }

        private static IQueryable<AssignmentModel> ApplyFilter(IQueryable<AssignmentModel> source, IFilterDescriptor filter)
        {
            if (filter is CompositeFilterDescriptor)
            {
                foreach (IFilterDescriptor childFilter in ((CompositeFilterDescriptor)filter).FilterDescriptors)
                {
                    source = ApplyFilter(source, childFilter);
                }
            }
            else
            {
                var CCurrentUser = (WordAutomationDemo.Helpers.CurrentUser)HttpContext.Current.Session["CCurrentUser"];
                int UID = CCurrentUser.UserID;

                FilterDescriptor filterDescriptor = (FilterDescriptor)filter;
                AssignmentFields UserEnum = (AssignmentFields)Enum.Parse(typeof(AssignmentFields), filterDescriptor.Member);
                switch (UserEnum)
                {



                    case AssignmentFields.StatusButtonContentForMerge:
                        string actionDescription = Convert.ToString(filterDescriptor.Value);

                        switch (filterDescriptor.Operator)
                        {

                            case FilterOperator.IsEqualTo:
                                if (!string.IsNullOrEmpty(actionDescription) && actionDescription == "Edit Action")
                                    source = source.Where(o => (o.Action == (int)Global.Action.Assigned));
                                if (!string.IsNullOrEmpty(actionDescription) && actionDescription == "Completed")
                                    source = source.Where(o => (o.Action == (int)Global.Action.Completed && o.UserID == UID));

                                if (!string.IsNullOrEmpty(actionDescription) && actionDescription == "Approved/Decline Action")
                                    source = source.Where(o => (o.Action == (int)Global.Action.Completed && o.UserID != UID));

                                if (!string.IsNullOrEmpty(actionDescription) && actionDescription == "Approved")
                                    source = source.Where(o => (o.Action == (int)Global.Action.Approved));
                                if (!string.IsNullOrEmpty(actionDescription) && actionDescription == "Decline")
                                    source = source.Where(o => (o.UserID == WordAutomationDemo.Helpers.CurrentUserSession.UserID && o.Action == (int)Global.Action.Declined));
                                else if (!string.IsNullOrEmpty(actionDescription) && actionDescription == "Reassign")
                                    source = source.Where(o => (o.UserID != WordAutomationDemo.Helpers.CurrentUserSession.UserID && o.Action == (int)Global.Action.Declined));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                if (!string.IsNullOrEmpty(actionDescription) && actionDescription == "Edit Action")
                                    source = source.Where(o => (o.Action != (int)Global.Action.Assigned));
                                if (!string.IsNullOrEmpty(actionDescription) && actionDescription == "Completed")
                                    source = source.Where(o => (o.Action != (int)Global.Action.Completed && o.UserID == UID));

                                if (!string.IsNullOrEmpty(actionDescription) && actionDescription == "Approved/Decline Action")
                                    source = source.Where(o => (o.Action != (int)Global.Action.Completed && o.UserID != UID));

                                if (!string.IsNullOrEmpty(actionDescription) && actionDescription == "Approved")
                                    source = source.Where(o => (o.Action != (int)Global.Action.Approved));
                                if (!string.IsNullOrEmpty(actionDescription) && actionDescription == "Decline")
                                    source = source.Where(o => (o.UserID == WordAutomationDemo.Helpers.CurrentUserSession.UserID && o.Action != (int)Global.Action.Declined));
                                else if (!string.IsNullOrEmpty(actionDescription) && actionDescription == "Reassign")
                                    source = source.Where(o => (o.UserID != WordAutomationDemo.Helpers.CurrentUserSession.UserID && o.Action == (int)Global.Action.Declined));

                                break;
                        }
                        break;

                    case AssignmentFields.DateTime:
                        
                        DateTime assgDate = Convert.ToDateTime(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => o.DateTime != null && (DateTime)o.DateTime.Value.Date == assgDate);
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => o.DateTime != null && (DateTime)o.DateTime.Value.Date != assgDate);
                                break;
                            case FilterOperator.IsLessThan:
                                source = source.Where(o => o.DateTime != null && (DateTime)o.DateTime.Value.Date < assgDate);
                                break;
                            case FilterOperator.IsLessThanOrEqualTo:
                                source = source.Where(o => o.DateTime != null && (DateTime)o.DateTime.Value.Date <= assgDate);
                                break;
                            case FilterOperator.IsGreaterThan:
                                source = source.Where(o => o.DateTime != null && (DateTime)o.DateTime.Value.Date > assgDate);
                                break;
                            case FilterOperator.IsGreaterThanOrEqualTo:
                                source = source.Where(o => o.DateTime != null && (DateTime)o.DateTime.Value.Date >= assgDate);
                                break;
                        }
                        break;

                    case AssignmentFields.dtDueDate:
                        DateTime dueDate = Convert.ToDateTime(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => o.dtDueDate != null && (DateTime)o.dtDueDate.Value.Date == dueDate);
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => o.dtDueDate != null && (DateTime)o.dtDueDate.Value.Date != dueDate);
                                break;
                            case FilterOperator.IsLessThan:
                                source = source.Where(o => o.dtDueDate != null && (DateTime)o.dtDueDate.Value.Date < dueDate);
                                break;
                            case FilterOperator.IsLessThanOrEqualTo:
                                source = source.Where(o => o.dtDueDate != null && (DateTime)o.dtDueDate.Value.Date <= dueDate);
                                break;
                            case FilterOperator.IsGreaterThan:
                                source = source.Where(o => o.dtDueDate != null && (DateTime)o.dtDueDate.Value.Date > dueDate);
                                break;
                            case FilterOperator.IsGreaterThanOrEqualTo:
                                source = source.Where(o => o.dtDueDate != null && (DateTime)o.dtDueDate.Value.Date >= dueDate);
                                break;
                        }
                        break;

                    case AssignmentFields.dtCompletedDate:
                        DateTime CompletedDate = Convert.ToDateTime(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => o.dtCompletedDate != null && (DateTime)o.dtCompletedDate.Value.Date == CompletedDate);
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => o.dtCompletedDate != null && (DateTime)o.dtCompletedDate.Value.Date != CompletedDate);
                                break;
                            case FilterOperator.IsLessThan:
                                source = source.Where(o => o.dtCompletedDate != null && (DateTime)o.dtCompletedDate.Value.Date < CompletedDate);
                                break;
                            case FilterOperator.IsLessThanOrEqualTo:
                                source = source.Where(o => o.dtCompletedDate != null && (DateTime)o.dtCompletedDate.Value.Date <= CompletedDate);
                                break;
                            case FilterOperator.IsGreaterThan:
                                source = source.Where(o => o.dtCompletedDate != null && (DateTime)o.dtCompletedDate.Value.Date > CompletedDate);
                                break;
                            case FilterOperator.IsGreaterThanOrEqualTo:
                                source = source.Where(o => o.dtCompletedDate != null && (DateTime)o.dtCompletedDate.Value.Date >= CompletedDate);
                                break;
                        }
                        break;


                    case AssignmentFields.Remarks:
                        string remarks = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Remarks) && o.Remarks.ToLower().Contains(remarks));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Remarks) && !o.Remarks.ToLower().Contains(remarks));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Remarks) && Equals(o.Remarks.ToLower(), remarks) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Remarks) && Equals(o.Remarks.ToLower(), remarks) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Remarks) && o.Remarks.ToLower().StartsWith(remarks) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Remarks) && o.Remarks.ToLower().EndsWith(remarks) == true));
                                break;
                        }
                        break;


                    //case AssignmentFields.Comments:
                    //    string comments = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                    //    switch (filterDescriptor.Operator)
                    //    {
                    //        case FilterOperator.Contains:
                    //            source = source.Where(o => !string.IsNullOrEmpty(o.Comments) && o.Comments.ToLower().Contains(comments));
                    //            break;
                    //        case FilterOperator.DoesNotContain:
                    //            source = source.Where(o => !string.IsNullOrEmpty(o.Comments) && !o.Comments.ToLower().Contains(comments));
                    //            break;
                    //        case FilterOperator.IsEqualTo:
                    //            source = source.Where(o => (!string.IsNullOrEmpty(o.Comments) && Equals(o.Comments.ToLower(), comments) == true));
                    //            break;
                    //        case FilterOperator.IsNotEqualTo:
                    //            source = source.Where(o => (!string.IsNullOrEmpty(o.Comments) && Equals(o.Comments.ToLower(), comments) == false));
                    //            break;
                    //        case FilterOperator.StartsWith:
                    //            source = source.Where(o => (!string.IsNullOrEmpty(o.Comments) && o.Comments.ToLower().StartsWith(comments) == true));
                    //            break;
                    //        case FilterOperator.EndsWith:
                    //            source = source.Where(o => (!string.IsNullOrEmpty(o.Comments) && o.Comments.ToLower().EndsWith(comments) == true));
                    //            break;
                    //    }
                    //    break;

                    case AssignmentFields.TaskName:
                        string taskName = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.TaskName) && o.TaskName.ToLower().Contains(taskName));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.TaskName) && !o.TaskName.ToLower().Contains(taskName));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.TaskName) && Equals(o.TaskName.ToLower(), taskName) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.TaskName) && Equals(o.TaskName.ToLower(), taskName) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.TaskName) && o.TaskName.ToLower().StartsWith(taskName) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.TaskName) && o.TaskName.ToLower().EndsWith(taskName) == true));
                                break;
                        }
                        break;

                    case AssignmentFields.ProjectName:
                        string project = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.ProjectName) && o.ProjectName.ToLower().Contains(project));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.ProjectName) && !o.ProjectName.ToLower().Contains(project));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.ProjectName) && Equals(o.ProjectName.ToLower(), project) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.ProjectName) && Equals(o.ProjectName.ToLower(), project) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.ProjectName) && o.ProjectName.ToLower().StartsWith(project) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.ProjectName) && o.ProjectName.ToLower().EndsWith(project) == true));
                                break;
                        }
                        break;



                    case AssignmentFields.AssignedTo:
                        string assignedto = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => o.AssignedTo != null && o.AssignedTo.Where(at=>at.ToLower().Contains(assignedto)).Count() > 0);
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => o.AssignedTo != null && o.AssignedTo.Where(at => at.ToLower().Contains(assignedto)).Count() == 0);
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => o.AssignedTo != null && o.AssignedTo.Where(at => at.ToLower().Equals(assignedto)).Count() > 0);
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => o.AssignedTo != null && o.AssignedTo.Where(at => at.ToLower().Equals(assignedto)).Count() == 0);
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => o.AssignedTo != null && o.AssignedTo.Where(at => at.ToLower().StartsWith(assignedto)).Count() > 0);
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => o.AssignedTo != null && o.AssignedTo.Where(at => at.ToLower().StartsWith(assignedto)).Count() == 0);
                                break;
                        }
                        break;

                    case AssignmentFields.AssignedBy:

                        string assignedby = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.AssignedBy) && o.AssignedBy.ToLower().Contains(assignedby));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.AssignedBy) && !o.AssignedBy.ToLower().Contains(assignedby));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.AssignedBy) && Equals(o.AssignedBy.ToLower(), assignedby) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.AssignedBy) && Equals(o.AssignedBy.ToLower(), assignedby) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.AssignedBy) && o.AssignedBy.ToLower().StartsWith(assignedby) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.AssignedBy) && o.AssignedBy.ToLower().EndsWith(assignedby) == true));
                                break;
                        }
                        break;




                }
            }
            return source;
        }


        public static IEnumerable ApplyGrouping(this IQueryable<AssignmentModel> data, IList<GroupDescriptor> groupDescriptors)
        {
            Func<IEnumerable<AssignmentModel>, IEnumerable<AggregateFunctionsGroup>> selector = null;

            foreach (var group in groupDescriptors.Reverse())
            {
                AssignmentFields FieldEnum = GetRoleFieldEnum(group.Member);
                if (selector == null)
                {
                    if (FieldEnum == AssignmentFields.ProjectName)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.ProjectName, group.Member);
                    }
                    //else if (RoleEnum == AssignmentFields.DoctorName)
                    //{
                    //    selector = orders => BuildInnerGroup(orders, o => o.DoctorName, group.Member);
                    //}
                    //else if (RoleEnum == AssignmentFields.PatientName)
                    //{
                    //    selector = orders => BuildInnerGroup(orders, o => o.PatientName, group.Member);
                    //}
                    //else if (RoleEnum == AssignmentFields.CreatedDate)
                    //{
                    //    selector = orders => BuildInnerGroup(orders, o => o.CreatedDate, group.Member);
                    //}
                    //else if (RoleEnum == AssignmentFields.PaymentDate)
                    //{
                    //    selector = orders => BuildInnerGroup(orders, o => o.PaymentDate, group.Member);
                    //}
                }
                else
                {
                    if (FieldEnum == AssignmentFields.ProjectName)
                    {
                        selector = BuildGroup(o => o.ProjectName, selector, group.Member);
                    }
                    //else if (RoleEnum == AssignmentFields.DoctorName)
                    //{
                    //    selector = BuildGroup(o => o.DoctorName, selector, group.Member);
                    //}
                    //else if (RoleEnum == AssignmentFields.PatientName)
                    //{
                    //    selector = BuildGroup(o => o.PatientName, selector, group.Member);
                    //}
                    //else if (RoleEnum == AssignmentFields.CreatedDate)
                    //{
                    //    selector = BuildGroup(o => o.CreatedDate, selector, group.Member);
                    //}
                    //else if (RoleEnum == AssignmentFields.PaymentDate)
                    //{
                    //    selector = BuildGroup(o => o.PaymentDate, selector, group.Member);
                    //}
                }
            }
            return selector.Invoke(data).ToList();
        }

        private static Func<IEnumerable<AssignmentModel>, IEnumerable<AggregateFunctionsGroup>> BuildGroup<T>(Func<AssignmentModel, T> groupSelector, Func<IEnumerable<AssignmentModel>, IEnumerable<AggregateFunctionsGroup>> selectorBuilder, string Value)
        {
            var tempSelector = selectorBuilder;

            return g => g.GroupBy(groupSelector)
                         .Select(c => new AggregateFunctionsGroup
                         {
                             Key = c.Key,
                             Member = Value,
                             HasSubgroups = true,
                             Items = tempSelector.Invoke(c).ToList()
                         });
        }

        private static IEnumerable<AggregateFunctionsGroup> BuildInnerGroup<T>(IEnumerable<AssignmentModel> group, Func<AssignmentModel, T> groupSelector, string Value)
        {
            return group.GroupBy(groupSelector)
                    .Select(i => new AggregateFunctionsGroup
                    {
                        Key = i.Key,
                        Member = Value,
                        Items = i.ToList()
                    });
        }

        private static AssignmentFields GetRoleFieldEnum(string FieldValue)
        {
            return (AssignmentFields)Enum.Parse(typeof(AssignmentFields), FieldValue);
        }



        public static IQueryable<AssignmentModel> ApplyPaging(this IQueryable<AssignmentModel> data, int currentPage, int pageSize)
        {
            if (pageSize > 0 && currentPage > 0)
            {
                data = data.Skip((currentPage - 1) * pageSize);
            }

            data = data.Take(pageSize);
            return data;
        }

        public static IQueryable<AssignmentModel> ApplySorting(this IQueryable<AssignmentModel> data, IList<GroupDescriptor> groupDescriptors, IList<SortDescriptor> sortDescriptors)
        {
            //if (groupDescriptors.Any())
            //{
            //    foreach (var groupDescriptor in groupDescriptors.Reverse())
            //    {
            //        data = AddSortExpression(data, groupDescriptor.SortDirection, groupDescriptor.Member);
            //    }
            //}

            if (sortDescriptors.Any())
            {
                foreach (SortDescriptor sortDescriptor in sortDescriptors)
                {
                    data = AddSortExpression(data, sortDescriptor.SortDirection, sortDescriptor.Member);
                }
            }
            return data;
        }

        private static IQueryable<AssignmentModel> AddSortExpression(IQueryable<AssignmentModel> data, ListSortDirection sortDirection, string memberName)
        {
            AssignmentFields UserEnum = GetUserFieldEnum(memberName);
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (UserEnum)
                {
                    case AssignmentFields.AssignedBy:
                        data = data.OrderBy(order => order.AssignedBy);
                        break;
                    case AssignmentFields.AssignedTo:
                        data = data.OrderBy(order => order.AssignedTo);
                        break;
                    //case AssignmentFields.Comments:
                    //    data = data.OrderBy(order => order.Comments);
                    //    break;
                    case AssignmentFields.TaskName:
                        data = data.OrderBy(order => order.TaskName);
                        break;
                    case AssignmentFields.ProjectName:
                        data = data.OrderBy(order => order.ProjectName);
                        break;
                    case AssignmentFields.DateTime:
                        data = data.OrderBy(order => order.DateTime);
                        break;
                    case AssignmentFields.dtCompletedDate:
                        data = data.OrderBy(order => order.dtCompletedDate);
                        break;
                    case AssignmentFields.dtDueDate:
                        data = data.OrderBy(order => order.dtDueDate);
                        break;
                    case AssignmentFields.Remarks:
                        data = data.OrderBy(order => order.Remarks);
                        break;
                }
            }
            else
            {
                switch (UserEnum)
                {
                    case AssignmentFields.AssignedBy:
                        data = data.OrderByDescending(order => order.AssignedBy);
                        break;
                    case AssignmentFields.AssignedTo:
                        data = data.OrderByDescending(order => order.AssignedTo);
                        break;
                    //case AssignmentFields.Comments:
                    //    data = data.OrderByDescending(order => order.Comments);
                    //    break;
                    case AssignmentFields.TaskName:
                        data = data.OrderByDescending(order => order.TaskName);
                        break;
                    case AssignmentFields.ProjectName:
                        data = data.OrderByDescending(order => order.ProjectName);
                        break;
                    case AssignmentFields.DateTime:
                        data = data.OrderByDescending(order => order.DateTime);
                        break;
                    case AssignmentFields.dtCompletedDate:
                        data = data.OrderByDescending(order => order.dtCompletedDate);
                        break;
                    case AssignmentFields.dtDueDate:
                        data = data.OrderByDescending(order => order.dtDueDate);
                        break;
                    case AssignmentFields.Remarks:
                        data = data.OrderByDescending(order => order.Remarks);
                        break;
                }
            }
            return data;
        }

        private static AssignmentFields GetUserFieldEnum(string FieldValue)
        {
            return (AssignmentFields)Enum.Parse(typeof(AssignmentFields), FieldValue);
        }
    }
}