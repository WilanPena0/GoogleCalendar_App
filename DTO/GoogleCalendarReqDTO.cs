using System.Text.Json.Serialization;

public class GoogleCalendarReqDTO
{
    public string Summary
    {
        get;
        set;
    }

    public string Description
    {
        get;
        set;
    }

    public DateTime StartTime
    {
        get;
        set;
    }

    public DateTime EndTime
    {
        get;
        set;
    }

    public string CalendarId
    {
        get;
        set;
    }

    public string refreshToken
    {
        get;
        set;
    }

    [JsonIgnore]
    public string RefreshToken { get; set; }
}

public class GoogleTokenResponse
{
    public string access_type
    {
        get;
        set;
    }

    public long expires_in
    {
        get;
        set;
    }

    public string refresh_token
    {
        get;
        set;
    }

    public string scope
    {
        get;
        set;
    }

    public string token_type
    {
        get;
        set;
    }
}