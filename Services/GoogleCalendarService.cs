
using System.Text;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Services;
public class GoogleCalendarService : IGoogleCalendarService
{

    private readonly HttpClient _httpClient;
    private TokenResponse token;

    public GoogleCalendarService()
    {
        _httpClient = new HttpClient();
    }

    public string GetAuthCode()
    {
        try
        {
            var redirectURL = "https://localhost:5175/auth/callback";
            string prompt = "consent";
            string response_type = "code";
            string clientID = "434687675627-c2g8sitn3d3fn687lb2qo8qcfrusf18t.apps.googleusercontent.com";
            string scope = "https://www.googleapis.com/auth/calendar";
            string access_type = "offline";
            string redirect_uri_encode = Uri.EscapeDataString(redirectURL);
            var mainURL = $"https://accounts.google.com/o/oauth2/auth?redirect_uri={redirect_uri_encode}&prompt={prompt}&response_type={response_type}&client_id={clientID}&scope={scope}&access_type={access_type}";

            return mainURL;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    public async Task<GoogleTokenResponse> GetTokens(string code)
    {
        var clientId = "434687675627-c2g8sitn3d3fn687lb2qo8qcfrusf18t.apps.googleusercontent.com";
        var clientSecret = "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";
        var redirectURL = "https://localhost:5175/auth/callback";
        var tokenEndpoint = "https://oauth2.googleapis.com/token";
        var content = new StringContent($"code={code}&redirect_uri={Uri.EscapeDataString(redirectURL)}&client_id={clientId}&client_secret={clientSecret}&grant_type=authorization_code", Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await _httpClient.PostAsync(tokenEndpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleTokenResponse>(responseContent);

            // Armazena o refresh_token para usar depois
            var refresh_token = tokenResponse.refresh_token;

            return tokenResponse;
        }
        else
        {
            // Caso de erro ao autenticar
            throw new Exception($"Falha ao autenticar: {responseContent}");
        }
    }

    public string AddToGoogleCalendar(GoogleCalendarReqDTO googleCalendarReqDTO)
    {
        try
        {
            var token = new TokenResponse
            {
                RefreshToken = googleCalendarReqDTO.RefreshToken
            };

            var credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = "434687675627-c2g8sitn3d3fn687lb2qo8qcfrusf18t.apps.googleusercontent.com",
                        ClientSecret = "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"
                    }

                }), "user", token);

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = "MeuAppGoogleCL"
            });

            Event newEvent = new Event()
            {
                Summary = googleCalendarReqDTO.Summary,
                Description = googleCalendarReqDTO.Description,
                Start = new EventDateTime()
                {
                    DateTime = googleCalendarReqDTO.StartTime,
                },
                End = new EventDateTime()
                {
                    DateTime = googleCalendarReqDTO.EndTime,
                },
                Reminders = new Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new EventReminder[] {
                    new EventReminder() {
                        Method = "email", Minutes = 30
                    },
                    new EventReminder() {
                        Method = "popup", Minutes = 15
                    },
                    new EventReminder() {
                        Method = "popup", Minutes = 1
                    },
                }
                }
            };

            EventsResource.InsertRequest insertRequest = service.Events.Insert(newEvent, googleCalendarReqDTO.CalendarId);
            Event createdEvent = insertRequest.Execute();
            return createdEvent.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return string.Empty;
        }
    }
}