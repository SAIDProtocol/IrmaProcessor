using System;

namespace IrmaProcessor
{
	public class Tweet
	{
		public DateTime createdAt { private set; get; }

		public long id{ private set; get; }

		public string text { private set; get; }

		public GeoLocation geoLocation{ private set; get; }

		public Place place;
	}

	public class GeoLocation
	{
		public double latitude { private set; get; }

		public double longitude { private set; get; }
	}

	public class Place
	{
		public string name { private set; get; }

		public string id { private set; get; }

		public string placeType { private set; get; }
	}
}