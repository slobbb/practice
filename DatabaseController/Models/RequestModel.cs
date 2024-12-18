using System;
using System.Collections.Generic;

namespace DatabaseController.Models
{
    public class RequestModel
    {
        public int requestID;
        public DateTime startDate;
        public KeyValuePair<int, string> homeTechModel;
        public string problemDescription;
        public KeyValuePair<int, string> requestStatus;
        public DateTime? completionDate;
        public KeyValuePair <int, string>? master;
        public KeyValuePair<int, string>  customer;

        public RequestModel
            (
            int requestID, 
            DateTime startDate,
            KeyValuePair<int, string> homeTechModel, 
            string problemDescription,
            KeyValuePair<int, string> requestStatus,
            DateTime? completionDate,
            KeyValuePair<int, string>? master,
            KeyValuePair<int, string> customer
            ) 
        { 
            this.requestID = requestID;
            this.startDate = startDate;
            this.homeTechModel = homeTechModel;
            this.problemDescription = problemDescription;
            this.requestStatus = requestStatus;
            this.completionDate = completionDate;
            this.master = master;
            this.customer = customer;
        }
    }
}
