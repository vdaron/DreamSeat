using MindTouch.Dream;

namespace LoveSeat.Interfaces
{
    public interface IListResult: System.IEquatable<IListResult>
    {
        DreamStatus StatusCode { get; }
        string Etag { get; }
        string RawString { get; }

    }
}