using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Qtimetable
{
	public class Set
	{
		public DateTime dateTime;
		public string name = "";
	}

	public class Stage
	{
		public int sortOrder;

		public string stage = "";
		public string mc = "";
		public string channel = "";
		public string emoji = "";
		public string url = "";
		public List<Set> sets = new List<Set>();
		public List<string> faq = new List<string>();
	}

	class Program
	{
		private static List<Stage> Stages = new List<Stage>();

		private static Stage GetStage(string name)
		{
			foreach (var stage in Stages) {
				if (stage.stage == name) {
					return stage;
				}
			}

			var ret = new Stage() {
				stage = name
			};
			Stages.Add(ret);

			return ret;
		}

		private static DateTime EpochToDate(long epoch)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch).ToLocalTime();
		}

		static void Main(string[] args)
		{
			// Download data
			string dataUrl = @"https://prod.api-dev.q-dance.com/v1/eventsPage/eventEditionOverview/defqon-1-weekend-festival-2019?locale=nl";

			var wc = new WebClient();
			wc.Proxy = null;
			wc.Headers[HttpRequestHeader.UserAgent] = "r/Hardstyle / discord.gg/hardstyle / qdance@nimble.tools";
			string data = wc.DownloadString(dataUrl);

			// Parse data
			var obj = JObject.Parse(data);
			var objDays = obj.SelectToken("data.modules.timetable.data.eventEdition.days");
			foreach (var objDay in objDays) {
				var date = EpochToDate((long)objDay.SelectToken("dateTimeStart"));
				Console.WriteLine("{0} [ {1}, {2}, {3} ]", date.DayOfWeek, date.Year, date.Month, date.Day);

				var objStages = objDay.SelectToken("stages");
				foreach (var objStage in objStages) {
					// "The Closing Ceremony" is a separate stage in the timetable, but it's still on Red
					var stageTitle = (string)objStage.SelectToken("title");
					if (stageTitle == "The Closing Ceremony") {
						stageTitle = "Red";
					}

					var stage = GetStage(stageTitle);
					Console.WriteLine("  {0}", stageTitle);

					// There's a sort order in the json, so let's use that for our export too (why not?)
					stage.sortOrder = (int)objStage.SelectToken("sortOrder");

					DateTime lastEndTime = EpochToDate(0);
					var objTimeslots = objStage.SelectToken("timeSlots");
					foreach (var objTimeslot in objTimeslots) {
						var title = ((string)objTimeslot.SelectToken("title")).Trim();

						// The contest winner is already scheduled, but still listed in Q's json. We just ignore it.
						if (title == "Defqon.1 Contest Winner") {
							continue;
						}

						// Remove characters that mess with the formatting (remove instead of escape, because it'll make .find easier)
						title = title.Replace("*", ""); // A*S*Y*S

						// The MC for a stage has its own "timeslot" prefixed with "hosted by", so we catch that here
						if (title.ToLower().StartsWith("hosted by ")) {
							var host = title.Substring("hosted by ".Length);
							if (stage.mc == "") {
								stage.mc = host;
							} else if (!stage.mc.Contains(host)) {
								stage.mc += " & " + host;
							}
							continue;
						}

						var timeStart = EpochToDate((long)objTimeslot.SelectToken("dateTimeStart"));
						var timeEnd = EpochToDate((long)objTimeslot.SelectToken("dateTimeEnd"));

						// Add 3 minutes to all stages that have a video stream
						if (stageTitle == "Red" || stageTitle == "Blue" || stageTitle == "Black" || (stageTitle == "UV" && timeStart.Day == 30)) {
							timeStart += TimeSpan.FromMinutes(3);
						}

						stage.sets.Add(new Set() {
							name = title,
							dateTime = timeStart
						});
						Console.WriteLine("    [ {0}, {1} ]: {2}", timeStart.Hour, timeStart.Minute, title);

						if (timeEnd > lastEndTime) {
							lastEndTime = timeEnd;
						}
					}

					// Add "Nothing" at the end of the day
					stage.sets.Add(new Set() {
						name = "Nothing",
						dateTime = lastEndTime
					});
					Console.WriteLine("    [ {0}, {1} ]: <Nothing>", lastEndTime.Hour, lastEndTime.Minute);
				}
			}

			// Fill in any missing information
			GetStage("Red").channel = "591927725594378240";
			GetStage("Red").emoji = ":heart:";
			GetStage("Red").url = "https://www.q-dance.com/en/videos/red-live-2019";

			GetStage("Blue").channel = "591928007351074816";
			GetStage("Blue").emoji = ":blue_heart:";
			GetStage("Blue").url = "https://www.q-dance.com/en/videos/blue-live-2019";

			GetStage("Black").channel = "591928033523269632";
			GetStage("Black").emoji = ":black_heart:";
			GetStage("Black").url = "https://www.q-dance.com/en/videos/black-live-2019";

			GetStage("UV").channel = "591928065416757248";
			GetStage("UV").emoji = ":purple_heart:";
			GetStage("UV").url = "https://www.q-dance.com/en/videos/uv-live-2019";

			GetStage("Magenta").channel = "591928254877794324";
			GetStage("Magenta").emoji = ":loud_sound:";
			GetStage("Magenta").url = "";

			GetStage("Indigo").channel = "591928282295828480";
			GetStage("Indigo").emoji = ":loud_sound:";
			GetStage("Indigo").url = "";

			GetStage("Yellow").channel = "591928310582345738";
			GetStage("Yellow").emoji = ":loud_sound:";
			GetStage("Yellow").url = "";

			GetStage("Gold").channel = "591928331159601154";
			GetStage("Gold").emoji = ":loud_sound:";
			GetStage("Gold").url = "";

			GetStage("Silver").channel = "591928389938446356";
			GetStage("Silver").emoji = ":loud_sound:";
			GetStage("Silver").url = "";

			GetStage("Purple").channel = "591928410209517568";
			GetStage("Purple").emoji = ":loud_sound:";
			GetStage("Purple").url = "";

			// Remove stages that don't have a channel set
			for (int i = Stages.Count - 1; i >= 0; i--) {
				if (Stages[i].channel == "") {
					Stages.RemoveAt(i);
				}
			}

			// Sort stages
			Stages.Sort((Stage a, Stage b) => {
				if (a.sortOrder < b.sortOrder) {
					return -1;
				} else if (a.sortOrder > b.sortOrder) {
					return 1;
				}
				return 0;
			});

			// Write to file
			if (File.Exists("Defqon2019.json")) {
				File.Delete("Defqon2019.json");
			}
			using (var writer = new StreamWriter("Defqon2019.json")) {
				writer.WriteLine("[");
				for (int j = 0; j < Stages.Count; j++) {
					var stage = Stages[j];

					writer.WriteLine("\t{");
					writer.WriteLine("\t\t\"stage\": \"{0}\",", stage.stage);
					writer.WriteLine("\t\t\"mc\": \"{0}\",", stage.mc);
					writer.WriteLine("\t\t\"channel\": \"{0}\",", stage.channel);
					writer.WriteLine("\t\t\"emoji\": \"{0}\",", stage.emoji);
					writer.WriteLine("\t\t\"url\": \"{0}\",", stage.url);
					writer.WriteLine("\t\t\"sets\": [");
					for (int i = 0; i < stage.sets.Count; i++) {
						var set = stage.sets[i];
						if (set.name == "Nothing") {
							writer.Write("\t");
						}
						writer.Write("\t\t\t[ {0}, {1}, {2}, {3}, {4}, \"{5}\" ]", set.dateTime.Year, set.dateTime.Month, set.dateTime.Day, set.dateTime.Hour, set.dateTime.Minute, set.name);
						if (i == stage.sets.Count - 1) {
							writer.WriteLine();
						} else {
							writer.WriteLine(",");
						}
					}
					writer.WriteLine("\t\t],");
					writer.WriteLine("\t\t\"faq\": [");
					for (int i = 0; i < stage.faq.Count; i++) {
						writer.Write("\t\t\t\"{0}\"", stage.faq[i]);
						if (i == stage.faq.Count - 1) {
							writer.WriteLine();
						} else {
							writer.WriteLine(",");
						}
					}
					writer.WriteLine("\t\t]");
					writer.Write("\t}");

					if (j == Stages.Count - 1) {
						writer.WriteLine();
					} else {
						writer.WriteLine(",");
					}
				}
				writer.WriteLine("]");
			}

			Console.ReadKey();
		}
	}
}
