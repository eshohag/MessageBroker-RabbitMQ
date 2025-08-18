using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email.Consumer.Models
{
    public class Message
    {
        public Message()
        {
            Id = 10;
            Name = "Shohag";
            Address = "Mohakhali 1212, Dhaka";
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
