using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using WordAutomationDemo.Models;
using WordAutomationDemo.Common;

namespace WordAutomationDemo.SignalR
{
    public class ChatHub : Hub, IChatHub
    {
        ReadyPortalDBEntities objConn = new ReadyPortalDBEntities();
        /// <summary>
        /// This STUB. In a normal situation, there would be multiple rooms and the user room would have to be 
        /// determined by the user profile
        /// </summary>
        //public const string ROOM_ID_STUB = "chatjs-room";
        public static string ROOM_ID_STUB = "";

        /// <summary>
        /// Current connections
        /// 1 room has many users that have many connections (2 open browsers from the same user represents 2 connections)
        /// </summary>
        private static Dictionary<string, Dictionary<int, List<string>>> connections = new Dictionary<string, Dictionary<int, List<string>>>();

        /// <summary>
        /// This is STUB. This will SIMULATE a database of chat messages
        /// </summary>
        private static List<ChatMessage> chatMessages = new List<ChatMessage>();

        /// <summary>
        /// This method is STUB. This will SIMULATE a database of users
        /// </summary>
        private static List<ChatUser> chatUsers = new List<ChatUser>();

        public static List<ChatUser> GetAllUsers()
        {
            return chatUsers;
        }

        public static void ClearUser()
        {
            chatUsers.Clear();
        }

        public static void RemoveInactiveUser(int UserID, string RoomID)
        {
            bool IsRegistered = false;
            IsRegistered = ChatHub.CheckAvailability(UserID, RoomID);
            if (IsRegistered)
            {
                ChatUser objChatUser = chatUsers.Where(e => e.Id == UserID && e.RoomId == RoomID).FirstOrDefault();
                chatUsers.Remove(objChatUser);
            }
        }
        /// <summary>
        /// This method is STUB. In a normal situation, the user info would come from the database so this method wouldn't be necessary.
        /// It's only necessary because this class is simulating the database
        /// </summary>
        /// <param name="newUser"></param>
        public static void RegisterNewUser(ChatUser newUser)
        {
            if (newUser == null)
            {
                throw new ArgumentNullException("newUser");
            }
            chatUsers.Add(newUser);
        }

        /// <summary>
        /// This method is STUB. Returns if a user is registered in the FAKE DB.
        /// Normally this wouldn't be necessary.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsUserRegistered(ChatUser user)
        {
            return chatUsers.Any(u => u.Id == user.Id);
        }

        /// <summary>
        /// Tries to find a user with the provided e-mail
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static ChatUser FindUserByUserID(int userID, string CompId)
        {
            return userID == 0 ? null : chatUsers.FirstOrDefault(u => u.Id == userID && u.RoomId == CompId);
        }

        /// <summary>
        /// If the specified user is connected, return information about the user
        /// </summary>
        public ChatUser GetUserInfo(int userId)
        {
            var user = chatUsers.FirstOrDefault(u => u.Id == userId);
            return user == null ? null : this.GetUserById(userId);
        }

        //---- Updated Code----------------------------
        public static bool CheckAvailability(int userId, string CompId)
        {
            var user = chatUsers.FirstOrDefault(u => u.Id == userId && u.RoomId == CompId);
            return user == null ? false : true;
        }
        //---- Update Code --------------------------
        public static void ChangeStatus(string GroupId, int userId, ChatUser.StatusType status)
        {
            //chatUsers[0].Url = "Testing Url";

            for (int x = 0; x < chatUsers.Count(); x++)
            {
                if (chatUsers[x].Id == userId && chatUsers[x].RoomId == GroupId)
                {
                    chatUsers[x].Status = status;
                    chatUsers[x].LastActiveOn = DateTime.Now;
                }
            }
        }
        public ChatUser GetUserById(int id)
        {
            var myRoomId = this.GetMyRoomId();

            // this is STUB. Normally you would go to the database get the real user
            var dbUser = chatUsers.First(u => u.Id == id);

            ChatUser.StatusType userStatus;
            lock (connections)
            {
                userStatus = connections.ContainsKey(myRoomId)
                                 ? (connections[myRoomId].ContainsKey(dbUser.Id)
                                        ? ChatUser.StatusType.Online
                                        : ChatUser.StatusType.Offline)
                                 : ChatUser.StatusType.Offline;
            }
            return new ChatUser()
            {
                Id = dbUser.Id,
                Name = dbUser.Name,
                Status = userStatus,
                ProfilePictureUrl = dbUser.ProfilePictureUrl,
                RandomColor = dbUser.RandomColor,
            };
        }

