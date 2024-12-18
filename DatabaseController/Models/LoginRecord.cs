using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseController.Models
{
    public class LoginRecord
    {
        public DateTime Timestamp { get; set; }
        public string Username { get; set; }
        public bool IsSuccessful { get; set; }

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss} | {Username} | {(IsSuccessful ? "Успешно" : "Ошибка")}";
        }
    }
}
