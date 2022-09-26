namespace KolmeoCodingTest.Core.Domain.Entities;

public class Product : EntityBase
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public float Price { get; set; }
}