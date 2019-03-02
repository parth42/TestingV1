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
namespace WordAutomationDemo.Controllers
{
    public static class SectionCustomBinding
    {
        public enum SectionFields
        {
            SectionName,
            SectionURL,
            Description,
            CreatedBy,
            CreatedDate,
        }

        public static IQueryable<SectionModel> ApplyFiltering(this IQueryable<SectionModel> data, IList<IFilterDescriptor> filterDescriptors)
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

        private static IQueryable<SectionModel> ApplyFilter(IQueryable<SectionModel> source, IFilterDescriptor filter)
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
                SectionFields UserEnum = (SectionFields)Enum.Parse(typeof(SectionFields), filterDescriptor.Member);
                switch (UserEnum)
                {
                    case SectionFields.CreatedDate:
                        DateTime created = Convert.ToDateTime(filterDescriptor.Value);
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Value.Date == created);
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Value.Date != created);
                                break;
                            case FilterOperator.IsLessThan:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Value.Date < created);
                                break;
                            case FilterOperator.IsLessThanOrEqualTo:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Value.Date <= created);
                                break;
                            case FilterOperator.IsGreaterThan:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Value.Date > created);
                                break;
                            case FilterOperator.IsGreaterThanOrEqualTo:
                                source = source.Where(o => o.CreatedDate != null && (DateTime)o.CreatedDate.Value.Date >= created);
                                break;
                        }
                        break;




                    case SectionFields.CreatedBy:
                        string createdby = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.CreatedBy) && o.CreatedBy.ToLower().Contains(createdby));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.CreatedBy) && !o.CreatedBy.ToLower().Contains(createdby));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.CreatedBy) && Equals(o.CreatedBy.ToLower(), createdby) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.CreatedBy) && Equals(o.CreatedBy.ToLower(), createdby) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.CreatedBy) && o.CreatedBy.ToLower().StartsWith(createdby) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.CreatedBy) && o.CreatedBy.ToLower().EndsWith(createdby) == true));
                                break;
                        }
                        break;


                    case SectionFields.Description:
                        string description = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Description) && o.Description.ToLower().Contains(description));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Description) && !o.Description.ToLower().Contains(description));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Description) && Equals(o.Description.ToLower(), description) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Description) && Equals(o.Description.ToLower(), description) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Description) && o.Description.ToLower().StartsWith(description) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Description) && o.Description.ToLower().EndsWith(description) == true));
                                break;
                        }
                        break;



                    case SectionFields.SectionName:
                        string sectionname = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.SectionName) && o.SectionName.ToLower().Contains(sectionname));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.SectionName) && !o.SectionName.ToLower().Contains(sectionname));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.SectionName) && Equals(o.SectionName.ToLower(), sectionname) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.SectionName) && Equals(o.SectionName.ToLower(), sectionname) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.SectionName) && o.SectionName.ToLower().StartsWith(sectionname) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.SectionName) && o.SectionName.ToLower().EndsWith(sectionname) == true));
                                break;
                        }
                        break;

                    case SectionFields.SectionURL:

                        string sectionurl = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.SectionURL) && o.SectionURL.ToLower().Contains(sectionurl));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.SectionURL) && !o.SectionURL.ToLower().Contains(sectionurl));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.SectionURL) && Equals(o.SectionURL.ToLower(), sectionurl) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.SectionURL) && Equals(o.SectionURL.ToLower(), sectionurl) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.SectionURL) && o.SectionURL.ToLower().StartsWith(sectionurl) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.SectionURL) && o.SectionURL.ToLower().EndsWith(sectionurl) == true));
                                break;
                        }
                        break;




                }
            }
            return source;
        }




        public static IQueryable<SectionModel> ApplyPaging(this IQueryable<SectionModel> data, int currentPage, int pageSize)
        {
            if (pageSize > 0 && currentPage > 0)
            {
                data = data.Skip((currentPage - 1) * pageSize);
            }

            data = data.Take(pageSize);
            return data;
        }

        public static IQueryable<SectionModel> ApplySorting(this IQueryable<SectionModel> data, IList<GroupDescriptor> groupDescriptors, IList<SortDescriptor> sortDescriptors)
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

        private static IQueryable<SectionModel> AddSortExpression(IQueryable<SectionModel> data, ListSortDirection sortDirection, string memberName)
        {
            SectionFields UserEnum = GetUserFieldEnum(memberName);
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (UserEnum)
                {
                    case SectionFields.CreatedBy:
                        data = data.OrderBy(order => order.CreatedBy);
                        break;
                    case SectionFields.CreatedDate:
                        data = data.OrderBy(order => order.CreatedDate);
                        break;
                    case SectionFields.Description:
                        data = data.OrderBy(order => order.Description);
                        break;
                    case SectionFields.SectionName:
                        data = data.OrderBy(order => order.SectionName);
                        break;
                    case SectionFields.SectionURL:
                        data = data.OrderBy(order => order.SectionURL);
                        break;
                }
            }
            else
            {
                switch (UserEnum)
                {
                    case SectionFields.CreatedBy:
                        data = data.OrderByDescending(order => order.CreatedBy);
                        break;
                    case SectionFields.CreatedDate:
                        data = data.OrderByDescending(order => order.CreatedDate);
                        break;
                    case SectionFields.Description:
                        data = data.OrderByDescending(order => order.Description);
                        break;
                    case SectionFields.SectionName:
                        data = data.OrderByDescending(order => order.SectionName);
                        break;
                    case SectionFields.SectionURL:
                        data = data.OrderByDescending(order => order.SectionURL);
                        break;
                }
            }
            return data;
        }

        private static SectionFields GetUserFieldEnum(string FieldValue)
        {
            return (SectionFields)Enum.Parse(typeof(SectionFields), FieldValue);
        }
    }
}