using Fumen;
using Fumen.ChartDefinition;
using Fumen.Converters;
using static Fumen.Converters.SMCommon;
using Path = Fumen.Path;
using Debug = System.Diagnostics.Debug;

await new CouplesToRoutineConverter.Program().Main();

namespace CouplesToRoutineConverter
{
	internal sealed class Program
	{
		private readonly List<string> PackFolders =
		[
			@"C:\Games\StepMania 5\Songs\Only One Couples Pack",
			@"C:\Games\StepMania 5\Songs\Only One Couples Pack 2",
			@"C:\Games\StepMania 5\Songs\Only One Couples Pack 3",
			@"C:\Games\StepMania 5\Songs\Only One Couples Pack 4",
		];

		private const string DestinationFolder = @"C:\Games\StepMania 5\Songs\Only One Couples Pack 2025";

		private const string Assets0101 = "01-01";
		private const string Assets0102 = "01-02";
		private const string Assets0201 = "02-01";
		private const string Assets0301 = "03-01";
		private const string Assets0401 = "04-01";

		private static readonly Dictionary<string, string> Assets = new()
		{
			// Pack 1
			{ "Bangarang", Assets0101 },
			{ "Bubble Pop", Assets0102 },
			{ "Cat Groove", Assets0102 },
			{ "Connect", Assets0101 },
			{ "Could This Be Real", Assets0101 },
			{ "First Kiss", Assets0102 },
			{ "Jounetsu no Wobble", Assets0101 },
			{ "Little Busters! (livetune Remix)", Assets0102 },
			{ "Mermaid girl", Assets0102 },
			{ "PONPONPON (Twintale's Hardcore Bootleg Mix)", Assets0101 },
			{ "Pop Culture", Assets0102 },
			{ "Science Pop", Assets0102 },
			{ "Tabla n Bass (Raja Maharaja mix)", Assets0101 },
			{ "Tonight (Cutline Remix)", Assets0101 },
			{ "waxing and wanding (JAKAZID's Shingetsu Remix)", Assets0101 },

			// Pack 2
			{ "Be With You", Assets0201 },
			{ "Call Me Maybe", Assets0201 },
			{ "Centipede", Assets0201 },
			{ "Change and Chance", Assets0201 },
			{ "Drive Me Crazy", Assets0201 },
			{ "End The Earth", Assets0201 },
			{ "Everybody yume CHU de o hata ni Angel", Assets0201 },
			{ "Face", Assets0201 },
			{ "Icarus", Assets0201 },
			{ "Mr Taxi", Assets0201 },
			{ "No Idea", Assets0201 },
			{ "Seiya no Special Shooting Star", Assets0201 },
			{ "Spring of Life", Assets0201 },
			{ "The Sampling Paradise", Assets0201 },
			{ "Yume no future", Assets0201 },

			// Pack 3
			{ "Always Yours", Assets0301 },
			{ "Clarity", Assets0301 },
			{ "Echo Of Forever", Assets0301 },
			{ "Eight Bit Princess", Assets0301 },
			{ "Fearless", Assets0301 },
			{ "Flow In Da Sky", Assets0301 },
			{ "For You", Assets0301 },
			{ "I Knew You Were Trouble", Assets0301 },
			{ "Irony", Assets0301 },
			{ "Koisuru Meido no Ohanashi", Assets0301 },
			{ "Longing Back", Assets0301 },
			{ "Open Canvas", Assets0301 },
			{ "Panic Holic", Assets0301 },
			{ "PopLove", Assets0301 },
			{ "Shaking Pink", Assets0301 },
			{ "Six Trillion Years and a Night", Assets0301 },
			{ "Snow Prism", Assets0301 },
			{ "Spinning Around", Assets0301 },
			{ "Stay Awake", Assets0301 },
			{ "Three Way", Assets0301 },
			{ "You and I", Assets0301 },

			// Pack 4
			{ "Alive", Assets0401 },
			{ "Bari 3 Kyouwakoku", Assets0401 },
			{ "Bunny! Bunny!! Bunny!!!", Assets0401 },
			{ "Happy", Assets0401 },
			{ "Invitation From Mr. C", Assets0401 },
			{ "Invoker", Assets0401 },
			{ "Kimi no Koe wa", Assets0401 },
			{ "Kodomo Live", Assets0401 },
			{ "Marble Soda", Assets0401 },
			{ "Megitsune", Assets0401 },
			{ "Miracle 5ympho X", Assets0401 },
			{ "Night Sky", Assets0401 },
			{ "Paparazzi", Assets0401 },
			{ "Party", Assets0401 },
			{ "Proluvies", Assets0401 },
			{ "Rainbow", Assets0401 },
			{ "Red", Assets0401 },
			{ "Reflux", Assets0401 },
			{ "Sad Machine", Assets0401 },
			{ "Sugar Free", Assets0401 },
			{ "UN Owen Was Her", Assets0401 },
		};

