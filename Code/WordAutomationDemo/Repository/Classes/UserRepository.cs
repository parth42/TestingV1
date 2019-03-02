using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Interfaces;

namespace WordAutomationDemo.Repository.Classes
{
    public class UserRepository : IUser
    {
        private readonly ReadyPortalDBEntities _entities;
        ILogActivity iLogActivity;

        public UserRepository(ReadyPortalDBEntities entities, ILogActivity ILogActivity)
        {
            _entities = entities;
            iLogActivity = ILogActivity;
        }

        public IQueryable<UserModel> Userlist()
        {
            if (CurrentUserSession.User.IsSuperAdmin)
            {
                var UserData = (from p in _entities.tblUserDepartments
                                orderby p.UserName
                                select new UserModel
                                {
                                    UserId = p.UserId,
                                    UserName = p.UserName,
                                    FullName = p.FullName,
                                    EmailID = p.EmailID,
                                    Role_ID = p.RoleID.HasValue ? p.RoleID.Value : 0,
                                    Role = p.tblRole != null ? p.tblRole.Role : "",
                                    IsAdminUser = p.tblRole != null ? p.tblRole.IsAdminRole : false,
                                    IsCurrentUser = CurrentUserSession.UserID == p.UserId ? true : false,
                                    Active = p.IsActive == true ? Global.ActiveText : Global.InActiveText,
                                    Company_ID = p.CompanyID.HasValue ? p.CompanyID.Value : 0,
                                    Company = p.tblCompany != null ? p.tblCompany.Name : "",
                                    IsAppointmentEnable = p.IsAppointmentEnable
                                });
                return UserData;
            }
            else
            {
                var UserData = (from p in _entities.tblUserDepartments
                                where p.CompanyID == CurrentUserSession.User.CompanyID
                                orderby p.UserName
                                select new UserModel
                                {
                                    UserId = p.UserId,
                                    UserName = p.UserName,
                                    FullName = p.FullName,
                                    EmailID = p.EmailID,
                                    Role_ID = p.RoleID.HasValue ? p.RoleID.Value : 0,
                                    Role = p.tblRole != null ? p.tblRole.Role : "",
                                    IsAdminUser = p.tblRole != null ? p.tblRole.IsAdminRole : false,
                                    IsCurrentUser = CurrentUserSession.UserID == p.UserId ? true : false,
                                    Active = p.IsActive == true ? Global.ActiveText : Global.InActiveText,
                                    Company_ID = p.CompanyID.HasValue ? p.CompanyID.Value : 0,
                                    Company = p.tblCompany != null ? p.tblCompany.Name : "",
                                    IsAppointmentEnable = p.IsAppointmentEnable,
                                    CanCreateSubtasks = p.CanCreateSubtasks.HasValue ? p.CanCreateSubtasks.Value : false,
                                    CanEdit = p.CanEdit.HasValue ? p.CanEdit.Value : false,
                                    CanApprove = p.CanApprove.HasValue ? p.CanApprove.Value : false
                                });
                return UserData;
            }
        }

        public int GetUserCount(string userName, string email, int userID, int companyID = 0)
        {
            if (companyID > 0)
            {
                return (from p in _entities.tblUserDepartments
                        where
                        (string.IsNullOrEmpty(userName) || p.UserName == userName) && (string.IsNullOrEmpty(email) || p.EmailID == email)
                        && (userID == 0 || p.UserId != userID)
                        && p.CompanyID == companyID
                        select p.UserId).Count();
            }
            else {
                return (from p in _entities.tblUserDepartments
                        where
                        (string.IsNullOrEmpty(userName) || p.UserName == userName) && (string.IsNullOrEmpty(email) || p.EmailID == email)
                        && (userID == 0 || p.UserId != userID)
                        select p.UserId).Count();
            }
        }

        public bool IsUserExistsByEmail(string FieldList, string ValueList, int strAddOrEditID, int companyID = 0)
        {
            bool IsExists = false;
            string strUserName = string.Empty;
            if (!string.IsNullOrEmpty(FieldList))
            {
                if (strAddOrEditID == -1 && strAddOrEditID != 0)
                {
                    if (FieldList == "UserName")
                        strUserName = this.GetUserCount(ValueList.Trim(), string.Empty, 0, companyID) == 0 ? null : "1";
                    else
                        strUserName = this.GetUserCount(string.Empty, ValueList.Trim(), 0, companyID) == 0 ? null : "1";
                }
                else if (strAddOrEditID != 0)
                {
                    if (FieldList == "UserName")
                        strUserName = this.GetUserCount(ValueList.Trim(), string.Empty, strAddOrEditID, companyID) == 0 ? null : "1";
                    else
                        strUserName = this.GetUserCount(string.Empty, ValueList.Trim(), strAddOrEditID, companyID) == 0 ? null : "1";
                }

                if (!string.IsNullOrEmpty(strUserName))
                {
                    IsExists = true;
                }
            }
            return IsExists;
        }

