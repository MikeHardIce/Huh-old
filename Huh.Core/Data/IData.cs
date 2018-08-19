namespace Huh.Core.Data
{
    public interface IData<T>
    {
        string Key { get; set; }

        T Data { get; set; }
    }
}