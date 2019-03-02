﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kendo.Mvc;
using System.Collections;
using Kendo.Mvc.Infrastructure;
using System.ComponentModel;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.CustomGridBinding
{
    public static class RoleCustomBinding
    {
        public enum RoleFields
        {
            Role,
            Company,
            Active
        }

        public static IQueryable<RoleModel> ApplyFiltering(this IQueryable<RoleModel> data, IList<IFilterDescriptor> filterDescriptors)
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

        private static IQueryable<RoleModel> ApplyFilter(IQueryable<RoleModel> source, IFilterDescriptor filter)
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
                RoleFields RoleEnum = (RoleFields)Enum.Parse(typeof(RoleFields), filterDescriptor.Member);
                switch (RoleEnum)
                {
                    case RoleFields.Role:
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

                    case RoleFields.Company:
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

                    case RoleFields.Active:
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

        public static IEnumerable ApplyGrouping(this IQueryable<RoleModel> data, IList<GroupDescriptor> groupDescriptors)
        {
            Func<IEnumerable<RoleModel>, IEnumerable<AggregateFunctionsGroup>> selector = null;

            foreach (var group in groupDescriptors.Reverse())
            {
                RoleFields RoleEnum = GetRoleFieldEnum(group.Member);
                if (selector == null)
                {
                    if (RoleEnum == RoleFields.Role)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.Role, group.Member);
                    }
                    else if (RoleEnum == RoleFields.Company)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.Company, group.Member);
                    }
                    else if (RoleEnum == RoleFields.Active)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.Active, group.Member);
                    }
                }
                else
                {
                    if (RoleEnum == RoleFields.Role)
                    {
                        selector = BuildGroup(o => o.Role, selector, group.Member);
                    }
                    else if (RoleEnum == RoleFields.Company)
                    {
                        selector = BuildGroup(o => o.Company, selector, group.Member);
                    }
                    else if (RoleEnum == RoleFields.Active)
                    {
                        selector = BuildGroup(o => o.Active, selector, group.Member);
                    }
                }
            }
            return selector.Invoke(data).ToList();
        }

        private static Func<IEnumerable<RoleModel>, IEnumerable<AggregateFunctionsGroup>> BuildGroup<T>(Func<RoleModel, T> groupSelector, Func<IEnumerable<RoleModel>, IEnumerable<AggregateFunctionsGroup>> selectorBuilder, string Value)
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

        private static IEnumerable<AggregateFunctionsGroup> BuildInnerGroup<T>(IEnumerable<RoleModel> group, Func<RoleModel, T> groupSelector, string Value)
        {
            return group.GroupBy(groupSelector)
                    .Select(i => new AggregateFunctionsGroup
                    {
                        Key = i.Key,
                        Member = Value,
                        Items = i.ToList()
                    });
        }

        public static IQueryable<RoleModel> ApplyPaging(this IQueryable<RoleModel> data, int currentPage, int pageSize)
        {
            if (pageSize > 0 && currentPage > 0)
            {
                data = data.Skip((currentPage - 1) * pageSize);
            }

            data = data.Take(pageSize);
            return data;
        }

        public static IQueryable<RoleModel> ApplySorting(this IQueryable<RoleModel> data, IList<GroupDescriptor> groupDescriptors, IList<SortDescriptor> sortDescriptors)
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

        private static IQueryable<RoleModel> AddSortExpression(IQueryable<RoleModel> data, ListSortDirection sortDirection, string memberName)
        {
            RoleFields RoleEnum = GetRoleFieldEnum(memberName);
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (RoleEnum)
                {
                    case RoleFields.Role:
                        data = data.OrderBy(order => order.Role);
                        break;
                    case RoleFields.Company:
                        data = data.OrderBy(order => order.Company);
                        break;
                    case RoleFields.Active:
                        data = data.OrderBy(order => order.Active);
                        break;
                }
            }
            else
            {
                switch (RoleEnum)
                {
                    case RoleFields.Role:
                        data = data.OrderByDescending(order => order.Role);
                        break;
                    case RoleFields.Company:
                        data = data.OrderByDescending(order => order.Company);
                        break;
                    case RoleFields.Active:
                        data = data.OrderByDescending(order => order.Active);
                        break;
                }
            }
            return data;
        }

        private static RoleFields GetRoleFieldEnum(string FieldValue)
        {
            return (RoleFields)Enum.Parse(typeof(RoleFields), FieldValue);
        }
    }
}