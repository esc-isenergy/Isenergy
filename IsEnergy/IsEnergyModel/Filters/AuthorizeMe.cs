using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace IsEnergyModel.Filters
{
    public class AuthorizeMeAll
    {

        public static bool Authorize(string Groups, string UserName)
        {
            try
            {

                if (String.IsNullOrEmpty(Groups))
                    return true;
                var groups = Groups.Split(',').ToList<string>();
                foreach (string group in groups)
                {
                    // разбиваем группу на 2 части основная групппа и подгруппа типа: Administrators.MainAdministrators
                    string[] groupSet = group.Split('.');
                    if (groupSet[0] == "Administrators")
                    {
                        if (AutorizeAdministrators(UserName, groupSet)) return true;
                    }
                    else if (groupSet[0] == "Operators")
                    {
                        if (AutorizeOperators(UserName, groupSet)) return true;
                    }
                    else if (groupSet[0] == "AppointChiefSubscriber")
                    {
                        if (GetCurrentAppointChiefSubscriber(UserName)) return true;
                    }
                }
                return false;
            }
            catch { return false; }
        }

        private static bool AutorizeAdministrators(string UserName, string[] groupSet)
        {        
            Is_EnergyEntities db = new Is_EnergyEntities();
            Administrators adminset = db.Administrators.FirstOrDefault(u => u.Users.Login == UserName) ;
            if (groupSet.Count() == 2 && adminset != null)
            {
                switch (groupSet[1])
                {
                    case "MainAdministrators": { if (adminset.MainAdministrators) return true; else return false; }
                    case "ModifyingAdministrators": { if (adminset.ModifyingAdministrators) return true; else return false; }
                    case "ModifyingOperators": { if (adminset.ModifyingOperators) return true; else return false; }
                    case "ModifyingNews": { if (adminset.ModifyingNews) return true; else return false; }
                    case "ModifyingMailing": { if (adminset.ModifyingMailing) return true; else return false; }
                    default: return false;
                }
            }
            else if (adminset != null) { return true; }
            else return false;

        }

        private static bool AutorizeOperators(string UserName, string[] groupSet)
        {
            Is_EnergyEntities db = new Is_EnergyEntities();
            Operators operators = db.Operators.FirstOrDefault(u => u.Users.Login == UserName) ?? null;
            if (groupSet.Count() == 2 && operators != null)
            {
                switch (groupSet[1])
                {
                    case "MainOperator": { if (operators.MainOperator) return true; else return false; }
                    case "ModifyingNews": { if (operators.ModifyingNews) return true; else return false; }
                    case "ModifyingMailing": { if (operators.ModifyingMailing) return true; else return false; }
                    default: return false;
                }
            }
            else if (operators != null) { return true; }
            else return false;

        }

        public static Users GetCurrentUser(string UserName)
        {
            Is_EnergyEntities db = new Is_EnergyEntities();
            return db.Users.FirstOrDefault(u => u.Login == UserName);
        }

        public static Subscribers GetCurrentSubscriber(Users user)
        {
            Is_EnergyEntities db = new Is_EnergyEntities();
            if (user.IdentifierSubscriberDefault != null)
            {
                if (user != null) { return db.Subscribers.FirstOrDefault(u => u.IdentifierSubscriber == user.IdentifierSubscriberDefault); }
            }
            return null;
        }

        public static Subscribers GetCurrentSubscriber(string userName)
        {
            Users user = GetCurrentUser(userName);
            Is_EnergyEntities db = new Is_EnergyEntities();
            if (user.IdentifierSubscriberDefault != null)
            {
                if (user != null) { return db.Subscribers.FirstOrDefault(u => u.IdentifierSubscriber == user.IdentifierSubscriberDefault); }
            }
            return null;
        }

        public static SubscribersSubdivision GetCurrentSubscribersSubdivision(string userName)
        {
            Users user = GetCurrentUser(userName);
            if (user != null)
            {
                Subscribers subscribers = GetCurrentSubscriber(user);
                if (subscribers != null)
                {
                    Is_EnergyEntities db = new Is_EnergyEntities();
                    foreach (SubscribersSubdivision subscribersSubdivision in subscribers.SubscribersSubdivision)
                    {
                        SubscribersUserList userList = subscribersSubdivision.SubscribersUserList.FirstOrDefault(u => u.IdUser == user.IdUser);
                        if (userList != null) return subscribersSubdivision;
                    }
                }
            }
            return null;
        }

        public static bool GetCurrentAppointChiefSubscriber(string userName)
        {
            try
            {
                Users user = GetCurrentUser(userName);
                if (user != null)
                {
                    Subscribers subscribers = GetCurrentSubscriber(user);
                    if (subscribers != null)
                    {
                        Is_EnergyEntities db = new Is_EnergyEntities();
                        foreach (SubscribersSubdivision subscribersSubdivision in subscribers.SubscribersSubdivision)
                        {
                            SubscribersUserList userList = subscribersSubdivision.SubscribersUserList.FirstOrDefault(u => u.IdUser == user.IdUser);
                            if (userList != null) return userList.Active;
                        }

                    }
                }
                return false;
            }
            catch { return false; }
        }

        public static List<Subscribers> GetSubscribersUser(Users user)
        {
            Is_EnergyEntities db = new Is_EnergyEntities();
            List<Subscribers> ListSubscribers = new List<Subscribers>();
            if (user != null)
            {
                ICollection<SubscribersUserList> subscribersUserList = user.SubscribersUserList;
                foreach (SubscribersUserList SubscribersUser in subscribersUserList)
                {
                    ListSubscribers.Add(SubscribersUser.SubscribersSubdivision.Subscribers);
                }
            }
            return ListSubscribers;
        }

    }

    public class AuthorizeMeAttribute : AuthorizeAttribute
    {
        public string Groups { get; set; }


        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return AuthorizeMeAll.Authorize(Groups, httpContext.User.Identity.Name);
        }
    }


}