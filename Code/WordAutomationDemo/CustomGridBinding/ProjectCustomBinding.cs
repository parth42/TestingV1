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
    public static class ProjectCustomBinding
    {
        public enum ProjectFields
        {
            Name,
            Description,
            StartDate,
            EndDate,
            CreatedDate,
            strStatus,

        }

        public static IQueryable<ProjectModel> ApplyFiltering(this IQueryable<ProjectModel> data, IList<IFilterDescriptor> filterDescriptors)
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

        private static IQueryable<ProjectModel> ApplyFilter(IQueryable<ProjectModel> source, IFilterDescriptor filter)
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
                ProjectFields UserEnum = (ProjectFields)Enum.Parse(typeof(ProjectFields), filterDescriptor.Member);
                switch (UserEnum)
                {

                    case ProjectFields.Name:
                        string name = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Name) && o.Name.ToLower().Contains(name));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Name) && !o.Name.ToLower().Contains(name));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Name) && Equals(o.Name.ToLower(), name) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Name) && Equals(o.Name.ToLower(), name) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Name) && o.Name.ToLower().StartsWith(name) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Name) && o.Name.ToLower().EndsWith(name) == true));
                                break;
                        }
                        break;

                    case ProjectFields.Description:
                        string desc = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Description) && o.Description.ToLower().Contains(desc));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Description) && !o.Description.ToLower().Contains(desc));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Description) && Equals(o.Description.ToLower(), desc) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Description) && Equals(o.Description.ToLower(), desc) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Description) && o.Description.ToLower().StartsWith(desc) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Description) && o.Description.ToLower().EndsWith(desc) == true));
                                break;
                        }
                        break;

                    case ProjectFields.StartDate:
                        DateTime startDate = Convert.ToDateTime(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => o.StartDate != null && (DateTime)o.StartDate.Date == startDate);
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => o.StartDate != null && (DateTime)o.StartDate.Date != startDate);
                                break;
                            case FilterOperator.IsLessThan:
                                source = source.Where(o => o.StartDate != null && (DateTime)o.StartDate.Date < startDate);
                                break;
                            case FilterOperator.IsLessThanOrEqualTo:
                                source = source.Where(o => o.StartDate != null && (DateTime)o.StartDate.Date <= startDate);
                                break;
                            case FilterOperator.IsGreaterThan:
                                source = source.Where(o => o.StartDate != null && (DateTime)o.StartDate.Date > startDate);
                                break;
                            case FilterOperator.IsGreaterThanOrEqualTo:
                                source = source.Where(o => o.StartDate != null && (DateTime)o.StartDate.Date >= startDate);
                                break;
                        }
                        break;

                    case ProjectFields.EndDate:
                        DateTime? endDate = Convert.ToDateTime(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => o.EndDate != null && (DateTime)o.EndDate.Date == endDate);
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => o.EndDate != null && (DateTime)o.EndDate.Date != endDate);
                                break;
                            case FilterOperator.IsLessThan:
                                source = source.Where(o => o.EndDate != null && (DateTime)o.EndDate.Date < endDate);
                                break;
                            case FilterOperator.IsLessThanOrEqualTo:
                                source = source.Where(o => o.EndDate != null && (DateTime)o.EndDate.Date <= endDate);
                                break;
                            case FilterOperator.IsGreaterThan:
                                source = source.Where(o => o.EndDate != null && (DateTime)o.EndDate.Date > endDate);
                                break;
                            case FilterOperator.IsGreaterThanOrEqualTo:
                                source = source.Where(o => o.EndDate != null && (DateTime)o.EndDate.Date >= endDate);
                                break;
                        }
                        break;

                    case ProjectFields.CreatedDate:
                        DateTime created = Convert.ToDateTime(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Date == created);
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Date != created);
                                break;
                            case FilterOperator.IsLessThan:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Date < created);
                                break;
                            case FilterOperator.IsLessThanOrEqualTo:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Date <= created);
                                break;
                            case FilterOperator.IsGreaterThan:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Date > created);
                                break;
                            case FilterOperator.IsGreaterThanOrEqualTo:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Date >= created);
                                break;
                        }
                        break;





                    case ProjectFields.strStatus:
                        string status = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.strStatus) && o.strStatus.ToLower().Contains(status));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.strStatus) && !o.strStatus.ToLower().Contains(status));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.strStatus) && Equals(o.strStatus.ToLower(), status) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.strStatus) && Equals(o.strStatus.ToLower(), status) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.strStatus) && o.strStatus.ToLower().StartsWith(status) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.strStatus) && o.strStatus.ToLower().EndsWith(status) == true));
                                break;
                        }
                        break;
                }
            }
            return source;
        }




        public static IQueryable<ProjectModel> ApplyPaging(this IQueryable<ProjectModel> data, int currentPage, int pageSize)
        {
            if (pageSize > 0 && currentPage > 0)
            {
                data = data.Skip((currentPage - 1) * pageSize);
            }

            data = data.Take(pageSize);
            return data;
        }

        public static IQueryable<ProjectModel> ApplySorting(this IQueryable<ProjectModel> data, IList<GroupDescriptor> groupDescriptors, IList<SortDescriptor> sortDescriptors)
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

        private static IQueryable<ProjectModel> AddSortExpression(IQueryable<ProjectModel> data, ListSortDirection sortDirection, string memberName)
        {
            ProjectFields UserEnum = GetUserFieldEnum(memberName);
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (UserEnum)
                {
                    case ProjectFields.Name:
                        data = data.OrderBy(order => order.Name);
                        break;
                    case ProjectFields.Description:
                        data = data.OrderBy(order => order.Description);
                        break;
                    case ProjectFields.EndDate:
                        data = data.OrderBy(order => order.EndDate);
                        break;
                    case ProjectFields.StartDate:
                        data = data.OrderBy(order => order.StartDate);
                        break;
                    case ProjectFields.CreatedDate:
                        data = data.OrderBy(order => order.CreatedDate);
                        break;
                    case ProjectFields.strStatus:
                        data = data.OrderBy(order => order.strStatus);
                        break;

                }
            }
            else
            {
                switch (UserEnum)
                {
                    case ProjectFields.Name:
                        data = data.OrderByDescending(order => order.Name);
                        break;
                    case ProjectFields.Description:
                        data = data.OrderByDescending(order => order.Description);
                        break;
                    case ProjectFields.EndDate:
                        data = data.OrderByDescending(order => order.EndDate);
                        break;
                    case ProjectFields.CreatedDate:
                        data = data.OrderByDescending(order => order.CreatedDate);
                        break;
                    case ProjectFields.StartDate:
                        data = data.OrderByDescending(order => order.StartDate);
                        break;
                    case ProjectFields.strStatus:
                        data = data.OrderByDescending(order => order.strStatus);
                        break;
                }
            }
            return data;
        }

        private static ProjectFields GetUserFieldEnum(string FieldValue)
        {
            return (ProjectFields)Enum.Parse(typeof(ProjectFields), FieldValue);
        }
    }
}