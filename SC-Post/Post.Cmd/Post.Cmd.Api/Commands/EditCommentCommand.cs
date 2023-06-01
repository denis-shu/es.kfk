using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands
{
    public class EditCommentCommand:BaseCommand
    {
        public Guid CommentId { get; set; }
        public string? CommentText { get; set; }
        public string? UserName { get; set; }    
    }
}
