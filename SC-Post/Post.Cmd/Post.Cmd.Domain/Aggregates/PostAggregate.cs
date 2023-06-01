using CQRS.Core.Domain;
using CQRS.Core.Events;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string _author;
    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public bool Active
    {
        get => _active; set => _active = value;
    }

    public PostAggregate()
    {

    }
    public PostAggregate(Guid id, string author, string msg)
    {
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = msg,
            DatePosted = DateTime.Now
        });
    }

    public void Apply(PostCreatedEvent e)
    {
        _id = e.Id;
        _active = true;
        _author = e.Author;
    }

    public void EditMessage(string msg)
    {
        if (!_active) throw new InvalidOperationException("cnt edit inactive");

        if (string.IsNullOrWhiteSpace(msg)) throw new InvalidOperationException($"the value of{nameof(msg)} cnt be null");

        RaiseEvent(new MessageUpdatedEvent
        {
            Id = _id,
            Message = msg
        });
    }

    public void Apply(MessageUpdatedEvent e)
    {
        _id = e.Id;
    }

    public void LikePost()
    {
        if (!_active) throw new InvalidOperationException();

        RaiseEvent(new PostLikeEvent
        {
            Id = _id
        });
    }

    public void Apply(PostLikeEvent e)
    {
        _id = e.Id;
    }

    public void AddComment(string comment, string userName)
    {
        if (!_active) throw new InvalidOperationException("cnt comment inactive");

        if (string.IsNullOrWhiteSpace(comment)) throw new InvalidOperationException($"the value of{nameof(comment)} cnt be null");

        RaiseEvent(new CommentAddedEvent
        {
            Id = _id,
            CommentId = Guid.NewGuid(),
            Comment = comment,
            Username = userName,
            CommentDate = DateTime.Now,

        });
    }

    public void Apply(CommentAddedEvent e)
    {
        _id = e.Id;
        _comments.Add(e.CommentId, new Tuple<string, string>(e.Comment, e.Username));
    }

    public void EditComent(Guid commentId, string comment, string userName)
    {
        if (!_active) throw new InvalidOperationException("cnt edit comment inactive");

        if (string.IsNullOrWhiteSpace(comment)) throw new InvalidOperationException($"the value of{nameof(comment)} cnt be null");

        if (!_comments[commentId].Item2.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("forbidden to edit");
        }

        RaiseEvent(new CommentUpdatedEvent
        {
            Id = _id,
            CommentId = commentId,
            Comment = comment,
            UserName = userName,
            EditDate = DateTime.Now
        });

    }

    public void Apply(CommentUpdatedEvent e)
    {
        _id = e.Id;
        _comments[e.CommentId] = new Tuple<string, string>(e.Comment, e.UserName);
    }

    public void RemoveComment(Guid commentId, string userName)
    {
        if (!_active) throw new InvalidOperationException("cnt remove comment inactive");

        if (!_comments[commentId].Item2.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("forbidden to edit by userName");
        }

        RaiseEvent(new CommentRemoveEvent
        {
            Id = _id,
            CommentId = commentId
        });

    }

    public void Apply(CommentRemoveEvent e)
    {
        _id = e.Id;
        _comments.Remove(e.CommentId);
    }

    public void DeletePost(string userName)
    {
        if (!_active) throw new InvalidOperationException("the post has been deleted");

        if (!_author.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("forbidden to delete. post was made by someone else");

        RaiseEvent(new PostRemoveEvent
        {
            Id = _id
        });

    }

    public void Apply(PostRemoveEvent e)
    {
        _id = e.Id;
        _active = false;
    }

}
