namespace WebAPI.Contracts;

public record ApiSuccessResponse<T>(int StatusCode, string Message, T Data);

public record ApiPagedSuccessResponse<T>(
    int StatusCode,
    string Message,
    int Page,
    int PageSize,
    int TotalCount,
    IReadOnlyCollection<T> Items);

public record ApiErrorResponse(int StatusCode, string Message, IReadOnlyCollection<string> Errors, string TraceId);
