using System.ComponentModel.DataAnnotations;

namespace LocksApi.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    [Timestamp]
    public uint Version { get; set; }
}