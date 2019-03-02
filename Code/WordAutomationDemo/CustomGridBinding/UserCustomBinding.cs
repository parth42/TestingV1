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
    public static class UserCustomBinding
    {
        public enum UserFields
        {
            UserName,
            FullName,
            EmailID,
            Company,
            Role,
            Active
        }

        public static IQueryable<UserModel> ApplyFiltering(this IQueryable<UserModel> data, IList<IFilterDescriptor> filterDescriptors)
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

        private static IQueryable<UserModel> ApplyFilter(IQueryable<UserModel> source, IFilterDescriptor filter)
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
                UserFields UserEnum = (UserFields)Enum.Parse(typeof(UserFields), filterDescriptor.Member);
                switch (UserEnum)
                {
                    case UserFields.UserName:
                        string Username = Convert.ToString(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.UserName) && o.UserName.Contains(Username));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.UserName) && !o.UserName.Contains(Username));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.UserName) && Equals(o.UserName, Username) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.UserName) && Equals(o.UserName, Username) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.UserName) && o.UserName.StartsWith(Username) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.UserName) && o.UserName.EndsWith(Username) == true));
                                break;
                        }
                        break;

                    case UserFields.FullName:
                        string FullName = Convert.ToString(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.FullName) && o.FullName.Contains(FullName));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.FullName) && !o.FullName.Contains(FullName));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.FullName) && Equals(o.FullName, FullName) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.FullName) && Equals(o.FullName, FullName) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.FullName) && o.FullName.StartsWith(FullName) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.FullName) && o.FullName.EndsWith(FullName) == true));
                                break;
                        }
                        break;

                    case UserFields.EmailID:
                        string EmailID = Convert.ToString(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.EmailID) && o.EmailID.Contains(EmailID));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.EmailID) && !o.EmailID.Contains(EmailID));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.EmailID) && Equals(o.EmailID, EmailID) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.EmailID) && Equals(o.EmailID, EmailID) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.EmailID) && o.EmailID.StartsWith(EmailID) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.EmailID) && o.EmailID.EndsWith(EmailID) == true));
                                break;
                        }
                        break;

                    case UserFields.Company:
                        string Company = Convert.ToString(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Company) && o.Company.Contains(Company));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Company) && !o.Company.Contains(Company));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Company) && Equals(o.Company, Company) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Company) && Equals(o.Company, Company) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Company) && o.Company.StartsWith(Company) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Company) && o.Company.EndsWith(Company) == true));
                                break;
                        }
                        break;

                    case UserFields.Role:
                        string Role = Convert.ToString(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Role) && o.Role.Contains(Role));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Role) && !o.Role.Contains(Role));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Role) && Equals(o.Role, Role) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Role) && Equals(o.Role, Role) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Role) && o.Role.StartsWith(Role) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Role) && o.Role.EndsWith(Role) == true));
                                break;
                        }
                        break;

                    case UserFields.Active:
                        string Active = Convert.ToString(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Active) && o.Active.Contains(Active));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Active) && !o.Active.Contains(Active));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Active) && Equals(o.Active, Active) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Active) && Equals(o.Active, Active) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Active) && o.Active.StartsWith(Active) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Active) && o.Active.EndsWith(Active) == true));
                                break;
                        }
                        break;
                }
            }
            return source;
        }

        public static IEnumerable ApplyGrouping(this IQueryable<UserModel> data, IList<GroupDescriptor> groupDescriptors)
        {
            Func<IEnumerable<UserModel>, IEnumerable<AggregateFunctionsGroup>> selector = null;

            foreach (var group in groupDescriptors.Reverse())
            {
                UserFields UserEnum = GetUserFieldEnum(group.Member);
                if (selector == null)
                {
                    if (UserEnum == UserFields.UserName)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.UserName, group.Member);
                    }
                    else if (UserEnum == UserFields.FullName)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.FullName, group.Member);
                    }
                    else if (UserEnum == UserFields.EmailID)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.EmailID, group.Member);
                    }
                    else if (UserEnum == UserFields.Company)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.Company, group.Member);
                    }
                    else if (UserEnum == UserFields.Role)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.Role, group.Member);
                    }
                    else if (UserEnum == UserFields.Active)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.Active, group.Member);
                    }
                }
                else
                {
                    if (UserEnum == UserFields.UserName)
                    {
                        selector = BuildGroup(o => o.UserName, selector, group.Member);
                    }
                    else if (UserEnum == UserFields.FullName)
                    {
                        selector = BuildGroup(o => o.FullName, selector, group.Member);
                    }
                    else if (UserEnum == UserFields.EmailID)
                    {
                        selector = BuildGroup(o => o.EmailID, selector, group.Member);
                    }
                    else if (UserEnum == UserFields.Company)
                    {
                        selector = BuildGroup(o => o.Company, selector, group.Member);
                    }
                    else if (UserEnum == UserFields.Role)
                    {
                        selector = BuildGroup(o => o.Role, selector, group.Member);
                    }
                    else if (UserEnum == UserFields.Active)
                    {
                        selector = BuildGroup(o => o.Active, selector, group.Member);
                    }
                }
            }
            return selector.Invoke(data).ToList();
        }

        private static Func<IEnumerable<UserModel>, IEnumerable<AggregateFunctionsGroup>> BuildGroup<T>(Func<UserModel, T> groupSelector, Func<IEnumerable<UserModel>, IEnumerable<AggregateFunctionsGroup>> selectorBuilder, string Value)
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

        private static IEnumerable<AggregateFunctionsGroup> BuildInnerGroup<T>(IEnumerable<UserModel> group, Func<UserModel, T> groupSelector, string Value)
        {
            return group.GroupBy(groupSelector)
                    .Select(i => new AggregateFunctionsGroup
                    {
                        Key = i.Key,
                        Member = Value,
                        Items = i.ToList()
                    });
        }

        public static IQueryable<UserModel> ApplyPaging(this IQueryable<UserModel> data, int currentPage, int pageSize)
        {
            if (pageSize > 0 && currentPage > 0)
            {
                data = data.Skip((currentPage - 1) * pageSize);
            }

            data = data.Take(pageSize);
            return data;
        }

        public static IQueryable<UserModel> ApplySorting(this IQueryable<UserModel> data, IList<GroupDescriptor> groupDescriptors, IList<SortDescriptor> sortDescriptors)
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

        private static IQueryable<UserModel> AddSortExpression(IQueryable<UserModel> data, ListSortDirection sortDirection, string memberName)
        {
            UserFields UserEnum = GetUserFieldEnum(memberName);
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (UserEnum)
                {
                    case UserFields.UserName:
                        data = data.OrderBy(order => order.UserName);
                        break;
                    case UserFields.FullName:
                        data = data.OrderBy(order => order.FullName);
                        break;
                    case UserFields.EmailID:
                        data = data.OrderBy(order => order.EmailID);
                        break;
                    case UserFields.Company:
                        data = data.OrderBy(order => order.Company);
                        break;
                    case UserFields.Role:
                        data = data.OrderBy(order => order.Role);
                        break;
                    case UserFields.Active:
                        data = data.OrderBy(order => order.Active);
                        break;
                }
            }
            else
            {
                switch (UserEnum)
                {
                    case UserFields.UserName:
                        data = data.OrderByDescending(order => order.UserName);
                        break;
                    case UserFields.FullName:
                        data = data.OrderByDescending(order => order.FullName);
                        break;                    
                    case UserFields.EmailID:
                        data = data.OrderByDescending(order => order.EmailID);
                        break;
                    case UserFields.Company:
                        data = data.OrderByDescending(order => order.Company);
                        break;
                    case UserFields.Role:
                        data = data.OrderByDescending(order => order.Role);
                        break;
                    case UserFields.Active:
                        data = data.OrderByDescending(order => order.Active);
                        break;
                }
            }
            return data;
        }

        private static UserFields GetUserFieldEnum(string FieldValue)
        {
            return (UserFields)Enum.Parse(typeof(UserFields), FieldValue);
        }
    }
}