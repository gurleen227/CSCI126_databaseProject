using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Parser.Models;

Console.WriteLine("Item DB Parser\n");

// Keep track of ETA for the program
DateTime startTime = DateTime.Now;

using (var context = new ItemDb())
{
    Console.WriteLine("Starting up!");
    context.Database.EnsureCreated();

    var fileList = Directory.EnumerateFiles("json_data", "*.json");
    var fileCount = fileList.Count();
    var currentFile = 1;

    List<double> averageProcessTime = new List<double>();

    foreach (string file in fileList)
    {
        var fileName = file.Split("/");
        Console.WriteLine("Processing [" + currentFile + "/" + fileCount + "] " + fileName[1]);

        // Capture date from the file name
        string str = fileName[1];
        string _year = str.Substring(0, 4);
        string _month = str.Substring(5, 2);
        string _day = str.Substring(8, 2);
        string _hours = str.Substring(11, 2);
        string _minutes = str.Substring(13, 2);

        var date = _year + "-" + _month + "-" + _day + " " + _hours + ":" + _minutes; // "2021-10-26 01:32"
        DateTime dateValue;
        DateTime.TryParse(date, out dateValue);
        // Console.WriteLine(dateValue);

        Console.Write("\tDeserializing json...");
        var tmp = await File.ReadAllTextAsync(file);
        var json = JsonConvert.DeserializeObject<List<Dictionary<string, Item>>>(tmp);
        Console.WriteLine(" done!");

        // Start Insertion into DB
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        var itemList = json?.SelectMany(x => x);

        if (itemList == null)
        {
            Console.WriteLine("No data in " + file);
            break;
        }

        Console.Write("\tInserting items into DB...");
        HashSet<int> DbHashSet = new HashSet<int>();
        HashSet<int> currItemDataHashSet = new HashSet<int>();
        List<ItemData> itemDataToAdd = new List<ItemData>();

        // Get the current items in the DB
        // We want unique items and no duplicates to later join to the item data
        var currItems = await context.Items.ToListAsync();

        // Populate hashset with current items in the DB:
        foreach (ItemList v in currItems)
        {
            DbHashSet.Add(v.ItemId);
        }

        foreach (KeyValuePair<string, Item> item in itemList)
        {
            //string _itemid = item.Key; //The json object array does not contain static attributes--the attribute is dynamic item ids
            Item _item = item.Value;

            //Console.WriteLine("Adding " + _item.Name);
            if (!DbHashSet.Contains(_item.Id))
            { // Powdered Wig sometimes appears twice--this fixes any other potential issues as well
                var createItem = new ItemList
                {
                    ItemId = _item.Id,
                };
                await context.Items.AddAsync(createItem);
                DbHashSet.Add(_item.Id);
            }

            var itemData = new ItemData
            {
                ItemId = _item.Id,
                Name = _item.Name,
                CurrentPrice = _item.Current.Price,
                Trend = _item.Today.Trend,
                TrendValue = _item.Today.Price,
                ImageIcon = (_item.IconLarge ?? _item.Icon) ?? string.Empty,
                Date = dateValue,
            };

            // Check if json has any duplicate data that shouldn't be there
            if (currItemDataHashSet.Contains(_item.Id))
            {
                // Duplicate item in the current json data of items -- skip
                continue;
            }
            currItemDataHashSet.Add(_item.Id);
            itemDataToAdd.Add(itemData);
        }
        await context.ItemHistory.AddRangeAsync(itemDataToAdd);
        await context.SaveChangesAsync();

        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        averageProcessTime.Add(ts.TotalMilliseconds);
        Console.WriteLine(" {0:0.0#}ms (Avg: {1:0.0#}ms)", ts.TotalMilliseconds, average(averageProcessTime));

        // Display elapsed time and estimated time remaining
        var elapsedTime = DateTime.Now - startTime;
        var timeRemaining = TimeSpan.FromTicks(DateTime.Now.Subtract(startTime).Ticks * (fileCount - (currentFile)) / (currentFile));
        Console.WriteLine("\nElapsed Time: {0:D2}:{1:D2}:{2:D2} - ETA: {3} minutes", elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds, Math.Round(timeRemaining.TotalMinutes, 2));

        currentFile++;
    }
}

Console.WriteLine("\nFinished");
using (var context = new ItemDb())
{
    var itemCount = await context.Items.CountAsync();
    Console.WriteLine("Total unique items tracked: " + itemCount);

    var historyCount = await context.ItemHistory.CountAsync();
    Console.WriteLine("Total item record history: " + historyCount);
}

double average(List<double> input)
{
    double sum = 0;
    for (int i = 0; i < input.Count; i++)
    {
        sum += input[i];
    }
    return sum / input.Count;
}