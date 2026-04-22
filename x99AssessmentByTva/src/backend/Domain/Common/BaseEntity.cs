using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace x99AssessmentByTva.Domain.Common;

public abstract class BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
    public long Id { get; set; }

    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}
