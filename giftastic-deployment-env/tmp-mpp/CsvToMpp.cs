using System;
using System.Globalization;
using System.IO;
using Aspose.Tasks;
using Aspose.Tasks.Saving;

internal static class CsvToMpp
{
    private static string[] ParseCsvLine(string line)
    {
        var values = new System.Collections.Generic.List<string>();
        var current = new System.Text.StringBuilder();
        var quoted = false;

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (ch == '"')
            {
                if (quoted && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    quoted = !quoted;
                }
            }
            else if (ch == ',' && !quoted)
            {
                values.Add(current.ToString());
                current.Length = 0;
            }
            else
            {
                current.Append(ch);
            }
        }

        values.Add(current.ToString());
        return values.ToArray();
    }

    public static int Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine("Usage: CsvToMpp <input.csv> <output.mpp>");
            return 2;
        }

        var csvPath = args[0];
        var outputPath = args[1];
        var project = new Project();
        project.Set(Prj.Name, "Mentora Project 2 WBS");
        project.Set(Prj.Title, "Mentora Project 2 WBS");
        project.Set(Prj.Company, "Mentora");
        project.Set(Prj.ScheduleFromStart, true);
        project.Set(Prj.MinutesPerDay, 480);
        project.Set(Prj.MinutesPerWeek, 2400);
        project.Set(Prj.DaysPerMonth, 20);
        project.Set(Prj.DefaultStartTime, new DateTime(1, 1, 1, 8, 0, 0));
        project.Set(Prj.DefaultFinishTime, new DateTime(1, 1, 1, 17, 0, 0));

        var lines = File.ReadAllLines(csvPath);
        var count = 0;

        for (var i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                continue;
            }

            var columns = ParseCsvLine(lines[i]);
            if (columns.Length < 4 || string.IsNullOrWhiteSpace(columns[0]))
            {
                continue;
            }

            var name = columns[0].Trim();
            var durationDays = int.Parse(columns[1], CultureInfo.InvariantCulture);
            var start = DateTime.ParseExact(columns[2], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var finish = DateTime.ParseExact(columns[3], "yyyy-MM-dd", CultureInfo.InvariantCulture).Date.AddHours(17);

            var task = project.RootTask.Children.Add(name);
            task.Set(Tsk.Start, start);
            task.Set(Tsk.Finish, finish);
            task.Set(Tsk.Duration, project.GetDuration(durationDays, TimeUnitType.Day));
            task.Set(Tsk.Work, project.GetDuration(durationDays * 8, TimeUnitType.Hour));
            count++;
        }

        if (count == 0)
        {
            Console.Error.WriteLine("No tasks were imported.");
            return 3;
        }

        project.Set(Prj.StartDate, DateTime.ParseExact(ParseCsvLine(lines[1])[2], "yyyy-MM-dd", CultureInfo.InvariantCulture));
        project.Save(outputPath, SaveFileFormat.Mpp);
        Console.WriteLine("Created: " + outputPath);
        Console.WriteLine("Tasks imported: " + count);
        return 0;
    }
}
