
using System.Threading.Tasks;

public interface ITaskHandlerPanel //: IQuickUnloadable 
{
    TaskCompletionSource<bool> TCS { get; }
    void HandleTask(bool isTrue);
}
