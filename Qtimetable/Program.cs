using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
		public string stage = "";
		public string mc = "";
		public string channel = "";
		public string emoji = "";
		public string url = "";
		public List<Set> sets = new List<Set>();
		public Dictionary<string, string> responses = new Dictionary<string, string>();
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
			return null;
		}

		private static DateTime EpochToDate(long epoch)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch).ToLocalTime();
		}

		private static string JsonEncode(string str)
		{
			return str.Replace("\\", "\\\\").Replace("\"", "\\\"");
		}

		static async Task Main(string[] args)
		{
			// Create stages with info that we care about
			Stages.Add(new() {
				stage = "RED",
				channel = "1120713959763357706",
				emoji = "<:dq_red:988093668219031603>",
				url = "https://live.q-dance.com/",
			});

			Stages.Add(new() {
				stage = "BLUE",
				channel = "1120713989320609813",
				emoji = "<:dq_blue:988094952280055808>",
				url = "https://live.q-dance.com/",
			});

			Stages.Add(new() {
				stage = "BLACK",
				channel = "1120714004277510174",
				emoji = "<:dq_black:988094951038537778>",
				url = "https://live.q-dance.com/",
			});

			Stages.Add(new() {
				stage = "UV",
				channel = "1120714012204748931",
				emoji = "<:dq_uv:988094948199006298>",
				url = "https://live.q-dance.com/",
			});

			Stages.Add(new() {
				stage = "YELLOW",
				channel = "1120714035315363851",
				emoji = "<:dq_yellow:988094949780246548>",
				url = "https://live.q-dance.com/",
			});

			Stages.Add(new() {
				stage = "INDIGO",
				channel = "1120714048229617694",
				emoji = "<:dq_indigo:988094943790792724>",
				url = "https://live.q-dance.com/",
			});

			Stages.Add(new() {
				stage = "MAGENTA",
				channel = "1120714060355338310",
				emoji = "<:dq_magenta:988094945057452203>",
				url = "https://live.q-dance.com/",
			});

			Stages.Add(new() {
				stage = "GOLD",
				channel = "1120714070803366018",
				emoji = "<:dq_gold:988094953903231036>",
				url = "https://live.q-dance.com/",
			});

			Stages.Add(new() {
				stage = "SILVER",
				channel = "1120714083042336848",
				emoji = "<:dq_silver:988094946756141156>",
				url = "https://live.q-dance.com/",
			});

			Stages.Add(new() {
				stage = "PURPLE",
				channel = "1120714092060090429",
				emoji = "<:dq_purple:988094360006574103>",
				url = "https://live.q-dance.com/",
			});

			Stages.Add(new() {
				stage = "GREEN",
				channel = "1120714101354668123",
				emoji = ":leafy_green:",
				url = "https://live.q-dance.com/",
			});

			Stages.Add(new() {
				stage = "ORANGE",
				channel = "1120714108912799744",
				emoji = ":orange_heart:",
				url = "https://live.q-dance.com/",
			});

			// Download data
			string dataUrl = @"https://dc9h6qmsoymbq.cloudfront.net/api/content/event-editions/141735599/timetable?id=141735599&version=2";

			var hc = new HttpClient();
			hc.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Reddit/Hardstyle, discord.gg/hardstyle, melissa@nimble.tools");
			var res = await hc.GetAsync(dataUrl);
			var data = await res.Content.ReadAsStringAsync();

			// Parse data
			var obj = JObject.Parse(data);
			var objDays = obj.SelectToken("data");
			foreach (var objDay in objDays) {
				var dayOfWeek = (string)objDay.SelectToken("day");
				var date = ((string)objDay.SelectToken("date")).Split("-");
				var year = int.Parse(date[0]);
				var month = int.Parse(date[1]);
				var day = int.Parse(date[2]);
				Console.WriteLine("{0} [ {1}, {2}, {3} ]", dayOfWeek, year, month, day);

				var objStages = objDay.SelectToken("stages");
				foreach (var objStage in objStages) {
					var stageTitle = (string)objStage.SelectToken("title");
					if (stageTitle == "BLUE NIGHT PARTY") {
						stageTitle = "BLUE";
					}
					if (stageTitle == "MAGENTA NIGHT PARTY (SILENT)" || stageTitle == "MAGENTA NIGHT PARTY (SILENT) HOSTED BY QULT") {
						stageTitle = "MAGENTA";
					}
					if (stageTitle == "SILVER SILENT DISCO  HOSTED BY THE FUNKY CAT" || stageTitle == "SILVER SILENT DISCO") {
						stageTitle = "SILVER";
					}
					if (stageTitle == "UV HOSTED BY DESPERADOS") {
						stageTitle = "UV";
					}

					var stage = GetStage(stageTitle);
					if (stage == null) {
						Console.WriteLine("  {0} (not mapped to any stage)", stageTitle);
						continue;
					}
					Console.WriteLine("  {0}", stageTitle);

					DateTime lastEndTime = EpochToDate(0);
					var objPerformances = objStage.SelectToken("performances");
					foreach (var objPerformance in objPerformances) {
						var title = ((string)objPerformance.SelectToken("title")).Trim();

						// Remove characters that mess with the formatting (remove instead of escape, because it'll make .find easier)
						title = title.Replace("*", ""); // A*S*Y*S

						// Clarifications
						if (title.StartsWith("2 ") && title.EndsWith(" 1")) {
							title += " (Sound Rush & Atmozfears)";
						}
						if (title == "Ghost Stories") {
							title += " (D-Block & S-te-Fan)";
						}
						if (title == "De Nachtbrakers") {
							title += " (Endymion, Degos & Re-Done, Bass Chaserz)";
						}
						if (title == "Frenchcore Familia") {
							title += " (Dr. Peacock, BillX, The Sickest Squad)";
						}

						// Fix encoding
						title = Encoding.UTF8.GetString(Encoding.Default.GetBytes(title));

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

						var timeStart = (DateTime)objPerformance.SelectToken("start");
						var timeEnd = (DateTime)objPerformance.SelectToken("end");

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

			// Remove stages that don't have a channel set
			for (int i = Stages.Count - 1; i >= 0; i--) {
				if (Stages[i].channel == "") {
					Stages.RemoveAt(i);
				}
			}

			// Add common responses to all stages
			foreach (var stage in Stages) {
				stage.responses["^\\.(url|stream|link|watch)$"] =
					":tv: Tune in to the livestream here: **<" + stage.url + ">**";
				stage.responses["^\\.(recording|rec)$"] =
					"<:police:359836811285102593> Recording is considered piracy which is against the rules. Do not ask for or " +
					"share recordings!";
				stage.responses["^\\.(tickets|buy|paid)$"] =
					":money_with_wings: This is a **paid** livestream. It requires a Dediqated membership (<http://bit.ly/DDQ-Membership>) OR a pay-per-view ticket (<http://bit.ly/DQ1-PPV>).";
				stage.responses["^\\.(hidechat|removechat|fuckchat)$"] =
					":thinking: Press F12 and paste this into the console to hide the chat on live.q-dance.com: " +
					"```sc=document.getElementById(\"scrollContainer\");sc.classList.remove(\"col-l--9\");sc.classList.remove(\"col-m--8\");" +
					"sc.classList.add(\"col-l--12\");sc.classList.remove(\"col-m--12\");``` :woman_tipping_hand: You can also use this userscript: " +
					"<https://greasyfork.org/en/scripts/446916-hide-q-dance-chat>";
				stage.responses["^\\.(mc|mcs)$"] =
					":microphone2: The MC(s) on this stage: **" + stage.mc + "**";
			}

			// Write to file
			if (File.Exists("Defqon2023.json")) {
				File.Delete("Defqon2023.json");
			}
			using (var writer = new StreamWriter("Defqon2023.json", false, Encoding.UTF8)) {
				writer.NewLine = "\n";
				writer.WriteLine("[");
				for (int j = 0; j < Stages.Count; j++) {
					var stage = Stages[j];

					writer.WriteLine("\t{");
					writer.WriteLine("\t\t\"stage\": \"{0}\",", stage.stage);
					writer.WriteLine("\t\t\"channel\": \"{0}\",", stage.channel);
					writer.WriteLine("\t\t\"emoji\": \"{0}\",", stage.emoji);
					writer.WriteLine("\t\t\"url\": \"{0}\",", stage.url);
					writer.WriteLine("\t\t\"streamdelay\":0,");
					writer.WriteLine("\t\t\"sets\": [");
					for (int i = 0; i < stage.sets.Count; i++) {
						var set = stage.sets[i];
						if (set.name == "") {
							writer.Write("\t\t\t\t[ {0}, {1}, {2}, {3}, {4} ]", set.dateTime.Year, set.dateTime.Month, set.dateTime.Day, set.dateTime.Hour, set.dateTime.Minute);
						} else {
							writer.Write("\t\t\t[ {0}, {1}, {2}, {3}, {4}, \"{5}\" ]", set.dateTime.Year, set.dateTime.Month, set.dateTime.Day, set.dateTime.Hour, set.dateTime.Minute, JsonEncode(set.name));
						}
						if (i == stage.sets.Count - 1) {
							writer.WriteLine();
						} else {
							writer.WriteLine(",");
						}
					}
					writer.WriteLine("\t\t],");
					writer.WriteLine("\t\t\"responses\": {");
					for (int i = 0; i < stage.responses.Count; i++) {
						var pair = stage.responses.ElementAt(i);
						writer.Write("\t\t\t\"{0}\":\"{1}\"", JsonEncode(pair.Key), JsonEncode(pair.Value));
						if (i == stage.responses.Count - 1) {
							writer.WriteLine();
						} else {
							writer.WriteLine(",");
						}
					}
					writer.WriteLine("\t\t}");
					writer.Write("\t}");

					if (j == Stages.Count - 1) {
						writer.WriteLine();
					} else {
						writer.WriteLine(",");
					}
				}
				writer.WriteLine("]");
			}

			// Write Markdown table to file
			if (File.Exists("Defqon2023.md")) {
				File.Delete("Defqon2023.md");
			}
			using (var writer = new StreamWriter("Defqon2023.md", false, Encoding.UTF8)) {
				writer.NewLine = "\n";

				writer.WriteLine("# Schedule");
				writer.WriteLine("This is the full timetable for the Defqon 2023 livestreams. I will keep it updated the best I can.");
				writer.WriteLine();

				foreach (var stage in Stages) {
					writer.WriteLine("## " + stage.stage);
					writer.WriteLine("Watch live: {0}", stage.url);
					writer.WriteLine();
					writer.WriteLine("Day | Time ([CEST](https://time.is/CEST)) | Set");
					writer.WriteLine("--|--|--");
					for (int i = 0; i < stage.sets.Count; i++) {
						var set = stage.sets[i];
						if (set.name == "" && i == stage.sets.Count - 1) {
							continue;
						}
						writer.WriteLine("{0} | {1} | {2}", set.dateTime.ToString("dddd"), set.dateTime.ToString("HH:mm"), set.name);
					}
					writer.WriteLine();
				}
			}
		}
	}
}
