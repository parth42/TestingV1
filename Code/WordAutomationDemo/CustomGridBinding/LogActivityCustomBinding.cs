using Kendo.Mvc;
using Kendo.Mvc.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.CustomGridBinding
{
    public static class LogActivityCustomBinding
    {
        public enum LogActivityFields
        {
            LogActivityID,
            LogActivityTypeName,
            ActivityDetails,
            IPAddress,
            ChangedRoleNameOrID,
            CreatedByName,
            CreatedDate
        }

        public static IQueryable<LogActivityModel> ApplyFiltering(this IQueryable<LogActivityModel> data, IList<IFilterDescriptor> filterDescriptors)
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

        private static IQueryable<LogActivityModel> ApplyFilter(IQueryable<LogActivityModel> source, IFilterDescriptor filter)
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
                FilterDescriptor filterDescriptor = (FilterDescriptor)filter;
                LogActivityFields UserEnum = (LogActivityFields)Enum.Parse(typeof(LogActivityFields), filterDescriptor.Member);
                switch (UserEnum)
                {
                    case LogActivityFields.LogActivityTypeName:
                        string Username = Convert.ToString(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.LogActivityTypeName) && o.LogActivityTypeName.Contains(Username));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.LogActivityTypeName) && !o.LogActivityTypeName.Contains(Username));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.LogActivityTypeName) && Equals(o.LogActivityTypeName, Username) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.LogActivityTypeName) && Equals(o.LogActivityTypeName, Username) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.LogActivityTypeName) && o.LogActivityTypeName.StartsWith(Username) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.LogActivityTypeName) && o.LogActivityTypeName.EndsWith(Username) == true));
                                break;
                        }
                        break;

                    case LogActivityFields.ActivityDetails:
                        string ActivityDetails = Convert.ToString(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.ActivityDetails) && o.ActivityDetails.Contains(ActivityDetails));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.ActivityDetails) && !o.ActivityDetails.Contains(ActivityDetails));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.ActivityDetails) && Equals(o.ActivityDetails, ActivityDetails) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.ActivityDetails) && Equals(o.ActivityDetails, ActivityDetails) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.ActivityDetails) && o.ActivityDetails.StartsWith(ActivityDetails) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.ActivityDetails) && o.ActivityDetails.EndsWith(ActivityDetails) == true));
                                break;
                        }
                        break;

                    case LogActivityFields.IPAddress:
                        string IPAddress = Convert.ToString(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.IPAddress) && o.IPAddress.Contains(IPAddress));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.IPAddress) && !o.IPAddress.Contains(IPAddress));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.IPAddress) && Equals(o.IPAddress, IPAddress) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.IPAddress) && Equals(o.IPAddress, IPAddress) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.IPAddress) && o.IPAddress.StartsWith(IPAddress) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.IPAddress) && o.IPAddress.EndsWith(IPAddress) == true));
                                break;
                        }
                        break;

                    case LogActivityFields.ChangedRoleNameOrID:
                        string ChangedRoleNameOrID = Convert.ToString(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.ChangedRoleNameOrID) && o.ChangedRoleNameOrID.Contains(ChangedRoleNameOrID));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.ChangedRoleNameOrID) && !o.ChangedRoleNameOrID.Contains(ChangedRoleNameOrID));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.ChangedRoleNameOrID) && Equals(o.ChangedRoleNameOrID, ChangedRoleNameOrID) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.ChangedRoleNameOrID) && Equals(o.ChangedRoleNameOrID, ChangedRoleNameOrID) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.ChangedRoleNameOrID) && o.ChangedRoleNameOrID.StartsWith(ChangedRoleNameOrID) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.ChangedRoleNameOrID) && o.ChangedRoleNameOrID.EndsWith(ChangedRoleNameOrID) == true));
                                break;
                        }
                        break;

                    case LogActivityFields.CreatedByName:
                        string CreatedByName = Convert.ToString(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.CreatedByName) && o.CreatedByName.Contains(CreatedByName));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.CreatedByName) && !o.CreatedByName.Contains(CreatedByName));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.CreatedByName) && Equals(o.CreatedByName, CreatedByName) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.CreatedByName) && Equals(o.CreatedByName, CreatedByName) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.CreatedByName) && o.CreatedByName.StartsWith(CreatedByName) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.CreatedByName) && o.CreatedByName.EndsWith(CreatedByName) == true));
                                break;
                        }
                        break;

                    case LogActivityFields.CreatedDate:
                        DateTime CreatedDate = Convert.ToDateTime(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Date == CreatedDate);
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Date != CreatedDate);
                                break;
                            case FilterOperator.IsLessThan:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Date < CreatedDate);
                                break;
                            case FilterOperator.IsLessThanOrEqualTo:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Date <= CreatedDate);
                                break;
                            case FilterOperator.IsGreaterThan:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Date > CreatedDate);
                                break;
                            case FilterOperator.IsGreaterThanOrEqualTo:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Date >= CreatedDate);
                                break;
                        }
                        break;
                }
            }
            return source;
        }

        public static IEnumerable ApplyGrouping(this IQueryable<LogActivityModel> data, IList<GroupDescriptor> groupDescriptors)
        {
            Func<IEnumerable<LogActivityModel>, IEnumerable<AggregateFunctionsGroup>> selector = null;

            foreach (var group in groupDescriptors.Reverse())
            {
                LogActivityFields UserEnum = GetUserFieldEnum(group.Member);
                if (selector == null)
                {
                    if (UserEnum == LogActivityFields.LogActivityTypeName)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.LogActivityTypeName, group.Member);
                    }
                    else if (UserEnum == LogActivityFields.ActivityDetails)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.ActivityDetails, group.Member);
                    }
                    else if (UserEnum == LogActivityFields.IPAddress)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.IPAddress, group.Member);
                    }
                    else if (UserEnum == LogActivityFields.ChangedRoleNameOrID)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.ChangedRoleNameOrID, group.Member);
                    }
                    else if (UserEnum == LogActivityFields.CreatedByName)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.CreatedByName, group.Member);
                    }
                    else if (UserEnum == LogActivityFields.CreatedDate)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.CreatedDate, group.Member);
                    }
                }
                else
                {
                    if (UserEnum == LogActivityFields.LogActivityTypeName)
                    {
                        selector = BuildGroup(o => o.LogActivityTypeName, selector, group.Member);
                    }
                    else if (UserEnum == LogActivityFields.ActivityDetails)
                    {
                        selector = BuildGroup(o => o.ActivityDetails, selector, group.Member);
                    }
                    else if (UserEnum == LogActivityFields.IPAddress)
                    {
                        selector = BuildGroup(o => o.IPAddress, selector, group.Member);
                    }
                    else if (UserEnum == LogActivityFields.ChangedRoleNameOrID)
                    {
                        selector = BuildGroup(o => o.ChangedRoleNameOrID, selector, group.Member);
                    }
                    else if (UserEnum == LogActivityFields.CreatedByName)
                    {
                        selector = BuildGroup(o => o.CreatedByName, selector, group.Member);
                    }
                    else if (UserEnum == LogActivityFields.CreatedDate)
                    {
                        selector = BuildGroup(o => o.CreatedDate, selector, group.Member);
                    }
                }
            }
            return selector.Invoke(data).ToList();
        }

        private static Func<IEnumerable<LogActivityModel>, IEnumerable<AggregateFunctionsGroup>> BuildGroup<T>(Func<LogActivityModel, T> groupSelector, Func<IEnumerable<LogActivityModel>, IEnumerable<AggregateFunctionsGroup>> selectorBuilder, string Value)
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

        private static IEnumerable<AggregateFunctionsGroup> BuildInnerGroup<T>(IEnumerable<LogActivityModel> group, Func<LogActivityModel, T> groupSelector, string Value)
        {
            return group.GroupBy(groupSelector)
                    .Select(i => new AggregateFunctionsGroup
                    {
                        Key = i.Key,
                        Member = Value,
                        Items = i.ToList()
                    });
        }


        public static IQueryable<LogActivityModel> ApplyPaging(this IQueryable<LogActivityModel> data, int currentPage, int pageSize)
        {
            if (pageSize > 0 && currentPage > 0)
            {
                data = data.Skip((currentPage - 1) * pageSize);
            }

            data = data.Take(pageSize);
            return data;
        }

        public static IQueryable<LogActivityModel> ApplySorting(this IQueryable<LogActivityModel> data, IList<GroupDescriptor> groupDescriptors, IList<SortDescriptor> sortDescriptors)
        {
            if (groupDescriptors.Any())
            {
                foreach (var groupDescriptor in groupDescriptors.Reverse())
                {
                    data = AddSortExpression(data, groupDescriptor.SortDirection, groupDescriptor.Member);
                }
            }

            if (sortDescriptors.Any())
            {
                foreach (SortDescriptor sortDescriptor in sortDescriptors)
                {
                    data = AddSortExpression(data, sortDescriptor.SortDirection, sortDescriptor.Member);
                }
            }
            return data;
        }

        private static IQueryable<LogActivityModel> AddSortExpression(IQueryable<LogActivityModel> data, ListSortDirection sortDirection, string memberName)
        {
            LogActivityFields UserEnum = GetUserFieldEnum(memberName);
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (UserEnum)
                {
                    case LogActivityFields.LogActivityID:
                        data = data.OrderBy(order => order.LogActivityID);
                        break;
                    case LogActivityFields.LogActivityTypeName:
                        data = data.OrderBy(order => order.LogActivityTypeName);
                        break;
                    case LogActivityFields.ActivityDetails:
                        data = data.OrderBy(order => order.ActivityDetails);
                        break;
                    case LogActivityFields.IPAddress:
                        data = data.OrderBy(order => order.IPAddress);
                        break;
                    case LogActivityFields.ChangedRoleNameOrID:
                        data = data.OrderBy(order => order.ChangedRoleNameOrID);
                        break;
                    case LogActivityFields.CreatedByName:
                        data = data.OrderBy(order => order.CreatedByName);
                        break;
                    case LogActivityFields.CreatedDate:
                        data = data.OrderBy(order => order.CreatedDate);
                        break;
                }
            }
            else
            {
                switch (UserEnum)
                {
                    case LogActivityFields.LogActivityID:
                        data = data.OrderByDescending(order => order.LogActivityID);
                        break;
                    case LogActivityFields.LogActivityTypeName:
                        data = data.OrderByDescending(order => order.LogActivityTypeName);
                        break;
                    case LogActivityFields.ActivityDetails:
                        data = data.OrderByDescending(order => order.ActivityDetails);
                        break;
                    case LogActivityFields.IPAddress:
                        data = data.OrderByDescending(order => order.IPAddress);
                        break;
                    case LogActivityFields.ChangedRoleNameOrID:
                        data = data.OrderByDescending(order => order.ChangedRoleNameOrID);
                        break;
                    case LogActivityFields.CreatedByName:
                        data = data.OrderByDescending(order => order.CreatedByName);
                        break;
                    case LogActivityFields.CreatedDate:
                        data = data.OrderByDescending(order => order.CreatedDate);
                        break;
                }
            }
            return data;
        }

        private static LogActivityFields GetUserFieldEnum(string FieldValue)
        {
            return (LogActivityFields)Enum.Parse(typeof(LogActivityFields), FieldValue);
        }
    }
}