namespace Data.ViewModels.Comment;

public class CreateCommentRequest
{
    public string BookId  { set; get; } 
    public string UserId { set; get; } 
    public string PostId { set; get; }
    public string Text { set; get; }
    public int Rating { set; get; }

}