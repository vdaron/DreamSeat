using MindTouch.Dream;

namespace DreamSeat.Interfaces
{
    public interface IListResult: System.IEquatable<IListResult>
    {
        DreamStatus StatusCode { get; }
        string Etag { get; }
        string RawString { get; }

    }
}