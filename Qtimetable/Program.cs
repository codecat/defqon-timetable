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
			string dataUrl = @"https://prod.api-dev.q-dance.com/v1/eventsPage/eventEditionOverview/defqon-1-weekend-festival-2022?locale=nl";

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
					var stageTitle = (string)objStage.SelectToken("title");
					if (stageTitle == "BLUE AFTERPARTY") {
						stageTitle = "BLUE";
					}
					if (stageTitle == "MAGENTA AFTERPARY (SILENT)") {
						stageTitle = "MAGENTA";
					}
					if (stageTitle == "Heineken SILVER (SILENT)") {
						stageTitle = "SILVER";
					}

					var stage = GetStage(stageTitle);
					Console.WriteLine("  {0}", stageTitle);

					// There's a sort order in the json, so let's use that for our export too (why not?)
					stage.sortOrder = (int)objStage.SelectToken("sortOrder");

					DateTime lastEndTime = EpochToDate(0);
					var objTimeslots = objStage.SelectToken("timeSlots");
					foreach (var objTimeslot in objTimeslots) {
						var title = ((string)objTimeslot.SelectToken("title")).Trim();

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
						name = "",
						dateTime = lastEndTime
					});
					Console.WriteLine("    [ {0}, {1} ]: <Nothing>", lastEndTime.Hour, lastEndTime.Minute);
				}
			}

			// Fill in any missing information
			GetStage("RED").channel = "989101365345263646";
			GetStage("RED").emoji = "<:dq_red:988093668219031603>";
			GetStage("RED").url = "https://www.q-dance.com/";

			GetStage("BLUE").channel = "989101395351318589";
			GetStage("BLUE").emoji = "<:dq_blue:988094952280055808>";
			GetStage("BLUE").url = "https://www.q-dance.com/";

			GetStage("BLACK").channel = "989101406025822329";
			GetStage("BLACK").emoji = "<:dq_black:988094951038537778>";
			GetStage("BLACK").url = "https://www.q-dance.com/";

			GetStage("UV").channel = "989101415228141578";
			GetStage("UV").emoji = "<:dq_uv:988094948199006298>";
			GetStage("UV").url = "https://www.q-dance.com/";

			GetStage("YELLOW").channel = "989102581232042035";
			GetStage("YELLOW").emoji = "<:dq_yellow:988094949780246548>";
			GetStage("YELLOW").url = "https://www.q-dance.com/";

			GetStage("INDIGO").channel = "989102615843450880";
			GetStage("INDIGO").emoji = "<:dq_indigo:988094943790792724>";
			GetStage("INDIGO").url = "https://www.q-dance.com/";

			GetStage("MAGENTA").channel = "989102625570041866";
			GetStage("MAGENTA").emoji = "<:dq_magenta:988094945057452203>";
			GetStage("MAGENTA").url = "https://www.q-dance.com/";

			GetStage("GOLD").channel = "989102635187576843";
			GetStage("GOLD").emoji = "<:dq_gold:988094953903231036>";
			GetStage("GOLD").url = "https://www.q-dance.com/";

			GetStage("SILVER").channel = "989102642808631357";
			GetStage("SILVER").emoji = "<:dq_silver:988094946756141156>";
			GetStage("SILVER").url = "https://www.q-dance.com/";

			GetStage("PURPLE").channel = "989102651864154122";
			GetStage("PURPLE").emoji = "<:dq_purple:988094360006574103>";
			GetStage("PURPLE").url = "https://www.q-dance.com/";

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
			if (File.Exists("Defqon2022.json")) {
				File.Delete("Defqon2022.json");
			}
			using (var writer = new StreamWriter("Defqon2022.json", false, Encoding.UTF8)) {
				writer.NewLine = "\n";
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
						var setName = set.name.Replace("\"", "\\\"");

						// Fix encoding
						setName = Encoding.UTF8.GetString(Encoding.Default.GetBytes(setName));

						if (setName == "") {
							writer.Write("\t\t\t\t[ {0}, {1}, {2}, {3}, {4} ]", set.dateTime.Year, set.dateTime.Month, set.dateTime.Day, set.dateTime.Hour, set.dateTime.Minute);
						} else {
							writer.Write("\t\t\t[ {0}, {1}, {2}, {3}, {4}, \"{5}\" ]", set.dateTime.Year, set.dateTime.Month, set.dateTime.Day, set.dateTime.Hour, set.dateTime.Minute, setName);
						}
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
