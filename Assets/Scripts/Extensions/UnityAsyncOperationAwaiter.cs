namespace KickblipsTwo.Extensions
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using UnityEngine;

    internal static class UnityAsyncOperationAwaiter
    {
        /// <summary>
        /// Fetches an awaiter from a task.
        /// </summary>
        /// <param name="asyncOp">The async operation belonging there.</param>
        /// <returns>The awaiter for async methods</returns>
        internal static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            asyncOp.completed += _ => { tcs.SetResult(null); };
            return ((Task)tcs.Task).GetAwaiter();
        }
    }
}