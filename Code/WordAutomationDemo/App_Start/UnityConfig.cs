using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using WordAutomationDemo.Common;
using WordAutomationDemo.Repository.Interfaces;
using WordAutomationDemo.Repository.Classes;

namespace WordAutomationDemo
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            
            container.RegisterType<ICompanyHelper, CompanyHelper>();
            container.RegisterType<IRole, RoleRepository>();
            container.RegisterType<IUser, UserRepository>();
            container.RegisterType<ICompany, CompanyRepository>();
            container.RegisterType<IRolePrivileges, RolePrivilegesRepository>();
            container.RegisterType<IUser, UserRepository>();
            container.RegisterType<ILogin, LoginRepository>();
            container.RegisterType<ILogActivity, LogActivityRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}