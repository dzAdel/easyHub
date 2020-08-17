using System;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace easyLib.DB
{
    public interface ITable : IEnumerable<byte[]>
    {
        event Action<int, byte[]> RowAdded;
        event Action<int> RowReplacing;
        event Action<int, byte[]> RowReplaced;
        event Action<int> RowDeleting;
        event Action<int> RowDeleted;

        int Count { get; }
        int RowSizeLimit { get; }

        int Append(byte[] row);
        /* pre:
         * row != null
         * row.length <= MaxRowSize
         * post: 
         * Count > 0
         * Result < Count
         */

        int Replace(int ndxRow, byte[] row);
        /* pre:
         * ndxRow < Count
         * row != null
         * row.Length <= MaxRowSize
         * post:
         * Result < Count
         */
        void Delete(int ndxRow);
        /* pre:
         * ndxRow < Count
         */
        void Flush();
        byte[] Get(int ndxRow);
        /* pre:
         * ndwRow < Count
         * post:
         * Result != null
         * Result.Length <= MaxRowSize
         */

    }
    //-----------------------------------------------------

    public interface IAsyncTable
    {
        event Action<int, byte[]> RowAdded;
        event Action<int> RowReplacing;
        event Action<int, byte[]> RowReplaced;
        event Action<int> RowDeleting;
        event Action<int> RowDeleted;

        int Count { get; }
        int RowSizeLimit { get; }

        Task<int> AppendAsync(byte[] row);
        /* pre:
         * row != null
         * row.Length < MaxRowSize
         */

        Task<int> ReplaceAsync(int ndxRow, byte[] row);
        /* pre:
         * ndwRow < Count
         * row != null
         * row.Length <= MaxRowSize
         */

        Task DeleteAsync(int ndxRow);
        /* pre:
         * ndxRow < Count
         */

        Task FlushAsync();

        Task<byte[]> GetAsync(int ndxRow);
        /* pre:
         * ndxRow < Count
         */
    }
    //---------------------------------------------------------



}