		public async Task Main()
		{
			CreateLogger();
			await ProcessFolders();
			Logger.Info("Done.");
			Logger.Shutdown();
		}

		private void CreateLogger()
		{
			Logger.StartUp(new Logger.Config
			{
				WriteToConsole = true,
				Level = LogLevel.Info,
			});
		}

		private async Task ProcessFolders()
		{
			foreach (var packFolder in PackFolders)
			{
				Logger.Info($"Processing Pack \"{packFolder}\".");

				string[] songDirs;
				try
				{
					songDirs = Directory.GetDirectories(packFolder);
				}
				catch (Exception e)
				{
					Logger.Warn($"Could not get directories in \"{packFolder}\". {e}");
					continue;
				}

				foreach (var songDir in songDirs)
				{
					var songFolderName = new FileInfo(songDir).Name;
					if (string.IsNullOrEmpty(songFolderName))
					{
						Logger.Warn($"Could not parse song folder name from \"{songDir}\"");
						continue;
					}

					if (!Assets.TryGetValue(songFolderName, out var assets))
					{
						Logger.Warn($"No asset mapping defined for \"{songFolderName}\". Using {assets}.");
					}


					DirectoryInfo newSongDir;
					string[] songFiles;
					try
					{
						newSongDir = Directory.CreateDirectory(System.IO.Path.Join(DestinationFolder, songFolderName));
						songFiles = Directory.GetFiles(songDir);
					}
					catch (Exception e)
					{
						Logger.Warn($"Could not get song files from  \"{songFolderName}\". {e}");
						continue;
					}

					foreach (var file in songFiles)
					{
						if (!Path.GetExtension(file, out var extension))
						{
							continue;
						}

						var fileInfo = new FileInfo(file);
						var fileName = fileInfo.Name;
						if (extension == ".ogg" || extension == ".sm" || extension == ".ssc")
						{
							File.Copy(file, System.IO.Path.Join(newSongDir.FullName, fileName), true);
						}

						if (extension == ".sm" || extension == ".ssc")
						{
							await ProcessSongFile(fileInfo, $"banner-{assets}.png", $"background-{assets}.png");
						}
					}
				}
			}
		}

