using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RoomReservation.Web.Models;

namespace RoomReservation.Web.Services;

public class ApiService(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public void SetToken(string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private StringContent ToJson<T>(T obj) =>
        new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    private async Task<T?> GetAsync<T>(string url)
    {
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return default;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    private async Task<(T? data, string? error)> PostAsync<T>(string url, object body)
    {
        var response = await httpClient.PostAsync(url, ToJson(body));
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            var err = TryGetDetail(json);
            return (default, err ?? response.ReasonPhrase);
        }
        return (JsonSerializer.Deserialize<T>(json, JsonOptions), null);
    }

    private async Task<(T? data, string? error)> PutAsync<T>(string url, object body)
    {
        var response = await httpClient.PutAsync(url, ToJson(body));
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) return (default, TryGetDetail(json) ?? response.ReasonPhrase);
        return (JsonSerializer.Deserialize<T>(json, JsonOptions), null);
    }

    private async Task<(bool ok, string? error)> PatchAsync(string url, object body)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, url) { Content = ToJson(body) };
        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return (false, TryGetDetail(json) ?? response.ReasonPhrase);
        }
        return (true, null);
    }

    private async Task<(bool ok, string? error)> DeleteAsync(string url)
    {
        var response = await httpClient.DeleteAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return (false, TryGetDetail(json) ?? response.ReasonPhrase);
        }
        return (true, null);
    }

    private static string? TryGetDetail(string json)
    {
        try
        {
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("detail", out var d)) return d.GetString();
            if (doc.RootElement.TryGetProperty("title", out var t)) return t.GetString();
        }
        catch { }
        return null;
    }

    public Task<(AuthViewModel? data, string? error)> LoginAsync(string email, string password) =>
        PostAsync<AuthViewModel>("api/auth/login", new { email, password });

    public Task<(AuthViewModel? data, string? error)> RegisterAsync(object body) =>
        PostAsync<AuthViewModel>("api/auth/register", body);

    public Task<PagedViewModel<RoomViewModel>?> GetRoomsAsync(int page = 1, int pageSize = 10, string? building = null, string? roomType = null, string? sortBy = null, string? sortOrder = null)
    {
        var query = BuildQuery(("page", page), ("pageSize", pageSize), ("building", building), ("roomType", roomType), ("sortBy", sortBy), ("sortOrder", sortOrder));
        return GetAsync<PagedViewModel<RoomViewModel>>($"api/rooms{query}");
    }

    public Task<IEnumerable<RoomViewModel>?> GetAvailableRoomsAsync(string date, string startTime, string endTime, int? minCapacity = null)
    {
        var query = BuildQuery(("date", date), ("startTime", startTime), ("endTime", endTime), ("minCapacity", minCapacity));
        return GetAsync<IEnumerable<RoomViewModel>>($"api/rooms/available{query}");
    }

    public Task<RoomViewModel?> GetRoomAsync(int id) => GetAsync<RoomViewModel>($"api/rooms/{id}");

    public Task<(RoomViewModel? data, string? error)> CreateRoomAsync(object body) =>
        PostAsync<RoomViewModel>("api/rooms", body);

    public Task<(RoomViewModel? data, string? error)> UpdateRoomAsync(int id, object body) =>
        PutAsync<RoomViewModel>($"api/rooms/{id}", body);

    public Task<(bool ok, string? error)> DeleteRoomAsync(int id) => DeleteAsync($"api/rooms/{id}");

    public Task<PagedViewModel<ReservationViewModel>?> GetReservationsAsync(int page = 1, int pageSize = 10, string? status = null, string? sortBy = null, string? sortOrder = null)
    {
        var query = BuildQuery(("page", page), ("pageSize", pageSize), ("status", status), ("sortBy", sortBy), ("sortOrder", sortOrder));
        return GetAsync<PagedViewModel<ReservationViewModel>>($"api/reservations{query}");
    }

    public Task<PagedViewModel<ReservationViewModel>?> GetMyReservationsAsync(int page = 1, int pageSize = 10, string? status = null)
    {
        var query = BuildQuery(("page", page), ("pageSize", pageSize), ("status", status));
        return GetAsync<PagedViewModel<ReservationViewModel>>($"api/reservations/my{query}");
    }

    public Task<ReservationViewModel?> GetReservationAsync(int id) => GetAsync<ReservationViewModel>($"api/reservations/{id}");

    public Task<(ReservationViewModel? data, string? error)> CreateReservationAsync(object body) =>
        PostAsync<ReservationViewModel>("api/reservations", body);

    public Task<(bool ok, string? error)> CancelReservationAsync(int id) => DeleteAsync($"api/reservations/{id}");

    public Task<(bool ok, string? error)> UpdateReservationStatusAsync(int id, string status, string? notes = null) =>
        PatchAsync($"api/reservations/{id}/status", new { status, notes });

    public Task<PagedViewModel<UserViewModel>?> GetUsersAsync(int page = 1, int pageSize = 10, string? role = null, string? firstName = null, string? lastName = null)
    {
        var query = BuildQuery(("page", page), ("pageSize", pageSize), ("role", role), ("firstName", firstName), ("lastName", lastName));
        return GetAsync<PagedViewModel<UserViewModel>>($"api/users{query}");
    }

    public Task<(bool ok, string? error)> ToggleUserActiveAsync(int id, object body) =>
        PutAsync<UserViewModel>($"api/users/{id}", body).ContinueWith(t => (t.Result.data is not null, t.Result.error));

    private static string BuildQuery(params (string key, object? value)[] pairs)
    {
        var parts = pairs
            .Where(p => p.value is not null && p.value.ToString() != string.Empty)
            .Select(p => $"{p.key}={Uri.EscapeDataString(p.value!.ToString()!)}");
        var qs = string.Join("&", parts);
        return qs.Length > 0 ? "?" + qs : string.Empty;
    }
}
