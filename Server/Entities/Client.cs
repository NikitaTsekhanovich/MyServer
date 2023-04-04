using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Server
{
    public class Client
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        
        public string Registration { get; set; }
        
        public List<Operation> Operations { get; set; }
        public List<Car> Cars { get; set; }
    }
    
    [ComplexType]
    public class Car
    {
        [Key]
        public string Id { get; set; }
        
        [ForeignKey("client")]
        public string ClientId { get; set; }
        
        public string Model { get; set; }
        public string Color { get; set; }
    }
    
    [ComplexType]
    public class Operation
    {
        [Key]
        public string Id { get; set; }
        
        [ForeignKey("client")]
        public string ClientId { get; set; }
        
        public string Name { get; set; }
        public string Price { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
    }
}