        /// <summary>
        /// Returns my user id
        /// </summary>
        /// <returns></returns>
        private int GetMyUserId()
        {
            // This would normally be done like this:
            //var userPrincipal = this.Context.User as AuthenticatedPrincipal;
            //if (userPrincipal == null)
            //    throw new NotAuthorizedException();

            //var userData = userPrincipal.Profile;
            //return userData.Id;

            // But for this example, it will get my user from the cookie
            ChatUser objChatUser = ChatHelper.GetChatUserFromCookie(this.Context.Request);
            return objChatUser != null ? objChatUser.Id : 0;
            //return ChatHelper.GetChatUserFromCookie(this.Context.Request).Id;
        }

        public string GetMyRoomId()
        {
            //----- Modificaton for Getting GroupId -----------------------

            //int Uid = GetMyUserId();
            //string GroupName = chatUsers.FirstOrDefault(u => u.Id == Uid).RoomId.ToString();
            // return GroupName;

            //--------------------------------------------------------------------------

            // This would normally be done like this:
            //var userPrincipal = this.Context.User as AuthenticatedPrincipal;
            //if (userPrincipal == null)
            //    throw new NotAuthorizedException();

            //var userData = userPrincipal.Profile;
            //return userData.MyTenancyIdentifier;

            // But for this example, it will always return "chatjs-room", because we have only one room.
            //return ROOM_ID_STUB;
            ChatUser objChartUser = ChatHelper.GetChatUserFromCookie(this.Context.Request);
            return objChartUser != null ? objChartUser.RoomId : string.Empty;
            //return ChatHelper.GetChatUserFromCookie(this.Context.Request).RoomId;


        }

        /// <summary>
        /// Broadcasts to all users in the same room the new users list
        /// </summary>
        /// <param name="myUserId">
        /// user Id that has to be excluded in the broadcast. That is, all users
        /// should receive the message, except this.
        /// </param>
        private void NotifyUsersChanged()
        {
            var myRoomId = this.GetMyRoomId();
            var myUserId = this.GetMyUserId();


            if (connections.ContainsKey(myRoomId))
            {
                foreach (var userId in connections[myRoomId].Keys)
                {
                    // we don't want to broadcast to the current user
                    if (userId == myUserId)
                        continue;

                    var userIdClusure = userId;

                    // creates a list of users that contains all users with the exception of the user to which 
                    // the list will be sent
                    // every user will receive a list of user that exclude him/hearself
                    var usersList = chatUsers.Where(u => u.Id != userIdClusure && u.RoomId == myRoomId).OrderBy(u => u.Name);

                    if (connections[myRoomId][userId] != null)
                        foreach (var connectionId in connections[myRoomId][userId])
                            this.Clients.Client(connectionId).usersListChanged(usersList);
                }
            }

        }

        //private ChatMessage PersistMessage(int otherUserId, string message, string clientGuid)
        private tblChatMessage PersistMessage(int otherUserId, string message, string clientGuid)
        {
            var myUserId = this.GetMyUserId();

            // this is STUB. Normally you would go to the real database to get the my user and the other user
            var myUser = chatUsers.FirstOrDefault(u => u.Id == myUserId);
            var otherUser = chatUsers.FirstOrDefault(u => u.Id == otherUserId);

            if (myUser == null || otherUser == null)
                return null;

            //var chatMessage = new ChatMessage
            //{
            //    DateTime = DateTime.Now,
            //    Message = message,
            //    ClientGuid = clientGuid,
            //    UserFromId = myUserId,
            //    UserToId = otherUserId
            //};

            // this is STUB. Normally you would add the dbMessage to the real database
            //chatMessages.Add(chatMessage);
            bool isread = false;
            if (otherUser.Status == ChatUser.StatusType.Online)
            {
                isread = true;
            }
            var chatMessage1 = new tblChatMessage
            {
                MessageSentDateTime = DateTime.Now,
                Message = message,
                ClientGuid = clientGuid,
                UserFromId = myUserId,
                UserToId = otherUserId,
                IsRead = isread,
                VisibleFrom = true,
                VisibleTo = true
            };
            objConn.tblChatMessages.Add(chatMessage1);
            this.objConn.SaveChanges();

            // normally you would save the database changes
            //this.db.SaveChanges();

            return chatMessage1;
        }

