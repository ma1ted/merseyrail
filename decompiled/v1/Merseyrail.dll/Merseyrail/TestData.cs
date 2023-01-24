using System;
using System.Collections.Generic;
using Common.Domain;
using Merseyrail.Domain;

namespace Merseyrail;

public static class TestData
{
	public static List<DepartureBoardItem> GetDeparturesItemsListDomain()
	{
		return new List<DepartureBoardItem>
		{
			new DepartureBoardItem
			{
				Destination = "Destination Text",
				ArrivalActual = new TimeSpan(11, 6, 41, 0, 0),
				ArrivalSchedule = new TimeSpan(11, 6, 40, 0, 0),
				DepartureActual = new TimeSpan(11, 6, 52, 0, 0),
				DepartureSchedule = new TimeSpan(11, 6, 50, 0, 0),
				TrainId = "trainidstring",
				Platform = "2",
				DestinationStation = new Station
				{
					CrsCode = "LVS",
					TipLoc = "1321",
					Name = "Liverpool Lime Street",
					HTMLInfo = "<h1>Info</h1>",
					IsMerseyrail = true,
					Lat = 1.3123,
					Lon = 52.3424
				}
			},
			new DepartureBoardItem
			{
				Destination = "Destination Text",
				ArrivalActual = new TimeSpan(11, 6, 41, 0, 0),
				ArrivalSchedule = new TimeSpan(11, 6, 40, 0, 0),
				DepartureActual = new TimeSpan(11, 6, 52, 0, 0),
				DepartureSchedule = new TimeSpan(11, 6, 50, 0, 0),
				TrainId = "trainidstring",
				Platform = "2",
				DestinationStation = new Station
				{
					CrsCode = "LVS",
					TipLoc = "1321",
					Name = "Liverpool Lime Street 2",
					HTMLInfo = "<h1>Info</h1>",
					IsMerseyrail = true,
					Lat = 1.3123,
					Lon = 52.3424
				}
			},
			new DepartureBoardItem
			{
				Destination = "Destination Text",
				ArrivalActual = new TimeSpan(11, 6, 41, 0, 0),
				ArrivalSchedule = new TimeSpan(11, 6, 40, 0, 0),
				DepartureActual = new TimeSpan(11, 6, 52, 0, 0),
				DepartureSchedule = new TimeSpan(11, 6, 50, 0, 0),
				TrainId = "trainidstring",
				Platform = "2",
				DestinationStation = new Station
				{
					CrsCode = "LVS",
					TipLoc = "1321",
					Name = "Liverpool Lime Street 3",
					HTMLInfo = "<h1>Info</h1>",
					IsMerseyrail = true,
					Lat = 1.3123,
					Lon = 52.3424
				}
			}
		};
	}

	public static List<TrainJourneyItem> GetDeparturesItemsList()
	{
		return new List<TrainJourneyItem>
		{
			new TrainJourneyItem
			{
				Destination = "Destination Text",
				Platform = "Platform Text",
				Countdown = "3 Minutes",
				TimeActual = "22:34",
				TimeScheduled = "22:30",
				ImageHomeIconResourceId = 2131165317,
				ImageArrowResourceId = 2131165271
			},
			new TrainJourneyItem
			{
				Destination = "Destination Text 2",
				Platform = "Platform Text 2",
				Countdown = "1 Minute",
				TimeActual = "20:21",
				TimeScheduled = "20:22",
				ImageHomeIconResourceId = 2131165316,
				ImageArrowResourceId = 2131165271
			},
			new TrainJourneyItem
			{
				Destination = "Destination Text 3",
				Platform = "Platform Text 2",
				Countdown = "1 Minute",
				TimeActual = "20:21",
				TimeScheduled = "20:22",
				ImageHomeIconResourceId = 2131165316,
				ImageArrowResourceId = 2131165271
			},
			new TrainJourneyItem
			{
				Destination = "Destination Text 4",
				Platform = "Platform Text 2",
				Countdown = "1 Minute",
				TimeActual = "20:21",
				TimeScheduled = "20:22",
				ImageHomeIconResourceId = 2131165316,
				ImageArrowResourceId = 2131165271
			},
			new TrainJourneyItem
			{
				Destination = "Destination Text 5",
				Platform = "Platform Text 2",
				Countdown = "1 Minute",
				TimeActual = "20:21",
				TimeScheduled = "20:22",
				ImageHomeIconResourceId = 2131165316,
				ImageArrowResourceId = 2131165271
			},
			new TrainJourneyItem
			{
				Destination = "Destination Text 6",
				Platform = "Platform Text 2",
				Countdown = "1 Minute",
				TimeActual = "20:21",
				TimeScheduled = "20:22",
				ImageHomeIconResourceId = 2131165316,
				ImageArrowResourceId = 2131165271
			},
			new TrainJourneyItem
			{
				Destination = "Destination Text 7",
				Platform = "Platform Text 2",
				Countdown = "1 Minute",
				TimeActual = "20:21",
				TimeScheduled = "20:22",
				ImageHomeIconResourceId = 2131165316,
				ImageArrowResourceId = 2131165271
			},
			new TrainJourneyItem
			{
				Destination = "Destination Text 8",
				Platform = "Platform Text 2",
				Countdown = "1 Minute",
				TimeActual = "20:21",
				TimeScheduled = "20:22",
				ImageHomeIconResourceId = 2131165316,
				ImageArrowResourceId = 2131165271
			}
		};
	}

	public static List<TrainJourneyItem> GetLastTrainssItemsList()
	{
		return new List<TrainJourneyItem>
		{
			new TrainJourneyItem
			{
				Destination = "Last train Text",
				Platform = "Platform Text",
				Countdown = "3 Minutes",
				TimeActual = "22:34",
				TimeScheduled = "22:30",
				ImageHomeIconResourceId = 2131165317,
				ImageArrowResourceId = 2131165271
			},
			new TrainJourneyItem
			{
				Destination = "Last train Text 2",
				Platform = "Platform Text 2",
				Countdown = "1 Minute",
				TimeActual = "20:21",
				TimeScheduled = "20:22",
				ImageHomeIconResourceId = 2131165316,
				ImageArrowResourceId = 2131165271
			},
			new TrainJourneyItem
			{
				Destination = "Last train Text 3",
				Platform = "Platform Text 2",
				Countdown = "1 Minute",
				TimeActual = "20:21",
				TimeScheduled = "20:22",
				ImageHomeIconResourceId = 2131165316,
				ImageArrowResourceId = 2131165271
			}
		};
	}
}
