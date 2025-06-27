using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Qtimetable;

public class GraphQLQuery(string query)
{
	[JsonPropertyName("query")]
	public string Query { get; } = query;
}

public class QdanceGraphQLResponse
{
	public static readonly string Query = @"{
	event {
		eventName
		editions {
			name
			_allReferencingEventEditionDays(orderBy: date_ASC) {
				name date
				_allReferencingEventEditionStageDays {
					cmsName
					stage { name }
					performances {
						startTime endTime
						name
						host
					}
				}
			}
		}
	}
}";

	[JsonPropertyName("data")]
	public DataResponse Data { get; set; }
	public class DataResponse
	{
		[JsonPropertyName("event")]
		public EventResponse Event { get; set; }
		public class EventResponse
		{
			[JsonPropertyName("eventName")]
			public string EventName { get; set; }

			[JsonPropertyName("editions")]
			public EventEditionResponse[] Editions { get; set; }
			public class EventEditionResponse
			{
				[JsonPropertyName("name")]
				public string Name { get; set; }

				[JsonPropertyName("_allReferencingEventEditionDays")]
				public EventEditionDayResponse[] Days { get; set; }
				public class EventEditionDayResponse
				{
					[JsonPropertyName("name")]
					public string Name { get; set; }

					[JsonPropertyName("date")]
					public string Date { get; set; }

					[JsonPropertyName("_allReferencingEventEditionStageDays")]
					public EventEditionStageDayResponse[] StageDays { get; set; }
					public class EventEditionStageDayResponse
					{
						[JsonPropertyName("cmsName")]
						public string CmsName { get; set; }

						[JsonPropertyName("stage")]
						public StageResponse Stage { get; set; }
						public class StageResponse
						{
							[JsonPropertyName("name")]
							public string Name { get; set; }
						}

						[JsonPropertyName("performances")]
						public PerformanceResponse[] Performances { get; set; }
						public class PerformanceResponse
						{
							[JsonPropertyName("startTime")]
							public DateTime? StartTime { get; set; }

							[JsonPropertyName("endTime")]
							public DateTime? EndTime { get; set; }

							[JsonPropertyName("name")]
							public string Name { get; set; }

							[JsonPropertyName("host")]
							public bool Host { get; set; }
						}
					}
				}
			}
		}
	}
}

public class Set
{
	public string reportedDay;
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
	private static readonly List<Stage> Stages = [];

