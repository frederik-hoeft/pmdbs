using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace debugclient
{
    public struct Commands
    {
        public static void FetchAll(string[] parameters)
        {

        }
        public static void FetchSync(string[] parameters)
        {

        }
        public static void Authenticate(string[] parameters)
        {

        }
        public static void BanAccount(string[] parameters)
        {

        }
        public static void BanClient(string[] parameters)
        {

        }
        public static void ChangeEmailAddress(string[] parameters)
        {

        }
        public static void ChangeName(string[] parameters)
        {

        }
        public static void GetClientLog(string[] parameters)
        {

        }
        public static void CommitAdminPasswordChange(string[] parameters)
        {

        }
        public static void CommitDeleteAccount(string[] parameters)
        {

        }
        public static void CommitPasswordChange(string[] parameters)
        {

        }
        public static void ConfirmNewDevice(string[] parameters)
        {

        }
        public static void GetCookie(string[] parameters)
        {

        }
        public static void Custom(string[] parameters)
        {

        }
        public static void CustomEncrypted(string[] parameters)
        {

        }
        public static void Delete(string[] parameters)
        {

        }
        public static void DisableDebugging(string[] parameters)
        {

        }
        public static void EnableDebugging(string[] parameters)
        {

        }
        public static void Error(string[] parameters)
        {

        }
        public static void Exit(string[] parameters)
        {
            for (int i = 1; i < parameters.Count(); i++)
            {
                switch (parameters[i])
                {
                    case "--help":
                    case"-h":
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(parameters[0]);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(" is used to quit the program.\n\nThis command takes no arguments.\n");
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }
        public static void GetAccountActivity(string[] parameters)
        {

        }
        public static void Help(string[] parameters)
        {

        }
        public static void InitAdminPasswordChange(string[] parameters)
        {

        }
        public static void InitPasswordChange(string[] parameters)
        {

        }
        public static void InitDeleteAccount(string[] parameters)
        {

        }
        public static void Insert(string[] parameters)
        {

        }
        public static void Kick(string[] parameters)
        {

        }
        public static void ListAllClients(string[] parameters)
        {

        }
        public static void ListAllUsers(string[] parameters)
        {

        }
        public static void Login(string[] parameters)
        {

        }
        public static void Logout(string[] parameters)
        {

        }
        public static void CheckCredentials(string[] parameters)
        {

        }
        public static void NewAdminDevice(string[] parameters)
        {

        }
        public static void Reboot(string[] parameters)
        {

        }
        public static void Register(string[] parameters)
        {

        }
        public static void ResendCode(string[] parameters)
        {

        }
        public static void Select(string[] parameters)
        {

        }
        public static void Shutdown(string[] parameters)
        {

        }
        public static void GetServerLog(string[] parameters)
        {

        }
        public static void Start(string[] parameters)
        {

        }
        public static void Sudo(string[] parameters)
        {

        }
        public static void Update(string[] parameters)
        {

        }
        public static void ActivateAccount(string[] parameters)
        {

        }
    }
}
