using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataAnalyzer.TaskExtensions;

public static class TaskExtensions
{
    /// <summary>
    ///     allows to call asynchronous method from synchronous code with no warnings;
    /// </summary>
    public static async void Forget(this Task task, params Type[] ignorableExceptions)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (!ignorableExceptions.Contains(ex.GetType()))
                throw;
        }
    }
}