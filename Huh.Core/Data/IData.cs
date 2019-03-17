namespace Huh.Core.Data
{
    public interface IData<T>
    {
        string Key { get; set; }

        DataContent ContentType { get; set; }
        
        string ContentHint { get; set; }
        T Data { get; set; }
    }
}