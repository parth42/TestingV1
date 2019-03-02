using Kendo.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Controllers
{
    public static class CompanyCustomBinding
    {
        public enum CompanyFields
        {
            Name,
            Address,
            Zip,
            City,
            State,
            Country,
            CreatedBy,
            CreatedDate,
            Status,
            WebsiteURL
        }

        public static IQueryable<CompanyModel> ApplyFiltering(this IQueryable<CompanyModel> data, IList<IFilterDescriptor> filterDescriptors)
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

        private static IQueryable<CompanyModel> ApplyFilter(IQueryable<CompanyModel> source, IFilterDescriptor filter)
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
                CompanyFields UserEnum = (CompanyFields)Enum.Parse(typeof(CompanyFields), filterDescriptor.Member);
                switch (UserEnum)
                {

                    case CompanyFields.Name:
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

                    case CompanyFields.Address:
                        string address = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Address) && o.Address.ToLower().Contains(address));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Address) && !o.Address.ToLower().Contains(address));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Address) && Equals(o.Address.ToLower(), address) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Address) && Equals(o.Address.ToLower(), address) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Address) && o.Address.ToLower().StartsWith(address) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Address) && o.Address.ToLower().EndsWith(address) == true));
                                break;
                        }
                        break;

                    case CompanyFields.Zip:
                        string zip = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Zip) && o.Zip.ToLower().Contains(zip));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Zip) && !o.Zip.ToLower().Contains(zip));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Zip) && Equals(o.Zip.ToLower(), zip) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Zip) && Equals(o.Zip.ToLower(), zip) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Zip) && o.Zip.ToLower().StartsWith(zip) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Zip) && o.Zip.ToLower().EndsWith(zip) == true));
                                break;
                        }
                        break;

                    case CompanyFields.City:
                        string city = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.City) && o.City.ToLower().Contains(city));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.City) && !o.City.ToLower().Contains(city));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.City) && Equals(o.City.ToLower(), city) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.City) && Equals(o.City.ToLower(), city) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.City) && o.City.ToLower().StartsWith(city) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.City) && o.City.ToLower().EndsWith(city) == true));
                                break;
                        }
                        break;

                    case CompanyFields.State:
                        string state = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.State) && o.State.ToLower().Contains(state));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.State) && !o.State.ToLower().Contains(state));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.State) && Equals(o.State.ToLower(), state) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.State) && Equals(o.State.ToLower(), state) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.State) && o.State.ToLower().StartsWith(state) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.State) && o.State.ToLower().EndsWith(state) == true));
                                break;
                        }
                        break;

                    case CompanyFields.Country:
                        string country = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Country) && o.Country.ToLower().Contains(country));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.Country) && !o.Country.ToLower().Contains(country));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Country) && Equals(o.Country.ToLower(), country) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Country) && Equals(o.Country.ToLower(), country) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Country) && o.Country.ToLower().StartsWith(country) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.Country) && o.Country.ToLower().EndsWith(country) == true));
                                break;
                        }
                        break;                                        

                    case CompanyFields.CreatedDate:
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

                    case CompanyFields.Status:
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

                    case CompanyFields.WebsiteURL:
                        string WebsiteURL = Convert.ToString(filterDescriptor.Value).ToLower().Trim();
                        switch (filterDescriptor.Operator)
                        {
                            case FilterOperator.Contains:
                                source = source.Where(o => !string.IsNullOrEmpty(o.WebsiteURL) && o.WebsiteURL.ToLower().Contains(WebsiteURL));
                                break;
                            case FilterOperator.DoesNotContain:
                                source = source.Where(o => !string.IsNullOrEmpty(o.WebsiteURL) && !o.WebsiteURL.ToLower().Contains(WebsiteURL));
                                break;
                            case FilterOperator.IsEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.WebsiteURL) && Equals(o.WebsiteURL.ToLower(), WebsiteURL) == true));
                                break;
                            case FilterOperator.IsNotEqualTo:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.WebsiteURL) && Equals(o.WebsiteURL.ToLower(), WebsiteURL) == false));
                                break;
                            case FilterOperator.StartsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.WebsiteURL) && o.WebsiteURL.ToLower().StartsWith(WebsiteURL) == true));
                                break;
                            case FilterOperator.EndsWith:
                                source = source.Where(o => (!string.IsNullOrEmpty(o.WebsiteURL) && o.WebsiteURL.ToLower().EndsWith(WebsiteURL) == true));
                                break;
                        }
                        break;
                }
            }
            return source;
        }

        public static IQueryable<CompanyModel> ApplyPaging(this IQueryable<CompanyModel> data, int currentPage, int pageSize)
        {
            if (pageSize > 0 && currentPage > 0)
            {
                data = data.Skip((currentPage - 1) * pageSize);
            }

            data = data.Take(pageSize);
            return data;
        }

        public static IQueryable<CompanyModel> ApplySorting(this IQueryable<CompanyModel> data, IList<GroupDescriptor> groupDescriptors, IList<SortDescriptor> sortDescriptors)
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

        private static IQueryable<CompanyModel> AddSortExpression(IQueryable<CompanyModel> data, ListSortDirection sortDirection, string memberName)
        {
            CompanyFields UserEnum = GetUserFieldEnum(memberName);
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (UserEnum)
                {
                    case CompanyFields.Name:
                        data = data.OrderBy(order => order.Name);
                        break;
                    case CompanyFields.Address:
                        data = data.OrderBy(order => order.Address);
                        break;
                    case CompanyFields.City:
                        data = data.OrderBy(order => order.City);
                        break;
                    case CompanyFields.Country:
                        data = data.OrderBy(order => order.Country);
                        break;
                    case CompanyFields.State:
                        data = data.OrderBy(order => order.State);
                        break;
                    case CompanyFields.Zip:
                        data = data.OrderBy(order => order.Zip);
                        break;
                    case CompanyFields.CreatedDate:
                        data = data.OrderBy(order => order.Zip);
                        break;
                    case CompanyFields.Status:
                        data = data.OrderBy(order => order.strStatus);
                        break;
                    case CompanyFields.WebsiteURL:
                        data = data.OrderBy(order => order.WebsiteURL);
                        break;
                }
            }
            else
            {
                switch (UserEnum)
                {
                    case CompanyFields.Name:
                        data = data.OrderByDescending(order => order.Name);
                        break;
                    case CompanyFields.Address:
                        data = data.OrderByDescending(order => order.Address);
                        break;
                    case CompanyFields.City:
                        data = data.OrderByDescending(order => order.City);
                        break;
                    case CompanyFields.Country:
                        data = data.OrderByDescending(order => order.Country);
                        break;
                    case CompanyFields.State:
                        data = data.OrderByDescending(order => order.State);
                        break;
                    case CompanyFields.Zip:
                        data = data.OrderByDescending(order => order.Zip);
                        break;
                    case CompanyFields.CreatedDate:
                        data = data.OrderByDescending(order => order.Zip);
                        break;
                    case CompanyFields.Status:
                        data = data.OrderByDescending(order => order.strStatus);
                        break;
                    case CompanyFields.WebsiteURL:
                        data = data.OrderByDescending(order => order.WebsiteURL);
                        break;
                }
            }
            return data;
        }

        private static CompanyFields GetUserFieldEnum(string FieldValue)
        {
            return (CompanyFields)Enum.Parse(typeof(CompanyFields), FieldValue);
        }
    }
}