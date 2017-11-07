using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace IrmaProcessor
{
	class TweetCount
	{
		public long countWithLocation;
		public long countWithPlaceButNoLocation;
		public long countWithoutLocationOrPlace;
	}

	class PlaceCount
	{
		public Place place;
		public long count;
	}

	class MainClass
	{
		private static void getFileList (string folder, List<string> fileNames)
		{
			foreach (string file in Directory.GetFiles (folder)) {
				fileNames.Add (file);
			}
			foreach (var subFolder in Directory.GetDirectories(folder)) {
				getFileList (subFolder, fileNames);
			}

		}

		public static void Main (string[] args)
		{
			string folder = "/media/root/8f92f579-f00a-4098-b3ca-f3d832b3e5ea/irma/";
			DateTime defaultStart = new DateTime (2017, 9, 1);

			JavaScriptSerializer jss = new JavaScriptSerializer ();
			Dictionary<long, TweetCount> counts = new Dictionary<long, TweetCount> ();
			Dictionary<string, PlaceCount> placeCounts = new Dictionary<string, PlaceCount> ();


			ReportObject ro = new ReportObject ();
			ro.SetKey ("total", 1);
			ro.SetKey ("location", 2);
			ro.SetKey ("place", 3);
			ro.SetKey ("files", 4);
			ro.SetKey ("sec_#", () => counts.Count.ToString ());
			ro.SetKey ("place_#", () => placeCounts.Count.ToString ());
			ro.BeginReport ();

			List<string> fileNames = new List<string> ();
			getFileList (folder, fileNames);
			foreach (var fileName in fileNames) {
				using (StreamReader reader = new StreamReader(fileName)) {
					while (!reader.EndOfStream) {
						String line = reader.ReadLine ();
						Tweet t = jss.Deserialize<Tweet> (line);
						long secDiff = (long)t.createdAt.Subtract (defaultStart).TotalSeconds;
						TweetCount count;
						if (!counts.TryGetValue (secDiff, out count)) {
							counts [secDiff] = count = new TweetCount ();
						}
						if (t.geoLocation != null) {
							ro [2]++;
							count.countWithLocation++;
						} else if (t.place != null) {
							ro [3]++;
							count.countWithPlaceButNoLocation++;
							PlaceCount placeCount;
							if (!placeCounts.TryGetValue (t.place.id, out placeCount)) {
								placeCounts [t.place.id] = placeCount = new PlaceCount { place=t.place };
							}
							placeCount.count++;
						} else {
							count.countWithoutLocationOrPlace++;
						}
						ro [1]++;
					}
					ro [4]++;
				}
//				break;
			}
			ro.EndReport ();

			using (StreamWriter writer = new StreamWriter("countOutput.txt")) {
				writer.WriteLine ("SecFrom20170901\tlocation\tplace\tnone");
				foreach (var item in counts) {
					writer.WriteLine ("{0}\t{1}\t{2}\t{3}", item.Key, item.Value.countWithLocation, item.Value.countWithPlaceButNoLocation, item.Value.countWithoutLocationOrPlace);
				}
				writer.Flush ();
			}
			using (StreamWriter writer = new StreamWriter("placeOutput.txt")) {
				writer.WriteLine ("PlaceID\tPlaceName\tPlaceType\tPlaceCount");
				foreach (var item in placeCounts) {
					writer.WriteLine ("{0}\t{1}\t{2}\t{3}", item.Key, item.Value.place.name, item.Value.place.placeType, item.Value.count);
				}
				writer.Flush ();
			}

		}
	}
}