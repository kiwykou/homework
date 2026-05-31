using System;
using System.Collections.Generic;
using System.Text;

namespace DbDemo.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string searchText { get; set; } = "";
    }
}
