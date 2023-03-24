namespace Com.Ctrip.Framework.Apollo.OpenApi.Model;

public class PageModel<T>
{
#if NET40
    public IList<T>? Content { get; set; }
#else
    public IReadOnlyList<T>? Content { get; set; }
#endif
    public int Page { get; set; }

    public int Size { get; set; }

    public int Total { get; set; }
}
