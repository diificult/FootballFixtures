# FootballFixtures
 
I have been sick and tired of my calender not being updated with change of fixture dates, times and cancellations.

So I am creating a project that will do that for me. 


## How to use 

Update the code with your API code from API-Football (It allows for 100 requests per day, which isn't anywhere near what you will use if you use it once a day).

Get client_secrets from Google API and put into same folder as code.

Create a new calendar in your google calendar or choose an existing one. Go to calendar settings and get calendar ID, and put into CALENDARID const. 

Run the code.

To make it run daily, it can be setup onto your computer or a server to run when desired. (In-depth tutorial coming soon)


## Using

Google Calender API
https://developers.google.com/api-client-library/dotnet/apis/calendar/v3

API-Football
https://www.api-football.com/


## Roadmap

- [x] 1. Download the data. 
- [x] 2. Create event on google calender using this data.
- [x] 3. Keep program running on server to update each day.
- [ ] 4. Add more advanced data e.g. previous meetings to description.
- [ ] 5. Add Score to previous games.
- [ ] 6. Create UI to select teams and choose which calendar to update to.

- [ ] Export code and upload to this GitHub.