		private async Task ProcessSongFile(FileInfo fileInfo, string banner, string background)
		{
			Logger.Info($"Processing Song \"{fileInfo.Name}\".");

			// Load song.
			var reader = Reader.CreateReader(fileInfo);
			if (reader == null)
			{
				Logger.Error($"Unsupported file format. Cannot parse {fileInfo.Name}.");
				return;
			}

			Song song;
			try
			{
				song = await reader.LoadAsync(CancellationToken.None);
			}
			catch (Exception e)
			{
				Logger.Error($"Failed to load {fileInfo.Name}. {e}");
				return;
			}

			// Update charts.
			ProcessSong(song, banner, background);

			// Save song.
			var saveFile = fileInfo.FullName;
			var config = new SMWriterBase.SMWriterBaseConfig
			{
				FilePath = saveFile,
				Song = song,
				MeasureSpacingBehavior = SMWriterBase.MeasureSpacingBehavior.UseSourceExtraOriginalMeasurePosition,
				PropertyEmissionBehavior = SMWriterBase.PropertyEmissionBehavior.MatchSource,
				WriteTemposFromExtras = true,
				WriteStopsFromExtras = true,
				WriteDelaysFromExtras = true,
				WriteWarpsFromExtras = true,
				WriteScrollsFromExtras = true,
				WriteSpeedsFromExtras = true,
				WriteTimeSignaturesFromExtras = true,
				WriteTickCountsFromExtras = true,
				WriteLabelsFromExtras = true,
				WriteFakesFromExtras = true,
				WriteCombosFromExtras = true,
			};
			var fileFormat = FileFormat.GetFileFormatByExtension(fileInfo.Extension);
			switch (fileFormat.Type)
			{
				case FileFormatType.SM:
					await new SMWriter(config).SaveAsync();
					break;
				case FileFormatType.SSC:
					await new SSCWriter(config).SaveAsync();
					break;
				default:
					Logger.Error($"Unsupported file format. Cannot save {fileInfo.Name}.");
					break;
			}
		}