        #region IChatHub

        /// <summary>
        /// Returns the message history
        /// </summary>
        //public List<ChatMessage> GetMessageHistory(int otherUserId)
        public List<tblChatMessage> GetMessageHistory(int otherUserId)
        {
            var myUserId = this.GetMyUserId();
            // this is STUB. Normally you would go to the real database to get the messages            

            var messages1 = objConn.tblChatMessages.Where(
                                    m =>
                                        (m.UserToId == myUserId && m.UserFromId == otherUserId && m.VisibleTo == true) ||
                                        (m.UserToId == otherUserId && m.UserFromId == myUserId && m.VisibleFrom == true))
                                        .OrderBy(m => m.ChatMessageID);

            foreach (tblChatMessage objMessage in messages1)
            {
                DateTime IndianDateTime = Global.GetIndianDateTime(objMessage.MessageSentDateTime);
                objMessage.MessageSentDateTime = IndianDateTime;
            }

            return messages1.ToList();
        }

        /// <summary>
        /// Returns the message history
        /// </summary>
        public List<ChatUser> GetUsersList()
        {
            var myUserId = this.GetMyUserId();
            var myRoomId = this.GetMyRoomId();
            var roomUsers = (from m in chatUsers
                             join n in objConn.tblUserDepartments
                             on m.Id equals n.UserId
                             where m.RoomId == myRoomId && m.Id != myUserId
                             select m).OrderBy(e => e.Name).ToList();

            //var roomUsers = chatUsers.Where(u => u.RoomId == myRoomId && u.Id != myUserId).OrderBy(u => u.Name).ToList();
            return roomUsers;

            //List<int> OfflineMsgUsers = getOffUsersMsgList();
            //UpdateMsgstatus();

            //this.Clients.Caller.usersListChanged(roomUsers, OfflineMsgUsers);
        }
        public List<int> getOffUsersMsgList()
        {

            var myUserId = this.GetMyUserId();
            var UsrList = objConn.tblChatMessages.Where(
                                     m =>
                                         (m.UserToId == myUserId) && (m.IsRead == false))
                                         .OrderByDescending(m => m.ChatMessageID).Select(m => m.UserFromId).Distinct().ToList();
            UpdateMsgstatus();
            return UsrList;
        }
        public void UpdateMsgstatus()
        {
            int myUserId = this.GetMyUserId();
            tblChatMessage tblChtMsg = new tblChatMessage();
            //tblChtMsg.UserToId = myUserId;
            //tblChtMsg.IsRead = true;
            var msglist = objConn.tblChatMessages.Where(m => m.UserToId == myUserId && m.IsRead == false);

            foreach (var tmp in msglist)
            {
                long MsgId = tmp.ChatMessageID;
                tblChtMsg = objConn.tblChatMessages.Where(m => m.ChatMessageID == MsgId).FirstOrDefault();
                tblChtMsg.IsRead = true;
            }
            objConn.SaveChanges();

        }
        /// <summary>
        /// Sends a message to a particular user
        /// </summary>
        public void SendMessage(int otherUserId, string message, string clientGuid)
        {
            var myUserId = this.GetMyUserId();
            var myRoomId = this.GetMyRoomId();
            string firstlastName = GetFirstLastName(myUserId);
            tblChatMessage dbChatMessage = this.PersistMessage(otherUserId, message, clientGuid);
            var connectionIds = new List<string>();
            lock (connections)
            {
                if (connections != null && connections.Count() > 0)
                {
                    if (connections[myRoomId].ContainsKey(otherUserId))
                        connectionIds.AddRange(connections[myRoomId][otherUserId]);
                    if (connections[myRoomId].ContainsKey(myUserId))
                        connectionIds.AddRange(connections[myRoomId][myUserId]);
                }
            }
            if (dbChatMessage != null)
            {
                dbChatMessage.MessageSentDateTime = Global.GetIndianDateTime(dbChatMessage.MessageSentDateTime);
                foreach (var connectionId in connectionIds)
                    this.Clients.Client(connectionId).sendMessage(dbChatMessage, firstlastName);
            }

        }

