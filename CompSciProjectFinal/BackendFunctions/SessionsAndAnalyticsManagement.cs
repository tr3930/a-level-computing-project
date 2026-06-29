using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompSciProjectFinal
{
    class SessionsAndAnalyticsManagement //Small class to handle some of the functions related to sesson management
    {
        public static Session StartAndReturnNewSession(UserDetails userDetails) //Starts a new session and returns the ID to be used by the program
        {
            long sessionId = DataFunctions.GenerateIdForPrimaryKey(); //Generate the session ID
            DatabaseManagementV2.InsertNewSession(sessionId, userDetails); //Inserts the session data into the database
            return new Session() //returns a sesssion containing the user details and session id
            {
                userDetails = userDetails,
                sessionId = sessionId
            };
        }

        public static void AddNewEventToSession(Session session, string comment) //Adds a new event to a specified session
        {
            DatabaseManagementV2.InsertNewSessionEvent(new SessionEvent()
            {
                session = session,
                comment = comment,
                eventId = DataFunctions.GenerateIdForPrimaryKey()
            });
        }
    }
}
