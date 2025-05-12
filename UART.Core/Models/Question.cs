namespace UART.Core.Models;

public class Question<T> : BasicMessage
{
    public T? OnYes { get; set; }
    
    public T? OnNo { get; set; }
}

public class Question : Question<Action>
{
    
}

public class AsyncQuestion : Question<Func<Task>>
{
    
}