        public void SendInstantMessage(int otherUserId, string message, string clientGuid, int MyUID, int myRId)
        {
            var myUserId = MyUID;
            var myRoomId = Convert.ToString(myRId);
            string firstlastName = GetFirstLastName(myUserId);
            tblChatMessage dbChatMessage = this.PersistInstcantMessage(otherUserId, message, clientGuid, MyUID);
            var connectionIds = new List<string>();
            lock (connections)
            {
                if (connections != null && connections.Count() > 0)
                {
                    if (connections[myRoomId].ContainsKey(otherUserId))
                        connectionIds.AddRange(connections[myRoomId][otherUserId]);
                    if (connections[myRoomId].ContainsKey(myUserId))
                        connectionIds.AddRange(connections[myRoomId][myUserId]);
                }
            }

            if (dbChatMessage != null)
            {
                dbChatMessage.MessageSentDateTime = Global.GetIndianDateTime(dbChatMessage.MessageSentDateTime);
                foreach (var connectionId in connectionIds)
                {
                    //this.Clients.Client(connectionId).sendMessage(dbChatMessage, firstlastName);
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    hubContext.Clients.Client(connectionId).sendMessage(dbChatMessage, firstlastName);
                }

            }
        }

        //Start  R-Discussion
        public void SendNotificationMessage(int otherUserId, string message, string clientGuid, int MyUID, int myRId)
        {
            var myUserId = MyUID;
            var myRoomId = Convert.ToString(myRId);
            string firstlastName = GetFirstLastName(myUserId);
            var connectionIds = new List<string>();
            lock (connections)
            {
                if (connections[myRoomId].ContainsKey(otherUserId))
                    connectionIds.AddRange(connections[myRoomId][otherUserId]);
                if (connections[myRoomId].ContainsKey(myUserId))
                    connectionIds.AddRange(connections[myRoomId][myUserId]);
            }

            var chatMessage1 = new tblChatMessage
            {
                MessageSentDateTime = DateTime.Now,
                Message = message,
                ClientGuid = clientGuid,
                UserFromId = myUserId,
                UserToId = otherUserId,
                IsRead = true,
                VisibleFrom = false,
                VisibleTo = false
            };
            foreach (var connectionId in connectionIds)
            {
                //this.Clients.Client(connectionId).sendMessage(dbChatMessage, firstlastName);
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                hubContext.Clients.Client(connectionId).sendMessage(chatMessage1, firstlastName);
            }
        }
        //End  R-Discussion

        private tblChatMessage PersistInstcantMessage(int otherUserId, string message, string clientGuid, int MyUID)
        {
            var myUserId = MyUID;

            // this is STUB. Normally you would go to the real database to get the my user and the other user
            var myUser = chatUsers.FirstOrDefault(u => u.Id == myUserId);
            var otherUser = chatUsers.FirstOrDefault(u => u.Id == otherUserId);

            if (myUser == null || otherUser == null)
                return null;

            // this is STUB. Normally you would add the dbMessage to the real database
            //chatMessages.Add(chatMessage);
            bool isread = false;
            if (otherUser.Status == ChatUser.StatusType.Online)
            {
                isread = true;
            }
            var chatMessage1 = new tblChatMessage
            {
                MessageSentDateTime = DateTime.Now,
                Message = message,
                ClientGuid = clientGuid,
                UserFromId = myUserId,
                UserToId = otherUserId,
                IsRead = isread,
                VisibleFrom = true,
                VisibleTo = true
            };
            objConn.tblChatMessages.Add(chatMessage1);
            this.objConn.SaveChanges();

            // normally you would save the database changes
            //this.db.SaveChanges();

            return chatMessage1;
        }

