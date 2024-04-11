using Google.Apis.Services;
using Newtonsoft.Json;
using Google.Apis.Discovery;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using System.Globalization;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
    public Response response;
    public string id;
}


public class Program
{


    //Most up to date games
    public static Root games = new Root();

    //List of upcoming games
    public static List<ResponseEvent> events = new List<ResponseEvent>();

    public const string CALENDARID = "[CALENDAR ID HERE]";
    public const string FILEPATH = "data.json";

    public static void Main()
    {
        Task setupAuth = SetupAuth();
        setupAuth.Wait();
       // Task t = GetAllEvents();
       // t.Wait();
        
        
        GetStoredGames();
        
        GetAndCheckUpdates();
        StoreGames();
        Console.WriteLine("DONE");
        Console.ReadLine();
        
    }

    public static void GetStoredGames()
    {
        if (File.Exists(FILEPATH))
        {
            string json = File.ReadAllText(FILEPATH);
            events = JsonConvert.DeserializeObject<List<ResponseEvent>>(json);
        }
    }

    public static void StoreGames()
    {
        string json = JsonConvert.SerializeObject(events);
        File.WriteAllText(FILEPATH, json);
    }


    public static void GetAndCreateAllGames()
    {
        Task GetGames = MainAsync();

        Task task = SetupAuth();
        
        GetGames.Wait();

        task.Wait();

        

        foreach (Response r in games.response)
        {
           Task createE = CreateEvent(r);
            createE.Wait();
        }
    }

    public static void GetAndCheckUpdates() { 
        Task GetUpdates = MainAsync();

        //Check to see if the most recent games are complete 
        if (events.Count > 0)
        {
            while (events.Count > 0 && events[0].response.fixture.date < DateTime.Now)
            {
                events.RemoveAt(0);
            }
        }

        Task GetUpdatedGmaes = MainAsync(); 
        GetUpdatedGmaes.Wait();

        foreach(Response r in games.response)
        {
            if (r.fixture.status.@short == "CANC") ;
            bool DoesExist = false;
            foreach (ResponseEvent er in events)
            {
                if (r.fixture.id == er.response.fixture.id)
                {
                    DoesExist = true;
                    //Check to see if date and time are the same
                    if (r.fixture.date == er.response.fixture.date)
                    {
                        //Same date
                    } else
                    {
                        //Update date
                        Task UpdateDate = UpdateEvent(r, er.id);
                        UpdateDate.Wait();
                        er.response = r;

                    }
                }
            }
            if (!DoesExist)
            {
                //Create new event
                Task createEvent = CreateEvent(r);
                createEvent.Wait();
            }


        }



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
        { "X-RapidAPI-Key", "[KEY HERE]" },
        { "X-RapidAPI-Host", "api-football-v1.p.rapidapi.com" },
    },
        };
        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(body);
            string docPath =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(body);
           // Console.WriteLine(myDeserializedClass.response[0].fixture.id);
            games = myDeserializedClass;
        }
    }
    public static UserCredential credential;
    

    private static async Task SetupAuth()
    { 
        
        using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                new[] { CalendarService.Scope.Calendar },
                "user", CancellationToken.None, new FileDataStore("Program"));
        }

    }

    

    private static async Task CreateEvent(Response r)
    { 
        var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "FootballFixture",
        });
        string title = r.teams.home.name + " v " + r.teams.away.name;
        if (r.teams.home.name == "Arsenal") title += "(H)";
        else title += "(A)";
        Event newEvent = new Event()
        {

            Summary = title,
            
            Description = r.fixture.status.@long + "",
            Start = new EventDateTime()
            {
                DateTime = r.fixture.date,
                TimeZone = r.fixture.timezone
            },
            End = new EventDateTime()
            {
                DateTime = r.fixture.date.AddHours(2),
                TimeZone = "GMT"
            }


        };
        EventsResource.InsertRequest request = service.Events.Insert(newEvent, CALENDARID);
        Event createdEvent = request.Execute();
        ResponseEvent re = new ResponseEvent();
        re.response = r;
        re.id = createdEvent.ICalUID;
        events.Add(re);
        Console.WriteLine("Event created: {0}", createdEvent.HtmlLink);
    }

    private static async Task UpdateEvent(Response r, string EventID)
    {
        var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "FootballFixture",
        });
        Event currentEvent = new Event();
        Console.WriteLine($"Attempting to get {EventID} from {CALENDARID}");
        EventID = Regex.Replace(EventID, @"@google\.com$", "");
        EventsResource.GetRequest req = service.Events.Get(CALENDARID, EventID);
        try
        {
            currentEvent = req.Execute();
        }catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        currentEvent.Start = new EventDateTime() {
            DateTime = r.fixture.date,
            TimeZone = r.fixture.timezone
        };
        currentEvent.End = new EventDateTime()
        {
            DateTime = r.fixture.date.AddHours(2),
            TimeZone = r.fixture.timezone
        };
        currentEvent.Summary = r.teams.home.name + " v " + r.teams.away.name;
        if (r.teams.home.name == "Arsenal") currentEvent.Summary += "(H)";
        else currentEvent.Summary += "(A)";
        EventsResource.UpdateRequest request = service.Events.Update(currentEvent, CALENDARID, EventID);
        Event updated  = request.Execute();
        Console.WriteLine($"Updated event: {currentEvent.HtmlLink} , new link? {updated.HtmlLink}");

         


    } 

    public static async Task GetAllEvents()
    {
        var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "FootballFixture",
        });

        // Define parameters of request.
        EventsResource.ListRequest request = service.Events.List("9e78488dc4a1ea135346438fbfa5d6ae7f388259b0595b24ae8dcf3ec1cdd525@group.calendar.google.com");
        request.TimeMin = DateTime.Now;
        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.MaxResults = 10;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

        // List events.
        Events events = request.Execute();
        Console.WriteLine("Upcoming events:");
        if (events.Items != null && events.Items.Count > 0)
        {
            foreach (var eventItem in events.Items)
            {
                string eventId = eventItem.Id;
                Console.WriteLine("{0} ({1})", eventItem.Summary, eventId);
            }
        }
        else
        {
            Console.WriteLine("No upcoming events found.");
        }
        Console.Read();
    }

}