	private static Stage GetStage(string name)
	{
		foreach (var stage in Stages) {
			if (stage.stage.Equals(name, StringComparison.InvariantCultureIgnoreCase)) {
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
			channel = "1387805277692104774",
			emoji = "<:dq_red:988093668219031603>",
			url = "https://www.youtube.com/@qdance/live",
		});

		Stages.Add(new() {
			stage = "BLUE",
			channel = "1387348870295457852",
			emoji = "<:dq_blue:988094952280055808>",
			url = "https://defqon1blue.mixlr.com/embed",
		});

		Stages.Add(new() {
			stage = "BLACK",
			channel = "1387348902914818170",
			emoji = "<:dq_black:988094951038537778>",
			url = "https://defqon1black.mixlr.com/embed",
		});

		Stages.Add(new() {
			stage = "U.V.",
			channel = "1387348921256382517",
			emoji = "<:dq_uv:988094948199006298>",
			url = "https://defqon1uv.mixlr.com/embed",
		});

		Stages.Add(new() {
			stage = "MAGENTA",
			channel = "1387348935785582602",
			emoji = "<:dq_magenta:988094945057452203>",
			url = "https://defqon-1-magenta.mixlr.com/embed",
		});

		Stages.Add(new() {
			stage = "INDIGO",
			channel = "1387348951346450432",
			emoji = "<:dq_indigo:988094943790792724>",
			url = "https://defqon1indigo.mixlr.com/embed",
		});

		Stages.Add(new() {
			stage = "GOLD",
			channel = "1387348963191164939",
			emoji = "<:dq_gold:988094953903231036>",
			url = "https://defqon1gold.mixlr.com/embed",
		});

		Stages.Add(new() {
			stage = "YELLOW",
			channel = "1387348977573298186",
			emoji = "<:dq_yellow:988094949780246548>",
			url = "https://defqon1yellow.mixlr.com/embed",
		});

		Stages.Add(new() {
			stage = "GREEN",
			channel = "1387348989808082995",
			emoji = "<:dq_green:1387365970779050064>",
			url = "https://defqon1green.mixlr.com/embed",
		});

		Stages.Add(new() {
			stage = "PINK",
			channel = "1387349000184922172",
			emoji = "<:dq_pink:1387365972502908929>",
			url = "https://defqon1pink.mixlr.com/embed",
		});

		Stages.Add(new() {
			stage = "PURPLE",
			channel = "1387349012910313584",
			emoji = "<:dq_purple:988094360006574103>",
			url = "https://defqon1purple.mixlr.com/embed",
		});

		Stages.Add(new() {
			stage = "ORANGE",
			channel = "1387349023354261515",
			emoji = "<:qdance:700038070594043986>",
			url = "https://defqon1orange.mixlr.com/embed",
		});

		Stages.Add(new() {
			stage = "WHITE",
			channel = "1387349040651305041",
			emoji = "<:dq_white:1387378251562745906>",
			url = "https://defqon1white.mixlr.com/embed",
		});

		Stages.Add(new() {
			stage = "SILVER",
			channel = "1387349050478690356",
			emoji = "<:dq_silver:988094946756141156>",
			url = "https://defqon1silver.mixlr.com/embed",
		});

		Stages.Add(new() {
			stage = "BROWN",
			channel = "1387799382379729036",
			emoji = "<:dq_brown:1387377060288401408>",
			url = "https://defqon-1-brown.mixlr.com/embed",
		});

		// Download data
		var hc = new HttpClient();
		hc.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Reddit/Hardstyle, discord.gg/hardstyle, melissa@nimble.tools");
		var res = await hc.PostAsJsonAsync<GraphQLQuery>("https://www.q-dance.com/graphql/", new(QdanceGraphQLResponse.Query));
		var data = await res.Content.ReadFromJsonAsync<QdanceGraphQLResponse>();

		// Parse data
		foreach (var editionDay in data.Data.Event.Editions[0].Days) {
			var date = editionDay.Date.Split('-');
			var year = int.Parse(date[0]);
			var month = int.Parse(date[1]);
			var day = int.Parse(date[2]);
			Console.WriteLine($"{editionDay.Name} [ {year}, {month}, {day} ]");

			foreach (var stageDay in editionDay.StageDays) {
				// 1 space = black, 2 spaces = indigo, no spaces = blue
				if (stageDay.Stage.Name == "The Gathering") {
					stageDay.Stage.Name = "Blue";
				} else if (stageDay.Stage.Name == "The Gathering ") {
					stageDay.Stage.Name = "Black";
				} else if (stageDay.Stage.Name == "The Gathering  ") {
					stageDay.Stage.Name = "Indigo";
				} else if (stageDay.Stage.Name == "The Closing Ceremony") {
					stageDay.Stage.Name = "Red";
				}

				if (stageDay.Stage.Name.EndsWith(" Silent")) {
					stageDay.Stage.Name = stageDay.Stage.Name[..^7];
				}
				if (stageDay.Stage.Name.EndsWith(" Night")) {
					stageDay.Stage.Name = stageDay.Stage.Name[..^6];
				}

				string logStage = $"  \"{stageDay.Stage.Name}\" ({stageDay.CmsName})";

				var stage = GetStage(stageDay.Stage.Name);
				if (stage == null) {
					Console.WriteLine($"  {logStage} (not mapped to any stage)");
					continue;
				}
				Console.WriteLine($"  {logStage}");

				DateTime lastEndTime = EpochToDate(0);
				var numSets = 0;

				foreach (var performance in stageDay.Performances.OrderBy(p => p.StartTime)) {
					var title = performance.Name.Trim();

					// Remove characters that mess with the formatting (remove instead of escape, because it'll make .find easier)
					title = title.Replace("*", ""); // A*S*Y*S

					// Fix Q-dance mistake
					if (title == "Crypsis pres. My I'll Style") {
						title = "Crypsis pres. My Ill Style";
					}

					if (performance.Host) {
						stage.mc = title;
						continue;
					}

					numSets++;
					stage.sets.Add(new Set() {
						name = title,
						reportedDay = editionDay.Name,
						dateTime = performance.StartTime.Value,
					});

					if (performance.EndTime > lastEndTime) {
						lastEndTime = performance.EndTime.Value;
					}
				}

				// Add "Nothing" at the end of the day if there were any sets
				if (numSets > 0) {
					stage.sets.Add(new Set() {
						name = "",
						dateTime = lastEndTime
					});
				}
			}
		}

		foreach (var stage in Stages) {
			// Warn on empty stages
			if (stage.sets.Count == 0) {
				Console.WriteLine($"-- WARNING: Stage {stage.stage} has no sets!");
			}

			// Sort sets again
			stage.sets.Sort((a, b) => {
				if (a.dateTime > b.dateTime) {
					return 1;
				} else if (a.dateTime < b.dateTime) {
					return -1;
				}
				return 0;
			});
		}

		// Add common responses to all stages
		foreach (var stage in Stages) {
			stage.responses["^\\.(url|stream|link|watch)$"] = $":tv: Tune in to the livestream here: **<{stage.url}>**";
			//stage.responses["^\\.(hidechat|removechat|fuckchat)$"] =
			//	":thinking: Press F12 and paste this into the console to hide the chat on live.q-dance.com: " +
			//	"```sc=document.getElementById(\"scrollContainer\");sc.classList.remove(\"col-l--9\");sc.classList.remove(\"col-m--8\");" +
			//	"sc.classList.add(\"col-l--12\");sc.classList.remove(\"col-m--12\");``` :woman_tipping_hand: You can also use this userscript: " +
			//	"<https://greasyfork.org/en/scripts/446916-hide-q-dance-chat>";

			if (stage.mc != "") {
				stage.responses["^\\.(mc|mcs)$"] = $":microphone2: The MC(s) on this stage: **{stage.mc}**";
			}
		}

		// Write to file
		if (File.Exists("Defqon2025.json")) {
			File.Delete("Defqon2025.json");
		}
		using (var writer = new StreamWriter("Defqon2025.json", false, new UTF8Encoding(false))) {
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
		if (File.Exists("Defqon2025.md")) {
			File.Delete("Defqon2025.md");
		}
		using (var writer = new StreamWriter("Defqon2025.md", false, Encoding.UTF8)) {
			writer.NewLine = "\n";

			writer.WriteLine("# Schedule");
			writer.WriteLine("This is the full (expected) timetable for the Defqon.1 2025 livestreams. I will keep it updated the best I can. Please feel free to let me know if something is wrong or needs to be updated.");
			writer.WriteLine();

			foreach (var stage in Stages) {
				writer.WriteLine("## " + stage.stage);
				writer.WriteLine("Watch live: {0}", stage.url);
				writer.WriteLine();
				writer.WriteLine("Day | Time ([CEST](https://time.is/CEST)) | Set");
				writer.WriteLine("--|--|--");
				string lastReportedDay = "";
				for (int i = 0; i < stage.sets.Count; i++) {
					var set = stage.sets[i];
					if (!string.IsNullOrEmpty(set.reportedDay)) {
						lastReportedDay = set.reportedDay;
					}
					if (string.IsNullOrEmpty(set.name) && i == stage.sets.Count - 1) {
						continue;
					}
					writer.WriteLine("{0} | {1} | {2}", lastReportedDay, set.dateTime.ToString("HH:mm"), set.name);
				}
				writer.WriteLine();
			}

			writer.WriteLine("(Data pulled with [github.com/codecat/defqon-timetable](https://github.com/codecat/defqon-timetable))");
		}

		// Write Reddit link list to file
		if (File.Exists("Defqon2025Reddit.md")) {
			File.Delete("Defqon2025Reddit.md");
		}
		using (var writer = new StreamWriter("Defqon2025Reddit.md", false, Encoding.UTF8)) {
			writer.NewLine = "\n";

			foreach (var stage in Stages) {
				if (stage.stage == "RED") {
					writer.Write("  * 🎥 ");
				} else {
					writer.Write("  * 📻 ");
				}
				writer.WriteLine("**{0}**: {1}", stage.stage, stage.url);
			}
		}

		Console.ReadKey();
	}
}
