using Google.Apis.Services;
using Newtonsoft.Json;
using Google.Apis.Discovery;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using System.Globalization;
using Google.Apis.Auth.OAuth2;
public class Extratime
{
    public object home;
    public object away;
}

public class Fixture
{
    public int id;
    public object referee;
    public string timezone;
    public DateTime date;
    public int timestamp;
    public Periods periods;
    public Venue venue;
    public Status status;
}

public class Fulltime
{
    public object home;
    public object away;
}

public class Goals
{
    public object home;
    public object away;
}

public class Halftime
{
    public object home;
    public object away;
}

public class Team
{
    public int id;
    public string name;
    public string logo;
    public object winner;
}

public class League
{
    public int id;
    public string name;
    public string country;
    public string logo;
    public string flag;
    public int season;
    public string round;
}

public class Paging
{
    public int current;
    public int total;
}

public class Parameters
{
    public string season;
    public string next;
    public string team;
}

public class Penalty
{
    public object home;
    public object away;
}

public class Periods
{
    public object first;
    public object second;
}

public class Response
{
    public Fixture fixture;
    public League league;
    public Teams teams;
    public Goals goals;
    public Score score;
}

public class Root
{
    public string get;
    public Parameters parameters;
    public List<object> errors;
    public int results;
    public Paging paging;
    public List<Response> response;
}

public class Score
{
    public Halftime halftime;
    public Fulltime fulltime;
    public Extratime extratime;
    public Penalty penalty;
}

public class Status
{
    public string @long;
    public string @short;
    public object elapsed;
}

public class Teams
{
    public Team home;
    public Team away;
}

public class Venue
{
    public int? id;
    public string name;
    public string city;
}


public class ResponseEvent
{
    }


public class Program
{

    public static void Main()
    {
        Task task = RunEventCreator();
        task.Wait();
    }

    public static async Task MainAsync()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://api-football-v1.p.rapidapi.com/v3/fixtures?season=2023&team=42&next=20"),
            Headers =
    {
        { "X-RapidAPI-Key", "KEY HERE" },
        { "X-RapidAPI-Host", "HOST HERE" },
    },
        };
        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine(body);
            string docPath =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // Write the string array to a new file named "WriteLines.txt".
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(body);
            Console.WriteLine(myDeserializedClass.response[0].fixture.id);
        }
    }

    

    private static async Task RunEventCreator()
    {
        /**  var client = new HttpClient();
          var request = new HttpRequestMessage
          {
              Method = HttpMethod.Post,
              RequestUri = new Uri("https://www.googleapis.com/calendar/v3/calendars/calendarId/events")
          };
        **/
        // Create the service.
        var service = new CalendarService(new BaseClientService.Initializer
        {
            ApplicationName = "NAME HERE",
            ApiKey = "KEY HERE",
        });


        Event newEvent = new Event()
        {
            Summary = "This is a test Event",
            Description = "Description",
            Start = new EventDateTime()
            {
                DateTime = DateTime.Parse("2024-03-26T09:00:00-07:00"),
                TimeZone = "GMT"
            },
            End = new EventDateTime()
            {
                DateTime = DateTime.Parse("2024-03-26T10:00:00-07:00"),
                TimeZone = "GMT"
            }
        };
        String calendarId = "owenhowarth@googlemail.com";
        EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);
        Event createdEvent = request.Execute();
        Console.WriteLine("Event created: {0}", createdEvent.HtmlLink);



    }
}