        public bool InsertUser(UserModel _UserModel, string strUserProfile, out tblUserDepartment tblUserDepartmentModel)
        {
            tblUserDepartment _tblUserDepartment = new tblUserDepartment();
            if (!string.IsNullOrEmpty(strUserProfile))
            {
                _tblUserDepartment.ProfileImage = UploadProfileImage(strUserProfile);
            }
            else
            {
                _tblUserDepartment.ProfileImage = string.Empty;
            }
            _tblUserDepartment.FullName = _UserModel.FullName.Trim();
            _tblUserDepartment.Department = !string.IsNullOrEmpty(_UserModel.Department) ? _UserModel.Department.Trim() : null;
            _tblUserDepartment.CompanyID = _UserModel.Company_ID;
            _tblUserDepartment.EmailID = _UserModel.EmailID;
            _tblUserDepartment.RoleID = _UserModel.Role_ID;
            _tblUserDepartment.IsActive = _UserModel.IsActive;
            _tblUserDepartment.UserName = _UserModel.UserName.Trim();
            _tblUserDepartment.Password = Global.Encryption.Encrypt(_UserModel.Password);
            _tblUserDepartment.CreatedBy = CurrentUserSession.UserID;
            _tblUserDepartment.CreatedDate = System.DateTime.Now;
            _tblUserDepartment.CanCreateSubtasks = _UserModel.CanCreateSubtasks;
            _tblUserDepartment.CanEdit = _UserModel.CanEdit;
            _tblUserDepartment.CanApprove = _UserModel.CanApprove;
            _entities.tblUserDepartments.Add(_tblUserDepartment);
            _entities.SaveChanges();
            tblUserDepartmentModel = _tblUserDepartment;
            return true;
        }

        public UserModel GetUserByID(int userID)
        {
            UserModel _UserModel = (from u in _entities.tblUserDepartments
                                    join r in _entities.tblRoles on u.RoleID equals r.RoleID
                                    where u.UserId == userID
                                    select new UserModel
                                    {
                                        UserId = u.UserId,
                                        UserName = u.UserName,
                                        Password = u.Password,
                                        FullName = u.FullName,
                                        Department = u.Department,
                                        EmailID = u.EmailID,
                                        IsActive = u.IsActive.HasValue ? u.IsActive.Value : false,
                                        Active = u.IsActive.HasValue && u.IsActive.Value ? Global.ActiveText : Global.InActiveText,
                                        Role_ID = u.RoleID.HasValue ? u.RoleID.Value : 0,
                                        Role = r.Role,
                                        ProfileImage = u.ProfileImage,
                                        Company_ID = u.CompanyID.HasValue ? u.CompanyID.Value : 0,
                                        Company = u.tblCompany != null ? u.tblCompany.Name : "",
                                        IsAppointmentEnable = u.IsAppointmentEnable,
                                        CanCreateSubtasks = u.CanCreateSubtasks.HasValue ? u.CanCreateSubtasks.Value : false,
                                        CanEdit = u.CanEdit.HasValue ? u.CanEdit.Value : false,
                                        CanApprove = u.CanApprove.HasValue ? u.CanApprove.Value : false
                                    }).FirstOrDefault();
            return _UserModel;
        }

        public bool UpdateUser(UserModel _UserModel, string strUserProfile, HttpPostedFileBase imgUserProfile)
        {
                tblUserDepartment _tblUserDepartment = _entities.tblUserDepartments.Find(_UserModel.UserId);
                if (_tblUserDepartment != null && _tblUserDepartment.UserId > 0)
                {
                    //Upload user profile
                    if (!string.IsNullOrEmpty(strUserProfile))
                    {
                        _tblUserDepartment.ProfileImage = UploadProfileImage(strUserProfile);

                    }
                    else if (!String.IsNullOrEmpty(_tblUserDepartment.ProfileImage) && !string.IsNullOrEmpty(strUserProfile))
                    {
                        DeleteUserProfileImage(_tblUserDepartment.ProfileImage); //Remove previous logo
                        _tblUserDepartment.ProfileImage = string.Empty;

                    }
                    else if (!String.IsNullOrEmpty(_tblUserDepartment.ProfileImage) && string.IsNullOrEmpty(_UserModel.ProfileImage))
                    {
                        DeleteUserProfileImage(_tblUserDepartment.ProfileImage); //Remove previous logo
                        _tblUserDepartment.ProfileImage = string.Empty;
                    }
                    _tblUserDepartment.FullName = _UserModel.FullName.Trim();
                    _tblUserDepartment.Department = !string.IsNullOrEmpty(_UserModel.Department) ? _UserModel.Department.Trim() : null;
                    _tblUserDepartment.EmailID = _UserModel.EmailID;
                    _tblUserDepartment.RoleID = _UserModel.Role_ID;
                    _tblUserDepartment.CompanyID = _UserModel.Company_ID;
                    _tblUserDepartment.UserName = _UserModel.UserName.Trim();
                    _tblUserDepartment.Password = WordAutomationDemo.Common.Global.Encryption.Encrypt(_UserModel.Password);
                    _tblUserDepartment.IsActive = _UserModel.IsActive;
                    _tblUserDepartment.ModifiedBy = CurrentUserSession.UserID;
                    _tblUserDepartment.ModifiedDate = System.DateTime.Now;
                    _tblUserDepartment.CanCreateSubtasks = _UserModel.CanCreateSubtasks;
                    _tblUserDepartment.CanEdit = _UserModel.CanEdit;
                    _tblUserDepartment.CanApprove = _UserModel.CanApprove;
                }
                _entities.SaveChanges();
                return true;
        }

