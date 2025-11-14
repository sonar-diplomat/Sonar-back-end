namespace Application.DTOs.UserExperience;

public class SubscriptionPackDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double DiscountMultiplier { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public List<SubscriptionFeatureDTO> Features { get; set; }
}

