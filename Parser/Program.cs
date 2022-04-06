using System.Diagnostics;
using System.Runtime;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Parser.Models;

Console.WriteLine("Item DB Parser\n");

// Keep track of ETA for the program
DateTime startTime = DateTime.Now;

var context = new ItemDb();

Console.WriteLine("Starting up!");
context.Database.EnsureCreated();

Console.WriteLine("Disabling auto detect changes for bulk insert");
context.ChangeTracker.AutoDetectChangesEnabled = false;

var fileList = Directory.EnumerateFiles("json_data", "*.json");
var fileCount = fileList.Count();
var currentFile = 1;

Console.WriteLine("Files Found: " + fileCount);

List<double> averageProcessTime = new();

// Keep track of item ids
Dictionary<int, ItemList> DbHashSet = new();
// Get the current items in the DB
// We want unique items and no duplicates to later join to the item data
var currItems = await context.ItemList.ToListAsync();

// List containing new items to add to the history table
List<ItemData> itemDataToAdd = new();
// HashSet to keep track of itemId's which prevent duplicate insertions on dirty data
HashSet<int> currItemDataHashSet = new();

//Populate hashset with current items in the DB:
foreach (ItemList v in currItems)
{
    DbHashSet.Add(v.ItemId, v);
}

foreach (string file in fileList)
{
    var fileName = Path.GetFileName(file);

    Console.WriteLine("Processing [" + currentFile + "/" + fileCount + "] " + fileName);

    // Capture date from the file name
    string str = fileName;
    string _year = str.Substring(0, 4);
    string _month = str.Substring(5, 2);
    string _day = str.Substring(8, 2);
    string _hours = str.Substring(11, 2);
    string _minutes = str.Substring(13, 2);

    var date = _year + "-" + _month + "-" + _day + " " + _hours + ":" + _minutes; // "2021-10-26 01:32"
    _ = DateTime.TryParse(date, out global::System.DateTime dateValue);

    Console.Write("\tDeserializing json...");
    var tmp = await File.ReadAllTextAsync(file);
    var json = JsonConvert.DeserializeObject<List<Dictionary<string, Item>>>(tmp);
    Console.WriteLine(" done!");

    // Start Insertion into DB
    Stopwatch stopWatch = new();
    stopWatch.Start();
    var itemList = json?.SelectMany(x => x);

    if (itemList == null)
    {
        Console.WriteLine("No data in " + file);
        break;
    }

    currItemDataHashSet.Clear();// Reset the item db hash set
    //itemDataToAdd.Clear(); // Clear all elements in the insert list

    foreach (KeyValuePair<string, Item> item in itemList)
    {
        //string _itemid = item.Key; //The json object array does not contain static attributes--the attribute is dynamic item ids
        Item _item = item.Value;

        //Console.WriteLine("Adding " + _item.Name);
        if (!DbHashSet.ContainsKey(_item.Id))
        { // Powdered Wig sometimes appears twice--this fixes any other potential issues as well
            var createItem = new ItemList
            {
                ItemId = _item.Id,
                Name = _item.Name,
                Icon = new ItemIcon
                {
                    Description = _item.Descripton ?? string.Empty,
                    ImageIcon = (_item.IconLarge ?? _item.Icon) ?? string.Empty,
                },
            };
            await context.ItemList.AddAsync(createItem);
            currItems.Append(createItem);
            DbHashSet.Add(_item.Id, createItem);
        }

        var itemData = new ItemData
        {
            ItemList = DbHashSet[_item.Id],
            CurrentPrice = _item.Current.Price,
            Trend = _item.Today.Trend,
            TrendValue = _item.Today.Price,
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
    //await context.ItemHistory.AddRangeAsync(itemDataToAdd);

    // Console.WriteLine(context.ChangeTracker.DebugView.LongView);

    //await context.SaveChangesAsync();
    if (currentFile % 25 == 0 || (currentFile == fileCount))
    {
        Console.Write("\tDetecting changes...");
        await context.ItemHistory.AddRangeAsync(itemDataToAdd);
        context.ChangeTracker.DetectChanges();
        Console.WriteLine(" Done!");

        Console.Write("\tInserting items into DB...");
        await context.SaveChangesAsync();
        // Console.WriteLine(context.ChangeTracker.DebugView.LongView);
        // context.Dispose();
        itemDataToAdd.Clear(); // Clear all elements in the insert list

        // Clean up memory
        Console.WriteLine("\n\tRunning garbage collector: {0:N0}", GC.GetTotalMemory(false));
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect();
        Console.WriteLine("\tMemory Usage:              {0:N0}", GC.GetTotalMemory(true));

        // Start new instance
        //  context = new ItemDb();
        // currItems.Clear();
        //  currItems = await context.ItemList.ToListAsync();
        // context.ChangeTracker.AutoDetectChangesEnabled = false;
    }
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
context.Dispose();
//await context.SaveChangesAsync();

Console.WriteLine("\nFinished");
using (var tmp_context = new ItemDb())
{
    var itemCount = await tmp_context.ItemList.CountAsync();
    Console.WriteLine("Total unique items tracked: " + itemCount);

    var historyCount = await tmp_context.ItemHistory.CountAsync();
    Console.WriteLine("Total item record history: " + historyCount);
}

static double average(List<double> input)
{
    double sum = 0;
    for (int i = 0; i < input.Count; i++)
    {
        sum += input[i];
    }
    return sum / input.Count;
}