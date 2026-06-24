namespace LeaveManagement.API.DTOs.Common;

public class ApiResponse<T>
{
    public bool    Success    { get; set; }
    public string  Message    { get; set; } = string.Empty;
    public T?      Data       { get; set; }
    public int?    TotalCount { get; set; }
    public int?    Page       { get; set; }
    public int?    PageSize   { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message)
        => new() { Success = false, Message = message };

    public static ApiResponse<T> Paginated(T data, int total, int page, int pageSize)
        => new() { Success = true, Message = "Success", Data = data,
                   TotalCount = total, Page = page, PageSize = pageSize };
}
