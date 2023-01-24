namespace Merseyrail.Domain;

public class TrainJourneyItem
{
	public string Destination { get; set; }

	public string Platform { get; set; }

	public string Countdown { get; set; }

	public string TimeActual { get; set; }

	public string TimeScheduled { get; set; }

	public int ImageArrowResourceId { get; set; }

	public int ImageHomeIconResourceId { get; set; }
}
