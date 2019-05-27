namespace Huh.Core.Data
{
    public interface IData<T>
    {
        string Key { get; set; }

        DataContent ContentType { get; set; }
        ///<summary>
        /// Any kind of hint that helps you to identify what the content
        /// of the data is.
        ///</summary>
        string ContentHint { get; set; }
        T Data { get; set; }
    }
}