        public bool ChangeProfile(UserModel _UserModel, string strUserProfile, HttpPostedFileBase imgUserProfile)
        {
                tblUserDepartment _tblUserDepartment = _entities.tblUserDepartments.Find(_UserModel.UserId);
                if (_tblUserDepartment != null && _tblUserDepartment.UserId > 0)
                {
                    //Upload user profile
                    if (!string.IsNullOrEmpty(strUserProfile))
                    {
                        _tblUserDepartment.ProfileImage = UploadProfileImage(strUserProfile);

                    }
                    else if (!String.IsNullOrEmpty(_tblUserDepartment.ProfileImage) && !string.IsNullOrEmpty(strUserProfile))
                    {
                        DeleteUserProfileImage(_tblUserDepartment.ProfileImage); //Remove previous logo
                        _tblUserDepartment.ProfileImage = string.Empty;

                    }
                    else if (!String.IsNullOrEmpty(_tblUserDepartment.ProfileImage) && string.IsNullOrEmpty(_UserModel.ProfileImage))
                    {
                        DeleteUserProfileImage(_tblUserDepartment.ProfileImage); //Remove previous logo
                        _tblUserDepartment.ProfileImage = string.Empty;
                    }
                    _tblUserDepartment.FullName = _UserModel.FullName.Trim();
                    _tblUserDepartment.EmailID = _UserModel.EmailID;
                    _tblUserDepartment.Password = WordAutomationDemo.Common.Global.Encryption.Encrypt(_UserModel.Password);
                    _tblUserDepartment.IsAppointmentEnable = _UserModel.IsAppointmentEnable;
                    _tblUserDepartment.ModifiedBy = CurrentUserSession.UserID;
                    _tblUserDepartment.ModifiedDate = System.DateTime.Now;
                }
                _entities.SaveChanges();
                return true;
        }

        public void DeleteUsers(int[] IDs)
        {
            List<UserModel> userList = new List<UserModel>();
            foreach (var id in IDs)
            {
                var userModel = GetUserByID(id);
                if (userModel != null)
                {
                    userList.Add(userModel);
                }
            }

            _entities.tblUserDepartments.RemoveRange(_entities.tblUserDepartments.Where(u => IDs.Contains(u.UserId)).AsEnumerable());
            _entities.SaveChanges();

            foreach(var item in userList)
            {
                iLogActivity.AddLogActivity((byte)Global.LogActivityTypes.UserDeleted, String.Format("User <{0}> deleted successfully", item.UserName), new int[] { item.UserId });
            }
        }

        public void DeleteUserProfileImage(string imgUserProfile)
        {
                string strImagePath = HttpContext.Current.Server.MapPath("~/ApplicationDocuments/ProfileImages/") + imgUserProfile;

                bool isPathExists = System.IO.File.Exists(strImagePath);

                if (isPathExists)
                {
                    System.IO.File.Delete(strImagePath);
                }
        }

        public string UploadProfileImage(string strUserProfile)
        {
            byte[] bytes = Convert.FromBase64String(strUserProfile);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                string strImagePath = HttpContext.Current.Server.MapPath("~/ApplicationDocuments/ProfileImages/");
                System.IO.Directory.CreateDirectory(strImagePath);
                Image image = Image.FromStream(ms);
                string strImageName = DateTime.UtcNow.ToString("yyyyMMddHHmmssffff") + ".png";
                image.Save(strImagePath + strImageName);
                CurrentUserSession.User.ProfileImage = "~/ApplicationDocuments/ProfileImages/" + strImageName;
                return strImageName;
            }
        }

        public void ResetUserPassword(string password, int userID)
        {
            tblUserDepartment objtblUserDepartment = _entities.tblUserDepartments.Find(userID);
            objtblUserDepartment.Password = password;

            _entities.SaveChanges();
        }
    }
}