        public string GetFirstLastName(int UserID)
        {
            string Name = string.Empty;
            var Usr = (from p in objConn.tblUserDepartments
                       where p.UserId == UserID
                       select new
                       {
                           FullName = p.FullName,
                       }).FirstOrDefault();
            if (Usr != null)
                Name = Usr.FullName;
            return Name;
        }

        /// <summary>
        /// Sends a typing signal to a particular user                                                                          
        /// </summary>
        public void SendTypingSignal(int otherUserId)
        {
            var myUserId = this.GetMyUserId();
            var myRoomId = this.GetMyRoomId();

            var connectionIds = new List<string>();
            lock (connections)
            {
                if (connections != null && connections.Count() > 0)
                {
                    if (connections[myRoomId].ContainsKey(otherUserId))
                        connectionIds.AddRange(connections[myRoomId][otherUserId]);
                }
            }
            foreach (var connectionId in connectionIds)
                this.Clients.Client(connectionId).sendTypingSignal(myUserId);
        }
        public void ClearMsgHistory(int myid, int othersid)
        {
            string mid = myid.ToString();
            tblChatMessage tblChtMsg = new tblChatMessage();
            var msglist = objConn.tblChatMessages.Where(
                                     m =>
                                         (m.UserToId == myid && m.UserFromId == othersid && m.VisibleTo == true) ||
                                        (m.UserToId == othersid && m.UserFromId == myid && m.VisibleFrom == true))
                                         .OrderBy(m => m.ChatMessageID).ToList();
            foreach (var tmp in msglist)
            {
                long MsgId = tmp.ChatMessageID;
                tblChtMsg = objConn.tblChatMessages.Where(m => m.ChatMessageID == MsgId).FirstOrDefault();
                if (tmp.UserFromId == myid)
                {
                    tblChtMsg.VisibleFrom = false;
                }
                else if (tmp.UserToId == myid)
                {
                    tblChtMsg.VisibleTo = false;
                }

            }
            objConn.SaveChanges();

        }
        /// <summary>
        /// Triggered when the user opens a new browser window
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            var myRoomId = this.GetMyRoomId();
            var myUserId = this.GetMyUserId();

            lock (connections)
            {
                if (!connections.ContainsKey(myRoomId))
                    connections[myRoomId] = new Dictionary<int, List<string>>();

                if (!connections[myRoomId].ContainsKey(myUserId))
                {
                    // in this case, this is a NEW connection for the current user,
                    // not another browser window opening
                    connections[myRoomId][myUserId] = new List<string>();
                    ChangeStatus(myRoomId, myUserId, ChatUser.StatusType.Online);
                    this.NotifyUsersChanged();
                }
                connections[myRoomId][myUserId].Add(this.Context.ConnectionId);
            }

            return base.OnConnected();
        }

        /// <summary>
        /// Triggered when the user closes the browser window
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnected()
        {
            var myRoomId = this.GetMyRoomId();
            var myUserId = this.GetMyUserId();

            lock (connections)
            {
                if (connections.ContainsKey(myRoomId))
                    if (connections[myRoomId].ContainsKey(myUserId))
                        if (connections[myRoomId][myUserId].Contains(this.Context.ConnectionId))
                        {
                            connections[myRoomId][myUserId].Remove(this.Context.ConnectionId);
                            if (!connections[myRoomId][myUserId].Any())
                            {
                                connections[myRoomId].Remove(myUserId);
                                ////Task.Factory.StartNew(() =>
                                //Task.Run(() =>
                                ////{
                                // this will run in separate thread.
                                // If the user is away for more than 10 seconds it will be removed from 
                                // the room.
                                // In a normal situation this wouldn't be done because normally the users in a
                                // chat room are fixed, like when you have 1 chat room for each tenancy
                                //// Thread.Sleep(10000);
                                if (!connections[myRoomId].ContainsKey(myUserId))
                                {
                                    var myDbUser = chatUsers.FirstOrDefault(u => u.Id == myUserId);
                                    if (myDbUser != null)
                                    {
                                        //chatUsers.Remove(myDbUser);
                                        ChangeStatus(myRoomId, myUserId, ChatUser.StatusType.Offline);
                                        this.NotifyUsersChanged();
                                    }
                                }
                                ////  });
                            }
                        }
            }

            return base.OnDisconnected();
        }

        #endregion
    }
}