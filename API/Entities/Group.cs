using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Group
    {
        
        public Group()
        {

        }

        public Group(string i_Name)
        {

            Name = i_Name;

        }

        [Key]
        public string Name { get; set; }//Group Name - Primary Key

        public ICollection<Connection> Connection { get; set; } = new List<Connection>(); //List of connections, we initialize here because when we initialize the group so we want that had list of connection already
    }
}