		private void ProcessSong(Song song, string banner, string background)
		{
			// Update the assets for this chart.
			song.SongSelectImage = $"../{banner}";
			song.Extras.AddDestExtra(TagBackground, $"../{background}", true);

			var chartsToAdd = new List<Chart>();
			var chartsToRemove = new List<Chart>();
			foreach (var sourceChart in song.Charts)
			{
				if (!TryGetChartType(sourceChart.Type, out var sourceChartType))
					continue;

				if (sourceChartType == ChartType.dance_routine)
				{
					chartsToRemove.Add(sourceChart);
					continue;
				}

				if (sourceChartType != ChartType.dance_double)
					continue;

				var destEvents = new List<Event>();

				var tapString = NoteChars[(int)NoteType.Tap].ToString();
				var mineString = NoteChars[(int)NoteType.Mine].ToString();
				var rollString = NoteChars[(int)NoteType.RollStart].ToString();
				var averageHoldLen = GetMostFrequentHoldLen(sourceChart.Layers[0].Events, sourceChart.NumInputs);

				var lastSourceEventsPerLane = new Event[sourceChart.NumInputs];
				var lastDestEventsPerLane = new LaneNote[sourceChart.NumInputs];
				var numPlayers = 2;

				foreach (var sourceChartEvent in sourceChart.Layers[0].Events)
				{
					if (sourceChartEvent is LaneHoldStartNote holdStart)
					{
						// Store the hold start for later so we can determine if it is short enough to convert to a tap.
						lastSourceEventsPerLane[holdStart.Lane] = holdStart;
					}
					else if (sourceChartEvent is LaneHoldEndNote holdEnd)
					{
						// Check the hold length to see if it is short enough to convert to a tap.
						var previousEvent = lastSourceEventsPerLane[holdEnd.Lane];
						Debug.Assert(previousEvent is LaneHoldStartNote);

						var length = holdEnd.IntegerPosition - previousEvent.IntegerPosition;
						var player = previousEvent.SourceType == rollString ? 1 : 0;

						// The hold is long, add a new hold.
						if (length > averageHoldLen)
						{
							var newStart = previousEvent.Clone();
							((LaneNote)newStart).Player = player;
							destEvents.Add(newStart);

							var newEnd = (LaneHoldEndNote)holdEnd.Clone();
							newEnd.Player = player;
							destEvents.Add(newEnd);

							lastDestEventsPerLane[newEnd.Lane] = newEnd;
						}

						// The hold is short, convert it to a tap.
						else
						{
							var newExtras = new Extras(previousEvent.Extras);
							var newTap = new LaneTapNote()
							{
								TimeSeconds = previousEvent.TimeSeconds,
								MetricPosition = new MetricPosition(previousEvent.MetricPosition),
								IntegerPosition = previousEvent.IntegerPosition,
								Lane = holdEnd.Lane,
								SourceType = tapString,
								DestType = tapString,
								Appendage = 0,
								Player = player,
								Extras = newExtras,
							};
							destEvents.Add(newTap);
							lastDestEventsPerLane[newTap.Lane] = newTap;
						}

						lastSourceEventsPerLane[holdEnd.Lane] = holdEnd;
					}
					else if (sourceChartEvent is LaneNote mine && mine.SourceType == mineString)
					{
						// Try to assign on the mine to the player which most recently stepped on the lane.
						var player = 0;
						if (lastDestEventsPerLane[mine.Lane] != null)
							player = lastDestEventsPerLane[mine.Lane].Player;

						// Make a new mine.
						var newMine = (LaneNote)mine.Clone();
						newMine.Player = player;
						destEvents.Add(newMine);
						lastDestEventsPerLane[newMine.Lane] = newMine;
						lastSourceEventsPerLane[mine.Lane] = mine;
					}
					else if (sourceChartEvent is LaneTapNote tap)
					{
						numPlayers = 3;

						var newTap = (LaneTapNote)tap.Clone();
						newTap.Player = 2;
						destEvents.Add(newTap);
						lastDestEventsPerLane[newTap.Lane] = newTap;
						lastSourceEventsPerLane[tap.Lane] = tap;
					}
					else
					{
						destEvents.Add(sourceChartEvent.Clone());
					}
				}

				destEvents.Sort(new SMEventComparer());

				var destChart = new Chart
				{
					Artist = sourceChart.Artist,
					ArtistTransliteration = sourceChart.ArtistTransliteration,
					Genre = sourceChart.Genre,
					GenreTransliteration = sourceChart.GenreTransliteration,
					Author = sourceChart.Author,
					Description = "Original Chart",
					MusicFile = sourceChart.MusicFile,
					ChartOffsetFromMusic = sourceChart.ChartOffsetFromMusic,
					Tempo = sourceChart.Tempo,
					DifficultyRating = sourceChart.DifficultyRating,
					DifficultyType = sourceChart.DifficultyType,
					Extras = new Extras(sourceChart.Extras),
					Type = ChartTypeString(ChartType.dance_routine),
					NumPlayers = numPlayers,
					NumInputs = sourceChart.NumInputs,
				};

				destChart.Layers.Add(new Layer { Events = destEvents });
				chartsToAdd.Add(destChart);
			}

			// Remove old routine chart since we are rewriting them.
			foreach (var chartToRemove in chartsToRemove)
				song.Charts.Remove(chartToRemove);

			// Add new routine charts.
			if (chartsToAdd.Count > 0)
				song.Charts.AddRange(chartsToAdd);
		}

		private int GetMostFrequentHoldLen(List<Event> events, int numInputs)
		{
			var lastEventsPerLane = new Event[numInputs];
			Dictionary<int, int> holdLengths = new();
			foreach (var chartEvent in events)
			{
				if (chartEvent is LaneHoldStartNote holdStart)
				{
					lastEventsPerLane[holdStart.Lane] = holdStart;
				}
				else if (chartEvent is LaneHoldEndNote holdEnd)
				{
					var len = holdEnd.IntegerPosition - lastEventsPerLane[holdEnd.Lane].IntegerPosition;
					holdLengths.TryAdd(len, 0);
					holdLengths[len]++;
				}
			}

			var averageHoldLen = 0;
			var highestCount = 0;
			foreach (var holdLenCount in holdLengths)
			{
				if (holdLenCount.Value > highestCount)
				{
					highestCount = holdLenCount.Value;
					averageHoldLen = holdLenCount.Key;
				}
			}

			return averageHoldLen;
		}
	}
}
