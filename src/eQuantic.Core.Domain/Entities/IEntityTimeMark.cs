namespace eQuantic.Core.Domain.Entities;

public interface IEntityTimeMark
{
    /// <summary>
    /// Gets or sets the value of the created on
    /// </summary>
    DateTime CreatedAt { get; set; }
}