namespace iPath.Application.Features;



public class GroupDTO
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int? NodeCount { get; set; } = null!;
}
