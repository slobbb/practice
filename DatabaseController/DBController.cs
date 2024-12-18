using Azure.Core;
using DatabaseController.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DatabaseController
{
    public class DatabasController
    {
        public string connectionString;

        public DatabasController(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static double CalculateAverageCompletionTime(List<RequestModel> requests)
        {
            if (requests.Any(req => req == null))
                return 0;
            var completedRequests = requests
                .Where(r => r.completionDate.HasValue)
                .ToList();


            if (completedRequests.Count > 0)
            {
                double averageDays = completedRequests
                    .Average(r => (r.completionDate.Value - r.startDate).TotalDays);
                double result = Math.Round(averageDays);
                return result;
            }
            else
            {
                return 0;
            }
        }
        public KeyValuePair<int, int>? Authorization(string login, string password)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {

                string query = "SELECT userID, userTypeID FROM Users WHERE login = @login AND password = @password";

                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@login", login);
                    sqlCommand.Parameters.AddWithValue("@password", password);
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userID = reader.GetInt32(0);
                            int userTypeID = reader.GetInt32(1);
                            return new KeyValuePair<int, int>(userID, userTypeID);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public User getUserData(int customerID)
        {
            User user;
            string name;
            int type;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "SELECT userID, fio, userTypeID FROM Users WHERE userID = @customerID";
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("customerID", customerID);
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            name = reader.GetString(1);
                            type = reader.GetInt32(2);
                            User user2 = new User(customerID, name, type);
                            user = user2;
                            //int id = reader.GetInt32(0); // userID
                            //string name = reader.GetString(1); // fio
                            //eUserTypes userType = (eUserTypes)reader.GetInt32(5);

                            //user = new User(id, name, userType); // Создаем объект User
                        }
                        else
                            user = null;
                    }
                }
            }
            return user;
        }

        public List<RequestModel> GetRequests(eUserTypes userType, int userID)
        {
            switch (userType)
            {
                case eUserTypes.Manager:
                    return GetAllRequests();

                case eUserTypes.Operator:
                    return GetAllRequests();

                case eUserTypes.Master:
                    return GetRequestsMaster(userID);

                case eUserTypes.Customer:
                    return GetRequestsCustomer(userID);

                default:
                    throw new ArgumentException("Неизвестный тип пользователя");
            }
        }

        public List<RequestModel> GetAllRequests()
        {
            List<RequestModel> requests = new List<RequestModel>();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "SELECT requestID, startDate, problemDescryption, completionDate, t2.requestStatus, t2.requestStatusID, t3.techType, t3.techModel, t3.techModelID, t5.userID AS clientID, t5.fio AS clientFio, t4.userID AS masterID, t4.fio AS masterFio FROM Requests as t1 LEFT JOIN RequestStatus as t2 ON t1.requestStatusID = t2.requestStatusID LEFT JOIN (SELECT t1.techModelID, t2.techType, t1.TechModel FROM HomeTechModel as t1 LEFT JOIN HomeTechType as t2 ON t1.techTypeID = t2.techTypeID ) as t3 ON t1.homeTechModelID = t3.techModelID LEFT JOIN Users as t4 ON t1.masterID = t4.userID LEFT JOIN Users as t5 ON t1.clientID = t5.userID";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlConnection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int requestID = reader.GetInt32(reader.GetOrdinal("requestID"));
                            DateTime startDate = reader.GetDateTime(reader.GetOrdinal("startDate"));
                            string problemDescription = reader.GetString(reader.GetOrdinal("problemDescryption"));
                            DateTime? completionDate = reader.IsDBNull(reader.GetOrdinal("completionDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("completionDate"));

                            int requestStatusID = reader.GetInt32(reader.GetOrdinal("requestStatusID"));
                            string requestStatusValue = reader.GetString(reader.GetOrdinal("requestStatus"));
                            KeyValuePair<int, string> requestStatus = new KeyValuePair<int, string>(requestStatusID, requestStatusValue);

                            int techModelID = reader.GetInt32(reader.GetOrdinal("techModelID"));
                            string techModel = reader.GetString(reader.GetOrdinal("techModel"));
                            KeyValuePair<int, string> homeTechModel = new KeyValuePair<int, string>(techModelID, techModel);

                            int clientID = reader.GetInt32(reader.GetOrdinal("clientID")); 
                            string clientFio = reader.GetString(reader.GetOrdinal("clientFio"));
                            KeyValuePair<int, string> customer = new KeyValuePair<int, string>(clientID, clientFio);

                            KeyValuePair<int, string>? master = reader.IsDBNull(reader.GetOrdinal("masterFio"))
                                ? (KeyValuePair<int, string>?)null
                                : new KeyValuePair<int, string>(reader.GetInt32(reader.GetOrdinal("masterID")), reader.GetString(reader.GetOrdinal("masterFio")));

                            RequestModel request = new RequestModel(
                                requestID,
                                startDate,
                                homeTechModel,
                                problemDescription,
                                requestStatus,
                                completionDate,
                                master,
                                customer
                            );

                            requests.Add(request);
                        }
                    }
                }
            }
            return requests;
        }

        public List<RequestModel> GetRequestsMaster(int masterID)
        {
            List<RequestModel> requests = new List<RequestModel>();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "SELECT  requestID, startDate, problemDescryption,  completionDate,  t2.requestStatus,             t2.requestStatusID,  t3.techType,   t3.techModel,   t3.techModelID,   t5.fio,  t5.userID,    t4.fio,   t4.userID FROM  Requests as t1  LEFT JOIN      RequestStatus as t2 ON t1.requestStatusID = t2.requestStatusID  LEFT JOIN (SELECT  t2.techType,                    t1.TechModel,  t1.techModelID FROM HomeTechModel as t1 LEFT JOIN HomeTechType as t2 ON t1.techTypeID = t2.techTypeID ) as t3 ON t1.homeTechModelID = t3.techModelID LEFT JOIN Users as t4 ON t1.masterID = t4.userID LEFT JOIN Users as t5 ON t1.clientID = t5.userID WHERE t1.masterID = @masterID;";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@masterID", masterID);
                    sqlConnection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            KeyValuePair<int, string>? master = null;
                            if (!reader.IsDBNull(12))
                            {
                                master = new KeyValuePair<int, string>(reader.GetInt32(12), reader.GetString(11));
                            }

                            RequestModel request = new RequestModel
                        (
                        requestID: reader.GetInt32(0),
                        startDate: reader.GetDateTime(1),
                        problemDescription: reader.GetString(2),
                        completionDate: reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                        requestStatus: new KeyValuePair<int, string>(reader.GetInt32(5), reader.GetString(4)),
                        homeTechModel: new KeyValuePair<int, string>(reader.GetInt32(8), reader.GetString(7)),
                        customer: new KeyValuePair<int, string>(reader.GetInt32(10), reader.GetString(9)),
                        master: master
                        );

                            requests.Add(request);
                        }
                    }
                }
            }
            return requests;
        }

        public List<RequestModel> GetRequestsCustomer(int customerID)
        {
            List<RequestModel> requests = new List<RequestModel>();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "SELECT  requestID, startDate, problemDescryption,  completionDate,  t2.requestStatus,             t2.requestStatusID,  t3.techType,   t3.techModel,   t3.techModelID,   t5.fio,  t5.userID,    t4.fio,   t4.userID FROM  Requests as t1  LEFT JOIN      RequestStatus as t2 ON t1.requestStatusID = t2.requestStatusID  LEFT JOIN (SELECT  t2.techType,                    t1.TechModel,  t1.techModelID FROM HomeTechModel as t1 LEFT JOIN HomeTechType as t2 ON t1.techTypeID = t2.techTypeID ) as t3 ON t1.homeTechModelID = t3.techModelID LEFT JOIN Users as t4 ON t1.masterID = t4.userID LEFT JOIN Users as t5 ON t1.clientID = t5.userID WHERE t1.clientID = @customerID;";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@customerID", customerID);
                    sqlConnection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            KeyValuePair<int, string>? master = null;
                            if (!reader.IsDBNull(12))
                            {
                                master = new KeyValuePair<int, string>(reader.GetInt32(12), reader.GetString(11));
                            }

                            RequestModel request = new RequestModel
                        (
                        requestID: reader.GetInt32(0),
                        startDate: reader.GetDateTime(1),
                        problemDescription: reader.GetString(2),
                        completionDate: reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                        requestStatus: new KeyValuePair<int, string>(reader.GetInt32(5), reader.GetString(4)),
                        homeTechModel: new KeyValuePair<int, string>(reader.GetInt32(8), reader.GetString(7)),
                        customer: new KeyValuePair<int, string>(reader.GetInt32(10), reader.GetString(9)),
                        master: master
                        );

                            requests.Add(request);
                        }
                    }
                }
            }
            return requests;
        }

        public List<TechType> GetTypesData()
        {
            List<TechType> types = new List<TechType>();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM HomeTechType";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlConnection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TechType type = new TechType
                             (
                                   techTypeID: reader.GetInt32(0),
                                   techType: reader.GetString(1)
                             );
                            types.Add(type);
                        }
                    }
                }
            }
            return types;
        }

        public List<Model> GetModelData()
        {
            List<Model> models = new List<Model>();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM HomeTechModel";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlConnection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Model model = new Model
                            (
                                techModelID: reader.GetInt32(0),
                                techTypeID: reader.GetInt32(1),
                                techModel: reader.GetString(2)
                            );
                            models.Add(model);
                        }
                    }
                }
            }
            return models;
        }
        public List<Specialist> GetMasters()
        {
            List<Specialist> masters = new List<Specialist>();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Users Where userTypeID = 2";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlConnection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Specialist master = new Specialist
                            (
                                masterID: reader.GetInt32(0),
                                masterName: reader.GetString(1)
                            );
                            masters.Add(master);
                        }
                    }
                }
            }

            return masters;
        }
        public bool InsertCustomerRequest(int modelID, string problemDescription, int customerID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Requests (startDate, homeTechModelID, problemDescryption, requestStatusID, completionDate, MasterID, ClientID) " +
                               "VALUES (@date, @modelID, @problemDescription, 3, NULL, NULL, @customerID)";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    DateTime dateTime = DateTime.Now;

                    sqlCommand.Parameters.AddWithValue("@date", dateTime);
                    sqlCommand.Parameters.AddWithValue("@modelID", modelID);
                    sqlCommand.Parameters.AddWithValue("@problemDescription", problemDescription);
                    sqlCommand.Parameters.AddWithValue("@customerID", customerID);

                    sqlConnection.Open();
                    int rows = sqlCommand.ExecuteNonQuery();
                    if (rows == 0)
                        return false;
                    return true;
                }
            }
        }
        public bool UpdateOperatorRequest(int requestID, DateTime completionDate, int masterID, int requestStatusID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Requests SET masterID = @masterID, requestStatusID = @requestStatus WHERE requestID = @requestID";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@requestID", requestID);
                    sqlCommand.Parameters.AddWithValue("@masterID", masterID);
                    sqlCommand.Parameters.AddWithValue("@requestStatus", requestStatusID);

                    sqlConnection.Open();
                    int rows = sqlCommand.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
        
        public bool UpdateManagerRequest(int requestID, DateTime completionDate, int masterID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Requests SET completionDate = @completionDate, masterID = @masterID WHERE requestID = @requestID";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@completionDate", completionDate);
                    sqlCommand.Parameters.AddWithValue("@requestID", requestID);
                    sqlCommand.Parameters.AddWithValue("@masterID", masterID);

                    sqlConnection.Open();
                    int rows = sqlCommand.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        public bool UpdateCustomerRequest(int requestID, int modelID, string problemDescription, string oldDescription)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Requests SET homeTechModelID = @modelID, problemDescryption = @problemDescription WHERE requestID = @requestID";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    if(problemDescription.Length == 0)
                    {
                        problemDescription = oldDescription;
                    }
                    sqlCommand.Parameters.AddWithValue("@requestID", requestID);
                    sqlCommand.Parameters.AddWithValue("@modelID", modelID);
                    sqlCommand.Parameters.AddWithValue("@problemDescription", problemDescription);

                    sqlConnection.Open();
                    int rows = sqlCommand.ExecuteNonQuery();
                    return rows > 0; 
                }
            }
        }

        public bool AddComment(int requestID, string message)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Comments VALUES (@message, @requestID)";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@message", message);
                    sqlCommand.Parameters.AddWithValue("@requestID", requestID);

                    sqlConnection.Open();
                    int rows = sqlCommand.ExecuteNonQuery();
                    if (rows == 0)
                        return false;
                    return true;
                }
            }
        }

        public bool OrderParts(int requestID, string part)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO RepairParts VALUES (@part, @requestID)";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@part", part);
                    sqlCommand.Parameters.AddWithValue("@requestID", requestID);

                    sqlConnection.Open();
                    int rows = sqlCommand.ExecuteNonQuery();
                    if (rows == 0)
                        return false;
                    return true;
                }
            }
        }

        public List<RequestType> GetStatuses()
        {
            List<RequestType> types = new List<RequestType>();
            using(SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "SELECT  * FROM RequestStatus";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlConnection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RequestType type = new RequestType
                            (
                                id: reader.GetInt32(0),
                                name: reader.GetString(1)
                            );
                            types.Add(type);
                        }
                    }
                }
            }
            return types;
        }

        public bool ManagerDelete(int requestID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Requests WHERE requestID=@requestID";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@requestID", requestID);

                    sqlConnection.Open();
                    int rows = sqlCommand.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
    }
}
