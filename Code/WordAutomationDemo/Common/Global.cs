using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Common
{
    public static class Global
    {
        public enum Action
        {
            [Description("Assigned")]
            Assigned = 1,
            [Description("Completed")]
            Completed = 2,
            [Description("Approved")]
            Approved = 3,
            [Description("Declined")]
            Declined = 4,
            [Description("Reassign")]
            Reassign = 5,
            [Description("Published")]
            Published = 6,
            [Description("Comment")]
            Comment = 7,
            [Description("Cancelled")]
            Cancelled = 8
        };

        public enum AssignmentStatus
        {
            [Description("Not Started")]
            NotStarted = 1,
            [Description("In Progress")]
            InProgress = 2,
            [Description("Completed")]
            Completed = 3,
            [Description("Overdue")]
            Overdue = 4
        };

        public enum LogAction
        {
            [Description("Uploaded")]
            Uploaded = 1,
            [Description("Assigned")]
            Assigned = 2,
            [Description("Saved")]
            Saved = 3,
            [Description("Completed")]
            Completed = 4,
            [Description("Approved")]
            Approved = 5,
            [Description("Declined")]
            Declined = 6,
            [Description("Published")]
            Published = 7
        };

        public enum Actions
        {
            Approval,
            Assignment,
            CreateAssignment,
            Index,
            Create,
            Edit,
            Details,
            Delete,
            Complete
        }
        public enum Controlers
        {
            Projects,
            Home,
            Section,
            Role,
            User,
            Company,
            RolePrivileges,
            LogActivity,
            Testing
        }

        public enum ProjectStatus
        {
            [Description("Active")]
            Active = 1,
            [Description("InActive")]
            InActive = 0,
            [Description("Completed")]
            Completed = 2,
        };

        public enum DocumentType
        {
            [Description("Word Document")]
            Word = 1,
            [Description("Excel Document")]
            Xls = 2,
            [Description("Powerpoint Presentation")]
            Ppt = 3,
            [Description("Simple Task")]
            SimpleTask = 4,
        };

        public enum LogActivityTypes
        {
            UserAdded = 1,
            UserDeleted = 2,
            UserUpdated = 3,
            UserRoleCreated = 4,
            UserRoleDeleted = 5,
            UserRoleUpdated = 6,
            RolePrivilegesAdded = 7,
            RolePrivilegesDeleted = 8,
            RolePrivilegesUpdated = 9,
            UserLoggedIn = 10,
            UserLoggedOut = 11,
            PasswordReset = 12,
            EmailSend = 13,
            ChangeProfile = 14,
            ActionVisited = 15,
            ExceptionOccured = 16,
            ErrorOccured = 17,
            Information = 18,
            ProjectCreated = 19,
            ProjectDeleted = 20,
            ProjectUpdated = 21,
            ProjectArchived = 22,
            ProjectCompleted = 23,
            ProjectDocumentAdded = 24,
            ProjectDocumentDeleted = 25,
            ProjectDocumentUpdated = 26,
            ProjectMemberAdded = 27,
            ProjectMemberDeleted = 28,
            ProjectMemberUpdated = 29,
            TaskAdded = 30,
            TaskDeleted = 31,
            TaskUpdated = 32,
            TaskAssigned = 33,
            TaskCompleted = 34,
            TaskApproved = 35,
            TaskDeclined = 36,
            TaskDocumentAdded = 37,
            TaskDocumentDeleted = 38,
            TaskDocumentUpdated = 39,
            DocumentCreated = 40,
            DocumentAdded = 41,
            DocumentDeleted = 42,
            DocumentUpdated = 43,
            DocumentUploaded = 44,
            PPTApproved = 45,
            PPTDeclined = 46,
            PPTAssigned = 47,
            SlideApproved = 48,
            SlideDeclined = 49,
            SlideDeleted = 50,
            SlideCompleted = 51,
            SlideSplited = 52,
            SlidesMerged = 53,
            SnippetSaved = 54,
            SectionAdded = 55,
            SectionDeleted = 56,
            SectionUpdated = 57,
            CompanyCreated = 58,
            CompanyDeleted = 59,
            CompanyUpdated = 60
        }



        public const string ImageExportExtention = ".png";
        public static string ActiveText = "Active";
        public static string InActiveText = "Inactive";
        public static string strEncryptionKey = "CSC";
        public static string ForeignKeyReference = "The DELETE statement conflicted with the REFERENCE constraint";

        public static int[] recordPerPageList = new[] { 5, 10, 20, 50, 100, 200, 500 };
        public static int PageSize = 20;
        public static byte buttonCount = 5;

        public enum SectionContentType
        {
            CreatedDocument = 1,
            Uploaded = 2,
        };

        public class Encryption
        {
            protected static string strKey = Global.strEncryptionKey;

            public static string Encrypt(string TextToBeEncrypted)
            {
                try
                {

                    if (TextToBeEncrypted == string.Empty || TextToBeEncrypted == null)
                    {
                        return TextToBeEncrypted;
                    }

                    RijndaelManaged RijndaelCipher = new RijndaelManaged();
                    string Password = Global.strEncryptionKey;
                    byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(TextToBeEncrypted);
                    byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
                    PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

                    //Creates a symmetric encryptor object.
                    ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                    MemoryStream memoryStream = new MemoryStream();

                    //Defines a stream that links data streams to cryptographic transformations
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
                    cryptoStream.Write(PlainText, 0, PlainText.Length);

                    //Writes the final state and clears the buffer
                    cryptoStream.FlushFinalBlock();
                    byte[] CipherBytes = memoryStream.ToArray();
                    memoryStream.Close();
                    cryptoStream.Close();
                    string EncryptedData = Convert.ToBase64String(CipherBytes);

                    return EncryptedData;
                }
                catch (Exception)
                {
                    return TextToBeEncrypted;
                }
            }

            public static string Decrypt(string TextToBeDecrypted)
            {
                try
                {

                    RijndaelManaged RijndaelCipher = new RijndaelManaged();
                    string Password = Global.strEncryptionKey;
                    string DecryptedData;

                    byte[] EncryptedData = Convert.FromBase64String(TextToBeDecrypted.Replace(' ', '+'));
                    byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

                    //Making of the key for decryption
                    PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

                    //Creates a symmetric Rijndael decryptor object.
                    ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                    MemoryStream memoryStream = new MemoryStream(EncryptedData);

                    //Defines the cryptographics stream for decryption.THe stream contains decrpted data
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
                    byte[] PlainText = new byte[EncryptedData.Length];
                    int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
                    memoryStream.Close();
                    cryptoStream.Close();

                    //Converting to string
                    DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);

                    return DecryptedData;
                }
                catch (Exception)
                {
                    return TextToBeDecrypted;
                }
            }
        }

        public static string GetUniqueKey(int maxSize = 10)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }


            return SiteUrl + result.ToString();

        }

        public static string SMTPServer
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];
            }
        }

        public static string Password
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Password"];
            }
        }
        public static string Port
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Port"];
            }
        }
        public static string Email
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Email"];
            }
        }

        public static string SiteUrl
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["SiteUrl"];
            }
        }

        public static string Role
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Role"];
            }
        }

        public static string MenuItemID
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["MenuItemID"];
            }
        }

        public static string CompanyID
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["CompanyID"];
            }
        }

        public static DateTime GetIndianDateTime(DateTime dt)
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            if (tzi.IsDaylightSavingTime(dt))
            {
                return dt.AddMinutes(570);
            }
            else
            {
                return dt.AddMinutes(630);
            }
        }

        public static string GetEnumDescription(Enum value)
        {
            try
            {

                System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());

                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

                if (attributes != null &&
                    attributes.Length > 0)
                    return attributes[0].Description;
                else
                    return value.ToString();
            }
            catch (Exception)
            {
                return value.ToString();
            }
        }


        public static bool SendEmail(string recipients, string subject, string emailBody, byte[] bytes = null, string BccEmail = "")
        {
            bool isSend = false;
            SmtpClient smtpClient = new SmtpClient();
            MailMessage msgCsutomer = new MailMessage();
            string strFrom = Convert.ToString(Global.Email);
            msgCsutomer.From = new MailAddress(strFrom);
            msgCsutomer.To.Add(recipients);

            if (!string.IsNullOrEmpty(BccEmail))
            {
                msgCsutomer.Bcc.Add(BccEmail);
            }
            msgCsutomer.Subject = subject;
            msgCsutomer.IsBodyHtml = true;
            msgCsutomer.Body = emailBody;

            if (bytes != null)
            {
                msgCsutomer.Attachments.Add(new Attachment(new MemoryStream(bytes), "TaskNotification.pdf"));
            }

            smtpClient.Host = Global.SMTPServer;
            smtpClient.Port = Convert.ToInt32(Global.Port);
            smtpClient.Credentials = new NetworkCredential(Global.Email, Global.Password);
            smtpClient.EnableSsl = true;
            smtpClient.Send(msgCsutomer);
            isSend = true;
            return isSend;
        }

        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = e, Name = e.ToString() };
            return new SelectList(values, "Id", "Name", enumObj);
        }

        public static bool IsADEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["LDAPDomain"]);
            }
        }

        public static string GetPermissionString(bool canPublish, bool canEdit, bool canApprove, string fullName)
        {
            if (canPublish)
            {
                return "<div class=\"tooltip-permission\">" + fullName + "<span class=\"tooltiptext\">Publish</span></div>";
            }
            else if (canEdit)
            {
                return "<div class=\"tooltip-permission\">" + fullName + "<span class=\"tooltiptext\">Edit</span></div>";
            }
            else if (canApprove)
            {
                return "<div class=\"tooltip-permission\">" + fullName + "<span class=\"tooltiptext\">Approve</span></div>";
            }
            else
            {
                return "<div class=\"tooltip-permission\">" + fullName + "<span class=\"tooltiptext\">View</span></div>";
            }
        }

        #region Encryption

        public static string UrlEncrypt(string TextToBeEncrypted)
        {
            if (TextToBeEncrypted == string.Empty || TextToBeEncrypted == null)
            {
                return TextToBeEncrypted;
            }

            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            string Password = strEncryptionKey;
            byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(TextToBeEncrypted);
            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

            //Creates a symmetric encryptor object.
            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream();

            //Defines a stream that links data streams to cryptographic transformations
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(PlainText, 0, PlainText.Length);

            //Writes the final state and clears the buffer
            cryptoStream.FlushFinalBlock();
            byte[] CipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string EncryptedData = Convert.ToBase64String(CipherBytes);

            return EncryptedData.Replace('+', '-').Replace('/', '_').Replace('=', ',');

        }

        #endregion

        #region Decryption

        public static string UrlDecrypt(string TextToBeDecrypted)
        {
            try
            {
                RijndaelManaged RijndaelCipher = new RijndaelManaged();
                TextToBeDecrypted = TextToBeDecrypted.Replace('-', '+').Replace('_', '/').Replace(',', '=');
                string Password = strEncryptionKey;
                string DecryptedData;

                byte[] EncryptedData = Convert.FromBase64String(TextToBeDecrypted.Replace(' ', '+'));
                byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

                //Making of the key for decryption
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

                //Creates a symmetric Rijndael decryptor object.
                ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                MemoryStream memoryStream = new MemoryStream(EncryptedData);

                //Defines the cryptographics stream for decryption.THe stream contains decrpted data
                CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
                byte[] PlainText = new byte[EncryptedData.Length];
                int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
                memoryStream.Close();
                cryptoStream.Close();

                //Converting to string
                DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
                return DecryptedData;
            }
            catch (Exception)
            {
                return TextToBeDecrypted;
            }
        }

        #endregion

        private static Random random = new Random();
        public static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string DateFormatString()
        {
            return WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid;
        }

        public static bool AssignmentMemberCanPublish(int AssignmentID, int UserID)
        {
            using (var _entities = new ReadyPortalDBEntities())
            {
                var assignmentMember = _entities.tblAssignmentMembers.FirstOrDefault(am => am.UserID == UserID && am.AssignmentID == AssignmentID);
                //var canApproveCount = _entities.tblAssignmentMembers.Where(am => am.CanApprove && am.AssignmentID == AssignmentID).Count();

                //Let user publish if no approvers available or if user can approve
                return assignmentMember.CanPublish.HasValue ? assignmentMember.CanPublish.Value : false;
            }
        }
        public static bool AssignmentMemberCanSendForApproval(int AssignmentID, int UserID)
        {
            using (var _entities = new ReadyPortalDBEntities())
            {
                var assignmentMember = _entities.tblAssignmentMembers.FirstOrDefault(am => am.UserID == UserID && am.AssignmentID == AssignmentID);
                //var canApproveCount = _entities.tblAssignmentMembers.Where(am => am.CanApprove && am.AssignmentID == AssignmentID).Count(); //Exclude current user
                var canPublish = assignmentMember.CanPublish.HasValue ? assignmentMember.CanPublish.Value : false;
                //Only send for review when there are other available approvers, o/w. just publish
                return !canPublish;
                
            }
        }

    }
}