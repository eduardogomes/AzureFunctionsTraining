using System;

public static void Run(QItem myQueueItem, ICollector<TableItem> myTable, TraceWriter log)
{
    TableItem myItem = new TableItem() {
        PartitionKey = "key",
        RowKey = Guid.NewGuid().ToString(),
        Time = DateTime.Now.ToString("hh.mm.ss.ffffff"),
        Msg = myQueueItem.msg,
        OriginalTime = myQueueItem.time
    };
    myTable.Add(myItem);
    log.Info($"C# Queue trigger function processed: {myItem.RowKey}, {myItem.Msg}");
}
public class TableItem{
    public string PartitionKey {get; set;}
    public string RowKey {get; set;}
    public string Time {get; set;}
    public string Msg {get; set;}
    public string OriginalTime {get; set;}
}
public class QItem {
    public string msg {get; set;}
    public string time {get; set;}
}